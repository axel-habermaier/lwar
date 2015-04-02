namespace Pegasus.Rendering.Pipeline
{
	using System;
	using Platform.Memory;
	using Utilities;

	/// <summary>
	///     Instructs the render pipeline to draw batched sprites.
	/// </summary>
	public sealed class SpriteBatchCommand : PipelineCommand
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="renderContext">The render context that should be used to initialize the sprite batch.</param>
		public SpriteBatchCommand(RenderContext renderContext)
			: this(new SpriteBatch(renderContext))
		{
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be drawn. The command takes ownership of the batch and disposes it.</param>
		public SpriteBatchCommand(SpriteBatch spriteBatch)
		{
			Assert.ArgumentNotNull(spriteBatch);
			SpriteBatch = spriteBatch;
		}

		/// <summary>
		///     Gets the sprite batch that is drawn by the command.
		/// </summary>
		public SpriteBatch SpriteBatch { get; private set; }

		/// <summary>
		///     Executes the command, performing the required rendering operations.
		/// </summary>
		public override void Execute()
		{
			Assert.NotNull(Output);
			SpriteBatch.DrawBatch(Output);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			SpriteBatch.SafeDispose();
		}
	}
}