namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	/// <summary>
	///     Represents a constant buffer that groups shader constants.
	/// </summary>
	internal class ConstantBuffer
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="name">The name of the constant buffer;</param>
		/// <param name="slot">The slot the constant buffer should be bound to.</param>
		/// <param name="constants">The constant that should be contained in the constant buffer.</param>
		/// <param name="shared">Indicating whether the constant buffer should be shared between different effects.</param>
		public ConstantBuffer(string name, int slot, ShaderConstant[] constants, bool shared = false)
		{
			Assert.ArgumentInRange(slot, 0, 16);
			Assert.ArgumentNotNull(constants);
			Assert.ArgumentSatisfies(constants.Length > 0, "A constant buffer must contain at least one constant.");

			Name = name ?? "ConstantBuffer" + slot;
			Slot = slot;
			Constants = constants;
			Shared = shared;
		}

		/// <summary>
		///     Gets the name of the constant buffer.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		///     Gets the slot of the constant buffer.
		/// </summary>
		public int Slot { get; private set; }

		/// <summary>
		///     Gets a value indicating whether the constant buffer is shared between different effects.
		/// </summary>
		public bool Shared { get; private set; }

		/// <summary>
		///     Gets the size of the constant buffer's contents in bytes.
		/// </summary>
		public int Size
		{
			get
			{
				var info = GetLayoutedConstants().Last();
				var size = info.Offset + SizeInBytes(info.Constant.Type);

				if (size % 16 != 0)
					size = 16 * (size / 16 + 1);

				return size;
			}
		}

		/// <summary>
		///     The ordered set of constant that are contained in the constant buffer.
		/// </summary>
		public ShaderConstant[] Constants { get; private set; }

		/// <summary>
		///     Returns the size in bytes required to store a value of the given data type.
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

		/// <summary>
		///     Computes the constant buffer layout in accordance with the D3D11 constant buffer layouting rules and returns the
		///     layouted constants contained in the constant buffer.
		///     http://msdn.microsoft.com/en-us/library/windows/desktop/bb509632%28v=vs.85%29.aspx
		/// </summary>
		public IEnumerable<LayoutedShaderConstant> GetLayoutedConstants()
		{
			// Basically, we only have to ensure that a constant never crosses a 16-byte boundary
			var remainingBytes = 16;
			var offset = 0;

			foreach (var constant in Constants)
			{
				var size = SizeInBytes(constant.Type);

				// All types with more than 16 bytes must start at a 16-byte boundary
				if (size > 16)
				{
					if (remainingBytes != 16)
						offset += remainingBytes;

					remainingBytes = size;
				}
				else if (size > remainingBytes)
				{
					offset += remainingBytes;
					remainingBytes = 16;
				}

				yield return new LayoutedShaderConstant(constant, offset);
				offset += size;
				remainingBytes -= size;
			}
		}

		/// <summary>
		///     Provides information about a shader constant stored inside a constant buffer.
		/// </summary>
		public struct LayoutedShaderConstant
		{
			/// <summary>
			///     Initializes a new instance.
			/// </summary>
			/// <param name="constant">The shader constant stored in a constant buffer.</param>
			/// <param name="offset">
			///     The zero-based offset in bytes from the beginning of the constant buffer to the first byte of the
			///     shader constant.
			/// </param>
			public LayoutedShaderConstant(ShaderConstant constant, int offset)
				: this()
			{
				Constant = constant;
				Offset = offset;
			}

			/// <summary>
			///     Gets the shader constant stored in a constant buffer.
			/// </summary>
			public ShaderConstant Constant { get; private set; }

			/// <summary>
			///     Gets the zero-based offset in bytes from the beginning of the constant buffer to the first byte of the shader
			///     constant.
			/// </summary>
			public int Offset { get; private set; }
		}
	}
}