namespace Pegasus.Framework.UserInterface.ViewModels
{
	using System;
	using System.Collections.Generic;
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
		///     The log entries that have been generated while the console was not shown. When the console becomes visible again, these
		///     entries are added to the list of shown entries.
		/// </summary>
		private Queue<LogEntry> _cachedLogEntries = new Queue<LogEntry>(MaxEntries);

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

				if (IsVisible)
					ShowCachedLogEntries();
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
		///     Shows or hides the console.
		/// </summary>
		/// <param name="show">Indicates whether the console should be shown or hidden.</param>
		private void ShowConsole(bool show)
		{
			IsVisible = show;
		}

		/// <summary>
		///     Initializes the console view model. The initialization is a two-stage process as most of the required subsystems are not
		///     yet initialized when the console view model is created. However, we have to create the console view model as early as
		///     possible in order to not miss any log entries.
		/// </summary>
		public void Initialize()
		{
			Commands.OnShowConsole += ShowConsole;
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
		///     Removes all log entries shown by the console.
		/// </summary>
		public void Clear()
		{
			LogEntries.Clear();
			ScrollToBottom();
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
			Commands.OnShowConsole -= ShowConsole;

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

			// If we exceed the maximum number of allowed log entries, remove the oldest one
			if (_cachedLogEntries.Count >= MaxEntries)
				_cachedLogEntries.Dequeue();

			_cachedLogEntries.Enqueue(entry);

			// If the console is visible, we have to show the new log entry immediately
			if (IsVisible)
				ShowCachedLogEntries();
		}

		/// <summary>
		///     Shows the cached log entries on the console.
		/// </summary>
		private void ShowCachedLogEntries()
		{
			var removeCount = Math.Max((_cachedLogEntries.Count + _logEntries.Count) - MaxEntries, 0);

			for (var i = 0; i < removeCount; ++i)
				_logEntries.RemoveAt(0);

			_logEntries.AddRange(_cachedLogEntries);
			_cachedLogEntries.Clear();
		}
	}
}