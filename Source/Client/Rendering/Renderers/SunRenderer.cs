using System;

namespace Lwar.Client.Rendering.Renderers
{
	using Assets.Effects;
	using Gameplay.Entities;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Rendering;

	/// <summary>
	///   Renders suns into a 3D scene.
	/// </summary>
	public class SunRenderer : Renderer<Sun>
	{
		private readonly Clock _clock = Clock.Create();
		private GaussianBlur _blur;

		/// <summary>
		///   The render target that is used to draw the sun effect.
		/// </summary>
		private RenderTarget _effectTarget;

		/// <summary>
		///   The texture that is bound to the effect render target.
		/// </summary>
		private Texture2D _effectTexture;

		/// <summary>
		///   The full-screen quad that is used to draw the sun special effects.
		/// </summary>
		private FullscreenQuad _fullscreenQuad;

		private RenderOutput _heatOutput;

		/// <summary>
		///   The sun model.
		/// </summary>
		private Model _model;

		private TexturedQuadEffect _quadEffect;
		private Vector2 _rotation = new Vector2(0, 4);

		private SphereEffect _sphereEffect;

		private SunEffect _sunEffect;

		/// <summary>
		///   Initializes the renderer.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used for drawing.</param>
		/// <param name="assets">The assets manager that should be used to load all required assets.</param>
		public override void Initialize(GraphicsDevice graphicsDevice, AssetsManager assets)
		{
			Assert.ArgumentNotNull(graphicsDevice, () => graphicsDevice);
			Assert.ArgumentNotNull(assets, () => assets);

			var sun = assets.LoadCubeMap("Textures/Sun");
			var turbulence = assets.LoadCubeMap("Textures/SunHeat");
			var heat = assets.LoadTexture2D("Textures/Heat");

			_model = Model.CreateSphere(graphicsDevice, 128, 20);
			_sphereEffect = new SphereEffect(graphicsDevice, assets) { SphereTexture = new CubeMapView(sun, SamplerState.TrilinearClamp) };

			_sunEffect = new SunEffect(graphicsDevice, assets)
			{
				CubeMap = new CubeMapView(turbulence, SamplerState.TrilinearClamp),
				HeatMap = new Texture2DView(heat, SamplerState.BilinearClampNoMipmaps)
			};

			uint w = 640;
			uint h = 360;
			var flags = TextureFlags.GenerateMipmaps | TextureFlags.RenderTarget;
			_effectTexture = new Texture2D(graphicsDevice, w, h, SurfaceFormat.Rgba8, flags);
			_effectTexture.SetName("SunRenderer.EffectTexture");

			_effectTarget = new RenderTarget(graphicsDevice, null, _effectTexture);
			_heatOutput = new RenderOutput(graphicsDevice) { RenderTarget = _effectTarget, Viewport = new Rectangle(0, 0, (int)w, (int)h) };

			_fullscreenQuad = new FullscreenQuad(graphicsDevice, assets);
			_quadEffect = new TexturedQuadEffect(graphicsDevice, assets) { World = Matrix.Identity };

			_blur = new GaussianBlur(graphicsDevice, assets, _effectTexture);
		}

		/// <summary>
		///   Draws all suns.
		/// </summary>
		/// <param name="output">The output that the bullets should be rendered to.</param>
		public override void Draw(RenderOutput output)
		{
			foreach (var sun in Elements)
			{
				var elapsed = (float)_clock.Seconds;
				_clock.Reset();

				_rotation.X += 0.1f * elapsed;
				_rotation.Y -= 0.05f * elapsed;

				_sphereEffect.World = Matrix.CreateRotationY(-_rotation.X * 2) * sun.Transform.Matrix;
				_model.Draw(output, _sphereEffect.Default);

				_sunEffect.World = Matrix.CreateScale(1.03f) * Matrix.CreateRotationY(-_rotation.X * 2) * sun.Transform.Matrix;
				_sunEffect.Rotation1 = Matrix.CreateRotationY(-_rotation.X) * Matrix.CreateRotationX(_rotation.Y * 2);
				_sunEffect.Rotation2 = Matrix.CreateRotationY(-_rotation.Y) * Matrix.CreateRotationZ(_rotation.X * 2);

				DepthStencilState.DepthRead.Bind();
				_heatOutput.ClearColor(new Color(0, 0, 0, 0));
				_heatOutput.Camera = output.Camera;
				_model.Draw(_heatOutput, _sunEffect.Default);

				var blurredTexture = _blur.Blur(output);

				DepthStencilState.DepthDisabled.Bind();
				_quadEffect.Texture = new Texture2DView(blurredTexture, SamplerState.BilinearClampNoMipmaps);
				_fullscreenQuad.Draw(output, _quadEffect.FullScreen);

				DepthStencilState.DepthEnabled.Bind();
			}
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_clock.SafeDispose();
			_sunEffect.SafeDispose();
			_sphereEffect.SafeDispose();
			_quadEffect.SafeDispose();
			_model.SafeDispose();
			_effectTexture.SafeDispose();
			_effectTarget.SafeDispose();
			_fullscreenQuad.SafeDispose();
			_heatOutput.SafeDispose();
			_blur.SafeDispose();
		}
	}
}