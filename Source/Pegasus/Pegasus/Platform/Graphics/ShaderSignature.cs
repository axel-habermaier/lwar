namespace Pegasus.Platform.Graphics
{
	using System;
	using System.Linq;

	/// <summary>
	///     Represents a shader signature.
	/// </summary>
	public struct ShaderSignature : IEquatable<ShaderSignature>
	{
		/// <summary>
		///     The compiled shader signature byte code.
		/// </summary>
		public readonly byte[] ByteCode;

		/// <summary>
		///     The inputs of the shader.
		/// </summary>
		public readonly ShaderInput[] Inputs;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="inputs">The inputs of the shader.</param>
		public ShaderSignature(ShaderInput[] inputs)
		{
			Assert.ArgumentNotNull(inputs);
			Assert.ArgumentSatisfies(inputs.Length > 0, "Expected at least one input.");

			Inputs = inputs;
			ByteCode = null;
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="inputs">The inputs of the shader.</param>
		/// <param name="byteCode">The compiled shader signature byte code.</param>
		public ShaderSignature(ShaderInput[] inputs, byte[] byteCode)
		{
			Assert.ArgumentNotNull(inputs);
			Assert.ArgumentNotNull(byteCode);
			Assert.ArgumentSatisfies(inputs.Length > 0, "Expected at least one input.");

			Inputs = inputs;
			ByteCode = byteCode;
		}

		/// <summary>
		///     Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(ShaderSignature other)
		{
			var byteCodeEqual = ReferenceEquals(ByteCode, other.ByteCode) ||
								(ByteCode != null && other.ByteCode != null && ByteCode.SequenceEqual(other.ByteCode));

			var inputsEqual = ReferenceEquals(Inputs, other.Inputs) ||
							  (Inputs != null && other.Inputs != null && Inputs.SequenceEqual(other.Inputs));

			return byteCodeEqual && inputsEqual;
		}

		/// <summary>
		///     Indicates whether this instance and a specified object are equal.
		/// </summary>
		/// <param name="obj">Another object to compare to. </param>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;

			return obj is ShaderSignature && Equals((ShaderSignature)obj);
		}

		/// <summary>
		///     Returns the hash code for this instance.
		/// </summary>
		public override int GetHashCode()
		{
			unchecked
			{
				var hash = 0;

				if (ByteCode != null)
				{
					foreach (var b in ByteCode)
						hash ^= b * 397;
				}

				if (Inputs != null)
				{
					foreach (var input in Inputs)
						hash ^= input.GetHashCode() * 397;
				}

				return hash;
			}
		}
	}
}