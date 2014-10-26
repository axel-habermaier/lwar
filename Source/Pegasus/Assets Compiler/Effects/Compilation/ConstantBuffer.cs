namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Platform.Logging;
	using Utilities;

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
				var size = info.Offset + info.Size;

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
		///     Returns the size in bytes required to store a value of the given constant.
		/// </summary>
		/// <param name="constant">The shader constant whose size should be returned.</param>
		private static int SizeInBytes(ShaderConstant constant)
		{
			switch (constant.Type)
			{
				case DataType.Boolean:
				case DataType.Integer:
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
		///     Array elements are always aligned to 16-byte boundaries.
		///     http://geidav.wordpress.com/2013/03/05/hidden-hlsl-performance-hit-accessing-unpadded-arrays-in-constant-buffers/
		/// </summary>
		public IEnumerable<LayoutedShaderConstant> GetLayoutedConstants()
		{
			// Basically, we only have to ensure that a constant never crosses a 16-byte boundary
			var remainingBytes = 16;
			var offset = 0;

			foreach (var constant in Constants)
			{
				var elementSize = SizeInBytes(constant);
				var size = elementSize;
				var padding = 0;

				if (constant.IsArray && elementSize < 16)
				{
					size = 16 * constant.ArrayLength;
					padding = 16 - elementSize;
				}
				else if (constant.IsArray)
				{
					size = (int)Math.Ceiling(elementSize / 16.0) * 16;
					padding = size - elementSize;
					size *= constant.ArrayLength;
				}

				// All types with more than 16 bytes as well as arrays must start at a 16-byte boundary
				if (size > 16 || constant.IsArray)
				{
					if (remainingBytes != 16)
						offset += remainingBytes;

					remainingBytes = size;
				}
				else if (size > remainingBytes && !constant.IsArray)
				{
					offset += remainingBytes;
					remainingBytes = 16;
				}

				yield return new LayoutedShaderConstant
				{
					Constant = constant,
					Offset = offset,
					ElementSize = elementSize,
					ElementCount = constant.ArrayLength,
					Padding = padding,
					Size = size
				};

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
			///     The shader constant stored in a constant buffer.
			/// </summary>
			public ShaderConstant Constant;

			/// <summary>
			///     The number of elements.
			/// </summary>
			public int ElementCount;

			/// <summary>
			///     The size in bytes of a single element.
			/// </summary>
			public int ElementSize;

			/// <summary>
			///     The zero-based offset in bytes from the beginning of the constant buffer to the first byte of the shader
			///     constant.
			/// </summary>
			public int Offset;

			/// <summary>
			///     The padding in bytes after each element.
			/// </summary>
			public int Padding;

			/// <summary>
			/// The total size of the constant in bytes.
			/// </summary>
			public int Size;
		}
	}
}