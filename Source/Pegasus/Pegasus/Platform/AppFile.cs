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
		///     Writes the given content into the file, overwriting all previous content.
		/// </summary>
		/// <param name="content">The content that should be written into the file.</param>
		public void Write(string content)
		{
			Assert.ArgumentNotNull(content);

			try
			{
				FileSystem.WriteAllText(FileName, content);
			}
			catch (Exception e)
			{
				throw new FileSystemException(e.Message);
			}
		}

		/// <summary>
		///     Appends the given content to the file.
		/// </summary>
		/// <param name="content">The content that should be appended to the file.</param>
		public void Append(string content)
		{
			Assert.ArgumentNotNull(content);

			try
			{
				FileSystem.AppendText(FileName, content);
			}
			catch (Exception e)
			{
				throw new FileSystemException(e.Message);
			}
		}

		/// <summary>
		///     Reads the content of the given file, with the line endings normalized to '\n'.
		/// </summary>
		public string Read()
		{
			try
			{
				return Normalize(FileSystem.ReadAllText(FileName));
			}
			catch (Exception e)
			{
				throw new FileSystemException(e.Message);
			}
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
		///     Deletes the file.
		/// </summary>
		public void Delete()
		{
			try
			{
				FileSystem.Delete(FileName);
			}
			catch (Exception e)
			{
				throw new FileSystemException(e.Message);
			}
		}
	}
}