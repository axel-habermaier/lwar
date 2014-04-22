namespace Pegasus.Platform.Assets
{
	using System;

	/// <summary>
	///     Represents a unique identifier for an asset that the asset manager uses to load the asset.
	/// </summary>
	public struct AssetIdentifier<T> : IEquatable<AssetIdentifier<T>>
		where T : class, IDisposable
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="assetName">The name of the asset.</param>
		/// <param name="projectIdentifier">The identifier of the assets project the asset belongs to.</param>
		/// <param name="assetIdentifier">The unique identifier of the asset within its asset project.</param>
		/// <param name="assetType">The type of the asset.</param>
		public AssetIdentifier(string assetName, byte projectIdentifier, ushort assetIdentifier, AssetType assetType)
			: this()
		{
			Assert.ArgumentNotNullOrWhitespace(assetName);
			Assert.ArgumentInRange(assetType);

			AssetName = assetName;
			ProjectIdentifier = projectIdentifier;
			Identifier = assetIdentifier;
			AssetType = assetType;
		}

		/// <summary>
		///     Gets the identifier of the assets project the asset belongs to.
		/// </summary>
		internal byte ProjectIdentifier { get; private set; }

		/// <summary>
		///     Gets the unique identifier of the asset within its asset project.
		/// </summary>
		internal ushort Identifier { get; private set; }

		/// <summary>
		///     Gets the type of the asset.
		/// </summary>
		internal AssetType AssetType { get; private set; }

		/// <summary>
		///     Gets the name of the asset.
		/// </summary>
		internal string AssetName { get; private set; }

		/// <summary>
		///     Gets the globally unique identifier of the asset.
		/// </summary>
		internal uint GlobalIdentifier
		{
			get { return (uint)((Identifier << 16) | ((byte)AssetType << 8) | (ProjectIdentifier)); }
		}

		/// <summary>
		///     Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(AssetIdentifier<T> other)
		{
			return ProjectIdentifier == other.ProjectIdentifier &&
				   Identifier == other.Identifier && AssetType == other.AssetType;
		}

		/// <summary>
		///     Indicates whether this instance and a specified object are equal.
		/// </summary>
		/// <param name="obj">Another object to compare to.</param>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			return obj is AssetIdentifier<T> && Equals((AssetIdentifier<T>)obj);
		}

		/// <summary>
		///     Returns the hash code for this instance.
		/// </summary>
		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = ProjectIdentifier.GetHashCode();
				hashCode = (hashCode * 397) ^ Identifier.GetHashCode();
				hashCode = (hashCode * 397) ^ (int)AssetType;
				return hashCode;
			}
		}

		/// <summary>
		///     Indicates whether the two asset identifiers are equal.
		/// </summary>
		public static bool operator ==(AssetIdentifier<T> left, AssetIdentifier<T> right)
		{
			return left.Equals(right);
		}

		/// <summary>
		///     Indicates whether the two asset identifiers are not equal.
		/// </summary>
		public static bool operator !=(AssetIdentifier<T> left, AssetIdentifier<T> right)
		{
			return !left.Equals(right);
		}
	}
}