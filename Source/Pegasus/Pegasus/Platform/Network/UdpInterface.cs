namespace Pegasus.Platform.Network
{
	using System;
	using Utilities;

	/// <summary>
	///     Represents the native device interface.
	/// </summary>
	[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
	internal unsafe interface IUdpInterface
	{
		bool Initialize();
		bool Send(byte* buffer, int sizeInBytes, IPEndPoint* remoteEndPoint);
		ReceiveStatus TryReceive(byte* buffer, int capacityInBytes, IPEndPoint* remoteEndPoint, int* receivedBytes);
		bool Bind(ushort port);
		bool BindMulticast(IPEndPoint endPoint, int timeToLive);
		void* GetErrorMessage();
	}
}