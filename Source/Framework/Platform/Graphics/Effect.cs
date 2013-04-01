using System;

namespace Pegasus.Framework.Platform.Graphics
{
	using Assets;

	/// <summary>
	///   Represents a shader-based rendering effect.
	/// </summary>
	public abstract class Effect : GraphicsObject
	{
		/// <summary>
		///   A value indicating whether a technique of the effect is currently active.
		/// </summary>
		private bool _isActive;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device this instance belongs to.</param>
		protected Effect(GraphicsDevice graphicsDevice)
			: base(graphicsDevice)
		{
		}

		/// <summary>
		///   Gets a value indicating whether a technique of the effect is currently active.
		/// </summary>
		public bool IsActive
		{
			get { return _isActive; }
			internal set
			{
				Assert.That(!value || !_isActive, "The effect is already active.");
				Assert.That(value || _isActive, "The effect is not active.");

				_isActive = value;

				if (_isActive)
					GraphicsDevice.ActiveEffect = this;
				else
					GraphicsDevice.ActiveEffect = null;
			}
		}

		/// <summary>
		///   Creates a texture binding for the given texture and sampler state.
		/// </summary>
		/// <typeparam name="T">The type of the texture that should be bound.</typeparam>
		/// <param name="texture">The texture that should be bound.</param>
		/// <param name="sampler">The sampler state that should be bound.</param>
		public static TextureBinding<T> Bind<T>(T texture, SamplerState sampler)
			where T : Texture
		{
			Assert.ArgumentNotNull(texture, () => texture);
			Assert.ArgumentNotNull(sampler, () => sampler);

			return new TextureBinding<T>(texture, sampler);
		}

		/// <summary>
		///   Binds the given texture binding to the GPU.
		/// </summary>
		/// <typeparam name="T">The type of the texture that should be bound.</typeparam>
		/// <param name="binding">The texture and sampler state that should be bound.</param>
		/// <param name="slot">The slot the texture and sampler state should be bound to.</param>
		protected static void Bind<T>(TextureBinding<T> binding, int slot)
			where T : Texture
		{
			binding.Bind(slot);
		}

		/// <summary>
		///   Loads a vertex shader.
		/// </summary>
		/// <param name="assets">The assets manager that should be used to load the shader.</param>
		/// <param name="shaderFilePath">The path to the vertex shader file.</param>
		protected static VertexShader LoadVertexShader(AssetsManager assets, string shaderFilePath)
		{
			Assert.ArgumentNotNull(assets, () => assets);
			return assets.LoadVertexShader(shaderFilePath);
		}

		/// <summary>
		///   Loads a fragment shader.
		/// </summary>
		/// <param name="assets">The assets manager that should be used to load the shader.</param>
		/// <param name="shaderFilePath">The path to the fragment shader file.</param>
		protected static FragmentShader LoadFragmentShader(AssetsManager assets, string shaderFilePath)
		{
			Assert.ArgumentNotNull(assets, () => assets);
			return assets.LoadFragmentShader(shaderFilePath);
		}

		/// <summary>
		///   Creates a constant buffer.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device the constant buffer should be created for.</param>
		/// <param name="size">The size of the constant buffer's contents in bytes.</param>
		/// <param name="slot">The slot the constant buffer should be bound to.</param>
		protected static ConstantBuffer CreateConstantBuffer(GraphicsDevice graphicsDevice, int size, int slot)
		{
			return new ConstantBuffer(graphicsDevice, size, slot);
		}

		/// <summary>
		///   Binds the given constant buffer to the GPU.
		/// </summary>
		/// <param name="buffer">The constant buffer that should be bound.</param>
		protected static void Bind(ConstantBuffer buffer)
		{
			Assert.ArgumentNotNull(buffer, () => buffer);
			buffer.Bind();
		}

		/// <summary>
		///   Updates the given constant buffer with the given data. The size of the data is determined by the parameter that has
		///   been passed to the constructor of the constant buffer instance.
		/// </summary>
		/// <param name="buffer">The buffer that should be updated.</param>
		/// <param name="data">The data that should be copied to the GPU.</param>
		protected static unsafe void Update(ConstantBuffer buffer, void* data)
		{
			Assert.ArgumentNotNull(buffer, () => buffer);
			Assert.That(data != null, "The pointer to the data cannot be null.");

			buffer.CopyData(data);
		}

		/// <summary>
		/// Creates a new effect technique instance.
		/// </summary>
		/// <param name="vertexShader">The vertex shader that should be used by the technique.</param>
		/// <param name="fragmentShader">The fragment shader that should be used by the technique.</param>
		protected EffectTechnique CreateTechnique(VertexShader vertexShader, FragmentShader fragmentShader)
		{
			Assert.ArgumentNotNull(vertexShader, () => vertexShader);
			Assert.ArgumentNotNull(fragmentShader, () => fragmentShader);

			vertexShader.Bind();
			fragmentShader.Bind();
			return new EffectTechnique(this);
		}
	}
}