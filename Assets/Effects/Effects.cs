//------------------------------------------------------------------------------
// <auto-generated>
//     Generated by the Pegasus Asset Compiler.
//     Tuesday, 02 April 2013, 11:27:14
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Pegasus.Framework;
using Pegasus.Framework.Math;
using Pegasus.Framework.Platform.Assets;
using Pegasus.Framework.Platform.Graphics;

namespace Lwar.Assets.Effects
{
	/// <summary>
	///   Applies a Gaussian blur filter to a texture.
	/// </summary>
	public sealed class BlurEffect : Effect
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device this instance belongs to.</param>
		/// <param name="assets">The assets manager that should be used to load required assets.</param>
		public BlurEffect(GraphicsDevice graphicsDevice, AssetsManager assets)
			: base(graphicsDevice, assets)
		{
			BlurHorizontally = __context.CreateTechnique("Effects/Lwar.Assets.Effects.BlurEffect.VertexShader", "Effects/Lwar.Assets.Effects.BlurEffect.HorizontalBlur");
			BlurVertically = __context.CreateTechnique("Effects/Lwar.Assets.Effects.BlurEffect.VertexShader", "Effects/Lwar.Assets.Effects.BlurEffect.VerticalBlur");
		}

		/// <summary>
		///   The texture that is blurred.
		/// </summary>
		public TextureBinding<Texture2D> Texture { get; set; }

		/// <summary>
		///   Applies a horizontal Gaussian blur filter.
		/// </summary>
		public EffectTechnique BlurHorizontally { get; private set; }

		/// <summary>
		///   Applies a vertical Gaussian blur filter.
		/// </summary>
		public EffectTechnique BlurVertically { get; private set; }

		/// <summary>
		///   Binds all textures and non-shared constant buffers required by the effect.
		/// </summary>
		private void Bind()
		{
			__context.Bind(Texture, 0);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void __OnDisposing()
		{
			// Nothing to do here
		}
	}
}

namespace Lwar.Assets.Effects
{
	public sealed class SkyboxEffect : Effect
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device this instance belongs to.</param>
		/// <param name="assets">The assets manager that should be used to load required assets.</param>
		public SkyboxEffect(GraphicsDevice graphicsDevice, AssetsManager assets)
			: base(graphicsDevice, assets)
		{
			DrawSkybox = __context.CreateTechnique("Effects/Lwar.Assets.Effects.SkyboxEffect.VertexShader", "Effects/Lwar.Assets.Effects.SkyboxEffect.FragmentShader");
		}

		public TextureBinding<CubeMap> Skybox { get; set; }

		public EffectTechnique DrawSkybox { get; private set; }

		/// <summary>
		///   Binds all textures and non-shared constant buffers required by the effect.
		/// </summary>
		private void Bind()
		{
			__context.Bind(Skybox, 0);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void __OnDisposing()
		{
			// Nothing to do here
		}
	}
}

namespace Lwar.Assets.Effects
{
	public sealed class SphereEffect : Effect
	{
		/// <summary>
		///   Indicates whether the contents of ConstantBuffer2 have changed.
		/// </summary>
		private bool __ConstantBuffer2Dirty = true;

		/// <summary>
		///   Passes the shader constants in the ConstantBuffer2 constant buffer to the GPU.
		/// </summary>
		private readonly ConstantBuffer _ConstantBuffer2;

		private Matrix _World;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device this instance belongs to.</param>
		/// <param name="assets">The assets manager that should be used to load required assets.</param>
		public SphereEffect(GraphicsDevice graphicsDevice, AssetsManager assets)
			: base(graphicsDevice, assets)
		{
			DrawSphere = __context.CreateTechnique("Effects/Lwar.Assets.Effects.SphereEffect.VertexShader", "Effects/Lwar.Assets.Effects.SphereEffect.FragmentShader");

			_ConstantBuffer2 = __context.CreateConstantBuffer(64, 2);
		}

		public Matrix World
		{
			get { return _World; }
			set
			{
				_World = value;
				__ConstantBuffer2Dirty = true;
			}
		}

		public TextureBinding<CubeMap> SphereTexture { get; set; }

		public EffectTechnique DrawSphere { get; private set; }

		/// <summary>
		///   Binds all textures and non-shared constant buffers required by the effect.
		/// </summary>
		private unsafe void Bind()
		{
			if (__ConstantBuffer2Dirty)
			{
				var __data = new __ConstantBuffer2();
				__data.World = World;

				__ConstantBuffer2Dirty = false;
				__context.Update(_ConstantBuffer2, &__data);
			}

			__context.Bind(SphereTexture, 0);
			__context.Bind(_ConstantBuffer2);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void __OnDisposing()
		{
			_ConstantBuffer2.SafeDispose();
		}

		[StructLayout(LayoutKind.Sequential, Size = 64)]
		private struct __ConstantBuffer2
		{
			public Matrix World;
		}
	}
}

namespace Lwar.Assets.Effects
{
	public sealed class SpriteEffect : Effect
	{
		/// <summary>
		///   Indicates whether the contents of ConstantBuffer2 have changed.
		/// </summary>
		private bool __ConstantBuffer2Dirty = true;

		/// <summary>
		///   Passes the shader constants in the ConstantBuffer2 constant buffer to the GPU.
		/// </summary>
		private readonly ConstantBuffer _ConstantBuffer2;

		private Matrix _World;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device this instance belongs to.</param>
		/// <param name="assets">The assets manager that should be used to load required assets.</param>
		public SpriteEffect(GraphicsDevice graphicsDevice, AssetsManager assets)
			: base(graphicsDevice, assets)
		{
			DrawSprite = __context.CreateTechnique("Effects/Lwar.Assets.Effects.SpriteEffect.VertexShader", "Effects/Lwar.Assets.Effects.SpriteEffect.FragmentShader");

			_ConstantBuffer2 = __context.CreateConstantBuffer(64, 2);
		}

		public Matrix World
		{
			get { return _World; }
			set
			{
				_World = value;
				__ConstantBuffer2Dirty = true;
			}
		}

		public TextureBinding<Texture2D> Sprite { get; set; }

		public EffectTechnique DrawSprite { get; private set; }

		/// <summary>
		///   Binds all textures and non-shared constant buffers required by the effect.
		/// </summary>
		private unsafe void Bind()
		{
			if (__ConstantBuffer2Dirty)
			{
				var __data = new __ConstantBuffer2();
				__data.World = World;

				__ConstantBuffer2Dirty = false;
				__context.Update(_ConstantBuffer2, &__data);
			}

			__context.Bind(Sprite, 0);
			__context.Bind(_ConstantBuffer2);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void __OnDisposing()
		{
			_ConstantBuffer2.SafeDispose();
		}

		[StructLayout(LayoutKind.Sequential, Size = 64)]
		private struct __ConstantBuffer2
		{
			public Matrix World;
		}
	}
}

namespace Lwar.Assets.Effects
{
	public sealed class SunEffect : Effect
	{
		/// <summary>
		///   Indicates whether the contents of ConstantBuffer2 have changed.
		/// </summary>
		private bool __ConstantBuffer2Dirty = true;

		/// <summary>
		///   Passes the shader constants in the ConstantBuffer2 constant buffer to the GPU.
		/// </summary>
		private readonly ConstantBuffer _ConstantBuffer2;

		private Matrix _Rotation1;

		private Matrix _Rotation2;

		private Matrix _World;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device this instance belongs to.</param>
		/// <param name="assets">The assets manager that should be used to load required assets.</param>
		public SunEffect(GraphicsDevice graphicsDevice, AssetsManager assets)
			: base(graphicsDevice, assets)
		{
			DrawSun = __context.CreateTechnique("Effects/Lwar.Assets.Effects.SunEffect.VertexShader", "Effects/Lwar.Assets.Effects.SunEffect.FragmentShader");

			_ConstantBuffer2 = __context.CreateConstantBuffer(192, 2);
		}

		public Matrix Rotation1
		{
			get { return _Rotation1; }
			set
			{
				_Rotation1 = value;
				__ConstantBuffer2Dirty = true;
			}
		}

		public Matrix Rotation2
		{
			get { return _Rotation2; }
			set
			{
				_Rotation2 = value;
				__ConstantBuffer2Dirty = true;
			}
		}

		public Matrix World
		{
			get { return _World; }
			set
			{
				_World = value;
				__ConstantBuffer2Dirty = true;
			}
		}

		public TextureBinding<CubeMap> CubeMap { get; set; }
		public TextureBinding<Texture2D> HeatMap { get; set; }

		public EffectTechnique DrawSun { get; private set; }

		/// <summary>
		///   Binds all textures and non-shared constant buffers required by the effect.
		/// </summary>
		private unsafe void Bind()
		{
			if (__ConstantBuffer2Dirty)
			{
				var __data = new __ConstantBuffer2();
				__data.Rotation1 = Rotation1;
				__data.Rotation2 = Rotation2;
				__data.World = World;

				__ConstantBuffer2Dirty = false;
				__context.Update(_ConstantBuffer2, &__data);
			}

			__context.Bind(CubeMap, 0);
			__context.Bind(HeatMap, 1);
			__context.Bind(_ConstantBuffer2);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void __OnDisposing()
		{
			_ConstantBuffer2.SafeDispose();
		}

		[StructLayout(LayoutKind.Sequential, Size = 192)]
		private struct __ConstantBuffer2
		{
			public Matrix Rotation1;
			public Matrix Rotation2;
			public Matrix World;
		}
	}
}

namespace Lwar.Assets.Effects
{
	public sealed class TexturedQuadEffect : Effect
	{
		/// <summary>
		///   Indicates whether the contents of ConstantBuffer2 have changed.
		/// </summary>
		private bool __ConstantBuffer2Dirty = true;

		/// <summary>
		///   Passes the shader constants in the ConstantBuffer2 constant buffer to the GPU.
		/// </summary>
		private readonly ConstantBuffer _ConstantBuffer2;

		private Matrix _World;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device this instance belongs to.</param>
		/// <param name="assets">The assets manager that should be used to load required assets.</param>
		public TexturedQuadEffect(GraphicsDevice graphicsDevice, AssetsManager assets)
			: base(graphicsDevice, assets)
		{
			DrawQuad = __context.CreateTechnique("Effects/Lwar.Assets.Effects.TexturedQuadEffect.VertexShader", "Effects/Lwar.Assets.Effects.TexturedQuadEffect.FragmentShader");

			_ConstantBuffer2 = __context.CreateConstantBuffer(64, 2);
		}

		public Matrix World
		{
			get { return _World; }
			set
			{
				_World = value;
				__ConstantBuffer2Dirty = true;
			}
		}

		public TextureBinding<Texture2D> Texture { get; set; }

		public EffectTechnique DrawQuad { get; private set; }

		/// <summary>
		///   Binds all textures and non-shared constant buffers required by the effect.
		/// </summary>
		private unsafe void Bind()
		{
			if (__ConstantBuffer2Dirty)
			{
				var __data = new __ConstantBuffer2();
				__data.World = World;

				__ConstantBuffer2Dirty = false;
				__context.Update(_ConstantBuffer2, &__data);
			}

			__context.Bind(Texture, 0);
			__context.Bind(_ConstantBuffer2);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void __OnDisposing()
		{
			_ConstantBuffer2.SafeDispose();
		}

		[StructLayout(LayoutKind.Sequential, Size = 64)]
		private struct __ConstantBuffer2
		{
			public Matrix World;
		}
	}
}

