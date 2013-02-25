using System;

namespace Pegasus.Framework.Platform.Graphics
{
	using System.Runtime.InteropServices;

	/// <summary>
	///   Describes a depth stencil state of the output merger pipeline stage. Objects of this class are immutable once
	///   they have been bound to the pipeline for the first time. In debug builds, an exception is
	///   thrown if an attempt is made to change a property after the state has been bound to the pipeline;
	///   in release builds, any such changes are simply ignored.
	/// </summary>
	public sealed class DepthStencilState : PipelineState
	{
		/// <summary>
		///   The description of the depth stencil state.
		/// </summary>
		private DepthStencilDescription _description;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		public DepthStencilState(GraphicsDevice graphicsDevice)
			: base(graphicsDevice)
		{
			NativeMethods.SetDepthStencilDescDefaults(out _description);
		}

		/// <summary>
		///   Gets or sets a value indicating whether the depth test is enabled.
		/// </summary>
		public bool DepthEnabled
		{
			get { return _description.DepthEnabled; }
			set
			{
				CheckIfCompiledOrDisposed();
				_description.DepthEnabled = value;
			}
		}

		/// <summary>
		///   Gets or sets a value indicating whether depth writes are enabled.
		/// </summary>
		public bool DepthWriteEnabled
		{
			get { return _description.DepthWriteEnabled; }
			set
			{
				CheckIfCompiledOrDisposed();
				_description.DepthWriteEnabled = value;
			}
		}

		/// <summary>
		///   Gets or sets the depth comparison function.
		/// </summary>
		public Comparison DepthFunction
		{
			get { return _description.DepthFunction; }
			set
			{
				CheckIfCompiledOrDisposed();
				_description.DepthFunction = value;
			}
		}

		/// <summary>
		///   Gets or sets a value indicating whether the stencil test is enabled.
		/// </summary>
		public bool StencilEnabled
		{
			get { return _description.StencilEnabled; }
			set
			{
				CheckIfCompiledOrDisposed();
				_description.StencilEnabled = value;
			}
		}

		/// <summary>
		///   Gets or sets the stencil read mask.
		/// </summary>
		public byte StencilReadMask
		{
			get { return _description.StencilReadMask; }
			set
			{
				CheckIfCompiledOrDisposed();
				_description.StencilReadMask = value;
			}
		}

		/// <summary>
		///   Gets or sets the stencil write mask.
		/// </summary>
		public byte StencilWriteMask
		{
			get { return _description.StencilWriteMask; }
			set
			{
				CheckIfCompiledOrDisposed();
				_description.StencilWriteMask = value;
			}
		}

		/// <summary>
		///   Gets or sets the stencil operation description for front faces.
		/// </summary>
		public StencilOperationDescription FrontFace
		{
			get { return _description.FrontFace; }
			set
			{
				CheckIfCompiledOrDisposed();
				_description.FrontFace = value;
			}
		}

		/// <summary>
		///   Gets or sets the stencil operation description for back faces.
		/// </summary>
		public StencilOperationDescription BackFace
		{
			get { return _description.BackFace; }
			set
			{
				CheckIfCompiledOrDisposed();
				_description.BackFace = value;
			}
		}

		/// <summary>
		///   Gets the default depth stencil state with depth read and write enabled, less comparison operation
		///   and stencil operations disabled.
		/// </summary>
		public static DepthStencilState Default { get; private set; }

		/// <summary>
		///   Gets a default depth stencil state with all depth and stencil operations disabled.
		/// </summary>
		public static DepthStencilState DepthDisabled { get; private set; }

		/// <summary>
		///   Gets a default depth stencil state with depth writes disabled and all stencil operations disabled.
		/// </summary>
		public static DepthStencilState DepthRead { get; private set; }

		/// <summary>
		///   Initializes the default instances.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with the default instances.</param>
		internal static void InitializeDefaultInstances(GraphicsDevice graphicsDevice)
		{
			Default = new DepthStencilState(graphicsDevice);
			DepthDisabled = new DepthStencilState(graphicsDevice) { DepthEnabled = false, DepthWriteEnabled = false };
			DepthRead = new DepthStencilState(graphicsDevice) { DepthWriteEnabled = false };

			DepthDisabled.Bind();
		}

		/// <summary>
		///   Disposes the default instances.
		/// </summary>
		internal static void DisposeDefaultInstances()
		{
			Default.SafeDispose();
			Default = null;

			DepthDisabled.SafeDispose();
			DepthDisabled = null;

			DepthRead.SafeDispose();
			DepthRead = null;
		}

		/// <summary>
		///   Compiles the pipeline state object.
		/// </summary>
		protected override void Compile()
		{
			State = NativeMethods.CreateDepthStencilState(GraphicsDevice.NativePtr, ref _description);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			NativeMethods.DestroyDepthStencilState(State);
		}

		/// <summary>
		///   Sets the state on the pipeline. The state can no longer be changed afterwards.
		/// </summary>
		public void Bind()
		{
			Assert.NotDisposed(this);

			CompileIfNecessary();
			NativeMethods.BindDepthStencilState(State);
		}

		/// <summary>
		///   Describes the depth stencil state.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		private struct DepthStencilDescription
		{
			public StencilOperationDescription BackFace;
			public StencilOperationDescription FrontFace;
			public Comparison DepthFunction;
			public bool DepthEnabled;
			public bool DepthWriteEnabled;
			public bool StencilEnabled;
			public byte StencilReadMask;
			public byte StencilWriteMask;
		}

		/// <summary>
		///   Provides access to the native depth stencil state functions.
		/// </summary>
#if !DEBUG
		[System.Security.SuppressUnmanagedCodeSecurity]
#endif
		private static class NativeMethods
		{
			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgCreateDepthStencilState")]
			public static extern IntPtr CreateDepthStencilState(IntPtr device, ref DepthStencilDescription description);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgDestroyDepthStencilState")]
			public static extern void DestroyDepthStencilState(IntPtr depthStencilState);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgBindDepthStencilState")]
			public static extern void BindDepthStencilState(IntPtr depthStencilState);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgSetDepthStencilDescDefaults")]
			public static extern void SetDepthStencilDescDefaults(out DepthStencilDescription description);
		}
	}
}