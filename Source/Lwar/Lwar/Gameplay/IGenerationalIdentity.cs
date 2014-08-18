namespace Lwar.Gameplay
{
	using System;

	/// <summary>
	///     Provides access to the generational identity of an object.
	/// </summary>
	public interface IGenerationalIdentity
	{
		/// <summary>
		///     Gets the generational identifier of the object.
		/// </summary>
		Identifier Identifier { get; }
	}
}