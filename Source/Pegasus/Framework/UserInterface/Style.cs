namespace Pegasus.Framework.UserInterface
{
	using System;

	/// <summary>
	///   Enables sharing of dependency property values between different dependency objects.
	/// </summary>
	public sealed class Style : ISealable
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
		///   Initializes a new instance.
		/// </summary>
		public Style()
		{
		}

		/// <summary>
		///   Initializes a new style instance, based on the given style.
		/// </summary>
		/// <param name="baseStyle">The style instance the current style is based on, inheriting all of its setters and triggers.</param>
		public Style(Style baseStyle)
		{
			Assert.ArgumentNotNull(baseStyle);

			BaseStyle = baseStyle;
			CopyBaseStyle(baseStyle);

			baseStyle.Seal();
		}

		/// <summary>
		///   Gets the base style of this style, or null if there is none.
		/// </summary>
		public Style BaseStyle { get; private set; }

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
			if (IsSealed)
				return;

			IsSealed = true;

			_setters.Seal();
			_triggers.Seal();

			RegisterTriggerStateChangedEvents();
		}

		/// <summary>
		///   Applies the style to the given UI element.
		/// </summary>
		/// <param name="element">The UI element the style should be applied to.</param>
		internal void Apply(UIElement element)
		{
			Assert.ArgumentNotNull(element);

			Seal();

			foreach (var setter in _setters)
				setter.Apply(element);

			foreach (var trigger in _triggers)
				trigger.Apply(element);
		}

		/// <summary>
		///   Unsets the style from the given UI element.
		/// </summary>
		/// <param name="element">The UI element the style should be unset from.</param>
		internal void Unset(UIElement element)
		{
			Assert.ArgumentNotNull(element);

			foreach (var setter in _setters)
				setter.Unset(element);

			foreach (var trigger in _triggers)
				trigger.Unset(element);
		}

		/// <summary>
		///   Invoked when the triggered state of a trigger has potentially changed. In that case, we have to reapply all triggers
		///   in order to guarantee that the correct triggered value is set.
		/// </summary>
		/// <param name="element">The UI element for which the triggered state has changed.</param>
		private void OnTriggerStateChanged(UIElement element)
		{
			Assert.ArgumentNotNull(element);

			// If this is not the style set on the element, don't reapply the triggers. This happens when the current style
			// is used as a base style of the element's actual style. In that case, the element's actual style is responsible
			// for reapplying the triggers
			if (element.Style != this)
				return;

			foreach (var trigger in _triggers)
				trigger.Reapply(element);
		}

		/// <summary>
		///   Registers the trigger state changed event handler for all triggers of the style.
		/// </summary>
		private void RegisterTriggerStateChangedEvents()
		{
			foreach (var trigger in _triggers)
				trigger.TriggerStateChanged += OnTriggerStateChanged;
		}

		/// <summary>
		///   Recursively copies all setters and triggers of the base style to this instance.
		/// </summary>
		/// <param name="baseStyle">The base style that should be copied.</param>
		private void CopyBaseStyle(Style baseStyle)
		{
			if (baseStyle.BaseStyle != null)
				CopyBaseStyle(baseStyle.BaseStyle);

			var setters = Setters;
			var triggers = Triggers;

			foreach (var setter in baseStyle._setters)
				setters.Add(setter);

			foreach (var trigger in baseStyle._triggers)
				triggers.Add(trigger);
		}
	}
}