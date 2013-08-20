using System;

namespace Pegasus.Platform.Graphics
{
	using System.Diagnostics;
	using System.Runtime.InteropServices;
	using Memory;

	/// <summary>
	///   Describes the state of the rasterizer pipeline stage. Objects of this class are immutable once
	///   they have been bound to the pipeline for the first time. In debug builds, an exception is
	///   thrown if an attempt is made to change a property after the state has been bound to the pipeline;
	///   in release builds, any such changes are simply ignored.
	/// </summary>
	public sealed class RasterizerState : PipelineState
	{
		/// <summary>
		///   The description of the rasterizer state.
		/// </summary>
		private RasterizerDescription _description;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		public RasterizerState(GraphicsDevice graphicsDevice)
			: base(graphicsDevice)
		{
			NativeMethods.SetRasterizerDescDefaults(out _description);
		}

		/// <summary>
		///   Gets a default rasterizer state with culling disabled.
		/// </summary>
		public static RasterizerState CullNone { get; private set; }

		/// <summary>
		///   Gets a default rasterizer state with clockwise culling enabled.
		/// </summary>
		public static RasterizerState CullClockwise { get; private set; }

		/// <summary>
		///   Gets a default rasterizer state with counter-clockwise culling enabled.
		/// </summary>
		public static RasterizerState CullCounterClockwise { get; private set; }

		/// <summary>
		///   Gets or sets the fill mode.
		/// </summary>
		public FillMode FillMode
		{
			get { return _description.FillMode; }
			set
			{
				CheckIfCompiledOrDisposed();
				_description.FillMode = value;
			}
		}

		/// <summary>
		///   Gets or sets the cull mode.
		/// </summary>
		public CullMode CullMode
		{
			get { return _description.CullMode; }
			set
			{
				CheckIfCompiledOrDisposed();
				_description.CullMode = value;
			}
		}

		/// <summary>
		///   Gets or sets a value indicating whether counter-clockwise faces are front-facing.
		/// </summary>
		public bool FrontIsCounterClockwise
		{
			get { return _description.FrontIsCounterClockwise; }
			set
			{
				CheckIfCompiledOrDisposed();
				_description.FrontIsCounterClockwise = value;
			}
		}

		/// <summary>
		///   Gets or sets the depth bias.
		/// </summary>
		public int DepthBias
		{
			get { return _description.DepthBias; }
			set
			{
				CheckIfCompiledOrDisposed();
				_description.DepthBias = value;
			}
		}

		/// <summary>
		///   Gets or sets the depth bias clamp.
		/// </summary>
		public float DepthBiasClamp
		{
			get { return _description.DepthBiasClamp; }
			set
			{
				CheckIfCompiledOrDisposed();
				_description.DepthBiasClamp = value;
			}
		}

		/// <summary>
		///   Gets or sets the slope scaled depth bias.
		/// </summary>
		public float SlopeScaledDepthBias
		{
			get { return _description.SlopeScaledDepthBias; }
			set
			{
				CheckIfCompiledOrDisposed();
				_description.SlopeScaledDepthBias = value;
			}
		}

		/// <summary>
		///   Gets or sets a value indicating whether depth clip is enabled.
		/// </summary>
		public bool DepthClipEnabled
		{
			get { return _description.DepthClipEnabled; }
			set
			{
				CheckIfCompiledOrDisposed();
				_description.DepthClipEnabled = value;
			}
		}

		/// <summary>
		///   Gets or sets a value indicating whether the scissor test is enabled.
		/// </summary>
		public bool ScissorEnabled
		{
			get { return _description.ScissorEnabled; }
			set
			{
				CheckIfCompiledOrDisposed();
				_description.ScissorEnabled = value;
			}
		}

		/// <summary>
		///   Gets or sets a value indicating whether multisampling is enabled.
		/// </summary>
		public bool MultisampleEnabled
		{
			get { return _description.MultisampleEnabled; }
			set
			{
				CheckIfCompiledOrDisposed();
				_description.MultisampleEnabled = value;
			}
		}

		/// <summary>
		///   Gets or sets a value indicating whether lines are antialiased.
		/// </summary>
		public bool AntialiasedLineEnabled
		{
			get { return _description.AntialiasedLineEnabled; }
			set
			{
				CheckIfCompiledOrDisposed();
				_description.AntialiasedLineEnabled = value;
			}
		}

		/// <summary>
		///   Initializes the default instances.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with the default instances.</param>
		internal static void InitializeDefaultInstances(GraphicsDevice graphicsDevice)
		{
			CullNone = new RasterizerState(graphicsDevice)
			{
				CullMode = CullMode.None
			};

			CullClockwise = new RasterizerState(graphicsDevice)
			{
				CullMode = CullMode.Back,
				FrontIsCounterClockwise = true
			};

			CullCounterClockwise = new RasterizerState(graphicsDevice)
			{
				CullMode = CullMode.Back,
				FrontIsCounterClockwise = false
			};

			CullNone.SetName("CullNone");
			CullClockwise.SetName("CullClockwise");
			CullCounterClockwise.SetName("CullCounterClockwise");

			CullNone.Bind();
		}

		/// <summary>
		///   Disposes the default instances.
		/// </summary>
		internal static void DisposeDefaultInstances()
		{
			CullNone.SafeDispose();
			CullClockwise.SafeDispose();
			CullCounterClockwise.SafeDispose();
		}

		/// <summary>
		///   Compiles the pipeline state object.
		/// </summary>
		protected override void Compile()
		{
			State = NativeMethods.CreateRasterizerState(GraphicsDevice.NativePtr, ref _description);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			NativeMethods.DestroyRasterizerState(State);
		}

		/// <summary>
		///   Sets the state on the pipeline. The state can no longer be changed afterwards.
		/// </summary>
		public void Bind()
		{
			Assert.NotDisposed(this);

			CompileIfNecessary();
			NativeMethods.BindRasterizerState(State);
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
		///   Provides access to the native rasterizer state functions.
		/// </summary>
#if !DEBUG
		[System.Security.SuppressUnmanagedCodeSecurity]
#endif
		private static class NativeMethods
		{
			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgCreateRasterizerState")]
			public static extern IntPtr CreateRasterizerState(IntPtr device, ref RasterizerDescription description);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgDestroyRasterizerState")]
			public static extern void DestroyRasterizerState(IntPtr rasterizerState);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgBindRasterizerState")]
			public static extern void BindRasterizerState(IntPtr rasterizerState);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgSetRasterizerDescDefaults")]
			public static extern void SetRasterizerDescDefaults(out RasterizerDescription description);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgSetRasterizerStateName")]
			[Conditional("DEBUG")]
			public static extern void SetName(IntPtr rasterizerState, string name);
		}

		/// <summary>
		///   Describes the rasterizer state.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		private struct RasterizerDescription
		{
			public FillMode FillMode;
			public CullMode CullMode;
			public int DepthBias;
			public float DepthBiasClamp;
			public float SlopeScaledDepthBias;
			public bool AntialiasedLineEnabled;
			public bool DepthClipEnabled;
			public bool FrontIsCounterClockwise;
			public bool MultisampleEnabled;
			public bool ScissorEnabled;
		}
	}
}