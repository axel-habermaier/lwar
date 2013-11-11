namespace Pegasus.Platform.Input
{
	using System;

	/// <summary>
	///   Determines the type of a binary input trigger.
	/// </summary>
	internal enum BinaryInputTriggerType
	{
		/// <summary>
		///   Indicates that the binary input trigger represents a chord, i.e., a trigger that triggers if and only
		///   if both of its constituting triggers trigger.
		/// </summary>
		Chord,

		/// <summary>
		///   Indicates that the binary input trigger represents a chord that triggers only for the first frame in which both of
		///   its sub-triggers trigger. The chord triggers again only after at least one of its two sub-triggers has not
		///   triggered for the duration of at least one frame.
		/// </summary>
		ChordOnce,

		/// <summary>
		///   Indicates that the binary input trigger represents an input alias, i.e., a trigger that triggers if and
		///   only if at least one of its two constituting triggers triggers.
		/// </summary>
		Alias
	}
}