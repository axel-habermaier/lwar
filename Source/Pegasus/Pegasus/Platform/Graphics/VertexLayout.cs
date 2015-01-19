namespace Pegasus.Platform.Graphics
{
	using System;
	using System.Linq;
	using Interface;
	using Memory;
	using Utilities;

	/// <summary>
	///     Represents a description of the memory layout and other properties of the vertex data that is fed into the
	///     input-assembler stage of the graphics pipeline.
	/// </summary>
	public sealed class VertexLayout : GraphicsObject
	{
		/// <summary>
		///     The underlying vertex layout object.
		/// </summary>
		private readonly IVertexLayout _vertexLayout;

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

			var shaderSignature = ShaderSignature.GetShaderSignature(vertexBindings);
			indexOffset = indexBuffer == null ? 0 : indexOffset;

			var description = new VertexLayoutDescription
			{
				IndexBuffer = indexBuffer,
				IndexOffset = indexOffset,
				Bindings = vertexBindings,
				ShaderSignature = shaderSignature
			};

			_vertexLayout = graphicsDevice.CreateVertexLayout(ref description);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_vertexLayout.SafeDispose();
		}

		/// <summary>
		///     Binds the input layout to the input-assembler stage of the pipeline.
		/// </summary>
		public void Bind()
		{
			if (DeviceState.Change(ref GraphicsDevice.State.VertexLayout, this))
				_vertexLayout.Bind();
		}
	}
}