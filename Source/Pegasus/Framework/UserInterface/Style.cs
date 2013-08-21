using System;

namespace Pegasus.Framework.UserInterface
{
	/// <summary>
	///   Enables sharing of dependency property values between different dependency objects.
	/// </summary>
	public class Style : ISealable
	{
		/// <summary>
		///   The style instance the current style is based on, inheriting all of its setters and triggers.
		/// </summary>
		private readonly Style _basedOn;

		/// <summary>
		///   The collection of setters that apply property values.
		/// </summary>
		private SealableCollection<Setter> _setters = SealableCollection<Setter>.Empty;

		/// <summary>
		///   The triggers that apply property values only when certain conditions hold.
		/// </summary>
		private SealableCollection<Trigger> _triggers = SealableCollection<Trigger>.Empty;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public Style()
		{
		}

		/// <summary>
		///   Initializes a new style instance, based on the given style.
		/// </summary>
		/// <param name="basedOn">The style instance the current style is based on, inheriting all of its setters and triggers.</param>
		public Style(Style basedOn)
		{
			Assert.ArgumentNotNull(basedOn);
			_basedOn = basedOn;
		}

		/// <summary>
		///   Gets the style instance the current style is based on, inheriting all of its setters and triggers.
		/// </summary>
		public Style BasedOn
		{
			get { return _basedOn; }
		}

		/// <summary>
		///   Gets the collection of setters that apply property values.
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
		///   Gets the triggers that apply property values only when certain conditions hold.
		/// </summary>
		public SealableCollection<Trigger> Triggers
		{
			get
			{
				if (_triggers == SealableCollection<Trigger>.Empty)
				{
					Assert.NotSealed(this);
					_triggers = new SealableCollection<Trigger>();
				}

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

			if (BasedOn != null)
				BasedOn.Seal();

			_setters.Seal();
			_triggers.Seal();
		}

		/// <summary>
		///   Applies the style to the given UI element.
		/// </summary>
		/// <param name="obj">The UI element the style should be applied to.</param>
		internal void Apply(UIElement obj)
		{
			Assert.ArgumentNotNull(obj);

			if (BasedOn != null)
				BasedOn.Apply(obj);

			foreach (var setter in _setters)
				setter.Apply(obj);

			foreach (var trigger in _triggers)
				trigger.Apply(obj);
		}

		/// <summary>
		///   Unsets the style from the given UI element.
		/// </summary>
		/// <param name="obj">The UI element the style should be unset from.</param>
		internal void Unset(UIElement obj)
		{
			Assert.ArgumentNotNull(obj);

			if (BasedOn != null)
				BasedOn.Unset(obj);

			foreach (var setter in _setters)
				setter.Unset(obj);

			foreach (var trigger in _triggers)
				trigger.Unset(obj);
		}
	}
}