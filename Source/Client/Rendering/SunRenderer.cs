using System;

namespace Lwar.Client.Rendering
{
	using System.Runtime.InteropServices;
	using Assets.Effects;
	using Gameplay;
	using Gameplay.Entities;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Platform.Assets;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Rendering;

	/// <summary>
	///   Renders suns into a 3D scene.
	/// </summary>
	public class SunRenderer : Renderer<Sun, SunRenderer.SunDrawState>
	{
		private readonly GaussianBlur _blur;

		private readonly Clock _clock = Clock.Create(true);

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

		/// <summary>
		///   The graphics device that is used to draw the game session.
		/// </summary>
		private readonly GraphicsDevice _graphicsDevice;

		private readonly RenderOutput _heatOutput;

		/// <summary>
		///   The sun model.
		/// </summary>
		private readonly Model _model;

		private readonly TexturedQuadEffect _quadEffect;

		/// <summary>
		///   The render target the sun is rendered into.
		/// </summary>
		private readonly RenderTarget _renderTarget;

		private readonly SphereEffect _sphereEffect;

		/// <summary>
		///   The sun cube map.
		/// </summary>
		private readonly CubeMap _sunCubeMap;

		private readonly SunEffect _sunEffect;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used to draw the game session.</param>
		/// <param name="renderTarget">The render target the sun should be rendered into.</param>
		/// <param name="assets">The assets manager that manages all assets of the game session.</param>
		public SunRenderer(GraphicsDevice graphicsDevice, RenderTarget renderTarget, AssetsManager assets)
		{
			Assert.ArgumentNotNull(graphicsDevice, () => graphicsDevice);
			Assert.ArgumentNotNull(renderTarget, () => renderTarget);
			Assert.ArgumentNotNull(assets, () => assets);

			_graphicsDevice = graphicsDevice;
			_renderTarget = renderTarget;

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

			_effectTexture = new Texture2D(graphicsDevice, 640, 360, SurfaceFormat.Rgba8,
										   TextureFlags.GenerateMipmaps | TextureFlags.RenderTarget);
			_effectTarget = new RenderTarget(graphicsDevice, null, _effectTexture);
			_heatOutput = new RenderOutput(graphicsDevice) { RenderTarget = _effectTarget, Viewport = new Rectangle(0, 0, 640, 360) };

			_fullscreenQuad = new FullscreenQuad(graphicsDevice, assets);
			_quadEffect = new TexturedQuadEffect(graphicsDevice, assets){World = Matrix.Identity};

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

				//	_renderTarget.Bind();
				//	_blur.Blur(_renderTarget);
				//	//_graphicsDevice.Viewport = viewport;
				//	//_effectTexture.GenerateMipmaps();

				//	DepthStencilState.DepthDisabled.Bind();
				//	BlendState.Premultiplied.Bind();
				//	_renderTarget.Bind();
				//	_graphicsDevice.Viewport = viewport;
				//	//_effectTexture.Bind(0);
				//	_quadFS.Bind();
				//	//_effectTexture.Bind(0);
				//	SamplerState.BilinearClampNoMipmaps.Bind(0);

				//	_fullscreenQuad.Draw();

				//	BlendState.Premultiplied.Bind();
				//	DepthStencilState.Default.Bind();
				//	_graphicsDevice.Viewport = viewport;
				//	_renderTarget.Bind();
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

		[StructLayout(LayoutKind.Sequential)]
		private struct SunData
		{
			public readonly Matrix World;
			public readonly Matrix Rotation1;
			public readonly Matrix Rotation2;
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