﻿using System;

namespace Lwar.Client.Rendering
{
	using System.Runtime.InteropServices;
	using Gameplay;
	using Gameplay.Entities;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Platform.Assets;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Rendering;
	using Texture2D = Pegasus.Framework.Platform.Graphics.Texture2D;

	/// <summary>
	///   Renders suns into a 3D scene.
	/// </summary>
	public class SunRenderer : Renderer<Sun, SunRenderer.SunDrawState>
	{
		//private readonly GaussianBlur _blur;

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
		///   The fragment shader that is used to draw the suns.
		/// </summary>
		private readonly FragmentShader _fragmentShader;

		/// <summary>
		///   The full-screen quad that is used to draw the sun special effects.
		/// </summary>
		//private readonly FullscreenQuad _fullscreenQuad;

		/// <summary>
		///   The graphics device that is used to draw the game session.
		/// </summary>
		private readonly GraphicsDevice _graphicsDevice;

		/// <summary>
		///   The heat cube map.
		/// </summary>
		private readonly CubeMap _heatCubeMap;

		/// <summary>
		///   The fragment shader that is used to draw the suns.
		/// </summary>
		private readonly FragmentShader _heatFS;

		private readonly Texture2D _heatTexture;

		/// <summary>
		///   The vertex shader that is used to draw the suns.
		/// </summary>
		private readonly VertexShader _heatVS;

		/// <summary>
		///   The sun model.
		/// </summary>
		private readonly Model _model;

		/// <summary>
		///   The fragment shader that is used to draw the suns.
		/// </summary>
		private readonly FragmentShader _quadFS;

		/// <summary>
		///   The render target the sun is rendered into.
		/// </summary>
		private readonly RenderTarget _renderTarget;

		/// <summary>
		///   The sun cube map.
		/// </summary>
		private readonly CubeMap _sunCubeMap;

		/// <summary>
		///   The transformation constant buffer.
		/// </summary>
		private readonly ConstantBuffer<SunData> _transform;

		/// <summary>
		///   The vertex shader that is used to draw the suns.
		/// </summary>
		private readonly VertexShader _vertexShader;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used to draw the game session.</param>
		/// <param name="renderTarget">The render target the sun should be rendered into.</param>
		/// <param name="assets">The assets manager that manages all assets of the game session.</param>
		public unsafe SunRenderer(GraphicsDevice graphicsDevice, RenderTarget renderTarget, AssetsManager assets)
		{
			Assert.ArgumentNotNull(graphicsDevice, () => graphicsDevice);
			Assert.ArgumentNotNull(renderTarget, () => renderTarget);
			Assert.ArgumentNotNull(assets, () => assets);

			_graphicsDevice = graphicsDevice;
			_renderTarget = renderTarget;

			//_vertexShader = assets.LoadVertexShader("Shaders/SphereVS");
			//_fragmentShader = assets.LoadFragmentShader("Shaders/SphereFS");
			//_heatVS = assets.LoadVertexShader("Shaders/SunHeatVS");
			//_heatFS = assets.LoadFragmentShader("Shaders/SunHeatFS");
			//_quadFS = assets.LoadFragmentShader("Shaders/QuadFS");
			//_transform = new ConstantBuffer<SunData>(graphicsDevice, (buffer, matrix) => buffer.Copy(&matrix));
			//_sunCubeMap = assets.LoadCubeMap("Textures/Sun");
			//_heatCubeMap = assets.LoadCubeMap("Textures/SunHeat");
			//_heatTexture = assets.LoadTexture2D("Textures/Heat");
			//_model = Model.CreateSphere(graphicsDevice, 200, 25);

			//_fullscreenQuad = new FullscreenQuad(graphicsDevice, assets);
			//_effectTexture = new Texture2D(graphicsDevice, 640, 360, SurfaceFormat.Rgba8,
			//							   TextureFlags.GenerateMipmaps | TextureFlags.RenderTarget);
			//_effectTarget = new RenderTarget(graphicsDevice, new Texture[] { _effectTexture }, null);
			//_blur = new GaussianBlur(graphicsDevice, assets, _effectTexture);
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
			//_transform.Bind(1);

			//foreach (var sun in RegisteredElements)
			//{
			//	var elapsed = (float)_clock.Seconds;
			//	_clock.Reset();
			//	sun.rot1 += 0.1f * elapsed;
			//	sun.rot2 -= 0.05f * elapsed;
			//	_transform.Data.World = Matrix.CreateRotationY(-sun.rot1 * 2) * sun.Transform.Matrix;
			//	_transform.Data.Rotation1 = Matrix.CreateRotationY(-sun.rot1) * Matrix.CreateRotationX(sun.rot2 * 2);
			//	_transform.Data.Rotation2 = Matrix.CreateRotationY(-sun.rot2) * Matrix.CreateRotationZ(sun.rot1 * 2);
			//	_transform.Update();

			//	_vertexShader.Bind();
			//	_fragmentShader.Bind();
			//	SamplerState.TrilinearClamp.Bind(0);
			//	_sunCubeMap.Bind(0);
			//	_model.Draw();

			//	_transform.Data.World = Matrix.CreateScale(1.03f) * Matrix.CreateRotationY(-sun.rot1 * 2) * sun.Transform.Matrix;
			//	_transform.Update();

			//	DepthStencilState.DepthRead.Bind();
			//	_heatVS.Bind();
			//	_heatFS.Bind();
			//	_heatTexture.Bind(1);
			//	_sunCubeMap.Bind(2);
			//	SamplerState.BilinearClampNoMipmaps.Bind(1);
			//	var viewport = _graphicsDevice.Viewport;
			//	_effectTarget.Bind();
			//	_graphicsDevice.Viewport = new Rectangle(0, 0, 640, 360);

			//	_effectTarget.ClearColor(new Color(0, 0, 0, 0));
			//	_heatCubeMap.Bind(0);

			//	_model.Draw();

				

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
			//}
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_clock.SafeDispose();
			//_blur.SafeDispose();
			//_model.SafeDispose();
			//_transform.SafeDispose();
			//_effectTexture.SafeDispose();
			//_effectTarget.SafeDispose();
			//_fullscreenQuad.SafeDispose();
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct SunData
		{
			public Matrix World;
			public Matrix Rotation1;
			public Matrix Rotation2;
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