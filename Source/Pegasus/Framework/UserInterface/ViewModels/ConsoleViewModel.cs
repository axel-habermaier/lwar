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
		public const int MaxLength = 4096;

		/// <summary>
		///     The maximum number of log entries that the console can display.
		/// </summary>
		private const int MaxEntries = 2048;

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
		///     Gets or sets the scroll controller of the console's contents.
		/// </summary>
		public IScrollController ScrollController { get; set; }

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
		///     Initializes the console prompt.
		/// </summary>
		public void InitializePrompt()
		{
			Prompt = new ConsolePromptViewModel();
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
				ScrollToBottom();
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
		///     Scrolls the console content up.
		/// </summary>
		public void ScrollUp()
		{
			ScrollController.ScrollUp();
		}

		/// <summary>
		///     Scrolls the console content down.
		/// </summary>
		public void ScrollDown()
		{
			ScrollController.ScrollDown();
		}

		/// <summary>
		///     Scrolls to the top of the console content.
		/// </summary>
		public void ScrollToTop()
		{
			ScrollController.ScrollToTop();
		}

		/// <summary>
		///     Scrolls to the bottom of the console content.
		/// </summary>
		public void ScrollToBottom()
		{
			ScrollController.ScrollToBottom();
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

			if (_logEntries.Count >= MaxEntries)
				_logEntries.RemoveAt(0);

			_logEntries.Add(entry);
		}
	}
}