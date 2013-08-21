using System;

namespace Pegasus.Framework.UserInterface
{
	/// <summary>
	///   Untyped base class for triggers that apply property values when a certain condition is met.
	/// </summary>
	public abstract class Trigger : ISealable
	{
		/// <summary>
		///   The collection of setters that apply property values when the trigger is triggered.
		/// </summary>
		protected SealableCollection<Setter> _setters = SealableCollection<Setter>.Empty;

		/// <summary>
		///   Gets the collection of setters that apply property values when the trigger is triggered.
		/// </summary>
		public SealableCollection<Setter> Setters
		{
			get
			{
				if (_setters == SealableCollection<Setter>.Empty)
				{
					Assert.NotSealed(this);
					_setters = new SealableCollection<Setter>();
				}

				return _setters;
			}
		}

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