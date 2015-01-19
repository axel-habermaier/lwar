namespace Pegasus.Platform.Graphics.OpenGL3
{
	using System;
	using Bindings;
	using Interface;

	/// <summary>
	///     Describes an OpenGL3-based sampler state of a shader pipeline stage.
	/// </summary>
	internal unsafe class SamplerStateGL3 : GraphicsObjectGL3, ISamplerState
	{
		/// <summary>
		///     The native state object.
		/// </summary>
		private readonly uint _state;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="description">A description of the sampler state that should be created.</param>
		public SamplerStateGL3(GraphicsDeviceGL3 graphicsDevice, ref SamplerDescription description)
			: base(graphicsDevice)
		{
			_state = GLContext.Allocate(_gl.GenSamplers, "Sampler state");

			var borderColor = stackalloc float[4];
			description.BorderColor.ToFloatArray(borderColor);

			_gl.SamplerParameteri(_state, GL.TextureMinFilter, (int)description.Filter.GetMinFilter());
			_gl.SamplerParameteri(_state, GL.TextureMagFilter, (int)description.Filter.GetMagFilter());
			_gl.SamplerParameteri(_state, GL.TextureWrapS, (int)description.AddressU.Map());
			_gl.SamplerParameteri(_state, GL.TextureWrapT, (int)description.AddressV.Map());
			_gl.SamplerParameteri(_state, GL.TextureWrapR, (int)description.AddressW.Map());
			_gl.SamplerParameteri(_state, GL.TextureMaxAnisotropyExt, description.MaximumAnisotropy);
			_gl.SamplerParameterfv(_state, GL.TextureBorderColor, borderColor);
			_gl.SamplerParameterf(_state, GL.TextureMinLod, description.MinimumLod);
			_gl.SamplerParameterf(_state, GL.TextureMaxLod, description.MaximumLod);
			_gl.SamplerParameterf(_state, GL.TextureLodBias, description.MipLodBias);
			_gl.SamplerParameteri(_state, GL.TextureCompareFunc, (int)description.Comparison.Map());
		}

		/// <summary>
		///     Sets the debug name of the state.
		/// </summary>
		/// <param name="name">The debug name of the state.</param>
		public void SetName(string name)
		{
			// Not supported by OpenGL
		}

		/// <summary>
		///     Binds the state on the given slot.
		/// </summary>
		/// <param name="slot">The slot the sampler state should be bound to.</param>
		public void Bind(int slot)
		{
			_gl.BindSampler((uint)slot, _state);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			GLContext.Deallocate(_gl.DeleteSamplers, _state);
		}
	}
}