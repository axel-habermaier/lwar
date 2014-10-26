namespace Pegasus.AssetsCompiler
{
	using System;
	using System.IO;
	using System.Security.Cryptography;
	using Utilities;

	/// <summary>
	///     Represents an MD5 hash of a file.
	/// </summary>
	public class Hash
	{
		/// <summary>
		///     The hash of the file.
		/// </summary>
		private byte[] _hash;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private Hash()
		{
		}

		/// <summary>
		///     Computes a hash for the given file.
		/// </summary>
		/// <param name="file">The file that should be hashed.</param>
		public static Hash Compute(string file)
		{
			Assert.ArgumentNotNullOrWhitespace(file);

			using (var cryptoProvider = new MD5CryptoServiceProvider())
			using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read))
				return new Hash { _hash = cryptoProvider.ComputeHash(stream) };
		}

		/// <summary>
		///     Loads a file hash saved on disk.
		/// </summary>
		/// <param name="file">The file that contains the hash.</param>
		public static Hash FromFile(string file)
		{
			Assert.ArgumentNotNullOrWhitespace(file);
			return new Hash { _hash = File.ReadAllBytes(file) };
		}

		/// <summary>
		///     Writes the hash to the given file.
		/// </summary>
		/// <param name="file">The file that the hash should be written to.</param>
		public void WriteTo(string file)
		{
			Assert.ArgumentNotNullOrWhitespace(file);
			File.WriteAllBytes(file, _hash);
		}

		/// <summary>
		///     Checks whether the given hash instance is equal to the current instance.
		/// </summary>
		protected bool Equals(Hash other)
		{
			if (other._hash.Length != _hash.Length)
				return false;

			for (var i = 0; i < other._hash.Length; ++i)
			{
				if (other._hash[i] != _hash[i])
					return false;
			}

			return true;
		}

		/// <summary>
		///     Determines whether the specified object is equal to the current instance.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != GetType())
				return false;

			return Equals((Hash)obj);
		}

		/// <summary>
		///     Serves as a hash function for a particular type.
		/// </summary>
		public override int GetHashCode()
		{
			return _hash != null ? _hash.GetHashCode() : 0;
		}

		/// <summary>
		///     Checks whether the two hash instances are equal.
		/// </summary>
		public static bool operator ==(Hash left, Hash right)
		{
			return Equals(left, right);
		}

		/// <summary>
		///     Checks whether the two hash instances are not equal.
		/// </summary>
		public static bool operator !=(Hash left, Hash right)
		{
			return !Equals(left, right);
		}
	}
}