namespace Pegasus.Platform.Graphics
{
	using System;
	using System.Runtime.InteropServices;

	/// <summary>
	///     Represents the data returned by a timestamp disjoint query.
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Size = 16)]
	internal struct TimestampDisjointQueryResult
	{
		/// <summary>
		///     The frequency of the GPU's internal timer.
		/// </summary>
		public ulong Frequency;

		/// <summary>
		///     Indicates whether the timestamp queries that have been executed while the timestamp disjoint query was active
		///     returned valid data or should be discarded.
		/// </summary>
		public bool Disjoint;
	}
}