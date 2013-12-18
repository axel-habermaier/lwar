namespace Pegasus.Platform.Graphics
{
	using System;

	/// <summary>
	///     Represents a combination of shaders that are currently set on the GPU to create a rendering effect.
	/// </summary>
	public struct EffectTechnique
	{
		/// <summary>
		///     The action that must be invoked to bind the required textures and constant buffers.
		/// </summary>
		private readonly Action _bind;

		/// <summary>
		///     The fragment shader that is used by the technique.
		/// </summary>
		private readonly FragmentShader _fragmentShader;

		/// <summary>
		///     The action that must be invoked to unbind the required textures.
		/// </summary>
		private readonly Action _unbind;

		/// <summary>
		///     The vertex shader that is used by the technique.
		/// </summary>
		private readonly VertexShader _vertexShader;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="vertexShader">The vertex shader that should be used by the technique.</param>
		/// <param name="fragmentShader">The fragment shader that should be used by the technique.</param>
		/// <param name="bind">The action that should be invoked to bind the required textures and constant buffers.</param>
		/// <param name="unbind">The action that should be invoked to unbind the required textures.</param>
		internal EffectTechnique(VertexShader vertexShader, FragmentShader fragmentShader, Action bind, Action unbind)
		{
			Assert.ArgumentNotNull(vertexShader);
			Assert.ArgumentNotNull(fragmentShader);
			Assert.ArgumentNotNull(bind);
			Assert.ArgumentNotNull(unbind);

			_vertexShader = vertexShader;
			_fragmentShader = fragmentShader;
			_bind = bind;
			_unbind = unbind;
		}

		/// <summary>
		///     Binds the shaders used by the technique.
		/// </summary>
		internal void Bind()
		{
			Assert.NotNull(_vertexShader, "No vertex shader has been set.");
			Assert.NotNull(_fragmentShader, "No fragment shader has been set.");
			Assert.NotNull(_bind, "No bind action has been set.");

			_bind();
			_vertexShader.Bind();
			_fragmentShader.Bind();
		}

		/// <summary>
		///     Unbinds the textures used by the technique.
		/// </summary>
		internal void Unbind()
		{
			_unbind();
		}
	}
}