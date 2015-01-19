namespace Pegasus.Platform.Graphics
{
	using System;

	/// <summary>
	///     Indicates which stencil operation should be used.
	/// </summary>
	public enum StencilOperation
	{
		Keep,
		Zero,
		Replace,
		IncrementAndClamp,
		DecrementAndClamp,
		Invert,
		Increment,
		Decrement
	}
}