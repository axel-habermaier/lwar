namespace Pegasus.Platform.Network
{
	using System;
	using System.Runtime.InteropServices;
	using Utilities;

	/// <summary>
	///     Raised when a network error occurred.
	/// </summary>
	public unsafe class NetworkException : Exception
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="udpInterface">The Udp interface the exception is raised for.</param>
		internal NetworkException(UdpInterface* udpInterface)
			: base(PlatformLibrary.NormalizeMessage(Marshal.PtrToStringAnsi(new IntPtr(udpInterface->GetErrorMessage()))))
		{
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="message">A message explaining the exception.</param>
		/// <param name="args">The format arguments for the exception message.</param>
		[StringFormatMethod("message")]
		public NetworkException(string message, params object[] args)
			: base(String.Format(message, args))
		{
		}
	}
}