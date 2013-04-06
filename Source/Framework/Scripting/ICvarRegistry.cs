using System;

namespace Pegasus.Framework.Scripting
{
	/// <summary>
	///   Declares the cvars required by the framework.
	/// </summary>
	public interface ICvarRegistry
	{
		/// <summary>
		///   The scaling factor that is applied to all timing values.
		/// </summary>
		[Cvar(1.0), Range(0.1, 10.0)]
		double TimeScale { get; set; }
	}
}