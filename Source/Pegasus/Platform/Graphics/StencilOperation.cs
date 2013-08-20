using System;

namespace Pegasus.Platform.Graphics
{
	/// <summary>
	///   Indicates which stencil operation should be used.
	/// </summary>
	public enum StencilOperation
	{
		Keep = 2001,
		Zero = 2002,
		Replace = 2003,
		IncrementAndClamp = 2004,
		DecrementAndClamp = 2005,
		Invert = 2006,
		Increment = 2007,
		Decrement = 2008,
	}
}