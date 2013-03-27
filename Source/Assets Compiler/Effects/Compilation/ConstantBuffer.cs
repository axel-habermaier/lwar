using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using Framework;

	/// <summary>
	///   Represents a constant buffer that groups shader constants.
	/// </summary>
	internal class ConstantBuffer
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="slot">The slot the constant buffer should be bound to.</param>
		/// <param name="constants">The constant that should be contained in the constant buffer.</param>
		public ConstantBuffer(int slot, ShaderConstant[] constants)
			: this("ConstantBuffer" + slot, slot, constants)
		{
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="name">The name of the constant buffer.</param>
		/// <param name="slot">The slot the constant buffer should be bound to.</param>
		/// <param name="constants">The constant that should be contained in the constant buffer.</param>
		public ConstantBuffer(string name, int slot, ShaderConstant[] constants)
		{
			Assert.ArgumentNotNullOrWhitespace(name, () => name);
			Assert.ArgumentInRange(slot, () => slot, 0, 16);
			Assert.ArgumentNotNull(constants, () => constants);
			Assert.ArgumentSatisfies(constants.Length > 0, () => constants, "A constant buffer must contain at least one constant.");

			Name = name;
			Slot = slot;
			Constants = constants;
		}

		/// <summary>
		///   Gets the name of the constant buffer.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		///   Gets the slot of the constant buffer.
		/// </summary>
		public int Slot { get; private set; }

		/// <summary>
		///   The ordered set of constant that are contained in the constant buffer.
		/// </summary>
		public ShaderConstant[] Constants { get; private set; }

		/// <summary>
		///   Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			return string.Format("Name: {0}, Slot: {1}", Name, Slot);
		}
	}
}