namespace Pegasus.Platform.Graphics.OpenGL3
{
	using System;
	using Bindings;
	using Interface;
	using Utilities;

	/// <summary>
	///     Represents a description of the memory layout and other properties of the vertex data that is fed into the
	///     input-assembler stage of the graphics pipeline.
	/// </summary>
	internal unsafe class VertexLayoutGL3 : GraphicsObjectGL3, IVertexLayout
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device this instance belongs to.</param>
		/// <param name="description">A description of the vertex layout.</param>
		public VertexLayoutGL3(GraphicsDeviceGL3 graphicsDevice, ref VertexLayoutDescription description)
			: base(graphicsDevice)
		{
			//NativeObject = GLContext.Allocate(_gl.GenVertexArrays, "VertexLayout");
			//_gl.BindVertexArray(NativeObject);

			//if (description.IndexBuffer != null)
			//{
			//	var handle = ((BufferGL3)description.IndexBuffer.BufferObject).Handle;
			//	_gl.BindBuffer(GL.ElementArrayBuffer, handle);

			//	IndexOffset = description.IndexOffset;
			//	IndexType = description.IndexBuffer.IndexSize.Map();
			//	IndexSizeInBytes = description.IndexBuffer.IndexSize.GetIndexSizeInBytes();
			//}

			//for (var i = 0; i < description.Bindings.Length; ++i)
			//{
			//	var type = description.Bindings[i].Format.GetVertexDataType();
			//	var size = description.Bindings[i].Format.GetVertexDataComponentCount();
			//	var slot = (uint)description.Bindings[i].Semantics;

			//	Assert.NotNull(description.Bindings[i].VertexBuffer);

			//	var handle = ((BufferGL3)description.Bindings[i].VertexBuffer.BufferObject).Handle;
			//	_gl.BindBuffer(GL.ArrayBuffer, handle);
			//	_gl.EnableVertexAttribArray(slot);

			//	var normalize = description.Bindings[i].Semantics == DataSemantics.Color0 ||
			//					description.Bindings[i].Semantics == DataSemantics.Color1 ||
			//					description.Bindings[i].Semantics == DataSemantics.Color2 ||
			//					description.Bindings[i].Semantics == DataSemantics.Color3;

			//	_gl.VertexAttribPointer(slot, size, type, normalize, description.Bindings[i].Stride, (void*)description.Bindings[i].Offset);
			//	_gl.VertexAttribDivisor(slot, description.Bindings[i].StepRate);
			//}

			//_gl.BindVertexArray(0);
		}

		/// <summary>
		///     Gets the native vertex layout.
		/// </summary>
		internal uint NativeObject { get; private set; }

		/// <summary>
		///     Gets the offset into the index buffer.
		/// </summary>
		internal int IndexOffset { get; private set; }

		/// <summary>
		///     Gets the type of the indices.
		/// </summary>
		internal uint IndexType { get; private set; }

		/// <summary>
		///     Gets the size of the indices in bytes.
		/// </summary>
		internal int IndexSizeInBytes { get; private set; }

		/// <summary>
		///     Binds the input layout to the input-assembler stage of the pipeline.
		/// </summary>
		public void Bind()
		{
			// Do not actually bind the input layout here, as that causes all sorts of problems with buffer updates between
			// the binding of the input layout and the actual draw call using the input layout
			DeviceState.VertexLayout = this;
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			GLContext.Deallocate(_gl.DeleteVertexArrays, NativeObject);
		}
	}
}