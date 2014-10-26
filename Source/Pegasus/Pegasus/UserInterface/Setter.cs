namespace Pegasus.UserInterface
{
	using System;
	using UserInterface;

	/// <summary>
	///     Untyped base class for setters that apply property values.
	/// </summary>
	public abstract class Setter : ISealable
	{
		/// <summary>
		///     Gets a value indicating whether the setter is sealed and can no longer be modified.
		/// </summary>
		public bool IsSealed { get; private set; }

		/// <summary>
		///     Seals the setter such that it can no longer be modified.
		/// </summary>
		public void Seal()
		{
			IsSealed = true;
		}

		/// <summary>
		///     Applies the setter's value to the given dependency object.
		/// </summary>
		/// <param name="element">The UI element the setter's value should be applied to.</param>
		internal abstract void Apply(UIElement element);

		/// <summary>
		///     Applies the setter's value to the given dependency object when the setter is applied as the result of a trigger being
		///     triggered.
		/// </summary>
		/// <param name="element">The UI element the setter's value should be applied to.</param>
		internal abstract void ApplyTriggered(UIElement element);

		/// <summary>
		///     Unsets the setter's value from the given dependency object when the setter is no longer applied as the result of a
		///     trigger being triggered.
		/// </summary>
		/// <param name="element">The UI element the setter's value should be applied to.</param>
		internal abstract void UnsetTriggered(UIElement element);

		/// <summary>
		///     Unsets the setter's value from the given UI element.
		/// </summary>
		/// <param name="element">The UI element the setter's value should be unset from.</param>
		internal abstract void Unset(UIElement element);
	}
}