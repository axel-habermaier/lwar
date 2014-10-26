namespace Pegasus.AssetsCompiler.Effects
{
	using System;

	/// <summary>
	///     When applied to a constant field of array type, indicates the size of the array.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class ArrayLengthAttribute : Attribute
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="count">The number of elements in the array.</param>
		public ArrayLengthAttribute(int count)
		{
		}
	}
}