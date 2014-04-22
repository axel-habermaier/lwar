namespace Lwar.Rendering.Renderers
{
	using System;
	using Assets;
	using Assets.Effects;
	using Gameplay.Entities;
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
		private readonly Clock _clock = Clock.Create();
		private GaussianBlur _blur;

		/// <summary>
		///     The render target that is used to draw the sun effect.
		/// </summary>
		private RenderTarget _effectTarget;

		/// <summary>
		///     The full-screen quad that is used to draw the sun special effects.
		/// </summary>
		private FullscreenQuad _fullscreenQuad;

		private RenderOutput _heatOutput;

		/// <summary>
		///     The sun model.
		/// </summary>
		private Model _model;

		private TexturedQuadEffect _quadEffect;
		private Vector2 _rotation = new Vector2(0, 4);

		private SphereEffect _sphereEffect;

		private SunEffect _sunEffect;

		/// <summary>
		///     Initializes the renderer.
		/// </summary>
		protected override void Initialize()
		{
			var sun = Assets.Load(Textures.SunCubemap);
			var turbulence = Assets.Load(Textures.SunHeatCubemap);
			var heat = Assets.Load(Textures.Heat);

			_model = Model.CreateSphere(EntityTemplates.Sun.Radius, 20);
			_sphereEffect = new SphereEffect(Assets) { SphereTexture = new CubeMapView(sun, SamplerState.TrilinearClamp) };

			_sunEffect = new SunEffect(Assets)
			{
				CubeMap = new CubeMapView(turbulence, SamplerState.TrilinearClamp),
				HeatMap = new Texture2DView(heat, SamplerState.BilinearClampNoMipmaps)
			};

			var w = 640;
			var h = 360;
			var flags = TextureFlags.GenerateMipmaps | TextureFlags.RenderTarget;
			var effectTexture = new Texture2D(w, h, SurfaceFormat.Rgba8, flags);
			effectTexture.SetName("SunRenderer.EffectTexture");

			_effectTarget = new RenderTarget(null, effectTexture);
			_heatOutput = new RenderOutput() { RenderTarget = _effectTarget, Viewport = new Rectangle(0, 0, w, h) };

			_fullscreenQuad = new FullscreenQuad(Assets);
			_quadEffect = new TexturedQuadEffect(Assets) { World = Matrix.Identity };

			_blur = new GaussianBlur(Assets, effectTexture);
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
			_clock.SafeDispose();
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