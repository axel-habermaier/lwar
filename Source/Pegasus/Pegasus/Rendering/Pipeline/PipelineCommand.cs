namespace Pegasus.Rendering.Pipeline
{
	using System;
	using Platform.Memory;

	/// <summary>
	///     Represents a unit of work of the rendering pipeline.
	/// </summary>
	public abstract class PipelineCommand : DisposableObject
	{
		/// <summary>
		///     Get the render context that is used by the pipeline command.
		/// </summary>
		public RenderContext RenderContext
		{
			get { return Pipeline.RenderContext; }
		}

		/// <summary>
		///     The rendering pipeline the command belongs to.
		/// </summary>
		public RenderPipeline Pipeline { get; internal set; }

		/// <summary>
		///     Gets or sets the render output the command draws its final contents to.
		/// </summary>
		public RenderOutput Output { get; set; }

		/// <summary>
		///     Executes the command, performing the required rendering operations.
		/// </summary>
		public abstract void Execute();
	}
}