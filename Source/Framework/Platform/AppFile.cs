using System;

namespace Pegasus.Framework.Platform
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;

	/// <summary>
	///   Represents a file in the application's user directory that can be read and written by the application.
	/// </summary>
	internal class AppFile
	{
		/// <summary>
		///   The name of the automatically executed configuration file.
		/// </summary>
		public const string AutoExec = "autoexec";

		/// <summary>
		///   The maximum allowed length of a file name.
		/// </summary>
		private const int MaximumFileNameLength = 50;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="appName">The name of the application.</param>
		/// <param name="fileName">The name of the file that should be read or written.</param>
		public AppFile(string appName, string fileName)
		{
			Assert.ArgumentNotNullOrWhitespace(appName, () => appName);
			Assert.ArgumentNotNullOrWhitespace(fileName, () => fileName);

			FileName = fileName;

			var folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			AbsolutePath = Path.Combine(folder, appName, fileName);
		}

		/// <summary>
		///   Gets the name of the file that is read or written.
		/// </summary>
		public string FileName { get; private set; }

		/// <summary>
		///   Gets the absolute path of the fiel that is read or written.
		/// </summary>
		public string AbsolutePath { get; private set; }

		/// <summary>
		///   Gets a value indicating whether the file name is valid, i.e., is not an absolute or relative file path.
		/// </summary>
		public bool IsValid
		{
			get
			{
				var invalidChars = Path.GetInvalidFileNameChars();
				return FileName == Path.GetFileName(FileName) &&
					   !FileName.Any(invalidChars.Contains) &&
					   FileName.Length < MaximumFileNameLength;
			}
		}

		/// <summary>
		///   Writes the given content into the file, overwriting all previous content.
		/// </summary>
		/// <param name="content">The content that should be written into the file.</param>
		/// <param name="onException">
		///   The action that should be executed if an I/O exception occurs during the execution of the
		///   method. If null, the exception is propagated to the calling scope.
		/// </param>
		public void Write(string content, Action<IOException> onException = null)
		{
			Assert.ArgumentNotNull(content, () => content);
			Assert.That(IsValid, "The file name is invalid.");

			Execute(() => File.WriteAllText(AbsolutePath, content), onException);
		}

		/// <summary>
		///   Appends the given lines to the file.
		/// </summary>
		/// <param name="lines">The lines that should be appended to the file.</param>
		/// <param name="onException">
		///   The action that should be executed if an I/O exception occurs during the execution of the
		///   method. If null, the exception is propagated to the calling scope.
		/// </param>
		public void Append(IEnumerable<string> lines, Action<IOException> onException = null)
		{
			Assert.ArgumentNotNull(lines, () => lines);
			Assert.That(IsValid, "The file name is invalid.");

			Execute(() => File.AppendAllLines(AbsolutePath, lines), onException);
		}

		/// <summary>
		///   Reads the content of the given file.
		/// </summary>
		/// <param name="onException">
		///   The action that should be executed if an I/O exception occurs during the execution of the
		///   method. If null, the exception is propagated to the calling scope.
		/// </param>
		public string Read(Action<IOException> onException = null)
		{
			Assert.That(IsValid, "The file name is invalid.");

			var content = String.Empty;
			Execute(() => content = File.ReadAllText(AbsolutePath), onException);
			return content;
		}

		/// <summary>
		///   Deletes the file.
		/// </summary>
		/// <param name="onException">
		///   The action that should be executed if an I/O exception occurs during the execution of the
		///   method. If null, the exception is propagated to the calling scope.
		/// </param>
		public void Delete(Action<IOException> onException = null)
		{
			Assert.That(IsValid, "The file name is invalid.");
			Execute(() => File.Delete(AbsolutePath), onException);
		}

		/// <summary>
		///   Executes the given file action. If an exception handler is provided and an I/O exception is thrown, the exception
		///   handler is invoked. Otherwise, I/O exception propagate to the calling scope.
		/// </summary>
		/// <param name="action">The file action that should be executed.</param>
		/// <param name="onException">
		///   The action that should be executed if an I/O exception occurs during the execution of the
		///   method. If null, the exception is propagated to the calling scope.
		/// </param>
		private void Execute(Action action, Action<IOException> onException)
		{
			Assert.ArgumentNotNull(action, () => action);

			try
			{
				EnsureDirectoryExists();
				action();
			}
			catch (IOException e)
			{
				if (onException == null)
					throw;

				onException(e);
			}
		}

		/// <summary>
		///   Ensures that the application's user directory exists.
		/// </summary>
		private void EnsureDirectoryExists()
		{
			var directory = Path.GetDirectoryName(AbsolutePath);
			if (!Directory.Exists(directory))
				Directory.CreateDirectory(directory);
		}
	}
}