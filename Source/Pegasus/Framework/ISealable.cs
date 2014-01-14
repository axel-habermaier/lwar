﻿namespace Pegasus.Framework
{
	using System;

	/// <summary>
	///     Represents a class that can be sealed, preventing any future modification of the class.
	/// </summary>
	public interface ISealable
	{
		/// <summary>
		///     Gets a value indicating whether the object is sealed and can no longer be modified.
		/// </summary>
		bool IsSealed { get; }

		/// <summary>
		///     Seals the object such that it can no longer be modified.
		/// </summary>
		void Seal();
	}
}