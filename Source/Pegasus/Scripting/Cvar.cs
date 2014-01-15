namespace Pegasus.Scripting
{
	using System;
	using System.Collections.Generic;
	using Platform.Logging;
	using Validators;

	/// <summary>
	///     Represents a configurable value.
	/// </summary>
	/// <typeparam name="T">The type of the value.</typeparam>
	public sealed class Cvar<T> : ICvar
	{
		/// <summary>
		///     The default value of the cvar.
		/// </summary>
		private readonly T _defaultValue;

		/// <summary>
		///     The validators that are used to validate the values of the cvar.
		/// </summary>
		private readonly ValidatorAttribute[] _validators;

		/// <summary>
		///     The current value of the cvar.
		/// </summary>
		private T _value;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="name">The external name of the cvar.</param>
		/// <param name="defaultValue">The default value of the cvar.</param>
		/// <param name="description">A description of the cvar's purpose.</param>
		/// <param name="mode">The update mode of the cvar.</param>
		/// <param name="persistent">Indicates whether the cvar's value should be persisted across sessions.</param>
		/// <param name="systemOnly">Indicates whether the cvar can only be set by the system and not via the console.</param>
		/// <param name="validators">The validators that should be used to validate a new cvar value before it is set.</param>
		public Cvar(string name, T defaultValue, string description, UpdateMode mode, bool persistent, bool systemOnly,
					params ValidatorAttribute[] validators)
		{
			Assert.ArgumentNotNullOrWhitespace(name);
			Assert.ArgumentNotNullOrWhitespace(description);
			Assert.InRange(mode);

			Name = name;
			Description = description;
			UpdateMode = mode;
			Persistent = persistent;
			SystemOnly = systemOnly;

			_defaultValue = defaultValue;
			_value = defaultValue;
			_validators = validators;
		}

		/// <summary>
		///     Gets or sets the value of the cvar.
		/// </summary>
		public T Value
		{
			get { return _value; }
			set
			{
				if (!ValidateValue(value))
					return;

				if (UpdateMode != UpdateMode.Immediate)
					DeferredUpdate(value);
				else
					UpdateValue(value);
			}
		}

		/// <summary>
		///     Gets the deferred value of the cvar that will be set the next time it is updated. This property has no meaning
		///     if the cvar's update mode is immediate.
		/// </summary>
		public T DeferredValue { get; private set; }

		/// <summary>
		///     Gets a value indicating whether the cvar is readonly and cannot be set from the console.
		/// </summary>
		public bool SystemOnly { get; private set; }

		/// <summary>
		///     Gets the validators that are used to validate the values of the cvar.
		/// </summary>
		public IEnumerable<ValidatorAttribute> Validators
		{
			get { return _validators; }
		}

		/// <summary>
		///     Sets the cvar's current value to the deferred one.
		/// </summary>
		public void SetDeferredValue()
		{
			Assert.That(HasDeferredValue, "The cvar does not have a deferred value.");

			UpdateValue(DeferredValue);
			DeferredValue = default(T);
			HasDeferredValue = false;
		}

		/// <summary>
		///     Gets a value indicating whether the cvar has a deferred update pending.
		/// </summary>
		public bool HasDeferredValue { get; private set; }

		/// <summary>
		///     Gets the deferred value of the cvar that will be set the next time it is updated. This property has no meaning
		///     if the cvar's update mode is immediate.
		/// </summary>
		object ICvar.DeferredValue
		{
			get { return DeferredValue; }
		}

		/// <summary>
		///     Gets the update mode of the cvar.
		/// </summary>
		public UpdateMode UpdateMode { get; private set; }

		/// <summary>
		///     Gets or sets the value of the cvar.
		/// </summary>
		object ICvar.Value
		{
			get { return _value; }
		}

		/// <summary>
		///     Gets the cvar's default value.
		/// </summary>
		object ICvar.DefaultValue
		{
			get { return _defaultValue; }
		}

		/// <summary>
		///     Indicates whether the cvar's value is persisted across sessions.
		/// </summary>
		public bool Persistent { get; private set; }

		/// <summary>
		///     Gets the external name of the cvar that is used to refer to the cvar in the console, for instance.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		///     Gets a string describing the usage and the purpose of the cvar.
		/// </summary>
		public string Description { get; private set; }

		/// <summary>
		///     Gets the type of the cvar's value.
		/// </summary>
		Type ICvar.ValueType
		{
			get { return typeof(T); }
		}

		/// <summary>
		///     Sets the cvar's value.
		/// </summary>
		/// <param name="value">The value that should be set.</param>
		/// <param name="setByUser">If true, indicates that the cvar was set by the user (e.g., via the console).</param>
		void ICvar.SetValue(object value, bool setByUser)
		{
			Assert.ArgumentNotNull(value);

			if (setByUser && SystemOnly)
				Log.Warn("'{0}' can only be set by the application.", Name);
			else
				Value = (T)value;
		}

		/// <summary>
		///     Validates the given value, returning true if the value is valid.
		/// </summary>
		/// <param name="value">The value that should be validated.</param>
		private bool ValidateValue(T value)
		{
			foreach (var validator in _validators)
			{
				if (validator.Validate(value))
					continue;

				Log.Error("'{0}' could not be set to '{1}\\\0': {2}", Name, TypeRegistry.ToString(value), validator.ErrorMessage);
				Log.Info("{0}", Help.GetHint(Name));
				return false;
			}

			return true;
		}

		/// <summary>
		///     Immediately updates the cvar's value to the given one, ignoring the cvar's update mode.
		/// </summary>
		/// <param name="value">The value that should be set.</param>
		public void SetImmediate(T value)
		{
			UpdateValue(value);
			if (HasDeferredValue && value.Equals(DeferredValue))
				HasDeferredValue = false;
		}

		/// <summary>
		///     Sets the cvar's current value to the new one.
		/// </summary>
		/// <param name="value">The value the cvar should be set to.</param>
		private void UpdateValue(T value)
		{
			if (_value.Equals(value))
			{
				Log.Warn("'{0}' has not been changed, because the new and the old value are the same.", Name);
				return;
			}

			if (Changing != null)
				Changing(value);

			var oldValue = _value;
			_value = value;
			Log.Info("'{0}' is now '{1}\\\0'.", Name, TypeRegistry.ToString(value));

			if (Changed != null)
				Changed(oldValue);
		}

		/// <summary>
		///     Sets the cvar's deferred value.
		/// </summary>
		/// <param name="value"></param>
		private void DeferredUpdate(T value)
		{
			// If both the current and the deferred value are the same as the given one, do nothing
			if (_value.Equals(value) && (!HasDeferredValue || DeferredValue.Equals(value)))
			{
				Log.Warn("'{0}' will not be changed, because the new and the old value are the same.", Name);
				return;
			}

			// If the given value resets the deferred update to the current value, cancel the deferred update
			if (_value.Equals(value) && HasDeferredValue && !DeferredValue.Equals(value))
			{
				HasDeferredValue = false;
				DeferredValue = default(T);

				Log.Info("'{0}' is now '{1}\\\0'.", Name, TypeRegistry.ToString(value));
				return;
			}

			// Otherwise, store the deferred value
			DeferredValue = value;
			HasDeferredValue = true;

			Log.Info("'{0}' will be set to '{1}\\\0'.", Name, TypeRegistry.ToString(value));
			Log.Warn("{0}", UpdateMode.ToDisplayString());
		}

		/// <summary>
		///     Raised when the value of the cvar is about to change, passing the new value to the event handlers.
		/// </summary>
		public event Action<T> Changing;

		/// <summary>
		///     Raised when the value of the cvar has changed, passing the old value to the event handlers.
		/// </summary>
		public event Action<T> Changed;
	}
}