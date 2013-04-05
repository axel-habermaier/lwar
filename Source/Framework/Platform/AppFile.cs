﻿using System;

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
		///   Writes the given content into the file, overwriting all previous content. Returns true to indicate that the file
		///   operation has been successful.
		/// </summary>
		/// <param name="content">The content that should be written into the file.</param>
		/// <param name="onException">
		///   The action that should be executed if an I/O exception occurs during the execution of the
		///   method. If null, the exception is propagated to the calling scope.
		/// </param>
		public bool Write(string content, Action<IOException> onException = null)
		{
			Assert.ArgumentNotNull(content, () => content);
			Assert.That(IsValid, "The file name is invalid.");

			return Execute(() => File.WriteAllText(AbsolutePath, content), onException);
		}

		/// <summary>
		///   Appends the given lines to the file. Returns true to indicate that the file operation has been successful.
		/// </summary>
		/// <param name="lines">The lines that should be appended to the file.</param>
		/// <param name="onException">
		///   The action that should be executed if an I/O exception occurs during the execution of the
		///   method. If null, the exception is propagated to the calling scope.
		/// </param>
		public bool Append(IEnumerable<string> lines, Action<IOException> onException = null)
		{
			Assert.ArgumentNotNull(lines, () => lines);
			Assert.That(IsValid, "The file name is invalid.");

			return Execute(() => File.AppendAllLines(AbsolutePath, lines), onException);
		}

		/// <summary>
		///   Reads the content of the given file, with the line endings normalized to '\n'. Returns true to indicate that the file
		///   operation has been successful.
		/// </summary>
		/// <param name="content">The content that has been read from the file.</param>
		/// <param name="onException">
		///   The action that should be executed if an I/O exception occurs during the execution of the
		///   method. If null, the exception is propagated to the calling scope.
		/// </param>
		public bool Read(out string content, Action<IOException> onException = null)
		{
			Assert.That(IsValid, "The file name is invalid.");

			var fileContent = String.Empty;
			var success = Execute(() => fileContent = File.ReadAllText(AbsolutePath), onException);

			content = NormalizeLineEndings(fileContent);
			return success;
		}

		/// <summary>
		/// Normalizes the line endings of the given input string to '\n'.
		/// </summary>
		/// <param name="input">The input whose line endings should be normalized.</param>
		private static string NormalizeLineEndings(string input)
		{
			var result = input.Replace("\r\n", "\n");
			result = result.Replace("\r", "\n");
			return result;
		}

		/// <summary>
		///   Deletes the file. Returns true to indicate that the file operation has been successful.
		/// </summary>
		/// <param name="onException">
		///   The action that should be executed if an I/O exception occurs during the execution of the
		///   method. If null, the exception is propagated to the calling scope.
		/// </param>
		public bool Delete(Action<IOException> onException = null)
		{
			Assert.That(IsValid, "The file name is invalid.");
			return Execute(() => File.Delete(AbsolutePath), onException);
		}

		/// <summary>
		///   Executes the given file action. If an exception handler is provided and an I/O exception is thrown, the exception
		///   handler is invoked. Otherwise, I/O exception propagate to the calling scope. Returns true to indicate that the file
		///   operation has been successful.
		/// </summary>
		/// <param name="action">The file action that should be executed.</param>
		/// <param name="onException">
		///   The action that should be executed if an I/O exception occurs during the execution of the
		///   method. If null, the exception is propagated to the calling scope.
		/// </param>
		private bool Execute(Action action, Action<IOException> onException)
		{
			Assert.ArgumentNotNull(action, () => action);

			try
			{
				EnsureDirectoryExists();
				action();
				return true;
			}
			catch (IOException e)
			{
				if (onException == null)
					throw;

				onException(e);
				return false;
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