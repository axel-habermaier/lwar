using System;

namespace Pegasus.Framework.Rendering
{
	using System.Runtime.InteropServices;
	using Math;
	using Platform.Graphics;
	using Platform.Logging;
	using Platform.Memory;
	using Math = System.Math;

	/// <summary>
	///   Efficiently draws large amounts of 2D sprites by batching together quads with the same texture.
	/// </summary>
	public sealed class SpriteBatch : DisposableObject
	{
		/// <summary>
		///   The maximum number of quads that can be queued.
		/// </summary>
		private const int MaxQuads = 8192;

		/// <summary>
		///   The effect that is used to draw the sprites.
		/// </summary>
		private readonly ISpriteEffect _effect;

		/// <summary>
		///   The graphics device that should be used to draw the sprites.
		/// </summary>
		private readonly GraphicsDevice _graphicsDevice;

		/// <summary>
		///   The index buffer that is used for drawing.
		/// </summary>
		private readonly IndexBuffer _indexBuffer;

		/// <summary>
		///   The size of a single quad in bytes.
		/// </summary>
		private readonly int _quadSize = Marshal.SizeOf(typeof(Quad));

		/// <summary>
		///   The list of all quads.
		/// </summary>
		private readonly Quad[] _quads = new Quad[MaxQuads];

		/// <summary>
		///   Rasterizer state for sprite batch rendering with active scissor test.
		/// </summary>
		private readonly RasterizerState _scissorRasterizerState;

		/// <summary>
		///   The vertex buffer that is used for drawing.
		/// </summary>
		private readonly VertexBuffer _vertexBuffer;

		/// <summary>
		///   The vertex input layout used by the sprite batch.
		/// </summary>
		private readonly VertexInputLayout _vertexLayout;

		/// <summary>
		///   The blend state that should be used for drawing.
		/// </summary>
		private BlendState _blendState;

		/// <summary>
		///   The index of the section that is currently in use.
		/// </summary>
		private int _currentSection;

		/// <summary>
		///   The texture of the last section that has been added to the list.
		/// </summary>
		private Texture2D _currentTexture;

		/// <summary>
		///   The depth stencil state that should be used for drawing.
		/// </summary>
		private DepthStencilState _depthStencilState;

		/// <summary>
		///   The number of quads that are currently queued.
		/// </summary>
		private int _numQuads;

		/// <summary>
		///   The number of section lists that are currently used.
		/// </summary>
		private int _numSectionLists;

		/// <summary>
		///   The number of sections that are currently used.
		/// </summary>
		private int _numSections;

		/// <summary>
		///   The output that should be used for drawing.
		/// </summary>
		private RenderOutput _output;

		/// <summary>
		///   The sampler state that should be used for drawing.
		/// </summary>
		private SamplerState _samplerState;

		/// <summary>
		///   The rectangle that should be used for the scissor test.
		/// </summary>
		private Rectangle _scissorArea;

		/// <summary>
		///   A mapping from a texture to its corresponding section list.
		/// </summary>
		private SectionList[] _sectionLists = new SectionList[4];

		/// <summary>
		///   The list of all sections.
		/// </summary>
		private Section[] _sections = new Section[16];

		/// <summary>
		///   Indicates whether a scissor test should be performed during rendering.
		/// </summary>
		private bool _useScissorTest;

		/// <summary>
		///   The current world matrix used by the sprite batch.
		/// </summary>
		private Matrix _worldMatrix = Matrix.Identity;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used for drawing.</param>
		/// <param name="effect">The effect that should be used to draw the sprites.</param>
		public SpriteBatch(GraphicsDevice graphicsDevice, ISpriteEffect effect)
		{
			Assert.ArgumentNotNull(graphicsDevice);
			Assert.ArgumentNotNull(effect);

			_graphicsDevice = graphicsDevice;
			_effect = effect;

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
			_vertexBuffer = VertexBuffer.Create<Quad>(graphicsDevice, MaxQuads, ResourceUsage.Dynamic);
			_indexBuffer = IndexBuffer.Create(graphicsDevice, indices);
			_vertexLayout = VertexPositionColorTexture.GetInputLayout(graphicsDevice, _vertexBuffer, _indexBuffer);

			_scissorRasterizerState = new RasterizerState(graphicsDevice)
			{
				CullMode = CullMode.None,
				ScissorEnabled = true
			};
			_scissorRasterizerState.SetName("SpriteBatch.Scissor");
		}

		/// <summary>
		///   Gets or sets the output that should be used for drawing.
		/// </summary>
		public RenderOutput Output
		{
			get { return _output; }
			set
			{
				Assert.ArgumentNotNull(value);
				Assert.NotDisposed(this);

				if (_output == value)
					return;

				if (_output != null)
					DrawBatch();

				_output = value;
			}
		}

		/// <summary>
		///   Gets or sets the sampler state that should be used for drawing.
		/// </summary>
		public SamplerState SamplerState
		{
			get { return _samplerState; }
			set
			{
				Assert.ArgumentNotNull(value);
				Assert.NotDisposed(this);

				if (_samplerState == value)
					return;

				if (_samplerState != null)
					DrawBatch();

				_samplerState = value;
			}
		}

		/// <summary>
		///   Gets or sets the depth stencil state that should be used for drawing.
		/// </summary>
		public DepthStencilState DepthStencilState
		{
			get { return _depthStencilState; }
			set
			{
				Assert.ArgumentNotNull(value);
				Assert.NotDisposed(this);

				if (_depthStencilState == value)
					return;

				if (_depthStencilState != null)
					DrawBatch();

				_depthStencilState = value;
			}
		}

		/// <summary>
		///   Gets or sets the blend state that should be used for drawing.
		/// </summary>
		public BlendState BlendState
		{
			get { return _blendState; }
			set
			{
				Assert.ArgumentNotNull(value);
				Assert.NotDisposed(this);

				if (_blendState == value)
					return;

				if (_blendState != null)
					DrawBatch();

				_blendState = value;
			}
		}

		/// <summary>
		///   Gets or sets the scissor area that should be used for the scissor test. All batched sprites are drawn before the area
		///   is changed.
		/// </summary>
		public Rectangle ScissorArea
		{
			get { return _scissorArea; }
			set
			{
				Assert.NotDisposed(this);

				if (_scissorArea == value)
					return;

				DrawBatch();
				_scissorArea = value;
			}
		}

		/// <summary>
		///   Gets or sets a value indicating whether a scissor test should be performed during rendering. All batched sprites are
		///   drawn before the scissor test is enabled or disabled.
		/// </summary>
		public bool UseScissorTest
		{
			get { return _useScissorTest; }
			set
			{
				Assert.NotDisposed(this);

				if (_useScissorTest == value)
					return;

				DrawBatch();
				_useScissorTest = value;
			}
		}

		/// <summary>
		///   Gets or sets the world matrix used by the sprite batch. All batched sprites are drawn before the world matrix is
		///   changed.
		/// </summary>
		public Matrix WorldMatrix
		{
			get
			{
				Assert.NotDisposed(this);
				return _worldMatrix;
			}
			set
			{
				Assert.NotDisposed(this);

				DrawBatch();
				_worldMatrix = value;
			}
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_vertexBuffer.SafeDispose();
			_indexBuffer.SafeDispose();
			_vertexLayout.SafeDispose();
			_scissorRasterizerState.SafeDispose();
		}

		/// <summary>
		///   Draws the given rectangle.
		/// </summary>
		/// <param name="rectangle">The rectangle that should be drawn.</param>
		/// <param name="texture">The texture that should be used to draw the quad.</param>
		/// <param name="color">The color of the quad.</param>
		public void Draw(Rectangle rectangle, Texture2D texture, Color color)
		{
			Draw(new RectangleF(rectangle.Left, rectangle.Top, rectangle.Width, rectangle.Height), texture, color);
		}

		/// <summary>
		///   Draws the given rectangle.
		/// </summary>
		/// <param name="rectangle">The rectangle that should be drawn.</param>
		/// <param name="texture">The texture that should be used to draw the quad.</param>
		/// <param name="color">The color of the quad.</param>
		public void Draw(RectangleF rectangle, Texture2D texture, Color color)
		{
			var quad = new Quad(rectangle, color);
			Draw(ref quad, texture);
		}

		/// <summary>
		///   Draws the given rectangle.
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

			Quad.Transform(ref quad, ref rotationMatrix);
			Quad.Offset(ref quad, ref unrotatedPosition);

			Draw(ref quad, texture);
		}

		/// <summary>
		///   Draws a textured rectangle at the given position with the texture's size.
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
		///   Draws a textured rectangle at the given position with the texture's size and rotation.
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
		///   Draws the given quad.
		/// </summary>
		/// <param name="quad">The quad that should be added.</param>
		/// <param name="texture">The texture that should be used to draw the quad.</param>
		internal void Draw(ref Quad quad, Texture2D texture)
		{
			Assert.NotDisposed(this);
			Assert.ArgumentNotNull(texture);

			DrawBatchIfNecessary(1);
			ChangeTexture(texture);

			Assert.That(_numQuads < MaxQuads, "Sprite batch quads overflow.");

			// Add the quad to the list
			_quads[_numQuads++] = quad;
			++_sections[_currentSection].NumQuads;
		}

		/// <summary>
		///   Draws the outline of a rectangle.
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
		///   Draws the outline of a circle.
		/// </summary>
		/// <param name="circle">The circle that should be drawn.</param>
		/// <param name="color">The color of the outline.</param>
		public void DrawOutline(CircleF circle, Color color)
		{
			// Work out the minimum step necessary using trigonometry + sine approximation.
			var step = 10.0f / circle.Radius;
			for (var angle = 0.0f; angle < Math.PI * 2; angle += step)
			{
				var x = (float)Math.Round(circle.Radius * Math.Cos(angle));
				var y = (float)Math.Round(circle.Radius * Math.Sin(angle));

				Draw(new RectangleF(circle.Position.X + x, circle.Position.Y + y, 1, 1), Texture2D.White, color);
			}
		}

		/// <summary>
		///   Draws a line.
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
		///   Draws the given quads.
		/// </summary>
		/// <param name="quads">The quads that should be added.</param>
		/// <param name="count">The number of quads to draw.</param>
		/// <param name="texture">The texture that should be used to draw the quads.</param>
		internal void Draw(Quad[] quads, int count, Texture2D texture)
		{
			Assert.NotDisposed(this);
			Assert.ArgumentNotNull(quads);
			Assert.ArgumentNotNull(texture);
			Assert.ArgumentSatisfies(count >= 0, "Out of bounds.");
			Assert.ArgumentSatisfies(count <= quads.Length, "Out of bounds.");

			if (count == 0)
				return;

			DrawBatchIfNecessary(count);
			ChangeTexture(texture);

			Assert.That(_numQuads + count < MaxQuads, "Sprite batch quads overflow.");

			// Add the quads to the list and update the quad counts
			Array.Copy(quads, 0, _quads, _numQuads, count);
			_numQuads += count;
			_sections[_currentSection].NumQuads += count;
		}

		/// <summary>
		///   Draws all batched sprites.
		/// </summary>
		public void DrawBatch()
		{
			Assert.That(Output != null, "No output has been set.");
			Assert.That(BlendState != null, "No blend state has been set.");
			Assert.That(DepthStencilState != null, "No depth stencil state has been set.");
			Assert.That(SamplerState != null, "No sampler state has been set.");
			Assert.NotDisposed(this);

			// Quit early if there's nothing to draw
			if (_numQuads == 0)
				return;

			// Prepare the graphics pipeline
			_effect.World = _worldMatrix;
			_blendState.Bind();
			_depthStencilState.Bind();

			_vertexLayout.Bind();
			_graphicsDevice.PrimitiveType = PrimitiveType.Triangles;

			if (!UseScissorTest)
				RasterizerState.CullNone.Bind();
			else
			{
				_scissorRasterizerState.Bind();
				Output.ScissorArea = ScissorArea;
			}

			// Prepare the vertex buffer
			UpdateVertexBuffer();

			// Finally, draw the quads
			var offset = 0;
			for (var i = 0; i < _numSectionLists; ++i)
			{
				// Bind the texture
				_effect.Sprite = new Texture2DView(_sectionLists[i].Texture, _samplerState);

				// Draw and increase the offset
				var numIndices = _sectionLists[i].NumQuads * 6;
				Output.DrawIndexed(_effect.Technique, numIndices, offset);
				offset += numIndices;
			}

			// Reset the internal state
			_numQuads = 0;
			_numSections = 0;
			_numSectionLists = 0;
			_currentTexture = null;
			_currentSection = -1;
		}

		/// <summary>
		///   Draws the batched quads even though the user hasn't explicitly requested it if adding the given
		///   quad batch with the given texture would overflow the internal quad buffer.
		/// </summary>
		/// <param name="quadCount">The additional quads that should be drawn.</param>
		private void DrawBatchIfNecessary(int quadCount)
		{
			Assert.ArgumentInRange(quadCount, 0, MaxQuads);

			// Check whether we would overflow if we added the given batch.
			var tooManyQuads = _numQuads + quadCount >= _quads.Length;
			if (tooManyQuads)
			{
				Log.DebugInfo(LogCategory.Graphics,
							  "Sprite batch buffer overflow: {0} out of {1} allocated quads in use (could not add {2} quad(s)).",
							  _numQuads, MaxQuads, quadCount);

				DrawBatch();
			}
		}

		/// <summary>
		///   Copies the quads to the vertex buffer, sorted by texture.
		/// </summary>
		private unsafe void UpdateVertexBuffer()
		{
			var vertexData = _vertexBuffer.Map(MapMode.WriteDiscard);
			var offset = 0;

			fixed (Quad* quadsPtr = &_quads[0])
			{
				var quads = new IntPtr(quadsPtr);
				for (var i = 0; i < _numSectionLists; ++i)
				{
					var section = _sectionLists[i].First;
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
		///   Checks whether the current texture has to be changed, and if so, creates a new section and possibly section list.
		/// </summary>
		/// <param name="texture">The texture that should be used for newly added quads.</param>
		private void ChangeTexture(Texture2D texture)
		{
			// If the texture hasn't changed, just continue to append to the current section
			if (texture == _currentTexture)
				return;

			// Add a new section
			AddSection();

			// Depending on whether we've already seen this texture, add it to the map or add a new section list
			var known = false;
			for (int i = 0; i < _numSectionLists; ++i) // Would a Dictionary be more efficient?
			{
				if (_sectionLists[i].Texture == texture)
				{
					// We've already seen this texture before, so add the new section to the list by setting the
					// list's tail section's next pointer to the newly allocated section
					_sections[_sectionLists[i].Last].Next = _numSections;
					// Set the section list's tail pointer to the newly allocated section
					_sectionLists[i].Last = _numSections;
					known = true;
					break;
				}
			}

			if (!known)
				AddSectionList(new SectionList(texture, _numSections));

			// Update the cached values
			_currentTexture = texture;
			_currentSection = _numSections;

			// Mark the newly allocated section as allocated
			++_numSections;
		}

		/// <summary>
		///   Adds a new section, allocating more space for the new section if required.
		/// </summary>
		private void AddSection()
		{
			if (_numSections >= _sections.Length)
			{
				var sections = new Section[_sections.Length * 2];
				Array.Copy(sections, _sections, _sections.Length);
				_sections = sections;
			}

			_sections[_numSections] = new Section(_numQuads);
		}

		/// <summary>
		///   Adds the given section list, allocating more space for the new section list if required.
		/// </summary>
		/// <param name="sectionList">The section list that should be added.</param>
		private void AddSectionList(SectionList sectionList)
		{
			if (_numSectionLists >= _sectionLists.Length)
			{
				var sectionLists = new SectionList[_sectionLists.Length * 2];
				Array.Copy(sectionLists, _sectionLists, _sectionLists.Length);
				_sectionLists = sectionLists;
			}

			_sectionLists[_numSectionLists++] = sectionList;
		}

		#region Nested type: Section

		/// <summary>
		///   Represents a section of the quad list, with each quad using the same texture.
		/// </summary>
		private struct Section
		{
			/// <summary>
			///   The offset into the quad list. All quads from [Offset, Offset + _numQuads) use the same texture.
			/// </summary>
			public readonly int Offset;

			/// <summary>
			///   The index of the next section of the quad list using the same texture or -1 if this is the last
			///   section.
			/// </summary>
			public int Next;

			/// <summary>
			///   The number of quads in this section.
			/// </summary>
			public int NumQuads;

			/// <summary>
			///   Initializes the instance.
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
		///   Represents a list of sections using the same texture.
		/// </summary>
		private struct SectionList
		{
			/// <summary>
			///   The index of the first section of the list or -1 if there is none.
			/// </summary>
			public readonly int First;

			/// <summary>
			///   The texture used by the sections.
			/// </summary>
			public readonly Texture2D Texture;

			/// <summary>
			///   The index of the last section of the list or -1 if there is none.
			/// </summary>
			public int Last;

			/// <summary>
			///   The total number of quads of the section list across all sections.
			/// </summary>
			public int NumQuads;

			/// <summary>
			///   Initializes the instance.
			/// </summary>
			/// <param name="texture">The texture used by the quads of this list.</param>
			/// <param name="section">The index of the (one and only) section of the list.</param>
			public SectionList(Texture2D texture, int section)
			{
				Texture = texture;
				First = section;
				Last = section;
				NumQuads = 0;
			}
		}

		#endregion
	}
}