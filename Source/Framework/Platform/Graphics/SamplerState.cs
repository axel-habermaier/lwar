using System;

namespace Pegasus.Framework.Platform.Graphics
{
	using System.Runtime.InteropServices;

	/// <summary>
	///   Describes a sampler state of a shader pipeline stage. Objects of this class are immutable once
	///   they have been bound to the pipeline for the first time. In debug builds, an exception is
	///   thrown if an attempt is made to change a property after the state has been bound to the pipeline;
	///   in release builds, any such changes are simply ignored.
	/// </summary>
	public sealed class SamplerState : PipelineState
	{
		/// <summary>
		///   The description of the sampler state.
		/// </summary>
		private SamplerDescription _description;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		public SamplerState(GraphicsDevice graphicsDevice)
			: base(graphicsDevice)
		{
			NativeMethods.SetSamplerDescDefaults(out _description);
		}

		/// <summary>
		///   Gets a default sampler state with point-filtering and wrap address mode.
		/// </summary>
		public static SamplerState PointWrap { get; private set; }

		/// <summary>
		///   Gets a default sampler state with point-filtering and clamp address mode.
		/// </summary>
		public static SamplerState PointClamp { get; private set; }

		/// <summary>
		///   Gets a default sampler state with linear filtering and wrap address mode.
		/// </summary>
		public static SamplerState LinearWrap { get; private set; }

		/// <summary>
		///   Gets a default sampler state with linear filtering and clamp address mode.
		/// </summary>
		public static SamplerState LinearClamp { get; private set; }

		/// <summary>
		///   Gets a default sampler state with anisotropic filtering and wrap address mode.
		/// </summary>
		public static SamplerState AnisotropicWrap { get; private set; }

		/// <summary>
		///   Gets a default sampler state with anisotropic filtering and clamp address mode.
		/// </summary>
		public static SamplerState AnisotropicClamp { get; private set; }

		/// <summary>
		///   Gets or sets the texture filter.
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
		///   Gets or sets the address mode along the U axis.
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
		///   Gets or sets the address mode along the V axis.
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
		///   Gets or sets the address mode along the W axis.
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
		///   Gets or sets the offset from the calculated mipmap level.
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
		///   Gets or sets the maximum anisotropy value.
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
		///   Gets or sets the comparison function.
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
		///   Gets or sets the border color.
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
		///   Gets or sets the minimum LOD.
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
		///   Gets or sets the maximum LOD.
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
		///   Initializes the default instances.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with the default instances.</param>
		internal static void InitializeDefaultInstances(GraphicsDevice graphicsDevice)
		{
			PointWrap = new SamplerState(graphicsDevice)
			{
				AddressU = TextureAddressMode.Wrap,
				AddressV = TextureAddressMode.Wrap,
				AddressW = TextureAddressMode.Wrap,
				Filter = TextureFilter.Nearest
			};

			PointClamp = new SamplerState(graphicsDevice) { Filter = TextureFilter.Nearest };

			LinearWrap = new SamplerState(graphicsDevice)
			{
				AddressU = TextureAddressMode.Wrap,
				AddressV = TextureAddressMode.Wrap,
				AddressW = TextureAddressMode.Wrap,
			};

			LinearClamp = new SamplerState(graphicsDevice);

			AnisotropicWrap = new SamplerState(graphicsDevice)
			{
				AddressU = TextureAddressMode.Wrap,
				AddressV = TextureAddressMode.Wrap,
				AddressW = TextureAddressMode.Wrap,
				Filter = TextureFilter.Anisotropic
			};

			AnisotropicClamp = new SamplerState(graphicsDevice) { Filter = TextureFilter.Anisotropic };
		}

		/// <summary>
		///   Disposes the default instances.
		/// </summary>
		internal static void DisposeDefaultInstances()
		{
			PointWrap.SafeDispose();
			PointWrap = null;

			PointClamp.SafeDispose();
			PointClamp = null;

			LinearWrap.SafeDispose();
			LinearWrap = null;

			LinearClamp.SafeDispose();
			LinearClamp = null;

			AnisotropicWrap.SafeDispose();
			AnisotropicWrap = null;

			AnisotropicClamp.SafeDispose();
			AnisotropicClamp = null;
		}

		/// <summary>
		///   Binds the state on the given slot.
		/// </summary>
		/// <param name="slot">The slot the sampler state should be bound to.</param>
		public void Bind(int slot)
		{
			Assert.NotDisposed(this);
			Assert.ArgumentInRange(slot, () => slot, 0, GraphicsDevice.SamplerSlotCount);

			if (DeviceState.Samplers[slot] == this)
				return;

			DeviceState.Samplers[slot] = this;
			CompileIfNecessary();

			NativeMethods.BindSamplerState(State, slot);
		}

		/// <summary>
		///   Compiles the pipeline state object.
		/// </summary>
		protected override void Compile()
		{
			State = NativeMethods.CreateSamplerState(GraphicsDevice.NativePtr, ref _description);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			if (State != IntPtr.Zero)
				NativeMethods.DestroySamplerState(State);
		}

		/// <summary>
		///   Provides access to the native sampler state functions.
		/// </summary>
#if !DEBUG
		[System.Security.SuppressUnmanagedCodeSecurity]
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
		}

		/// <summary>
		///   Describes the sampler state.
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