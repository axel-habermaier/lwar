using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using System.Linq;
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
		///   Gets the size of the constant buffer's contents in bytes.
		/// </summary>
		public int Size
		{
			get
			{
				var size = Constants.Sum(constant => SizeInBytes(constant.Type));
				if (size % 16 != 0)
					size = 16 * (size / 16 + 1);

				return size;
			}
		}

		/// <summary>
		///   The ordered set of constant that are contained in the constant buffer.
		/// </summary>
		public ShaderConstant[] Constants { get; private set; }

		/// <summary>
		///   Returns the size in bytes required to store a value of the given data type.
		/// </summary>
		/// <param name="dataType">The data type whose size should be returned.</param>
		private static int SizeInBytes(DataType dataType)
		{
			switch (dataType)
			{
				case DataType.Boolean:
					return 4;
				case DataType.Integer:
					return 4;
				case DataType.Float:
					return 4;
				case DataType.Vector2:
					return 8;
				case DataType.Vector3:
					return 12;
				case DataType.Vector4:
					return 16;
				case DataType.Matrix:
					return 64;
				default:
					throw new NotSupportedException("Unsupported data type.");
			}
		}
	}
}