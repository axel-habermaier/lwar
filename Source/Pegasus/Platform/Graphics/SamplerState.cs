namespace Pegasus.Platform.Graphics
{
	using System;
	using System.Diagnostics;
	using System.Runtime.InteropServices;
	using System.Security;
	using Memory;

	/// <summary>
	///     Describes a sampler state of a shader pipeline stage. Objects of this class are immutable once
	///     they have been bound to the pipeline for the first time. In debug builds, an exception is
	///     thrown if an attempt is made to change a property after the state has been bound to the pipeline;
	///     in release builds, any such changes are simply ignored.
	/// </summary>
	public sealed class SamplerState : PipelineState
	{
		/// <summary>
		///     The description of the sampler state.
		/// </summary>
		private SamplerDescription _description;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public SamplerState()
		{
			NativeMethods.SetSamplerDescDefaults(out _description);
		}

		/// <summary>
		///     Gets a default sampler state with point-filtering and wrap address mode.
		/// </summary>
		public static SamplerState PointWrap { get; private set; }

		/// <summary>
		///     Gets a default sampler state with point-filtering and clamp address mode.
		/// </summary>
		public static SamplerState PointClamp { get; private set; }

		/// <summary>
		///     Gets a default sampler state with point-filtering and wrap address mode for textures without mipmaps.
		/// </summary>
		public static SamplerState PointWrapNoMipmaps { get; private set; }

		/// <summary>
		///     Gets a default sampler state with point-filtering and clamp address mode for textures without mipmaps.
		/// </summary>
		public static SamplerState PointClampNoMipmaps { get; private set; }

		/// <summary>
		///     Gets a default sampler state with bilinear filtering and wrap address mode.
		/// </summary>
		public static SamplerState BilinearWrap { get; private set; }

		/// <summary>
		///     Gets a default sampler state with bilinear filtering and clamp address mode.
		/// </summary>
		public static SamplerState BilinearClamp { get; private set; }

		/// <summary>
		///     Gets a default sampler state with bilinear filtering and wrap address mode for textures without mipmaps.
		/// </summary>
		public static SamplerState BilinearWrapNoMipmaps { get; private set; }

		/// <summary>
		///     Gets a default sampler state with bilinear filtering and clamp address mode for textures without mipmaps.
		/// </summary>
		public static SamplerState BilinearClampNoMipmaps { get; private set; }

		/// <summary>
		///     Gets a default sampler state with trilinea filtering and wrap address mode.
		/// </summary>
		public static SamplerState TrilinearWrap { get; private set; }

		/// <summary>
		///     Gets a default sampler state with trilinear filtering and clamp address mode.
		/// </summary>
		public static SamplerState TrilinearClamp { get; private set; }

		/// <summary>
		///     Gets a default sampler state with anisotropic filtering and wrap address mode.
		/// </summary>
		public static SamplerState AnisotropicWrap { get; private set; }

		/// <summary>
		///     Gets a default sampler state with anisotropic filtering and clamp address mode.
		/// </summary>
		public static SamplerState AnisotropicClamp { get; private set; }

		/// <summary>
		///     Gets or sets the texture filter.
		/// </summary>
		public TextureFilter Filter
		{
			get { return _description.Filter; }
			set
			{
				CheckIfCompiledOrDisposed();
				_description.Filter = value;
			}
		}

		/// <summary>
		///     Gets or sets the address mode along the U axis.
		/// </summary>
		public TextureAddressMode AddressU
		{
			get { return _description.AddressU; }
			set
			{
				CheckIfCompiledOrDisposed();
				_description.AddressU = value;
			}
		}

		/// <summary>
		///     Gets or sets the address mode along the V axis.
		/// </summary>
		public TextureAddressMode AddressV
		{
			get { return _description.AddressV; }
			set
			{
				CheckIfCompiledOrDisposed();
				_description.AddressV = value;
			}
		}

		/// <summary>
		///     Gets or sets the address mode along the W axis.
		/// </summary>
		public TextureAddressMode AddressW
		{
			get { return _description.AddressW; }
			set
			{
				CheckIfCompiledOrDisposed();
				_description.AddressW = value;
			}
		}

		/// <summary>
		///     Gets or sets the offset from the calculated mipmap level.
		/// </summary>
		public float MipLodBias
		{
			get { return _description.MipLodBias; }
			set
			{
				CheckIfCompiledOrDisposed();
				_description.MipLodBias = value;
			}
		}

		/// <summary>
		///     Gets or sets the maximum anisotropy value.
		/// </summary>
		public int MaximumAnisotropy
		{
			get { return _description.MaximumAnisotropy; }
			set
			{
				CheckIfCompiledOrDisposed();
				_description.MaximumAnisotropy = value;
			}
		}

		/// <summary>
		///     Gets or sets the comparison function.
		/// </summary>
		public Comparison Comparison
		{
			get { return _description.Comparison; }
			set
			{
				CheckIfCompiledOrDisposed();
				_description.Comparison = value;
			}
		}

		/// <summary>
		///     Gets or sets the border color.
		/// </summary>
		public Color BorderColor
		{
			get { return _description.BorderColor; }
			set
			{
				CheckIfCompiledOrDisposed();
				_description.BorderColor = value;
			}
		}

		/// <summary>
		///     Gets or sets the minimum LOD.
		/// </summary>
		public float MinimumLod
		{
			get { return _description.MinimumLod; }
			set
			{
				CheckIfCompiledOrDisposed();
				_description.MinimumLod = value;
			}
		}

		/// <summary>
		///     Gets or sets the maximum LOD.
		/// </summary>
		public float MaximumLod
		{
			get { return _description.MaximumLod; }
			set
			{
				CheckIfCompiledOrDisposed();
				_description.MaximumLod = value;
			}
		}

		/// <summary>
		///     Initializes the default instances.
		/// </summary>
		internal static void InitializeDefaultInstances()
		{
			PointWrap = new SamplerState()
			{
				AddressU = TextureAddressMode.Wrap,
				AddressV = TextureAddressMode.Wrap,
				AddressW = TextureAddressMode.Wrap,
				Filter = TextureFilter.Nearest
			};

			PointClamp = new SamplerState() { Filter = TextureFilter.Nearest };

			PointWrapNoMipmaps = new SamplerState()
			{
				AddressU = TextureAddressMode.Wrap,
				AddressV = TextureAddressMode.Wrap,
				AddressW = TextureAddressMode.Wrap,
				Filter = TextureFilter.NearestNoMipmaps
			};

			PointClampNoMipmaps = new SamplerState() { Filter = TextureFilter.NearestNoMipmaps };

			BilinearWrap = new SamplerState()
			{
				AddressU = TextureAddressMode.Wrap,
				AddressV = TextureAddressMode.Wrap,
				AddressW = TextureAddressMode.Wrap,
			};

			BilinearClamp = new SamplerState();

			BilinearWrapNoMipmaps = new SamplerState()
			{
				AddressU = TextureAddressMode.Wrap,
				AddressV = TextureAddressMode.Wrap,
				AddressW = TextureAddressMode.Wrap,
				Filter = TextureFilter.BilinearNoMipmaps
			};

			BilinearClampNoMipmaps = new SamplerState() { Filter = TextureFilter.BilinearNoMipmaps };

			TrilinearWrap = new SamplerState()
			{
				AddressU = TextureAddressMode.Wrap,
				AddressV = TextureAddressMode.Wrap,
				AddressW = TextureAddressMode.Wrap,
				Filter = TextureFilter.Trilinear,
				MaximumAnisotropy = 1
			};

			TrilinearClamp = new SamplerState() { Filter = TextureFilter.Trilinear, MaximumAnisotropy = 1 };

			AnisotropicWrap = new SamplerState()
			{
				AddressU = TextureAddressMode.Wrap,
				AddressV = TextureAddressMode.Wrap,
				AddressW = TextureAddressMode.Wrap,
				Filter = TextureFilter.Anisotropic
			};

			AnisotropicClamp = new SamplerState() { Filter = TextureFilter.Anisotropic };

			PointWrap.SetName("PointWrap");
			PointClamp.SetName("PointClamp");
			PointWrapNoMipmaps.SetName("PointWrapNoMipmaps");
			PointClampNoMipmaps.SetName("PointClampNoMipmaps");
			BilinearWrap.SetName("BilinearWrap");
			BilinearClamp.SetName("BilinearClamp");
			PointWrap.SetName("PointWrap");
			BilinearWrapNoMipmaps.SetName("BilinearWrapNoMipmaps");
			BilinearClampNoMipmaps.SetName("BilinearClampNoMipmaps");
			TrilinearWrap.SetName("TrilinearWrap");
			TrilinearClamp.SetName("TrilinearClamp");
			AnisotropicWrap.SetName("AnisotropicWrap");
			AnisotropicClamp.SetName("AnisotropicClamp");
		}

		/// <summary>
		///     Disposes the default instances.
		/// </summary>
		internal static void DisposeDefaultInstances()
		{
			PointWrap.SafeDispose();
			PointClamp.SafeDispose();
			PointWrapNoMipmaps.SafeDispose();
			PointClampNoMipmaps.SafeDispose();

			BilinearWrap.SafeDispose();
			BilinearClamp.SafeDispose();
			BilinearWrapNoMipmaps.SafeDispose();
			BilinearClampNoMipmaps.SafeDispose();

			TrilinearWrap.SafeDispose();
			TrilinearClamp.SafeDispose();

			AnisotropicWrap.SafeDispose();
			AnisotropicClamp.SafeDispose();
		}

		/// <summary>
		///     Binds the state on the given slot.
		/// </summary>
		/// <param name="slot">The slot the sampler state should be bound to.</param>
		internal void Bind(int slot)
		{
			Assert.NotDisposed(this);

			CompileIfNecessary();
			NativeMethods.BindSamplerState(State, slot);
		}

		/// <summary>
		///     Compiles the pipeline state object.
		/// </summary>
		protected override void Compile()
		{
			State = NativeMethods.CreateSamplerState(GraphicsDevice.Current.NativePtr, ref _description);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			NativeMethods.DestroySamplerState(State);
		}

#if DEBUG
		/// <summary>
		///   Invoked after the name of the graphics object has changed. This method is only available in debug builds.
		/// </summary>
		protected override void OnRenamed()
		{
			if (State != IntPtr.Zero)
				NativeMethods.SetName(State, Name);
		}
#endif

		/// <summary>
		///     Provides access to the native sampler state functions.
		/// </summary>
#if !DEBUG
		[SuppressUnmanagedCodeSecurity]
#endif
		private static class NativeMethods
		{
			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgCreateSamplerState")]
			public static extern IntPtr CreateSamplerState(IntPtr device, ref SamplerDescription description);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgDestroySamplerState")]
			public static extern void DestroySamplerState(IntPtr samplerState);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgBindSamplerState")]
			public static extern void BindSamplerState(IntPtr samplerState, int slot);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgSetSamplerDescDefaults")]
			public static extern void SetSamplerDescDefaults(out SamplerDescription description);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgSetSamplerStateName")]
			[Conditional("DEBUG")]
			public static extern void SetName(IntPtr samplerState, string name);
		}

		/// <summary>
		///     Describes the sampler state.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		private struct SamplerDescription
		{
			public TextureAddressMode AddressU;
			public TextureAddressMode AddressV;
			public TextureAddressMode AddressW;
			public Color BorderColor;
			public Comparison Comparison;
			public TextureFilter Filter;
			public int MaximumAnisotropy;
			public float MaximumLod;
			public float MinimumLod;
			public float MipLodBias;
		}
	}
}