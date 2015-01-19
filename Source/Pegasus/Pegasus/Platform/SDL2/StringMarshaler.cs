namespace Pegasus.Platform.SDL2
{
	using System;
	using System.Runtime.InteropServices;
	using System.Text;
	using Utilities;

	/// <summary>
	///     Marshals a managed string to an unmanaged UTF8-encoded one and vice versa.
	/// </summary>
	internal class StringMarshaler : ICustomMarshaler
	{
		/// <summary>
		///     A cookie that indicates that native memory must not be freed.
		/// </summary>
		public const string NoFree = "NoFree";

		/// <summary>
		///     The default string marshaler instance that is cached for performance reasons.
		/// </summary>
		private static readonly StringMarshaler Instance = new StringMarshaler();

		/// <summary>
		///     The default string marshaler instance that frees native data after it has been used. Cached for performance reasons.
		/// </summary>
		private static readonly StringMarshaler InstanceFreeNative = new StringMarshaler { _freeNativeData = true };

		/// <summary>
		///     Indicates whether native data should be freed.
		/// </summary>
		private bool _freeNativeData;

		/// <summary>
		///     Converts the unmanaged UTF8-encoded string to a managed string.
		/// </summary>
		/// <param name="str">A pointer to the unmanaged string to be marshaled.</param>
		object ICustomMarshaler.MarshalNativeToManaged(IntPtr str)
		{
			return ToManagedString(str);
		}

		/// <summary>
		///     Converts the managed string to an unmanaged UTF8-encoded string.
		/// </summary>
		/// <param name="str">The managed string to be converted.</param>
		unsafe IntPtr ICustomMarshaler.MarshalManagedToNative(object str)
		{
			Assert.ArgumentNotNull(str);
			Assert.ArgumentOfType<string>(str);

			var s = (string)str;
			var bytes = Encoding.UTF8.GetBytes(s);
			var memory = Marshal.AllocHGlobal(bytes.Length + 1);
			Marshal.Copy(bytes, 0, memory, bytes.Length);
			((byte*)memory)[bytes.Length] = 0;

			return memory;
		}

		/// <summary>
		///     Performs necessary cleanup of the unmanaged string when it is no longer needed.
		/// </summary>
		/// <param name="str">A pointer to the unmanaged string to be destroyed.</param>
		void ICustomMarshaler.CleanUpNativeData(IntPtr str)
		{
			if (_freeNativeData)
				Marshal.FreeHGlobal(str);
		}

		/// <summary>
		///     Performs necessary cleanup of the managed string when it is no longer needed.
		/// </summary>
		/// <param name="str">The managed string to be destroyed.</param>
		void ICustomMarshaler.CleanUpManagedData(object str)
		{
			// Nothing to do here
		}

		/// <summary>
		///     Returns the size of the native data to be marshaled.
		/// </summary>
		int ICustomMarshaler.GetNativeDataSize()
		{
			return -1;
		}

		/// <summary>
		///     Gets the default marshaler instance. Implicitly used and required by .NET.
		/// </summary>
		[UsedImplicitly]
		public static ICustomMarshaler GetInstance(string cookie)
		{
			if (cookie != NoFree)
				return InstanceFreeNative;

			return Instance;
		}

		/// <summary>
		///     Converts the unmanaged UTF8-encoded string to a managed string.
		/// </summary>
		/// <param name="str">A pointer to the unmanaged string to be marshaled.</param>
		internal static unsafe string ToManagedString(IntPtr str)
		{
			if (str == IntPtr.Zero)
				return null;

			var ptr = (byte*)str;
			while (*ptr != 0)
				ptr++;

			var bytes = new byte[ptr - (byte*)str];
			Marshal.Copy(str, bytes, 0, bytes.Length);

			return Encoding.UTF8.GetString(bytes);
		}
	}
}