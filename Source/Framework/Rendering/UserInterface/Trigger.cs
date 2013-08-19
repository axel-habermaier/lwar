using System;

namespace Pegasus.Framework.Rendering.UserInterface
{
	/// <summary>
	///   Untyped base class for triggers that apply property values when a certain condition is met.
	/// </summary>
	public abstract class Trigger : ISealable
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		protected Trigger()
		{
			Setters = new SealableCollection<Setter>();
		}

		/// <summary>
		///   Gets the collection of setters that apply property values.
		/// </summary>
		public SealableCollection<Setter> Setters { get; private set; }

		/// <summary>
		///   Gets a value indicating whether the object is sealed and can no longer be modified.
		/// </summary>
		public bool IsSealed { get; private set; }

		/// <summary>
		///   Seals the object such that it can no longer be modified.
		/// </summary>
		public void Seal()
		{
			IsSealed = true;
		}

		/// <summary>
		///   Applies the trigger to the given UI element.
		/// </summary>
		/// <param name="obj">The UI element the setter should be applied to.</param>
		internal abstract void Apply(UIElement obj);

		/// <summary>
		///   Unsets the style from the given UI element.
		/// </summary>
		/// <param name="obj">The UI element the style should be unset from.</param>
		internal abstract void Unset(UIElement obj);
	}
}