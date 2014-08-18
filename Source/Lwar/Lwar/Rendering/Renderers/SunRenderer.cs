namespace Lwar.Rendering.Renderers
{
	using System;
	using Assets;
	using Assets.Effects;
	using Gameplay.Entities;
	using Pegasus.Assets;
	using Pegasus.Math;
	using Pegasus.Platform;
	using Pegasus.Platform.Graphics;
	using Pegasus.Platform.Memory;
	using Pegasus.Rendering;

	/// <summary>
	///     Renders suns into a 3D scene.
	/// </summary>
	public class SunRenderer : Renderer<Sun>
	{
		private GaussianBlur _blur;
		private Clock _clock = new Clock();

		/// <summary>
		///     The render target that is used to draw the sun effect.
		/// </summary>
		private RenderTarget _effectTarget;

		/// <summary>
		///     The full-screen quad that is used to draw the sun special effects.
		/// </summary>
		private FullscreenQuad _fullscreenQuad;

		private Texture2D _heat;

		private RenderOutput _heatOutput;

		/// <summary>
		///     The sun model.
		/// </summary>
		private Model _model;

		private TexturedQuadEffect _quadEffect;
		private Vector2 _rotation = new Vector2(0, 4);

		private SphereEffect _sphereEffect;

		private CubeMap _sun;
		private SunEffect _sunEffect;
		private CubeMap _turbulence;

		/// <summary>
		///     Loads the required assets of the renderer.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used for drawing.</param>
		/// <param name="assets">The assets manager that should be used to load all required assets.</param>
		public override void Load(GraphicsDevice graphicsDevice, AssetsManager assets)
		{
			_sun = assets.Load(Textures.SunCubemap);
			_turbulence = assets.Load(Textures.SunHeatCubemap);
			_heat = assets.Load(Textures.Heat);

			_sphereEffect = new SphereEffect(graphicsDevice, assets);
			_sunEffect = new SunEffect(graphicsDevice, assets);

			_fullscreenQuad = new FullscreenQuad(graphicsDevice);
			_quadEffect = new TexturedQuadEffect(graphicsDevice, assets) { World = Matrix.Identity };

			var w = 640;
			var h = 360;
			var flags = TextureFlags.GenerateMipmaps | TextureFlags.RenderTarget;
			var effectTexture = new Texture2D(graphicsDevice, w, h, SurfaceFormat.Rgba8, flags);
			effectTexture.SetName("SunRenderer.EffectTexture");

			_effectTarget = new RenderTarget(graphicsDevice, null, effectTexture);
			_heatOutput = new RenderOutput(graphicsDevice) { RenderTarget = _effectTarget, Viewport = new Rectangle(0, 0, w, h) };
			_blur = new GaussianBlur(graphicsDevice, assets, effectTexture);
		}

		/// <summary>
		///     Initializes the renderer.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used for drawing.</param>
		public override void Initialize(GraphicsDevice graphicsDevice)
		{
			_model = Model.CreateSphere(graphicsDevice, EntityTemplates.Sun.Radius, 20);
			_sphereEffect.SphereTexture = new CubeMapView(_sun, SamplerState.TrilinearClamp);

			_sunEffect.CubeMap = new CubeMapView(_turbulence, SamplerState.TrilinearClamp);
			_sunEffect.HeatMap = new Texture2DView(_heat, SamplerState.BilinearClampNoMipmaps);
		}

		/// <summary>
		///     Draws all suns.
		/// </summary>
		/// <param name="output">The output that the bullets should be rendered to.</param>
		public override void Draw(RenderOutput output)
		{
			foreach (var sun in Elements)
			{
				BlendState.Premultiplied.Bind();
				DepthStencilState.DepthEnabled.Bind();

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
			}
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposingCore()
		{
			_sunEffect.SafeDispose();
			_sphereEffect.SafeDispose();
			_quadEffect.SafeDispose();
			_model.SafeDispose();
			_effectTarget.SafeDispose();
			_fullscreenQuad.SafeDispose();
			_heatOutput.SafeDispose();
			_blur.SafeDispose();
		}
	}
}