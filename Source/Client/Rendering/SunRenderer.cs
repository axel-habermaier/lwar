using System;

namespace Lwar.Client.Rendering
{
	using Gameplay;
	using Gameplay.Entities;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform.Assets;
	using Pegasus.Framework.Platform.Graphics;

	/// <summary>
	///   Renders suns into a 3D scene.
	/// </summary>
	public class SunRenderer : Renderer<Sun, SunRenderer.SunDrawState>
	{
		/// <summary>
		///   The sun cube map.
		/// </summary>
		private readonly CubeMap _cubeMap;

		/// <summary>
		///   The fragment shader that is used to draw the suns.
		/// </summary>
		private readonly FragmentShader _fragmentShader;

		/// <summary>
		///   The sun model.
		/// </summary>
		private readonly Model _model;

		private readonly RenderTarget _renderTarget;
		private readonly Texture2D _texture;

		/// <summary>
		///   The transformation constant buffer.
		/// </summary>
		private readonly ConstantBuffer<Matrix> _transform;

		/// <summary>
		///   The vertex shader that is used to draw the suns.
		/// </summary>
		private readonly VertexShader _vertexShader;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that is used to draw the game session.</param>
		/// <param name="assets">The assets manager that manages all assets of the game session.</param>
		public unsafe SunRenderer(GraphicsDevice graphicsDevice, AssetsManager assets)
		{
			Assert.ArgumentNotNull(graphicsDevice, () => graphicsDevice);
			Assert.ArgumentNotNull(assets, () => assets);

			_vertexShader = assets.LoadVertexShader("Shaders/SphereVS");
			_fragmentShader = assets.LoadFragmentShader("Shaders/SphereFS");
			_transform = new ConstantBuffer<Matrix>(graphicsDevice, (buffer, matrix) => buffer.Copy(&matrix));
			_cubeMap = assets.LoadCubeMap("Textures/Sun");
			_model = Model.CreateSphere(graphicsDevice, 200, 25);

			_texture = new Texture2D(graphicsDevice, 512, 512, SurfaceFormat.Rgba8,
									 TextureFlags.GenerateMipmaps | TextureFlags.RenderTarget);
		}

		/// <summary>
		///   Invoked when an element has been added to the renderer.
		/// </summary>
		/// <param name="sun">The element that should be drawn by the renderer.</param>
		protected override SunDrawState OnAdded(Sun sun)
		{
			return new SunDrawState { Transform = sun.Transform };
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
			_cubeMap.Bind(0);

			foreach (var planet in RegisteredElements)
			{
				_transform.Data = planet.Transform.Matrix;
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
			_texture.SafeDispose();
			_renderTarget.SafeDispose();
		}

		/// <summary>
		///   The state required for drawing a planet.
		/// </summary>
		public struct SunDrawState
		{
			/// <summary>
			///   The transformation of the planet.
			/// </summary>
			public Transformation Transform;
		}
	}
}