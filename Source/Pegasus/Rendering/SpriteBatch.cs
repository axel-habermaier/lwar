namespace Pegasus.Rendering
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.InteropServices;
	using Assets.Effects;
	using Math;
	using Platform.Assets;
	using Platform.Graphics;
	using Platform.Logging;
	using Platform.Memory;

	/// <summary>
	///     Efficiently draws large amounts of 2D sprites by batching together quads with the same texture.
	/// </summary>
	public sealed class SpriteBatch : DisposableObject
	{
		/// <summary>
		///     The maximum number of quads that can be queued.
		/// </summary>
		private const int MaxQuads = 8192;

		/// <summary>
		///     The number of chunks that the dynamic vertex buffer allocates.
		/// </summary>
		private const int ChunkCount = 3;

		/// <summary>
		///     The effect that is used to draw the sprites.
		/// </summary>
		private readonly SpriteEffect _effect;

		/// <summary>
		///     The index buffer that is used for drawing.
		/// </summary>
		private readonly IndexBuffer _indexBuffer;

		/// <summary>
		///     The size of a single quad in bytes.
		/// </summary>
		private readonly int _quadSize = Marshal.SizeOf(typeof(Quad));

		/// <summary>
		///     The list of all quads.
		/// </summary>
		private readonly Quad[] _quads = new Quad[MaxQuads];

		/// <summary>
		///     Rasterizer state for sprite batch rendering with active scissor test.
		/// </summary>
		private readonly RasterizerState _scissorRasterizerState;

		/// <summary>
		///     The vertex buffer that is used for drawing.
		/// </summary>
		private readonly DynamicVertexBuffer _vertexBuffer;

		/// <summary>
		///     The vertex input layout used by the sprite batch.
		/// </summary>
		private readonly VertexInputLayout _vertexLayout;

		/// <summary>
		///     The blend state that should be used for drawing.
		/// </summary>
		private BlendState _blendState;

		/// <summary>
		///     The index of the section that is currently in use.
		/// </summary>
		private int _currentSection = -1;

		/// <summary>
		///     The index of the section list that is currently being used.
		/// </summary>
		private int _currentSectionList = -1;

		/// <summary>
		///     The depth stencil state that should be used for drawing.
		/// </summary>
		private DepthStencilState _depthStencilState;

		/// <summary>
		///     The number of quads that are currently queued.
		/// </summary>
		private int _numQuads;

		/// <summary>
		///     The number of section lists that are currently used.
		/// </summary>
		private int _numSectionLists;

		/// <summary>
		///     The number of sections that are currently used.
		/// </summary>
		private int _numSections;

		/// <summary>
		///     The sampler state that should be used for drawing.
		/// </summary>
		private SamplerState _samplerState;

		/// <summary>
		///     A mapping from a texture to its corresponding section list.
		/// </summary>
		private SectionList[] _sectionLists = new SectionList[4];

		/// <summary>
		///     The list of all sections.
		/// </summary>
		private Section[] _sections = new Section[16];

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="assets">The assets manager that should be used to load required assets.</param>
		public SpriteBatch(AssetsManager assets)
		{
			Assert.ArgumentNotNull(assets);

			WorldMatrix = Matrix.Identity;
			_effect = new SpriteEffect(assets);

			// Initialize the indices; this can be done once, so after the indices are copied to the index buffer,
			// we never have to change the index buffer again
			const ushort indexSize = MaxQuads * 6;
			var indices = new ushort[indexSize];
			ushort index = 0;

			for (var i = 0; i < indexSize; index += 4)
			{
				// Indices for the first triangle of the quad
				indices[i++] = index;
				indices[i++] = (ushort)(index + 1);
				indices[i++] = (ushort)(index + 2);

				// Indices for the second triangle of the quad
				indices[i++] = (ushort)(index + 3);
				indices[i++] = (ushort)(index + 1);
				indices[i++] = (ushort)(index + 2);
			}

			// Initialize the graphics objects
			_vertexBuffer = Quad.CreateDynamicVertexBuffer(MaxQuads, ChunkCount);
			_indexBuffer = IndexBuffer.Create(indices);
			_vertexLayout = Quad.GetInputLayout(_vertexBuffer.Buffer, _indexBuffer);

			_scissorRasterizerState = new RasterizerState()
			{
				CullMode = CullMode.None,
				ScissorEnabled = true
			};

			_vertexBuffer.SetName("SpriteBatch.VertexBuffer");
			_indexBuffer.SetName("SpriteBatch.IndexBuffer");
			_vertexLayout.SetName("SpriteBatch.VertexLayout");
			_scissorRasterizerState.SetName("SpriteBatch.Scissor");

			Reset();
		}

		/// <summary>
		///     Gets or sets the sampler state that should be used for drawing.
		/// </summary>
		public SamplerState SamplerState
		{
			get { return _samplerState; }
			set
			{
				Assert.ArgumentNotNull(value);
				Assert.NotDisposed(this);

				_samplerState = value;
			}
		}

		/// <summary>
		///     Gets or sets the depth stencil state that should be used for drawing.
		/// </summary>
		public DepthStencilState DepthStencilState
		{
			get { return _depthStencilState; }
			set
			{
				Assert.ArgumentNotNull(value);
				Assert.NotDisposed(this);

				_depthStencilState = value;
			}
		}

		/// <summary>
		///     Gets or sets the blend state that should be used for drawing.
		/// </summary>
		public BlendState BlendState
		{
			get { return _blendState; }
			set
			{
				Assert.ArgumentNotNull(value);
				Assert.NotDisposed(this);

				_blendState = value;
			}
		}

		/// <summary>
		///     Gets or sets the scissor area that should be used for the scissor test. All batched sprites are drawn before the area
		///     is changed.
		/// </summary>
		public Rectangle ScissorArea { get; set; }

		/// <summary>
		///     Gets or sets a value indicating whether a scissor test should be performed during rendering. All batched sprites are
		///     drawn before the scissor test is enabled or disabled.
		/// </summary>
		public bool UseScissorTest { get; set; }

		/// <summary>
		///     Gets or sets the world matrix used by the sprite batch. All batched sprites are drawn before the world matrix is
		///     changed.
		/// </summary>
		public Matrix WorldMatrix { get; set; }

		/// <summary>
		///     Gets or sets the layer of all subsequent drawing operations. All sprites within the same layer are drawn in some
		///     unspecified order. Layers, on the other hand, are drawn from lowest to highest, such that sprites in a higher layer
		///     overlap or hide sprites in a lower layer at the same position, depending on the blend and depth stencil
		///     state settings.
		/// </summary>
		public int Layer { get; set; }

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_vertexBuffer.SafeDispose();
			_indexBuffer.SafeDispose();
			_vertexLayout.SafeDispose();
			_scissorRasterizerState.SafeDispose();
			_effect.SafeDispose();
		}

		/// <summary>
		///     Draws the given rectangle.
		/// </summary>
		/// <param name="rectangle">The rectangle that should be drawn.</param>
		/// <param name="texture">The texture that should be used to draw the quad.</param>
		public void Draw(Rectangle rectangle, Texture2D texture)
		{
			Draw(rectangle, texture, Color.White);
		}

		/// <summary>
		///     Draws the given rectangle.
		/// </summary>
		/// <param name="rectangle">The rectangle that should be drawn.</param>
		/// <param name="texture">The texture that should be used to draw the quad.</param>
		/// <param name="color">The color of the quad.</param>
		public void Draw(Rectangle rectangle, Texture2D texture, Color color)
		{
			Draw(new RectangleF(rectangle.Left, rectangle.Top, rectangle.Width, rectangle.Height), texture, color);
		}

		/// <summary>
		///     Draws the given rectangle.
		/// </summary>
		/// <param name="rectangle">The rectangle that should be drawn.</param>
		/// <param name="texture">The texture that should be used to draw the quad.</param>
		/// <param name="color">The color of the quad.</param>
		/// <param name="texCoords">The texture coordinates that should be used.</param>
		public void Draw(RectangleF rectangle, Texture2D texture, Color color, RectangleF? texCoords = null)
		{
			var quad = new Quad(rectangle, color, texCoords);
			Draw(ref quad, texture);
		}

		/// <summary>
		///     Draws the given rectangle.
		/// </summary>
		/// <param name="position">The position of the quad that should be drawn.</param>
		/// <param name="size">The size of the quad that should be drawn.</param>
		/// <param name="texture">The texture that should be used to draw the quad.</param>
		/// <param name="color">The color of the quad.</param>
		/// <param name="rotation">The rotation (in radians) that should be applied to the quad before it is drawn.</param>
		public void Draw(Vector2 position, Size size, Texture2D texture, Color color, float rotation)
		{
			var rectangle = new RectangleF(-size.Width / 2.0f, -size.Height / 2.0f, size.Width, size.Height);
			var quad = new Quad(rectangle, color);

			var rotationMatrix = Matrix.CreateRotationZ(rotation);
			var unrotatedPosition = new Vector3(position.X, position.Y, 0);
			var offset = new Vector2(unrotatedPosition.X, unrotatedPosition.Y);

			Quad.Transform(ref quad, ref rotationMatrix);
			Quad.Offset(ref quad, ref offset);

			Draw(ref quad, texture);
		}

		/// <summary>
		///     Draws a textured rectangle at the given position with the texture's size.
		/// </summary>
		/// <param name="texture">The texture that should be used to draw the quad.</param>
		/// <param name="position">The position of the quad.</param>
		/// <param name="color">The color of the quad.</param>
		public void Draw(Texture2D texture, Vector2 position, Color color)
		{
			var size = new SizeF(texture.Size.Width, texture.Size.Height);

			var quad = new Quad(new RectangleF(position, size), color);
			Draw(ref quad, texture);
		}

		/// <summary>
		///     Draws a textured rectangle at the given position with the texture's size and rotation.
		/// </summary>
		/// <param name="texture">The texture that should be used to draw the quad.</param>
		/// <param name="angle">The rotation of the quad.</param>
		/// <param name="position">The position of the quad.</param>
		/// <param name="color">The color of the quad.</param>
		public void Draw(Texture2D texture, Vector2 position, float angle, Color color)
		{
			var size = new SizeF(texture.Size.Width, texture.Size.Height);
			var shift = new Vector2(- size.Width, - size.Height) * 0.5f;
			var quad = new Quad(new RectangleF(shift, size), color);

			var rotation = Matrix.CreateRotationZ(angle);
			Quad.Transform(ref quad, ref rotation);

			var translation = Matrix.CreateTranslation(position.X, position.Y, 0);
			Quad.Transform(ref quad, ref translation);

			Draw(ref quad, texture);
		}

		/// <summary>
		///     Draws the given quad.
		/// </summary>
		/// <param name="quad">The quad that should be added.</param>
		/// <param name="texture">The texture that should be used to draw the quad.</param>
		internal void Draw(ref Quad quad, Texture2D texture)
		{
			Assert.NotDisposed(this);
			Assert.ArgumentNotNull(texture);

			if (!CheckQuadCount(1))
				return;

			ChangeTexture(texture);

			// Add the quad to the list
			_quads[_numQuads++] = quad;
			++_sections[_currentSection].NumQuads;
		}

		/// <summary>
		///     Draws the outline of a rectangle.
		/// </summary>
		/// <param name="rectangle">The rectangle that should be drawn.</param>
		/// <param name="color">The color of the outline.</param>
		public void DrawOutline(RectangleF rectangle, Color color)
		{
			DrawLine(rectangle.TopLeft, rectangle.TopRight, color, 1);
			DrawLine(rectangle.BottomLeft, rectangle.BottomRight, color, 1);
			DrawLine(rectangle.TopLeft, rectangle.BottomLeft, color, 1);
			DrawLine(rectangle.TopRight, rectangle.BottomRight, color, 1);
		}

		/// <summary>
		///     Draws the outline of a circle.
		/// </summary>
		/// <param name="circle">The circle that should be drawn.</param>
		/// <param name="color">The color of the outline.</param>
		/// <param name="width">The width of the outline.</param>
		/// <param name="precision">The number of lines that are used to approximate the circle.</param>
		public void DrawOutline(CircleF circle, Color color, int width, int precision)
		{
			Assert.ArgumentInRange(precision, 5, Int16.MaxValue);
			Assert.ArgumentInRange(width, 1, Int16.MaxValue);

			var theta = 2d * 3.1415926f / precision;
			var cosine = (float)Math.Cos(theta);
			var sine = (float)Math.Sin(theta);

			var current = new Vector2(circle.Radius, 0);

			for (var i = 0; i < precision; i++)
			{
				var start = current + circle.Position;

				// Calculate the next point
				var t = current.X;
				current.X = cosine * current.X - sine * current.Y;
				current.Y = sine * t + cosine * current.Y;

				var end = current + circle.Position;
				DrawLine(start, end, color, width);
			}
		}

		/// <summary>
		///     Draws a line.
		/// </summary>
		/// <param name="start">The start of the line.</param>
		/// <param name="end">The end of the line.</param>
		/// <param name="color">The color of the line.</param>
		/// <param name="width">The width of the line.</param>
		public void DrawLine(Vector2 start, Vector2 end, Color color, int width)
		{
			Assert.ArgumentInRange(width, 1, Int32.MaxValue);
			Assert.That(start != end, "Start and end must differ from each other.");

			// We first define a default quad to draw a line that goes from left to right with a length of 1 and the given width
			// (the "unit line"). Therefore, the rectangle's width is 1 and its height is the given width.
			// The top edge of the rectangle is offset by half the width, so that the center of the rectangle lies on the
			// start point of the line.
			var rectangle = new RectangleF(0, -width / 2.0f, 1, width);
			var quad = new Quad(rectangle, color);

			// The scale factor is simply the magnitude of the direction vector, whereas the rotation is computed relative to
			// the unit vector in X direction.
			var scale = (end - start).Length;
			var rotation = MathUtils.ComputeAngle(start, end, new Vector2(1, 0));

			// Construct the transformation matrix and draw the transformed quad
			var transformMatrix = Matrix.CreateScale(scale, 1, 1) * Matrix.CreateRotationZ(-rotation) *
								  Matrix.CreateTranslation(start.X, start.Y + width / 2.0f, 0);

			Quad.Transform(ref quad, ref transformMatrix);
			Draw(ref quad, Texture2D.White);
		}

		/// <summary>
		///     Draws the given quads.
		/// </summary>
		/// <param name="quads">The quads that should be added.</param>
		/// <param name="count">The number of quads to draw.</param>
		/// <param name="texture">The texture that should be used to draw the quads.</param>
		internal void Draw(Quad[] quads, int count, Texture2D texture)
		{
			Assert.NotDisposed(this);
			Assert.ArgumentNotNull(quads);
			Assert.ArgumentNotNull(texture);
			Assert.ArgumentInRange(count, 0, quads.Length);

			if (count == 0 || !CheckQuadCount(count))
				return;

			ChangeTexture(texture);

			// Add the quads to the list and update the quad counts
			Array.Copy(quads, 0, _quads, _numQuads, count);
			_numQuads += count;
			_sections[_currentSection].NumQuads += count;
		}

		/// <summary>
		///     Draws all batched sprites.
		/// </summary>
		/// <param name="output">The output the sprite batch should draw to.</param>
		public void DrawBatch(RenderOutput output)
		{
			Assert.ArgumentNotNull(output);
			Assert.That(BlendState != null, "No blend state has been set.");
			Assert.That(DepthStencilState != null, "No depth stencil state has been set.");
			Assert.That(SamplerState != null, "No sampler state has been set.");
			Assert.NotDisposed(this);

			// Quit early if there's nothing to draw
			if (_numQuads == 0)
				return;

			// Sort the section lists by layer
			Array.Sort(_sectionLists, 0, _numSectionLists, SectionList.LayerComparer.Instance);

			// Prepare the vertex buffer
			UpdateVertexBuffer();
			_vertexLayout.Bind();

			// Draw the quads, starting with the lowest layer
			var offset = 0;
			for (var i = 0; i < _numSectionLists; ++i)
			{
				var sectionList = _sectionLists[i];

				// Bind the texture and rendering state
				_effect.Sprite = new Texture2DView(sectionList.Texture, sectionList.SamplerState);
				_effect.World = sectionList.WorldMatrix;

				sectionList.BlendState.Bind();
				sectionList.DepthStencilState.Bind();

				if (!sectionList.UseScissorTest)
					RasterizerState.CullNone.Bind();
				else
				{
					_scissorRasterizerState.Bind();
					output.ScissorArea = sectionList.ScissorArea;
				}

				// Draw and increase the offset
				var numIndices = sectionList.NumQuads * 6;
				output.DrawIndexed(_effect.Default, numIndices, offset, _vertexBuffer.VertexOffset);
				offset += numIndices;
			}

			// Reset the internal state
			Reset();
		}

		/// <summary>
		///     Resets the internal state.
		/// </summary>
		private void Reset()
		{
			_numQuads = 0;
			_numSections = 0;
			_numSectionLists = 0;
			_currentSectionList = -1;
			_currentSection = -1;
		}

		/// <summary>
		///     Check whether adding the given number of quads would overflow the internal quad buffer. Returns true
		///     if the quads can be batched.
		/// </summary>
		/// <param name="quadCount">The additional number of quads that should be drawn.</param>
		private bool CheckQuadCount(int quadCount)
		{
			Assert.ArgumentInRange(quadCount, 0, MaxQuads);

			// Check whether we would overflow if we added the given batch.
			var tooManyQuads = _numQuads + quadCount >= _quads.Length;

			if (tooManyQuads)
				Log.Warn("Sprite batch buffer overflow: {0} out of {1} allocated quads in use (could not add {2} quad(s)).",
						 _numQuads, MaxQuads, quadCount);

			return !tooManyQuads;
		}

		/// <summary>
		///     Copies the quads to the vertex buffer, sorted by texture.
		/// </summary>
		private unsafe void UpdateVertexBuffer()
		{
			var vertexData = _vertexBuffer.Map();
			var offset = 0;

			fixed (Quad* quadsPtr = &_quads[0])
			{
				var quads = new IntPtr(quadsPtr);
				for (var i = 0; i < _numSectionLists; ++i)
				{
					var section = _sectionLists[i].FirstSection;
					while (section != -1)
					{
						// Calculate the offsets into the arrays and the amount of bytes to copy
						var vertexOffset = vertexData + offset * _quadSize;
						var quadOffset = quads + _sections[section].Offset * _quadSize;
						var bytes = _sections[section].NumQuads * _quadSize;

						// Copy the entire section to the vertex buffer
						Interop.Copy(vertexOffset, quadOffset, bytes);

						// Update the section list's total quad count
						_sectionLists[i].NumQuads += _sections[section].NumQuads;

						// Update the offset and advance to the next section
						offset += _sections[section].NumQuads;
						section = _sections[section].Next;
					}
				}
			}
			_vertexBuffer.Unmap();
		}

		/// <summary>
		///     Checks whether the current texture has to be changed, and if so, creates a new section and possibly a new section
		///     list. Or, if any rendering settings have been changed, a new section and/or section list might be added as well.
		/// </summary>
		/// <param name="texture">The texture that should be used for newly added quads.</param>
		private void ChangeTexture(Texture2D texture)
		{
			// If nothing has changed, just continue to append to the current section
			if (_currentSectionList != -1 && _currentSection != -1 && SectionListMatches(_currentSectionList, texture))
				return;

			// Add a new section
			AddSection();

			// Depending on whether we've already seen this texture and these rendering settings, add it to the corresponding 
			// section list or add a new one
			var known = false;
			for (var i = 0; i < _numSectionLists; ++i)
			{
				if (!SectionListMatches(i, texture))
					continue;

				// We've already seen this texture and these rendering settings before, so add the new section to the list by setting the
				// list's tail section's next pointer to the newly allocated section
				_sections[_sectionLists[i].LastSection].Next = _numSections;
				// Set the section list's tail pointer to the newly allocated section
				_sectionLists[i].LastSection = _numSections;

				known = true;
				_currentSectionList = i;
				break;
			}

			if (!known)
			{
				_currentSectionList = _numSectionLists;
				AddSectionList(new SectionList(BlendState, DepthStencilState, SamplerState,
											   WorldMatrix,
											   texture,
											   ScissorArea, UseScissorTest,
											   Layer,
											   _numSections));
			}

			_currentSection = _numSections;

			// Mark the newly allocated section as allocated
			++_numSections;
		}

		/// <summary>
		///     Checks whether the section list with the specified index matches the given texture and the current rendering
		///     settings.
		/// </summary>
		/// <param name="sectionList">The index of the section list that should be checked.</param>
		/// <param name="texture">The texture that should be used to draw the quads.</param>
		private bool SectionListMatches(int sectionList, Texture2D texture)
		{
			var list = _sectionLists[sectionList];

			return list.Texture == texture && list.BlendState == BlendState && list.DepthStencilState == DepthStencilState &&
				   list.SamplerState == SamplerState && list.UseScissorTest == UseScissorTest && list.ScissorArea == ScissorArea &&
				   list.WorldMatrix == WorldMatrix && list.Layer == Layer;
		}

		/// <summary>
		///     Adds a new section, allocating more space for the new section if required.
		/// </summary>
		private void AddSection()
		{
			if (_numSections >= _sections.Length)
			{
				var sections = new Section[_sections.Length * 2];
				Array.Copy(_sections, sections, _sections.Length);
				_sections = sections;
			}

			_sections[_numSections] = new Section(_numQuads);
		}

		/// <summary>
		///     Adds the given section list, allocating more space for the new section list if required.
		/// </summary>
		/// <param name="sectionList">The section list that should be added.</param>
		private void AddSectionList(SectionList sectionList)
		{
			if (_numSectionLists >= _sectionLists.Length)
			{
				var sectionLists = new SectionList[_sectionLists.Length * 2];
				Array.Copy(_sectionLists, sectionLists, _sectionLists.Length);
				_sectionLists = sectionLists;
			}

			_sectionLists[_numSectionLists++] = sectionList;
		}

		#region Nested type: Section

		/// <summary>
		///     Represents a section of the quad list, with each quad using the same texture and rendering settings.
		/// </summary>
		private struct Section
		{
			/// <summary>
			///     The offset into the quad list. All quads from [Offset, Offset + _numQuads) use the same texture and rendering
			///     settings.
			/// </summary>
			public readonly int Offset;

			/// <summary>
			///     The index of the next section of the quad list using the same texture and rendering settings or -1 if this is the
			///     last
			///     section.
			/// </summary>
			public int Next;

			/// <summary>
			///     The number of quads in this section.
			/// </summary>
			public int NumQuads;

			/// <summary>
			///     Initializes the instance.
			/// </summary>
			/// <param name="offset">The index of the first quad of this section.</param>
			public Section(int offset)
			{
				Offset = offset;
				NumQuads = 0;
				Next = -1;
			}
		}

		#endregion

		#region Nested type: SectionList

		/// <summary>
		///     Represents a list of sections using the same texture and rendering settings.
		/// </summary>
		private struct SectionList
		{
			/// <summary>
			///     The blend state used by the sections.
			/// </summary>
			public readonly BlendState BlendState;

			/// <summary>
			///     The depth stencil state used by the sections.
			/// </summary>
			public readonly DepthStencilState DepthStencilState;

			/// <summary>
			///     The index of the first section of the list or -1 if there is none.
			/// </summary>
			public readonly int FirstSection;

			/// <summary>
			///     The layer of the section list.
			/// </summary>
			public readonly int Layer;

			/// <summary>
			///     The sampler state used by the sections.
			/// </summary>
			public readonly SamplerState SamplerState;

			/// <summary>
			///     The scissor area used by the sections.
			/// </summary>
			public readonly Rectangle ScissorArea;

			/// <summary>
			///     The texture used by the sections.
			/// </summary>
			public readonly Texture2D Texture;

			/// <summary>
			///     Indicates whether the scissor test should be enabled when drawing the sections.
			/// </summary>
			public readonly bool UseScissorTest;

			/// <summary>
			///     The world matrix used by the sections.
			/// </summary>
			public readonly Matrix WorldMatrix;

			/// <summary>
			///     The index of the last section of the list or -1 if there is none.
			/// </summary>
			public int LastSection;

			/// <summary>
			///     The total number of quads of the section list across all sections.
			/// </summary>
			public int NumQuads;

			/// <summary>
			///     Initializes the instance.
			/// </summary>
			/// <param name="blendState">The blend state used by the sections.</param>
			/// <param name="depthStencilState">The depth stencil state used by the sections.</param>
			/// <param name="samplerState">The sampler state used by the sections.</param>
			/// <param name="worldMatrix">The world matrix used by the sections.</param>
			/// <param name="texture">The texture used by the quads of this list.</param>
			/// <param name="scissorArea">The scissor area used by the sections.</param>
			/// <param name="useScissorTest">Indicates whether the scissor test should be enabled when drawing the sections.</param>
			/// <param name="layer">The section list's layer.</param>
			/// <param name="section">The index of the (one and only) section of the list.</param>
			public SectionList(BlendState blendState,
							   DepthStencilState depthStencilState,
							   SamplerState samplerState,
							   Matrix worldMatrix,
							   Texture2D texture,
							   Rectangle scissorArea,
							   bool useScissorTest,
							   int layer,
							   int section)
			{
				Texture = texture;
				FirstSection = section;
				WorldMatrix = worldMatrix;
				ScissorArea = scissorArea;
				UseScissorTest = useScissorTest;
				BlendState = blendState;
				DepthStencilState = depthStencilState;
				SamplerState = samplerState;
				LastSection = section;
				Layer = layer;

				NumQuads = 0;
			}

			/// <summary>
			///     Used to compare the layers of two section lists.
			/// </summary>
			public class LayerComparer : IComparer<SectionList>
			{
				/// <summary>
				///     The singleton comparer instance.
				/// </summary>
				public static readonly LayerComparer Instance = new LayerComparer();

				/// <summary>
				///     Compares two section list and returns a value indicating whether one belongs to a lower, the same, or greater layer
				///     than the other.
				/// </summary>
				/// <param name="x">The first section list to compare.</param>
				/// <param name="y">The second section list to compare.</param>
				public int Compare(SectionList x, SectionList y)
				{
					// ReSharper disable ImpureMethodCallOnReadonlyValueField
					return x.Layer.CompareTo(y.Layer);
					// ReSharper restore ImpureMethodCallOnReadonlyValueField
				}
			}
		}

		#endregion
	}
}