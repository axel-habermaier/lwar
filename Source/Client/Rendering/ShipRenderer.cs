﻿using System;

namespace Lwar.Client.Rendering
{
	using Gameplay;
	using Gameplay.Entities;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform.Assets;
	using Pegasus.Framework.Platform.Graphics;
	using Texture2D = Pegasus.Framework.Platform.Graphics.Texture2D;

	/// <summary>
	///   Renders ships into a 3D scene.
	/// </summary>
	public class ShipRenderer : Renderer<Ship, ShipRenderer.ShipDrawState>
	{
		/// <summary>
		///   The fragment shader that is used to draw the ships.
		/// </summary>
		private readonly FragmentShader _fragmentShader;

		/// <summary>
		///   The ship model.
		/// </summary>
		private readonly Model _model;

		/// <summary>
		///   The texture of the ship.
		/// </summary>
		private readonly Texture2D _texture;

		/// <summary>
		///   The transformation constant buffer.
		/// </summary>
		private readonly ConstantBuffer<Matrix> _transform;

		/// <summary>
		///   The vertex shader that is used to draw the ships.
		/// </summary>
		private readonly VertexShader _vertexShader;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that is used to draw the game session.</param>
		/// <param name="assets">The assets manager that manages all assets of the game session.</param>
		public unsafe ShipRenderer(GraphicsDevice graphicsDevice, AssetsManager assets)
		{
			Assert.ArgumentNotNull(graphicsDevice, () => graphicsDevice);
			Assert.ArgumentNotNull(assets, () => assets);

			_vertexShader = assets.LoadVertexShader("Shaders/QuadVS");
			_fragmentShader = assets.LoadFragmentShader("Shaders/QuadFS");
			_transform = new ConstantBuffer<Matrix>(graphicsDevice, (buffer, matrix) => buffer.Copy(&matrix));
			_texture = assets.LoadTexture2D("Textures/Ship");
			_model = Model.CreateQuad(graphicsDevice, _texture.Size);
		}

		/// <summary>
		///   Invoked when an element has been added to the renderer.
		/// </summary>
		/// <param name="ship">The element that should be drawn by the renderer.</param>
		protected override ShipDrawState OnAdded(Ship ship)
		{
			return new ShipDrawState { Transform = ship.Transform };
		}

		/// <summary>
		///   Draws all registered elements.
		/// </summary>
		public void Draw()
		{
			_transform.Bind(1);
			_vertexShader.Bind();
			_fragmentShader.Bind();
			SamplerState.TrilinearClamp.Bind(0);
			_texture.Bind(0);

			foreach (var ship in RegisteredElements)
			{
				_transform.Data = ship.Transform.Matrix;
				_transform.Update();

				_model.Draw();
			}
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_model.SafeDispose();
			_transform.SafeDispose();
		}

		/// <summary>
		///   The state required for drawing a ship.
		/// </summary>
		public struct ShipDrawState
		{
			/// <summary>
			///   The transformation of the bullet.
			/// </summary>
			public Transformation Transform;
		}
	}
}