namespace Pegasus.Platform
{
	using System;
	using System.Linq;
	using Utilities;

	/// <summary>
	///     Represents a file in the application's user directory that can be read and written by the application.
	/// </summary>
	internal class AppFile
	{
		/// <summary>
		///     The number of spaces per tab.
		/// </summary>
		private const int SpacesPerTab = 4;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="fileName">The name of the file that should be read or written.</param>
		public AppFile(string fileName)
		{
			Assert.ArgumentNotNullOrWhitespace(fileName);
			FileName = fileName;
		}

		/// <summary>
		///     Gets the name of the file that is read or written.
		/// </summary>
		public string FileName { get; private set; }

		/// <summary>
		///     Writes the given content into the file, overwriting all previous content. Returns true to indicate that the file
		///     operation has been successful.
		/// </summary>
		/// <param name="content">The content that should be written into the file.</param>
		/// <param name="onException">
		///     The action that should be executed if an I/O exception occurs during the execution of the
		///     method. If null, the exception is propagated to the calling scope.
		/// </param>
		public bool Write(string content, Action<Exception> onException = null)
		{
			Assert.ArgumentNotNull(content);
			return Execute(() => FileSystem.WriteAllText(FileName, content), onException);
		}

		/// <summary>
		///     Appends the given content to the file. Returns true to indicate that the file operation has been successful.
		/// </summary>
		/// <param name="content">The content that should be appended to the file.</param>
		/// <param name="onException">
		///     The action that should be executed if an I/O exception occurs during the execution of the
		///     method. If null, the exception is propagated to the calling scope.
		/// </param>
		public bool Append(string content, Action<Exception> onException = null)
		{
			Assert.ArgumentNotNull(content);
			return Execute(() => FileSystem.AppendText(FileName, content), onException);
		}

		/// <summary>
		///     Reads the content of the given file, with the line endings normalized to '\n'. Returns true to indicate that the file
		///     operation has been successful.
		/// </summary>
		/// <param name="content">The content that has been read from the file.</param>
		/// <param name="onException">
		///     The action that should be executed if an I/O exception occurs during the execution of the
		///     method. If null, the exception is propagated to the calling scope.
		/// </param>
		public bool Read(out string content, Action<Exception> onException = null)
		{
			var fileContent = String.Empty;
			var success = Execute(() => fileContent = FileSystem.ReadAllText(FileName), onException);

			content = Normalize(fileContent);
			return success;
		}

		/// <summary>
		///     Normalizes the line endings of the given input string to '\n', and replaces all tabs with spaces.
		/// </summary>
		/// <param name="input">The input whose line endings should be normalized.</param>
		private static string Normalize(string input)
		{
			return input.Replace("\r\n", "\n")
						.Replace("\r", "\n")
						.Replace("\t", String.Join(" ", Enumerable.Range(0, SpacesPerTab).Select(_ => String.Empty)));
		}

		/// <summary>
		///     Deletes the file. Returns true to indicate that the file operation has been successful.
		/// </summary>
		/// <param name="onException">
		///     The action that should be executed if an I/O exception occurs during the execution of the
		///     method. If null, the exception is propagated to the calling scope.
		/// </param>
		public bool Delete(Action<Exception> onException = null)
		{
			return Execute(() => FileSystem.Delete(FileName), onException);
		}

		/// <summary>
		///     Executes the given file action. If an exception handler is provided and an I/O exception is thrown, the exception
		///     handler is invoked. Otherwise, I/O exception propagate to the calling scope. Returns true to indicate that the file
		///     operation has been successful.
		/// </summary>
		/// <param name="action">The file action that should be executed.</param>
		/// <param name="onException">
		///     The action that should be executed if an I/O exception occurs during the execution of the
		///     method. If null, the exception is propagated to the calling scope.
		/// </param>
		private static bool Execute(Action action, Action<Exception> onException)
		{
			Assert.ArgumentNotNull(action);

			try
			{
				action();
				return true;
			}
			catch (Exception e)
			{
				if (onException == null)
					throw;

				onException(e);
				return false;
			}
		}
	}
}