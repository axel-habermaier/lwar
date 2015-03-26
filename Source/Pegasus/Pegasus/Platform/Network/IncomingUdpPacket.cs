namespace Pegasus.Platform.Network
{
	using System;
	using Memory;
	using Utilities;

	partial class IncomingUdpPacket
	{
		/// <summary>
		///     Initializes the type.
		/// </summary>
		static IncomingUdpPacket()
		{
			ConstructorCache.Register(() => new IncomingUdpPacket());
			ResetCache.RegisterReset((IncomingUdpPacket packet) =>
			{
				ResetCache.Reset<object>(packet);
				packet.Size = default(int);
			});
			ResetCache.RegisterInit<IncomingUdpPacket>((Action<IncomingUdpPacket, int>)((packet, s) => packet.Initialize(s)));
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private IncomingUdpPacket()
		{
		}
	}

	

	internal static class Ext
	{
		public static Shared<IncomingUdpPacket> AllocateIncomingUdpPacket(this PoolAllocator p, int max)
		{
			var o = p.Allocate<IncomingUdpPacket>();
			((Action<IncomingUdpPacket, int>)ResetCache.GetInit<IncomingUdpPacket>())(o, max);
			return new Shared<IncomingUdpPacket>(o);
		}
	}










	public class Shared<T> : PooledObject
	{
		/// <summary>
		///     Invoked when an owner of the pooled object release its ownership. Returns true to indicate that
		///     the object should be returned to the pool.
		/// </summary>
		protected override bool OnOwnershipReleased()
		{
			throw new NotImplementedException();
		}
		public T Object { get; set; }
		public Shared(T t)
		{

		}
	}
	internal class ResetCache
	{
		public static void RegisterReset<T>(Action<T> a)
		{
		}

		public static void RegisterInit<T>(object a)
		{
		}

		public static void Reset<T>(T o)
		{
		}

		public static object GetInit<T>()
		{
			return null;
		}
	}

	internal enum Ownership
	{
		Unique,
		Shared
	}

	internal class NotZeroed : Attribute
	{
	}

	internal class PoolAllocated : Attribute
	{
		public PoolAllocated(Ownership t)
		{
		}

		public bool InternalAllocationOnly { get; set; }
		public bool ZeroMembers { get; set; }
	}

	internal class PoolInitializationAttribute : Attribute
	{
	}


















	/// <summary>
	///     Represents an incoming UDP data packet.
	/// </summary>
	[PoolAllocated(Ownership.Unique, InternalAllocationOnly = true, ZeroMembers = true)]
	public sealed partial class IncomingUdpPacket
	{
		/// <summary>
		///     Gets the buffer storing the data of the packet.
		/// </summary>
		[NotZeroed]
		internal byte[] Buffer { get; private set; }

		/// <summary>
		///     Gets the size of the stored data in bytes.
		/// </summary>
		public int Size { get; private set; }

		/// <summary>
		///     Creates a buffer reader that can be used to read from the packet.
		/// </summary>
		public BufferReader Reader
		{
			get { return new BufferReader(Buffer, 0, Size, Endianess.Big); }
		}

		/// <summary>
		///     Allocates a new UDP packet with the given capacity.
		/// </summary>
		/// <param name="capacity">The maximum number of bytes that can be stored in the UDP packet.</param>
		[PoolInitialization]
		private void Initialize(int capacity)
		{
			Assert.InRange(capacity, 1, UInt16.MaxValue);

			if (Buffer == null || Buffer.Length < capacity)
				Buffer = new byte[capacity];
		}
	}
}