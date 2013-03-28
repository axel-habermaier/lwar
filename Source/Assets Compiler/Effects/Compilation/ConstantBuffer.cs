﻿using System;

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
		/// <param name="shared">Indicating whether the constant buffer should be shared between different effects.</param>
		public ConstantBuffer(int slot, ShaderConstant[] constants, bool shared = false)
		{
			Assert.ArgumentInRange(slot, () => slot, 0, 16);
			Assert.ArgumentNotNull(constants, () => constants);
			Assert.ArgumentSatisfies(constants.Length > 0, () => constants, "A constant buffer must contain at least one constant.");

			Name = "ConstantBuffer" + slot;
			Slot = slot;
			Constants = constants;
			Shared = shared;
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
		///   Gets a value indicating whether the constant buffer is shared between different effects.
		/// </summary>
		public bool Shared { get; private set; }

		/// <summary>
		///   The ordered set of constant that are contained in the constant buffer.
		/// </summary>
		public ShaderConstant[] Constants { get; private set; }

		/// <summary>
		///   Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			return string.Format("Constant Buffer (Slot {0}, Shared: {1})", Slot, Shared);
		}
	}
}