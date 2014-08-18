namespace Pegasus.Platform.Graphics
{
	using System;

	/// <summary>
	///     Represents a combination of shaders that can be set on the GPU to create a rendering effect.
	/// </summary>
	public struct EffectTechnique
	{
		/// <summary>
		///     The action that must be invoked to bind the required textures and constant buffers.
		/// </summary>
		private readonly Action _bind;

		/// <summary>
		///     The shader program that is used by the technique.
		/// </summary>
		private readonly ShaderProgram _shaderProgram;

		/// <summary>
		///     The action that must be invoked to unbind the required textures.
		/// </summary>
		private readonly Action _unbind;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="shaderProgram">The shader program that should be used by the technique.</param>
		/// <param name="bind">The action that should be invoked to bind the required textures and constant buffers.</param>
		/// <param name="unbind">The action that should be invoked to unbind the required textures.</param>
		internal EffectTechnique(ShaderProgram shaderProgram, Action bind, Action unbind)
		{
			Assert.ArgumentNotNull(shaderProgram);
			Assert.ArgumentNotNull(bind);
			Assert.ArgumentNotNull(unbind);

			_shaderProgram = shaderProgram;
			_bind = bind;
			_unbind = unbind;
		}

		/// <summary>
		///     Binds the shaders, textures, and constant buffers used by the technique.
		/// </summary>
		internal void Bind()
		{
			Assert.NotNull(_shaderProgram, "No ShaderProgram has been set.");
			Assert.NotNull(_bind, "No bind action has been set.");

			_bind();
			_shaderProgram.Bind();
		}

		/// <summary>
		///     Unbinds the textures used by the technique.
		/// </summary>
		internal void Unbind()
		{
			Assert.NotNull(_unbind, "No unbind action has been set.");
			_unbind();
		}
	}
}