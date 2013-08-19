using System;

namespace Pegasus.Framework.Rendering.UserInterface
{
	/// <summary>
	///   Untyped base class for setters that apply property values.
	/// </summary>
	public abstract class Setter : ISealable
	{
		/// <summary>
		///   Gets a value indicating whether the setter is sealed and can no longer be modified.
		/// </summary>
		public bool IsSealed { get; private set; }

		/// <summary>
		///   Seals the setter such that it can no longer be modified.
		/// </summary>
		void ISealable.Seal()
		{
			IsSealed = true;
		}

		/// <summary>
		///   Applies the setter's value to the given dependency object and seals the setter.
		/// </summary>
		/// <param name="obj">The dependency object the setter should be applied to.</param>
		internal abstract void Apply(DependencyObject obj);
	}
}