namespace Pegasus.Platform
{
	using System;
	using System.Linq;
	using System.Runtime.InteropServices;
	using System.Security;
	using System.Text;

	/// <summary>
	///     Provides access to the operating system's file system.
	/// </summary>
	internal static class FileSystem
	{
		/// <summary>
		///     The maximum supported file size in bytes.
		/// </summary>
		private const int MaxFileSize = 6 * 1024 * 1024;

		/// <summary>
		///     The buffer used for all file system operations.
		/// </summary>
		private static readonly byte[] Buffer = new byte[MaxFileSize];

		/// <summary>
		///     Gets the path to the user directory.
		/// </summary>
		public static string UserDirectory
		{
			get
			{
				var error = NativeMethods.GetUserDirectory();
				if (error == IntPtr.Zero)
					return "<unknown>";

#if Windows
				return Marshal.PtrToStringUni(error);
#else
				return Marshal.PtrToStringAnsi(error);
#endif
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
		///     The contents of the returned array segment are invalidated by the next file system operation.
		/// </summary>
		/// <param name="path">The path of the file that should be read.</param>
		public static unsafe ArraySegment<byte> ReadAllBytes(string path)
		{
			Assert.ArgumentNotNullOrWhitespace(path);

			fixed (byte* buffer = Buffer)
			{
				var length = (uint)MaxFileSize;
				if (!NativeMethods.ReadAppFile(path, buffer, ref length))
					throw new FileSystemException();

				return new ArraySegment<byte>(Buffer, 0, (int)length);
			}
		}

		/// <summary>
		///     Reads the entire UTF8-encoded text content of the file and returns it as a string. This method can only read files in
		///     the application's user directory.
		/// </summary>
		/// <param name="fileName">The name of the file in the application's user directory that should be read.</param>
		public static unsafe string ReadAllText(string fileName)
		{
			Assert.ArgumentNotNullOrWhitespace(fileName);

			fixed (byte* buffer = Buffer)
			{
				var length = (uint)MaxFileSize;
				if (!NativeMethods.ReadUserFile(fileName, buffer, ref length))
					throw new FileSystemException();

				return Encoding.UTF8.GetString(Buffer, 0, (int)length);
			}
		}

		/// <summary>
		///     Writes the UTF8-encoded content to the file. If the file does not yet exist, it is created. If it does exist, its
		///     contents are overwritten. This method can only write files in the application's user directory.
		/// </summary>
		/// <param name="fileName">The name of the file in the application's user directory that should be written.</param>
		/// <param name="content">The content that should be written to the file.</param>
		public static unsafe void WriteAllText(string fileName, string content)
		{
			Assert.ArgumentNotNullOrWhitespace(fileName);
			Assert.ArgumentNotNull(content);

			var data = Encoding.UTF8.GetBytes(content);
			fixed (byte* dataPtr = data)
			{
				if (!NativeMethods.WriteUserFile(fileName, dataPtr, (uint)data.Length))
					throw new FileSystemException();
			}
		}

		/// <summary>
		///     Appends the UTF8-encoded content to the file. If the file does not yet exist, it is created. This method can only write
		///     files in the application's user directory.
		/// </summary>
		/// <param name="fileName">The name of the file in the application's user directory that should be written.</param>
		/// <param name="content">The content that should be written to the file.</param>
		public static unsafe void AppendText(string fileName, string content)
		{
			Assert.ArgumentNotNullOrWhitespace(fileName);
			Assert.ArgumentNotNull(content);

			var data = Encoding.UTF8.GetBytes(content);
			fixed (byte* dataPtr = data)
			{
				if (!NativeMethods.AppendUserFile(fileName, dataPtr, (uint)data.Length))
					throw new FileSystemException();
			}
		}

		/// <summary>
		///     Deletes the user file with the given name, if it exists. This method can only delete files in the application's user
		///     directory.
		/// </summary>
		/// <param name="fileName">The name of the file that should be deleted.</param>
		public static void DeleteFile(string fileName)
		{
			Assert.ArgumentNotNullOrWhitespace(fileName);

			if (!NativeMethods.DeleteUserFile(fileName))
				throw new FileSystemException();
		}

		/// <summary>
		///     Provides access to the native file system functions.
		/// </summary>
		[SuppressUnmanagedCodeSecurity]
		private static class NativeMethods
		{
			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgReadAppFile")]
			public static extern unsafe bool ReadAppFile([MarshalAs(UnmanagedType.LPStr)] string path, byte* buffer, ref uint sizeInBytes);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgReadUserFile")]
			public static extern unsafe bool ReadUserFile([MarshalAs(UnmanagedType.LPStr)] string fileName, byte* buffer, ref uint sizeInBytes);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgWriteUserFile")]
			public static extern unsafe bool WriteUserFile([MarshalAs(UnmanagedType.LPStr)] string fileName, byte* content, uint sizeInBytes);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgAppendUserFile")]
			public static extern unsafe bool AppendUserFile([MarshalAs(UnmanagedType.LPStr)] string fileName, byte* content, uint sizeInBytes);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgDeleteUserFile")]
			public static extern bool DeleteUserFile([MarshalAs(UnmanagedType.LPStr)] string fileName);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgGetUserDirectory")]
			public static extern IntPtr GetUserDirectory();
		}
	}
}