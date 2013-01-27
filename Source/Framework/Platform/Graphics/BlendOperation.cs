using System;

namespace Pegasus.Framework.Platform.Graphics
{
	/// <summary>
	///   Indicates which operation should be used for blending.
	/// </summary>
	public enum BlendOperation
	{
		/// <summary>
		///   Indicates that the blend operation adds the values.
		/// </summary>
		Add = 1001,

		/// <summary>
		///   Indicates that the blend operation subtracts the values.
		/// </summary>
		Subtract = 1002,

		ReverseSubtract = 1003,

		Minimum = 1004,

		Maximum = 1005
	}
}