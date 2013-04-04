using System;

namespace Pegasus.Framework.Platform.Graphics
{
	using System.Diagnostics;
	using System.Runtime.InteropServices;
	using System.Security;

	/// <summary>
	///   Describes a blend state of the output merger pipeline stage. Objects of this class are immutable once
	///   they have been bound to the pipeline for the first time. In debug builds, an exception is
	///   thrown if an attempt is made to change a property after the state has been bound to the pipeline;
	///   in release builds, any such changes are simply ignored.
	/// </summary>
	public sealed class BlendState : PipelineState
	{
		/// <summary>
		///   The description of the blend state.
		/// </summary>
		private BlendDescription _description;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		public BlendState(GraphicsDevice graphicsDevice)
			: base(graphicsDevice)
		{
			NativeMethods.SetBlendDescDefaults(out _description);
		}

		/// <summary>
		///   Gets or sets a value indicating whether blend is enabled.
		/// </summary>
		public bool BlendEnabled
		{
			get { return _description.BlendEnabled; }
			set
			{
				CheckIfCompiledOrDisposed();
				_description.BlendEnabled = value;
			}
		}

		/// <summary>
		///   Gets or sets the blend type for the source.
		/// </summary>
		public BlendOption SourceBlend
		{
			get { return _description.SourceBlend; }
			set
			{
				CheckIfCompiledOrDisposed();
				_description.SourceBlend = value;
			}
		}

		/// <summary>
		///   Gets or sets the blend type for the destination.
		/// </summary>
		public BlendOption DestinationBlend
		{
			get { return _description.DestinationBlend; }
			set
			{
				CheckIfCompiledOrDisposed();
				_description.DestinationBlend = value;
			}
		}

		/// <summary>
		///   Gets or sets the blend operation.
		/// </summary>
		public BlendOperation BlendOperation
		{
			get { return _description.BlendOperation; }
			set
			{
				CheckIfCompiledOrDisposed();
				_description.BlendOperation = value;
			}
		}

		/// <summary>
		///   Gets or sets the blend type for the source alpha.
		/// </summary>
		public BlendOption SourceBlendAlpha
		{
			get { return _description.SourceBlendAlpha; }
			set
			{
				CheckIfCompiledOrDisposed();
				_description.SourceBlendAlpha = value;
			}
		}

		/// <summary>
		///   Gets or sets the blend type for the destination alpha.
		/// </summary>
		public BlendOption DestinationBlendAlpha
		{
			get { return _description.DestinationBlendAlpha; }
			set
			{
				CheckIfCompiledOrDisposed();
				_description.DestinationBlendAlpha = value;
			}
		}

		/// <summary>
		///   Gets or sets the blend operation for the alpha channel.
		/// </summary>
		public BlendOperation BlendOperationAlpha
		{
			get { return _description.BlendOperationAlpha; }
			set
			{
				CheckIfCompiledOrDisposed();
				_description.BlendOperationAlpha = value;
			}
		}

		/// <summary>
		///   Gets or sets a value that determines which color channel writes are enabled.
		/// </summary>
		public ColorWriteChannels WriteMask
		{
			get { return _description.WriteMask; }
			set
			{
				CheckIfCompiledOrDisposed();
				_description.WriteMask = value;
			}
		}

		/// <summary>
		///   Gets a default blend state with blending disabled.
		/// </summary>
		public static BlendState Opaque { get; private set; }

		/// <summary>
		///   Gets a default blend state for premultiplied alpha blending.
		/// </summary>
		public static BlendState Premultiplied { get; private set; }

		/// <summary>
		///   Gets a default blend state for additive alpha blending.
		/// </summary>
		public static BlendState Additive { get; private set; }

		/// <summary>
		///   Gets a default blend state for alpha blending.
		/// </summary>
		public static BlendState Alpha { get; private set; }

		/// <summary>
		///   Initializes the default instances.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with the default instances.</param>
		internal static void InitializeDefaultInstances(GraphicsDevice graphicsDevice)
		{
			Opaque = new BlendState(graphicsDevice);

			Premultiplied = new BlendState(graphicsDevice)
			{
				BlendEnabled = true,
				SourceBlend = BlendOption.One,
				DestinationBlend = BlendOption.InverseSourceAlpha,
				SourceBlendAlpha = BlendOption.One,
				DestinationBlendAlpha = BlendOption.InverseSourceAlpha
			};

			Additive = new BlendState(graphicsDevice)
			{
				BlendEnabled = true,
				SourceBlend = BlendOption.One,
				DestinationBlend = BlendOption.One,
				SourceBlendAlpha = BlendOption.One,
				DestinationBlendAlpha = BlendOption.One
			};

			Alpha = new BlendState(graphicsDevice)
			{
				BlendEnabled = true,
				SourceBlend = BlendOption.SourceAlpha,
				DestinationBlend = BlendOption.InverseSourceAlpha,
				SourceBlendAlpha = BlendOption.SourceAlpha,
				DestinationBlendAlpha = BlendOption.InverseSourceAlpha
			};

			Opaque.SetName("Opaque");
			Premultiplied.SetName("Premultiplied");
			Additive.SetName("Additive");
			Alpha.SetName("Alpha");

			Opaque.Bind();
		}

		/// <summary>
		///   Disposes the default instances.
		/// </summary>
		internal static void DisposeDefaultInstances()
		{
			Opaque.SafeDispose();
			Premultiplied.SafeDispose();
			Additive.SafeDispose();
			Alpha.SafeDispose();
		}

		/// <summary>
		///   Compiles the pipeline state object.
		/// </summary>
		protected override void Compile()
		{
			State = NativeMethods.CreateBlendState(GraphicsDevice.NativePtr, ref _description);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			NativeMethods.DestroyBlendState(State);
		}

		/// <summary>
		///   Sets the state on the pipeline. The state can no longer be changed afterwards.
		/// </summary>
		public void Bind()
		{
			Assert.NotDisposed(this);

			CompileIfNecessary();
			NativeMethods.BindBlendState(State);
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
		///   Describes the blend state.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		private struct BlendDescription
		{
			public BlendOperation BlendOperation;
			public BlendOperation BlendOperationAlpha;
			public BlendOption DestinationBlend;
			public BlendOption DestinationBlendAlpha;
			public BlendOption SourceBlend;
			public BlendOption SourceBlendAlpha;
			public ColorWriteChannels WriteMask;
			public bool BlendEnabled;
		}

		/// <summary>
		///   Provides access to the native blend state functions.
		/// </summary>
#if !DEBUG
		[SuppressUnmanagedCodeSecurity]
#endif
		private static class NativeMethods
		{
			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgCreateBlendState")]
			public static extern IntPtr CreateBlendState(IntPtr device, ref BlendDescription description);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgDestroyBlendState")]
			public static extern void DestroyBlendState(IntPtr blendState);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgBindBlendState")]
			public static extern void BindBlendState(IntPtr blendState);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgSetBlendDescDefaults")]
			public static extern void SetBlendDescDefaults(out BlendDescription description);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgSetBlendStateName")]
			[Conditional("DEBUG")]
			public static extern void SetName(IntPtr blendState, string name);
		}
	}
}