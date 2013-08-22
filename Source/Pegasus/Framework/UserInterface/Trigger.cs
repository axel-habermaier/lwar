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
		///   Raised when the triggered state potentially changed for the UI element.
		/// </summary>
		internal event Action<UIElement> TriggerStateChanged;

		/// <summary>
		///   Raises the trigger state changed event.
		/// </summary>
		/// <param name="element">The UI element for which the trigger state has changed.</param>
		protected void RaiseTriggerStateChanged(UIElement element)
		{
			Assert.ArgumentNotNull(element);

			if (TriggerStateChanged != null)
				TriggerStateChanged(element);
		}

		/// <summary>
		///   Applies the trigger to the given UI element.
		/// </summary>
		/// <param name="element">The UI element the setter should be applied to.</param>
		internal abstract void Apply(UIElement element);

		/// <summary>
		///   Unsets the all triggered values from the given UI element.
		/// </summary>
		/// <param name="element">The UI element the style should be unset from.</param>
		internal abstract void Unset(UIElement element);

		/// <summary>
		///   Reapplies all setters of the trigger if it is currently triggered.
		/// </summary>
		/// <param name="element">The UI element the triggered setters should be reapplied to.</param>
		internal abstract void Reapply(UIElement element);
	}
}