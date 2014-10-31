namespace Pegasus.Platform
{
	using System;
	using System.IO;
	using System.Linq;
	using Utilities;

	/// <summary>
	///     Provides access to the operating system's file system.
	/// </summary>
	internal static class FileSystem
	{
		/// <summary>
		///     Gets the path to the user directory.
		/// </summary>
		public static string UserDirectory { get; private set; }

		/// <summary>
		///     Sets the name of the application's directory.
		/// </summary>
		public static string ApplicationDirectory
		{
			set
			{
				Assert.ArgumentNotNull(value);
				Assert.That(IsValidFileName(value), "Invalid directory name.");
				Assert.That(Path.GetFileName(value) == value, "Expected a directory name without a path.");

				UserDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), value);
				Directory.CreateDirectory(UserDirectory);
			}
		}

		/// <summary>
		///     Checks whether the given file name is valid.
		/// </summary>
		/// <param name="fileName">The file name that should be checked.</param>
		public static bool IsValidFileName(string fileName)
		{
			Assert.ArgumentNotNull(fileName);

			if (String.IsNullOrWhiteSpace(fileName))
				return false;

			return fileName.ToCharArray().Any(c => Char.IsLetterOrDigit(c) || c == '_' || c == '.');
		}

		/// <summary>
		///     Reads all bytes of the file at the given path. This method can only read files that were shipped with the application.
		/// </summary>
		/// <param name="path">The path of the file that should be read.</param>
		public static byte[] ReadAllBytes(string path)
		{
			Assert.ArgumentNotNullOrWhitespace(path);
			Assert.That(IsValidFileName(path), "Invalid file name.");
			Assert.That(Path.GetFullPath(path).StartsWith(Environment.CurrentDirectory),
				"The does not lie in a location accessible by the application.");

			return File.ReadAllBytes(path);
		}

		/// <summary>
		///     Reads the entire UTF8-encoded text content of the file and returns it as a string. This method can only read files in
		///     the application's user directory.
		/// </summary>
		/// <param name="fileName">The name of the file in the application's user directory that should be read.</param>
		public static string ReadAllText(string fileName)
		{
			return File.ReadAllText(GetUserFilePath(fileName));
		}

		/// <summary>
		///     Writes the UTF8-encoded content to the file. If the file does not yet exist, it is created. If it does exist, its
		///     contents are overwritten. This method can only write files in the application's user directory.
		/// </summary>
		/// <param name="fileName">The name of the file in the application's user directory that should be written.</param>
		/// <param name="content">The content that should be written to the file.</param>
		public static void WriteAllText(string fileName, string content)
		{
			Assert.ArgumentNotNull(content);
			File.WriteAllText(GetUserFilePath(fileName), content);
		}

		/// <summary>
		///     Appends the UTF8-encoded content to the file. If the file does not yet exist, it is created. This method can only write
		///     files in the application's user directory.
		/// </summary>
		/// <param name="fileName">The name of the file in the application's user directory that should be written.</param>
		/// <param name="content">The content that should be written to the file.</param>
		public static void AppendText(string fileName, string content)
		{
			Assert.ArgumentNotNull(content);
			File.AppendAllText(GetUserFilePath(fileName), content);
		}

		/// <summary>
		///     Deletes the user file with the given name, if it exists. This method can only delete files in the application's user
		///     directory.
		/// </summary>
		/// <param name="fileName">The name of the file that should be deleted.</param>
		public static void Delete(string fileName)
		{
			File.Delete(GetUserFilePath(fileName));
		}

		/// <summary>
		///     Checks whether a user file with the given name exists. This method can only check files in the application's user
		///     directory.
		/// </summary>
		/// <param name="fileName">The name of the file that should be checked for.</param>
		public static bool Exists(string fileName)
		{
			return File.Exists(GetUserFilePath(fileName));
		}

		/// <summary>
		///     Gets the full path to the file in the application's user directory.
		/// </summary>
		/// <param name="fileName">The name of the file the path should be returned for.</param>
		private static string GetUserFilePath(string fileName)
		{
			Assert.ArgumentNotNull(fileName);
			Assert.That(IsValidFileName(fileName), "Invalid file name.");
			Assert.That(Path.GetFileName(fileName) == fileName, "Expected a file name without a path.");

			return Path.Combine(UserDirectory, fileName);
		}
	}
}