namespace Pegasus.Rendering.Pipeline
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using Platform.Memory;
	using Utilities;

	/// <summary>
	///     Represents the render pipeline that is used to draw all visual content of the application.
	/// </summary>
	public class RenderPipeline : DisposableObject
	{
		/// <summary>
		///     The pipeline commands managed by the pipeline.
		/// </summary>
		private readonly List<PipelineCommand> _commands = new List<PipelineCommand>(8);

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="renderContext">The render context that should be used by the rendering pipeline.</param>
		/// <param name="renderOutput">The render output the rendering pipeline should render to.</param>
		public RenderPipeline(RenderContext renderContext, RenderOutput renderOutput)
		{
			Assert.ArgumentNotNull(renderContext);
			Assert.ArgumentNotNull(renderOutput);

			RenderContext = renderContext;
			RenderOutput = renderOutput;
		}

		/// <summary>
		///     Gets the number of commands contained in the pipeline.
		/// </summary>
		public int CommandCount
		{
			get { return _commands.Count; }
		}

		/// <summary>
		///     Gets the command at the given index.
		/// </summary>
		/// <param name="commandIndex">The index of the command that should be returned.</param>
		public PipelineCommand this[int commandIndex]
		{
			get { return _commands[commandIndex]; }
		}

		/// <summary>
		///     Get the render context that is used by the rendering pipeline.
		/// </summary>
		public RenderContext RenderContext { get; private set; }

		/// <summary>
		///     Gets the render output the rendering pipeline renders to.
		/// </summary>
		public RenderOutput RenderOutput { get; private set; }

		/// <summary>
		///     Executes the render pipeline, updating the window's contents.
		/// </summary>
		public void Execute()
		{
			Assert.That(_commands.Count > 0, "The pipeline must contain at least one command before it can be executed.");
			Assert.That(_commands[_commands.Count - 1].Output == RenderOutput, "The last command must draw to the pipeline's output.");

			foreach (var command in _commands)
				command.Execute();
		}

		/// <summary>
		///     Appends the given command to the end of the pipeline.
		/// </summary>
		/// <param name="command">The command that should be appended.</param>
		public void Append(PipelineCommand command)
		{
			EnsureCanAdd(command);

			_commands.Add(command);
			command.Pipeline = this;
		}

		/// <summary>
		///     Prepends the given command to the beginning of the pipeline.
		/// </summary>
		/// <param name="command">The command that should be prepended.</param>
		public void Prepend(PipelineCommand command)
		{
			EnsureCanAdd(command);

			_commands.Insert(0, command);
			command.Pipeline = this;
		}

		/// <summary>
		///     Inserts the given command immediately before the successor command into the pipeline.
		/// </summary>
		/// <param name="command">The command that should be inserted.</param>
		/// <param name="successorCommand">The command that should immediately follow the inserted command.</param>
		public void InsertBefore(PipelineCommand command, PipelineCommand successorCommand)
		{
			EnsureCanAdd(command);
			Assert.ArgumentNotNull(successorCommand);
			Assert.ArgumentSatisfies(_commands.Contains(successorCommand), "The pipeline does not contain the successor command.");

			_commands.Insert(_commands.IndexOf(successorCommand), command);
			command.Pipeline = this;
		}

		/// <summary>
		///     Inserts the given command immediately after the predecessor command into the pipeline.
		/// </summary>
		/// <param name="command">The command that should be inserted.</param>
		/// <param name="predecessorCommand">The command that should immediately precede the inserted command.</param>
		public void InsertAfter(PipelineCommand command, PipelineCommand predecessorCommand)
		{
			EnsureCanAdd(command);
			Assert.ArgumentNotNull(predecessorCommand);
			Assert.ArgumentSatisfies(_commands.Contains(predecessorCommand), "The pipeline does not contain the predecessor command.");

			_commands.Insert(_commands.IndexOf(predecessorCommand) + 1, command);
			command.Pipeline = this;
		}

		/// <summary>
		///     Removes the given command from the pipeline.
		/// </summary>
		/// <param name="command">The command that should be removed.</param>
		public void Remove(PipelineCommand command)
		{
			Assert.ArgumentNotNull(command);
			Assert.ArgumentSatisfies(_commands.Contains(command), "The pipeline does not contain the command.");

			_commands.Remove(command);
			command.Pipeline = null;
		}

		/// <summary>
		///     Checks whether the given command can be added to the pipeline.
		/// </summary>
		/// <param name="command">The command that should be checked.</param>
		[Conditional("DEBUG"), DebuggerHidden]
		private void EnsureCanAdd(PipelineCommand command)
		{
			Assert.ArgumentNotNull(command);
			Assert.ArgumentSatisfies(!_commands.Contains(command), "The command has already been added to the pipeline.");
			Assert.ArgumentSatisfies(command.Pipeline == null, "The command already belongs to a pipeline.");
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_commands.SafeDisposeAll();
		}
	}
}