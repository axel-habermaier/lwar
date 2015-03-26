namespace Pegasus.Platform.Graphics
{
	using System;
	using System.Linq;
	using Utilities;

	/// <summary>
	///     Represents a description of the memory layout and other properties of the vertex data that is fed into the
	///     input-assembler stage of the graphics pipeline.
	/// </summary>
	public sealed unsafe class VertexLayout : GraphicsObject
	{
		/// <summary>
		///     Initializes a new instance without an index buffer binding.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="vertexBindings">The bindings for the vertex inputs that should belong to the input layout.</param>
		public VertexLayout(GraphicsDevice graphicsDevice, params VertexBinding[] vertexBindings)
			: this(graphicsDevice, null, 0, vertexBindings)
		{
		}

		/// <summary>
		///     Initializes a new instance with an index buffer binding.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="indexBuffer">The input index buffer that should be used by the input layout.</param>
		/// <param name="vertexBindings">The bindings for the vertex inputs that should belong to the input layout.</param>
		public VertexLayout(GraphicsDevice graphicsDevice, IndexBuffer indexBuffer, params VertexBinding[] vertexBindings)
			: this(graphicsDevice, indexBuffer, 0, vertexBindings)
		{
		}

		/// <summary>
		///     Initializes a new instance with an index buffer binding.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="indexBuffer">The input index buffer that should be used by the input layout.</param>
		/// <param name="indexOffset">The offset into the index buffer.</param>
		/// <param name="vertexBindings">The bindings for the vertex inputs that should belong to the input layout.</param>
		public VertexLayout(GraphicsDevice graphicsDevice, IndexBuffer indexBuffer, int indexOffset, params VertexBinding[] vertexBindings)
			: base(graphicsDevice)
		{
			Assert.ArgumentNotNull(vertexBindings);
			Assert.ArgumentInRange(indexOffset, 0, GraphicsDevice.MaxVertexBindings);
			Assert.That(!vertexBindings.GroupBy(e => e.Semantics).Select((s, _) => s.Count()).Any(c => c > 1),
				"The list of vertex input elements contains at least two elements with the same value set for the Semantics property.");

			var signature = ShaderSignature.GetShaderSignature(vertexBindings);
			fixed (byte* signaturePtr = signature)
			fixed (VertexBinding* bindings = vertexBindings)
			{
				var description = new VertexLayoutDescription
				{
					Bindings = bindings,
					BindingsCount = vertexBindings.Length,
					IndexBuffer = indexBuffer != null ? indexBuffer.NativeObject : null,
					IndexOffset = indexOffset,
					IndexSize = indexBuffer != null ? indexBuffer.IndexSize : 0,
					Signature = signaturePtr,
					SignatureLength = signature.Length
				};
				NativeObject = DeviceInterface->InitializeVertexLayout(&description);
			}
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			DeviceInterface->FreeVertexLayout(NativeObject);
		}

		/// <summary>
		///     Binds the input layout to the input-assembler stage of the pipeline.
		/// </summary>
		public void Bind()
		{
			if (DeviceState.Change(ref DeviceState.VertexLayout, this))
				DeviceInterface->BindVertexLayout(NativeObject);
		}
	}
}