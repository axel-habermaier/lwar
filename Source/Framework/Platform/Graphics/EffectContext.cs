﻿using System;

namespace Pegasus.Framework.Platform.Graphics
{
	using System.Diagnostics;

	/// <summary>
	///   Represents the context of an effect.
	/// </summary>
	/// <remarks>
	///   These methods are not declared in the Effect class in order to avoid possible name clashes with variables
	///   declared by the effect writer.
	/// </remarks>
	public struct EffectContext
	{
		/// <summary>
		///   The assets manager that should be used to load the effect assets.
		/// </summary>
		private readonly AssetsManager _assets;

		/// <summary>
		///   The graphics device the graphics resources should be created for.
		/// </summary>
		private readonly GraphicsDevice _graphicsDevice;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device the graphics resources should be created for.</param>
		/// <param name="assets">The assets manager that should be used to load the effect assets.</param>
		internal EffectContext(GraphicsDevice graphicsDevice, AssetsManager assets)
		{
			Assert.ArgumentNotNull(graphicsDevice);
			Assert.ArgumentNotNull(assets);

			_graphicsDevice = graphicsDevice;
			_assets = assets;
		}

		/// <summary>
		///   Binds the given texture view to the GPU.
		/// </summary>
		/// <param name="view">The texture view that should be bound.</param>
		/// <param name="slot">The slot the texture view should be bound to.</param>
		public void Bind(Texture2DView view, int slot)
		{
			ValidateInitialization();
			view.Bind(slot);
		}

		/// <summary>
		///   Binds the given texture view to the GPU.
		/// </summary>
		/// <param name="view">The texture view that should be bound.</param>
		/// <param name="slot">The slot the texture view should be bound to.</param>
		public void Bind(CubeMapView view, int slot)
		{
			ValidateInitialization();
			view.Bind(slot);
		}

		/// <summary>
		///   Creates a constant buffer.
		/// </summary>
		/// <param name="size">The size of the constant buffer's contents in bytes.</param>
		/// <param name="slot">The slot the constant buffer should be bound to.</param>
		public ConstantBuffer CreateConstantBuffer(int size, int slot)
		{
			ValidateInitialization();
			return new ConstantBuffer(_graphicsDevice, size, slot);
		}

		/// <summary>
		///   Binds the given constant buffer to the GPU.
		/// </summary>
		/// <param name="buffer">The constant buffer that should be bound.</param>
		public void Bind(ConstantBuffer buffer)
		{
			Assert.ArgumentNotNull(buffer);
			ValidateInitialization();

			buffer.Bind();
		}

		/// <summary>
		///   Updates the given constant buffer with the given data. The size of the data is determined by the parameter that has
		///   been passed to the constructor of the constant buffer instance.
		/// </summary>
		/// <param name="buffer">The buffer that should be updated.</param>
		/// <param name="data">The data that should be copied to the GPU.</param>
		public unsafe void Update(ConstantBuffer buffer, void* data)
		{
			Assert.ArgumentNotNull(buffer);
			Assert.That(data != null, "The pointer to the data cannot be null.");
			ValidateInitialization();

			buffer.CopyData(data);
		}

		/// <summary>
		///   Creates a new effect technique instance.
		/// </summary>
		/// <param name="bind">The action that should be invoked to bind the required textures and constant buffers.</param>
		/// <param name="vertexShader">The vertex shader that should be used by the technique.</param>
		/// <param name="fragmentShader">The fragment shader that should be used by the technique.</param>
		public EffectTechnique CreateTechnique(Action bind, string vertexShader, string fragmentShader)
		{
			Assert.ArgumentNotNullOrWhitespace(vertexShader);
			Assert.ArgumentNotNullOrWhitespace(fragmentShader);
			Assert.ArgumentNotNull(bind);
			ValidateInitialization();

			return new EffectTechnique(_assets.LoadVertexShader(vertexShader), _assets.LoadFragmentShader(fragmentShader), bind);
		}

		/// <summary>
		///   Binds the given technique to the GPU.
		/// </summary>
		/// <param name="technique">The technique that should be bound.</param>
		public void Bind(EffectTechnique technique)
		{
			ValidateInitialization();
			technique.Bind();
		}

		/// <summary>
		///   Checks whether the instance is initialized.
		/// </summary>
		[Conditional("DEBUG")]
		private void ValidateInitialization()
		{
			Assert.NotNull(_graphicsDevice, "No graphics device has been set.");
			Assert.NotNull(_assets, "No assets manager has been set.");
		}
	}
}