namespace Pegasus.Platform.Graphics.Direct3D11
{
	using System;
	using System.Runtime.InteropServices;
	using Bindings;
	using Interface;

	/// <summary>
	///     Represents a description of the memory layout and other properties of the vertex data that is fed into the
	///     input-assembler stage of the graphics pipeline.
	/// </summary>
	internal class VertexLayoutD3D11 : GraphicsObjectD3D11, IVertexLayout
	{
		/// <summary>
		///     The vertex bindings.
		/// </summary>
		private readonly VertexElement[] _bindings;

		/// <summary>
		///     The underlying Direct3D11 index buffer.
		/// </summary>
		private readonly D3D11Buffer _indexBuffer;

		/// <summary>
		///     The format of the indices.
		/// </summary>
		private readonly DXGIFormat _indexFormat;

		/// <summary>
		///     The offset into the index buffer.
		/// </summary>
		private readonly int _indexOffset;

		/// <summary>
		///     The underlying Direct3D11 input layout.
		/// </summary>
		private D3D11InputLayout _layout;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device this instance belongs to.</param>
		/// <param name="description">A description of the vertex layout.</param>
		public unsafe VertexLayoutD3D11(GraphicsDeviceD3D11 graphicsDevice, ref VertexLayoutDescription description)
			: base(graphicsDevice)
		{
			if (description.IndexBuffer != null)
			{
				_indexBuffer = ((BufferD3D11)description.IndexBuffer.BufferObject).Buffer;
				_indexFormat = description.IndexBuffer.IndexSize.Map();
			}

			_indexOffset = description.IndexOffset;
			_bindings = new VertexElement[description.Bindings.Length];

			var inputElements = new D3D11InputElement[description.Bindings.Length];
			for (var i = 0u; i < description.Bindings.Length; ++i)
			{
				inputElements[i].AlignedByteOffset = 0;
				inputElements[i].Classification = description.Bindings[i].StepRate == 0
					? D3D11InputClassification.PerVertexData
					: D3D11InputClassification.PerInstanceData;
				inputElements[i].InstanceDataStepRate = (int)description.Bindings[i].StepRate;
				inputElements[i].Format = description.Bindings[i].Format.Map();
				inputElements[i].SemanticIndex = description.Bindings[i].Semantics.GetSemanticIndex();
				inputElements[i].SemanticName = Marshal.StringToHGlobalAnsi(description.Bindings[i].Semantics.GetSemanticName());
				inputElements[i].Slot = (int)description.Bindings[i].Semantics;

				_bindings[i] = new VertexElement
				{
					Slot = (int)description.Bindings[i].Semantics,
					Buffer = ((BufferD3D11)description.Bindings[i].VertexBuffer.BufferObject).Buffer,
					Offset = description.Bindings[i].Offset,
					Stride = description.Bindings[i].Stride
				};
			}

			fixed (D3D11InputElement* inputs = inputElements)
			fixed (byte* signatureShader = description.ShaderSignature)
			{
				Device.CreateInputLayout(inputs, description.Bindings.Length, signatureShader, description.ShaderSignature.Length, out _layout)
					  .CheckSuccess("Failed to create input layout.");
			}

			foreach (var binding in inputElements)
				Marshal.FreeHGlobal(binding.SemanticName);
		}

		/// <summary>
		///     Binds the input layout to the input-assembler stage of the pipeline.
		/// </summary>
		public unsafe void Bind()
		{
			Context.IASetInputLayout(_layout);

			for (var i = 0; i < _bindings.Length; ++i)
			{
				var binding = _bindings[i];
				Context.IASetVertexBuffers(binding.Slot, 1, &binding.Buffer, &binding.Stride, &binding.Offset);
			}

			if (_indexBuffer.IsInitialized)
				Context.IASetIndexBuffer(_indexBuffer, _indexFormat, _indexOffset);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_layout.Release();
		}

		/// <summary>
		///     Represents a Direct3D11 vertex binding.
		/// </summary>
		private struct VertexElement
		{
			/// <summary>
			///     The vertex buffer.
			/// </summary>
			public D3D11Buffer Buffer;

			/// <summary>
			///     The offset into the vertex buffer.
			/// </summary>
			public int Offset;

			/// <summary>
			///     The slot of the vertex binding.
			/// </summary>
			public int Slot;

			/// <summary>
			///     The stride within the vertex buffer.
			/// </summary>
			public int Stride;
		}
	}
}