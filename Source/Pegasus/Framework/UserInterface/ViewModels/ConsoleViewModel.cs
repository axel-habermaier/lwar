namespace Pegasus.Framework.UserInterface.ViewModels
{
	using System;
	using System.Collections.Generic;
	using Platform.Logging;

	/// <summary>
	///     Displays the in-game console.
	/// </summary>
	internal class ConsoleViewModel : DisposableNotifyPropertyChanged
	{
		/// <summary>
		///     The current console input.
		/// </summary>
		private string _consoleInput;

		/// <summary>
		///     Indicates whether the console is visible.
		/// </summary>
		private bool _isVisible;

		/// <summary>
		///     The log entries shown by the console.
		/// </summary>
		private ObservableCollection<LogEntry> _logEntries = new ObservableCollection<LogEntry>();

		/// <summary>
		///     The pending log entries that will be added to the console on the next frame.
		/// </summary>
		private List<LogEntry> _pendingLogEntries = new List<LogEntry>();

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
		///     Gets or sets a value indicating whether the console is visible.
		/// </summary>
		public bool IsVisible
		{
			get { return _isVisible; }
			set
			{
				ChangePropertyValue(ref _isVisible, value);
				ConsoleInput = String.Empty;
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
		///     Gets or sets the current console input.
		/// </summary>
		public string ConsoleInput
		{
			get { return _consoleInput; }
			set { ChangePropertyValue(ref _consoleInput, value); }
		}

		/// <summary>
		///     Hides the console.
		/// </summary>
		public void Hide()
		{
			IsVisible = false;
		}

		/// <summary>
		///     Updates the console.
		/// </summary>
		public void Update()
		{
			// Cannot use foreach here, as adding the log entries might generate new log entries, causing
			// a concurrent modification exception
			var count = _pendingLogEntries.Count;
			for (var i = 0; i < count; ++i)
				_logEntries.Add(_pendingLogEntries[i]);

			_pendingLogEntries.Clear();
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
		}

		/// <summary>
		///     Adds the given entry on the console.
		/// </summary>
		/// <param name="entry">The entry that should be added.</param>
		private void AddLogEntry(LogEntry entry)
		{
			_pendingLogEntries.Add(entry);
		}
	}
}