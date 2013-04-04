using System;

namespace Lwar.Client.Rendering
{
	using Assets.Effects;
	using Gameplay;
	using Gameplay.Entities;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Rendering;

	/// <summary>
	///   Renders suns into a 3D scene.
	/// </summary>
	public class SunRenderer : Renderer<Sun, SunRenderer.SunDrawState>
	{
		private readonly GaussianBlur _blur;

		private readonly Clock _clock = Clock.Create();

		/// <summary>
		///   The render target that is used to draw the sun effect.
		/// </summary>
		private readonly RenderTarget _effectTarget;

		/// <summary>
		///   The texture that is bound to the effect render target.
		/// </summary>
		private readonly Texture2D _effectTexture;

		/// <summary>
		///   The full-screen quad that is used to draw the sun special effects.
		/// </summary>
		private readonly FullscreenQuad _fullscreenQuad;

		private readonly RenderOutput _heatOutput;

		/// <summary>
		///   The sun model.
		/// </summary>
		private readonly Model _model;

		private readonly TexturedQuadEffect _quadEffect;

		private readonly SphereEffect _sphereEffect;

		private readonly SunEffect _sunEffect;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used to draw the game session.</param>
		/// <param name="assets">The assets manager that manages all assets of the game session.</param>
		public SunRenderer(GraphicsDevice graphicsDevice, AssetsManager assets)
		{
			Assert.ArgumentNotNull(graphicsDevice, () => graphicsDevice);
			Assert.ArgumentNotNull(assets, () => assets);

			var sun = assets.LoadCubeMap("Textures/Sun");
			var turbulence = assets.LoadCubeMap("Textures/SunHeat");
			var heat = assets.LoadTexture2D("Textures/Heat");

			_model = Model.CreateSphere(graphicsDevice, 200, 25);
			_sphereEffect = new SphereEffect(graphicsDevice, assets)
			{
				SphereTexture = new CubeMapView(sun, SamplerState.TrilinearClamp)
			};

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
			_heatOutput = new RenderOutput(graphicsDevice)
			{
				RenderTarget = _effectTarget,
				Viewport = new Rectangle(0, 0, (int)w, (int)h)
			};

			_fullscreenQuad = new FullscreenQuad(graphicsDevice, assets);
			_quadEffect = new TexturedQuadEffect(graphicsDevice, assets) { World = Matrix.Identity };

			_blur = new GaussianBlur(graphicsDevice, assets, _effectTexture);
		}

		/// <summary>
		///   Invoked when an element has been added to the renderer.
		/// </summary>
		/// <param name="sun">The element that should be drawn by the renderer.</param>
		protected override SunDrawState OnAdded(Sun sun)
		{
			return new SunDrawState { Transform = sun.Transform, rot1 = 4 };
		}

		/// <summary>
		///   Draws all registered elements.
		/// </summary>
		/// <param name="output">The output that the bullets should be rendered to.</param>
		public void Draw(RenderOutput output)
		{
			foreach (var sun in RegisteredElements)
			{
				var elapsed = (float)_clock.Seconds;
				_clock.Reset();

				sun.rot1 += 0.1f * elapsed;
				sun.rot2 -= 0.05f * elapsed;

				_sphereEffect.World = Matrix.CreateRotationY(-sun.rot1 * 2) * sun.Transform.Matrix;
				_model.Draw(output, _sphereEffect.Default);

				_sunEffect.World = Matrix.CreateScale(1.03f) * Matrix.CreateRotationY(-sun.rot1 * 2) * sun.Transform.Matrix;
				_sunEffect.Rotation1 = Matrix.CreateRotationY(-sun.rot1) * Matrix.CreateRotationX(sun.rot2 * 2);
				_sunEffect.Rotation2 = Matrix.CreateRotationY(-sun.rot2) * Matrix.CreateRotationZ(sun.rot1 * 2);

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

		/// <summary>
		///   The state required for drawing a sun.
		/// </summary>
		public class SunDrawState
		{
			/// <summary>
			///   The transformation of the sun.
			/// </summary>
			public Transformation Transform;

			public float rot1, rot2;
		}
	}
}