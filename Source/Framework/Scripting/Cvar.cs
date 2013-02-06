using System;

namespace Pegasus.Framework.Scripting
{
	/// <summary>
	///   Represents a configurable value.
	/// </summary>
	/// <typeparam name="T">The type of the value.</typeparam>
	public sealed class Cvar<T> : ICvar
		where T : IEquatable<T>
	{
		/// <summary>
		///   The default value of the cvar.
		/// </summary>
		private readonly T _defaultValue;

		/// <summary>
		///   The current value of the cvar.
		/// </summary>
		private T _value;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="name">The external name of the cvar.</param>
		/// <param name="defaultValue">The default value of the cvar.</param>
		/// <param name="description">A description of the cvar's purpose.</param>
		public Cvar(string name, T defaultValue, string description)
		{
			Assert.ArgumentNotNullOrWhitespace(name, () => name);
			Assert.ArgumentNotNullOrWhitespace(description, () => description);

			Name = name;
			Description = description;
			_defaultValue = defaultValue;
			_value = defaultValue;

			CvarRegistry.Register(this);
		}

		/// <summary>
		///   Gets or sets the value of the cvar.
		/// </summary>
		public T Value
		{
			get { return _value; }
			set
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
				Log.Info("'{0}' is now '{1}'.", Name, value);

				if (Changed != null)
					Changed(oldValue);
			}
		}

		/// <summary>
		///   Gets the external name of the cvar that is used to refer to the cvar in the console, for instance.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		///   Gets a string describing the usage and the purpose of the cvar.
		/// </summary>
		public string Description { get; private set; }

		/// <summary>
		///   Gets the type of the cvar's value.
		/// </summary>
		Type ICvar.ValueType
		{
			get { return typeof(T); }
		}

		/// <summary>
		///   Sets the cvar's value to the given value.
		/// </summary>
		/// <param name="value">The new value of the cvar.</param>
		void ICvar.SetValue(object value)
		{
			Assert.ArgumentNotNull(value, () => value);
			Value = (T)value;
		}

		/// <summary>
		///   Raised when the value of the cvar is about to change, passing the new value to the event handlers.
		/// </summary>
		public event Action<T> Changing;

		/// <summary>
		///   Raised when the value of the cvar has changed, passing the old value to the event handlers.
		/// </summary>
		public event Action<T> Changed;

		/// <summary>
		///   Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			return String.Format("'{0}' is '{1}', default '{2}' [{3}]", Name, Value, _defaultValue,
								 TypeDescription.GetDescription<T>());
		}
	}
}