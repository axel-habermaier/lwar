namespace Pegasus.Platform.Input
{
	using System;

	/// <summary>
	///   Represents an input layer. The active input layer of a logical input device determines which logical inputs are
	///   triggered. Input layers are prioritized, with higher-numbered layers having higher priorities. The
	///   application can use up to 31 unique input layers ranging from 1 to 31 (inclusive), with input layer 32 being
	///   reserved for the console.
	/// </summary>
	public struct InputLayer : IEquatable<InputLayer>
	{
		/// <summary>
		///   Represents the console input layer. The console always takes priority over all other input.
		/// </summary>
		public static readonly InputLayer Console = Create(32);

		/// <summary>
		///   Represents all 32 input layers.
		/// </summary>
		public static readonly InputLayer All = new InputLayer { BitMask = unchecked ((int)0xffffffff) };

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="priority">
		///   The layer's priority. The higher the number, the higher the priority of the layer. Valid values range from 1 to and
		///   including 31 as layer 32 is reserved for the console.
		/// </param>
		public InputLayer(int priority)
			: this()
		{
			Assert.ArgumentInRange(priority, 1, 31);

			Priority = priority;
			BitMask = 1 << (priority - 1);
		}

		/// <summary>
		///   Gets the bit mask representing the input layer.
		/// </summary>
		internal int BitMask { get; private set; }

		/// <summary>
		///   Gets the layer's priority in the range from 1 to and including 32.
		/// </summary>
		internal int Priority { get; private set; }

		/// <summary>
		///   Gets a value indicating whether the input layer is a primitive one or whether it has been composed using the |
		///   operator.
		/// </summary>
		public bool IsPrimitive
		{
			get { return Priority >= 1 && Priority <= 32; }
		}

		/// <summary>
		///   Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		public bool Equals(InputLayer other)
		{
			Assert.That(Priority != 0 && other.Priority != 0, "Use the Contains method to compare two non-primitive input layers.");
			return BitMask == other.BitMask;
		}

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="priority">
		///   The layer's priority. The higher the number, the higher the priority of the layer. Valid values range from 1 to and
		///   including 32.
		/// </param>
		internal static InputLayer Create(int priority)
		{
			Assert.ArgumentInRange(priority, 1, 32);

			return new InputLayer
			{
				Priority = priority,
				BitMask = 1 << (priority - 1)
			};
		}

		/// <summary>
		///   Indicates whether this instance and a specified object are equal.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;

			return obj is InputLayer && Equals((InputLayer)obj);
		}

		/// <summary>
		///   Returns the hash code for this instance.
		/// </summary>
		public override int GetHashCode()
		{
			return BitMask;
		}

		/// <summary>
		///   Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		public static bool operator ==(InputLayer left, InputLayer right)
		{
			return left.Equals(right);
		}

		/// <summary>
		///   Indicates whether the current object is not equal to another object of the same type.
		/// </summary>
		public static bool operator !=(InputLayer left, InputLayer right)
		{
			return !left.Equals(right);
		}

		/// <summary>
		///   Combines the two input layers.
		/// </summary>
		public static InputLayer operator |(InputLayer left, InputLayer right)
		{
			return new InputLayer { BitMask = left.BitMask | right.BitMask };
		}

		/// <summary>
		///   Checks whether the input layer contains the given one.
		/// </summary>
		/// <param name="layer">The layer that should be checked.</param>
		[Pure]
		public bool Contains(InputLayer layer)
		{
			return (BitMask & layer.BitMask) != 0;
		}
	}
}