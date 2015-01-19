namespace Lwar.Network
{
	using System;
	using System.Collections.Generic;
	using Pegasus.Utilities;

	/// <summary>
	///     Allocates generational identities.
	/// </summary>
	internal struct NetworkIdentityAllocator
	{
		/// <summary>
		///     The available identifiers that can be reassigned.
		/// </summary>
		private readonly Queue<ushort> _available;

		/// <summary>
		///     Stores the current generation for identifiers that are currently allocated and the next generation for identifiers that
		///     are currently available.
		/// </summary>
		private readonly ushort[] _generations;

		/// <summary>
		///     The next unused identifier.
		/// </summary>
		private ushort _nextIdentifier;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="maxIdentities">The maximum number of identities that can be allocated at the same time.</param>
		public NetworkIdentityAllocator(ushort maxIdentities)
			: this()
		{
			_available = new Queue<ushort>();
			_generations = new ushort[maxIdentities];
		}

		/// <summary>
		///     Allocates an identity, possibly reusing a previously allocated one with an increased generation count.
		/// </summary>
		public NetworkIdentity Allocate()
		{
			ushort identifier;
			if (_available.Count > 0)
				identifier = _available.Dequeue();
			else
			{
				identifier = _nextIdentifier++;
				Assert.That(identifier < _generations.Length, "Too many identifiers have been allocated.");
			}

			return new NetworkIdentity(identifier, _generations[identifier]);
		}

		/// <summary>
		///     Checks whether the given identity is valid, i.e., has been allocated previously and not has yet freed.
		/// </summary>
		/// <param name="identity">The identity that should be checked.</param>
		public bool IsValid(NetworkIdentity identity)
		{
			return _generations[identity.Identifier] == identity.Generation;
		}

		/// <summary>
		///     Invalidates the identity so that the next call to IsValid returns false. The identity will not be removed, however,
		///     before Free is called.
		/// </summary>
		/// <param name="identity">The identity that should be invalidated.</param>
		public void Invalidate(NetworkIdentity identity)
		{
			Assert.ArgumentSatisfies(IsValid(identity), "The identity is already invalid.");
			Assert.ArgumentInRange(identity.Identifier, 0, _nextIdentifier);
			Assert.ArgumentSatisfies(!_available.Contains(identity.Identifier), "The identity has already been freed.");

			++_generations[identity.Identifier];
		}

		/// <summary>
		///     Marks the given identity as unused, allowing it to be reallocated later.
		/// </summary>
		/// <param name="identity">The identity that can be reused.</param>
		public void Free(NetworkIdentity identity)
		{
			Assert.ArgumentInRange(identity.Identifier, 0, _nextIdentifier);
			Assert.ArgumentSatisfies(!_available.Contains(identity.Identifier), "The identity has already been freed.");

			if (IsValid(identity))
				Invalidate(identity);

			_available.Enqueue(identity.Identifier);
		}
	}
}