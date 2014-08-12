namespace Pegasus.Framework.UserInterface.ViewModels
{
	using System;
	using Platform.Logging;
	using Platform.Memory;
	using Scripting;

	/// <summary>
	///     Displays the in-game console.
	/// </summary>
	internal class ConsoleViewModel : DisposableNotifyPropertyChanged
	{
		/// <summary>
		///     The maximum allowed length of the console prompt input and displayed log entries.
		/// </summary>
		public const int MaxLength = 1024;

		/// <summary>
		///     Indicates whether the console is visible.
		/// </summary>
		private bool _isVisible;

		/// <summary>
		///     The log entries shown by the console.
		/// </summary>
		private ObservableCollection<LogEntry> _logEntries = new ObservableCollection<LogEntry>();

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public ConsoleViewModel()
		{
			Prompt = new ConsolePromptViewModel();

			Log.OnFatalError += AddLogEntry;
			Log.OnError += AddLogEntry;
			Log.OnWarning += AddLogEntry;
			Log.OnInfo += AddLogEntry;
			Log.OnDebugInfo += AddLogEntry;
		}

		/// <summary>
		///     Gets the view model for the console prompt.
		/// </summary>
		public ConsolePromptViewModel Prompt { get; private set; }

		/// <summary>
		///     Gets or sets a value indicating whether the console is visible.
		/// </summary>
		public bool IsVisible
		{
			get { return _isVisible; }
			set
			{
				ChangePropertyValue(ref _isVisible, value);
				Prompt.ClearInput();
			}
		}

		/// <summary>
		///     Gets the log entries shown by the console.
		/// </summary>
		public ObservableCollection<LogEntry> LogEntries
		{
			get { return _logEntries; }
		}

		/// <summary>
		///     Submits the prompt input.
		/// </summary>
		public void Submit()
		{
			Prompt.AddInputToHistory();

			if (!String.IsNullOrWhiteSpace(Prompt.Input))
			{
				Log.Info("{0}{1}", Prompt.Token, Prompt.Input);
				Commands.Execute(Prompt.Input);

				// Show the result of the user's input
				// TODO: _content.ScrollToBottom();
			}

			Prompt.ClearInput();
		}

		/// <summary>
		///     Hides the console.
		/// </summary>
		public void Hide()
		{
			IsVisible = false;
		}

		/// <summary>
		///     Removes all log entries shown by the console.
		/// </summary>
		public void Clear()
		{
			LogEntries.Clear();
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Log.OnFatalError -= AddLogEntry;
			Log.OnError -= AddLogEntry;
			Log.OnWarning -= AddLogEntry;
			Log.OnInfo -= AddLogEntry;
			Log.OnDebugInfo -= AddLogEntry;
			Prompt.SafeDispose();
		}

		/// <summary>
		///     Adds the given entry on the console.
		/// </summary>
		/// <param name="entry">The entry that should be added.</param>
		private void AddLogEntry(LogEntry entry)
		{
			if (entry.Message.Length > MaxLength)
				entry = new LogEntry(entry.LogType, entry.Message.Substring(0, MaxLength - 3) + "...");

			_logEntries.Add(entry);
		}
	}
}