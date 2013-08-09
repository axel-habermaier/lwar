using System;

namespace Pegasus.Framework.Platform.Assets
{
	/// <summary>
	///   Represents a unique identifier for an asset that the asset manager uses to load the asset.
	/// </summary>
	public struct AssetIdentifier<T> : IEquatable<AssetIdentifier<T>>
		where T : class
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="assetName">The name of the asset.</param>
		public AssetIdentifier(string assetName)
			: this()
		{
			Assert.ArgumentNotNullOrWhitespace(assetName);
			AssetName = assetName;
		}

		/// <summary>
		///   Gets the name of the asset.
		/// </summary>
		internal string AssetName { get; private set; }

		/// <summary>
		///   Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(AssetIdentifier<T> other)
		{
			return string.Equals(AssetName, other.AssetName);
		}

		/// <summary>
		///   Indicates whether this instance and a specified object are equal.
		/// </summary>
		/// <param name="obj">Another object to compare to. </param>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			return obj is AssetIdentifier<T> && Equals((AssetIdentifier<T>)obj);
		}

		/// <summary>
		///   Returns the hash code for this instance.
		/// </summary>
		public override int GetHashCode()
		{
			return (AssetName != null ? AssetName.GetHashCode() : 0);
		}

		/// <summary>
		///   Indicates whether the two asset identifiers are equal.
		/// </summary>
		public static bool operator ==(AssetIdentifier<T> left, AssetIdentifier<T> right)
		{
			return left.Equals(right);
		}

		/// <summary>
		///   Indicates whether the two asset identifiers are not equal.
		/// </summary>
		public static bool operator !=(AssetIdentifier<T> left, AssetIdentifier<T> right)
		{
			return !left.Equals(right);
		}
	}
}