namespace Pegasus.Framework.UserInterface.ViewModels
{
	using System;
	using System.Linq;
	using System.Text;
	using Platform;
	using Platform.Logging;
	using Scripting;

	/// <summary>
	///     Represents the prompt of the console.
	/// </summary>
	internal class ConsolePromptViewModel : DisposableNotifyPropertyChanged
	{
		/// <summary>
		///     The maximum history size.
		/// </summary>
		private const int MaxHistory = 64;

		/// <summary>
		///     The name of the console history file.
		/// </summary>
		private const string HistoryFileName = "console.txt";

		/// <summary>
		///     The input history.
		/// </summary>
		private readonly string[] _history;

		/// <summary>
		///     The current auto-completion index.
		/// </summary>
		private int _autoCompletionIndex;

		/// <summary>
		///     The currently active auto-completion list.
		/// </summary>
		private string[] _autoCompletionList;

		/// <summary>
		///     Stores the current index into the input history.
		/// </summary>
		private int _historyIndex;

		/// <summary>
		///     The current prompt input.
		/// </summary>
		private string _input;

		/// <summary>
		///     The number of history slots that are currently used.
		/// </summary>
		private int _numHistory;

		/// <summary>
		///     Indicates whether the input is currently being set to the result of an auto-completion operation.
		/// </summary>
		private bool _settingAutoCompletedInput;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public ConsolePromptViewModel()
		{
			_history = new string[MaxHistory];

			var file = new AppFile(HistoryFileName);
			string content;
			var success = file.Read(out content, e => Log.Error("Failed to load console history from '{0}/{1}': {2}",
				FileSystem.UserDirectory, file.FileName, e.Message));

			if (!success)
				return;

			var history = content.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
			history = history.Where(h => h.Length <= ConsoleViewModel.MaxLength).ToArray();

			var count = history.Length;
			var offset = 0;

			if (count > _history.Length)
			{
				offset = count - _history.Length;
				count = _history.Length;
			}

			Array.Copy(history, offset, _history, 0, count);
			_numHistory = count;
			_historyIndex = _numHistory;
		}

		/// <summary>
		///     Gets the prompt token.
		/// </summary>
		public string Token
		{
			get { return "]"; }
		}

		/// <summary>
		///     Gets or sets the current prompt input.
		/// </summary>
		public string Input
		{
			get { return _input; }
			set { ChangePropertyValue(ref _input, value); }
		}

		/// <summary>
		///     Gets the maximum allowed length of the console prompt input.
		/// </summary>
		public int MaxLength
		{
			get { return ConsoleViewModel.MaxLength; }
		}

		/// <summary>
		///     Clears the prompt, removing all current input.
		/// </summary>
		public void ClearInput()
		{
			Input = String.Empty;
			_historyIndex = _numHistory;
			_autoCompletionList = null;
		}

		/// <summary>
		///     Handles changes to the input text.
		/// </summary>
		public void TextChanged()
		{
			// Reset the auto completion list if an input was made other than setting the current completion value
			if (_settingAutoCompletedInput)
				return;

			_autoCompletionList = null;
		}

		/// <summary>
		///     Submits the current user input to the input history.
		/// </summary>
		public void AddInputToHistory()
		{
			if (String.IsNullOrWhiteSpace(Input))
				return;

			// Store the input in the input history, if it differs from the last entry
			if (_numHistory != 0 && _history[_numHistory - 1] == Input)
				return;

			// If the history is full, remove the oldest entry; this could be implemented more efficiently
			if (_numHistory == MaxHistory)
			{
				Array.Copy(_history, 1, _history, 0, MaxHistory - 1);
				--_numHistory;
			}

			_history[_numHistory++] = Input;
			_historyIndex = _numHistory;
		}

		/// <summary>
		///     Shows the next newer history entry, if any.
		/// </summary>
		public void ShowNewerHistoryEntry()
		{
			ShowHistory(_historyIndex + 1);
		}

		/// <summary>
		///     Shows the next older history entry, if any.
		/// </summary>
		public void ShowOlderHistoryEntry()
		{
			ShowHistory(_historyIndex - 1);
		}

		/// <summary>
		///     Shows the next auto-completed value if completion is possible.
		/// </summary>
		public void AutoCompleteNext()
		{
			AutoComplete(true);
		}

		/// <summary>
		///     Shows the previous auto-completed value if completion is possible.
		/// </summary>
		public void AutoCompletePrevious()
		{
			AutoComplete(false);
		}

		/// <summary>
		///     Shows the next or previous auto-completed value if completion is possible.
		/// </summary>
		/// <param name="next">If true, the next auto-completed entry is shown; otherwise, the previous one is shown.</param>
		private void AutoComplete(bool next)
		{
			if (_autoCompletionList == null)
			{
				_autoCompletionList = GetAutoCompletionList();

				// If auto-completion returned no results, we're done here
				if (_autoCompletionList == null)
					return;

				_autoCompletionIndex = next ? 0 : _autoCompletionList.Length - 1;
			}
			else
			{
				_autoCompletionIndex = (_autoCompletionIndex + (next ? 1 : -1)) % _autoCompletionList.Length;
				if (_autoCompletionIndex < 0)
					_autoCompletionIndex += _autoCompletionList.Length;
			}

			_settingAutoCompletedInput = true;
			Input = _autoCompletionList[_autoCompletionIndex] + " ";
			_settingAutoCompletedInput = false;
		}

		/// <summary>
		///     Gets the auto completion list for the current input.
		/// </summary>
		private string[] GetAutoCompletionList()
		{
			if (String.IsNullOrWhiteSpace(Input))
				return null;

			var commands = CommandRegistry.All
										  .Where(command => command.Name.ToLower().StartsWith(Input.ToLower()))
										  .Select(command => command.Name);

			var cvars = CvarRegistry.All
									.Where(cvar => cvar.Name.ToLower().StartsWith(Input.ToLower()))
									.Select(cvar => cvar.Name);

			var list = cvars.Union(commands).OrderBy(item => item).ToArray();
			if (list.Length == 0)
				return null;

			return list;
		}

		/// <summary>
		///     Shows the history entry at the given index.
		/// </summary>
		/// <param name="index">The index of the entry that should be shown.</param>
		private void ShowHistory(int index)
		{
			if (_numHistory == 0)
				return;

			// Ensure the index is inside the bounds
			if (index < 0)
				index = 0;

			// Empty the input box when going past the newest entry
			if (index >= _numHistory)
			{
				Input = String.Empty;
				index = _numHistory;
			}
			else
				Input = _history[index];

			_historyIndex = index;
			_autoCompletionList = null;
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			var builder = new StringBuilder();
			for (var i = 0; i < _numHistory; ++i)
				builder.Append(_history[i]).Append("\n");

			var file = new AppFile(HistoryFileName);
			file.Write(builder.ToString(),
				e => Log.Error("Failed to persist console history in '{0}/{1}': {2}", FileSystem.UserDirectory, file.FileName, e.Message));
		}
	}
}