namespace Pegasus.Platform.Graphics
{
	using System;
	using System.Diagnostics;

	/// <summary>
	///   Describes a state a pipeline stage. Objects of this class are immutable once
	///   they have been bound to the pipeline for the first time. In debug builds, an exception is
	///   thrown if an attempt is made to change a property after the state has been bound to the pipeline;
	///   in release builds, any such changes are simply ignored.
	/// </summary>
	public abstract class PipelineState : GraphicsObject
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		protected PipelineState(GraphicsDevice graphicsDevice)
			: base(graphicsDevice)
		{
		}

		/// <summary>
		///   Gets or sets the native state instance.
		/// </summary>
		protected IntPtr State { get; set; }

		/// <summary>
		///   Compiles the state; the state can no longer be changed afterwards.
		/// </summary>
		protected void CompileIfNecessary()
		{
			if (State == IntPtr.Zero)
				Compile();

#if DEBUG
			SetName(Name);
#endif
		}

		/// <summary>
		///   Compiles the pipeline state object.
		/// </summary>
		protected abstract void Compile();

		/// <summary>
		///   Checks whether the object is already disposed or whether it has already been compiled.
		/// </summary>
		[Conditional("DEBUG"), DebuggerHidden]
		protected void CheckIfCompiledOrDisposed()
		{
			Assert.NotDisposed(this);
			Assert.That(State == IntPtr.Zero, "The object cannot be changed as it has already been bound to the pipeline.");
		}
	}
}