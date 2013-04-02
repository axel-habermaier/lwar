using System;

namespace Pegasus.Framework.Platform.Graphics
{
	/// <summary>
	///   Represents a combination of shaders that are currently set on the GPU to create a rendering effect.
	/// </summary>
	public struct EffectTechnique
	{
		/// <summary>
		///   The action that must be invoked to bind the required textures and constant buffers.
		/// </summary>
		private readonly Action _bind;

		/// <summary>
		///   The fragment shader that is used by the technique.
		/// </summary>
		private readonly FragmentShader _fragmentShader;

		/// <summary>
		///   The vertex shader that is used by the technique.
		/// </summary>
		private readonly VertexShader _vertexShader;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="vertexShader">The vertex shader that should be used by the technique.</param>
		/// <param name="fragmentShader">The fragment shader that should be used by the technique.</param>
		/// <param name="bind">The action that should be invoked to bind the required textures and constant buffers.</param>
		internal EffectTechnique(VertexShader vertexShader, FragmentShader fragmentShader, Action bind)
		{
			Assert.ArgumentNotNull(vertexShader, () => vertexShader);
			Assert.ArgumentNotNull(fragmentShader, () => fragmentShader);
			Assert.ArgumentNotNull(bind, () => bind);

			_vertexShader = vertexShader;
			_fragmentShader = fragmentShader;
			_bind = bind;
		}

		/// <summary>
		///   Binds the shaders used by the technique.
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
	}
}