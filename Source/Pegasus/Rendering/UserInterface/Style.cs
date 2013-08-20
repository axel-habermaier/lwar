using System;

namespace Pegasus.Rendering.UserInterface
{
	/// <summary>
	///   Enables sharing of dependency property values between different dependency objects.
	/// </summary>
	public class Style : ISealable
	{
		/// <summary>
		///   The collection of setters that apply property values.
		/// </summary>
		private SealableCollection<Setter> _setters = SealableCollection<Setter>.Empty;

		/// <summary>
		///   The triggers that apply property values only when certain conditions hold.
		/// </summary>
		private SealableCollection<Trigger> _triggers = SealableCollection<Trigger>.Empty;

		/// <summary>
		///   Gets the collection of setters that apply property values.
		/// </summary>
		public SealableCollection<Setter> Setters
		{
			get
			{
				if (_setters == SealableCollection<Setter>.Empty)
					_setters = new SealableCollection<Setter>();

				return _setters;
			}
		}

		/// <summary>
		///   Gets the triggers that apply property values only when certain conditions hold.
		/// </summary>
		public SealableCollection<Trigger> Triggers
		{
			get
			{
				if (_triggers == SealableCollection<Trigger>.Empty)
					_triggers = new SealableCollection<Trigger>();

				return _triggers;
			}
		}

		/// <summary>
		///   Gets a value indicating whether the style is sealed and can no longer be modified.
		/// </summary>
		public bool IsSealed { get; private set; }

		/// <summary>
		///   Seals the object such that it can no longer be modified.
		/// </summary>
		public void Seal()
		{
			IsSealed = true;

			Setters.Seal();
			Triggers.Seal();
		}

		/// <summary>
		///   Applies the style to the given UI element.
		/// </summary>
		/// <param name="obj">The UI element the style should be applied to.</param>
		internal void Apply(UIElement obj)
		{
			Assert.ArgumentNotNull(obj);

			foreach (var setter in Setters)
				setter.Apply(obj);

			foreach (var trigger in Triggers)
				trigger.Apply(obj);
		}

		/// <summary>
		///   Unsets the style from the given UI element.
		/// </summary>
		/// <param name="obj">The UI element the style should be unset from.</param>
		internal void Unset(UIElement obj)
		{
			Assert.ArgumentNotNull(obj);

			foreach (var setter in Setters)
				setter.Unset(obj);

			foreach (var trigger in Triggers)
				trigger.Unset(obj);
		}
	}
}