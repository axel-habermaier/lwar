namespace Pegasus.Entities
{
	using System;
	using Platform.Memory;

	/// <summary>
	///     A base class for entity components.
	/// </summary>
	public abstract class Component : UniquePooledObject
	{
		/// <summary>
		///     Gets or sets the next component in an intrusive component list.
		/// </summary>
		internal Component Next { get; set; }

		/// <summary>
		///     Invoked when the pooled instance is returned to the pool.
		/// </summary>
		protected override void OnReturning()
		{
			Next = null;
		}
	}
}