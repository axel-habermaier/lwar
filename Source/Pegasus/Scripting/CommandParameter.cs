namespace Pegasus.Scripting
{
	using System;
	using System.Collections.Generic;
	using Platform;
	using Platform.Logging;
	using Validators;

	/// <summary>
	///   Represents a parameter of a command.
	/// </summary>
	public struct CommandParameter
	{
		/// <summary>
		///   The validators that are used to validate a parameter value.
		/// </summary>
		private readonly ValidatorAttribute[] _validators;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="type">The type of the parameter.</param>
		/// <param name="hasDefaultValue">Indicates whether the parameter has a default value.</param>
		/// <param name="defaultValue">The default value that should be used if no value has been specified.</param>
		/// <param name="description">The description of the parameter.</param>
		/// <param name="validators">The validators that should be used to validate a parameter value.</param>
		public CommandParameter(string name, Type type, bool hasDefaultValue, object defaultValue, string description,
								params ValidatorAttribute[] validators)
			: this()
		{
			Assert.ArgumentNotNullOrWhitespace(name);
			Assert.ArgumentNotNull(type);
			Assert.ArgumentNotNullOrWhitespace(description);

			Name = name;
			Type = type;
			HasDefaultValue = hasDefaultValue;
			DefaultValue = defaultValue;
			Description = description;
			_validators = validators;
		}

		/// <summary>
		///   Gets the validators that are used to validate the values of the parameter.
		/// </summary>
		public IEnumerable<ValidatorAttribute> Validators
		{
			get { return _validators; }
		}

		/// <summary>
		///   Gets the name of the parameter.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		///   Gets the type of the parameter.
		/// </summary>
		public Type Type { get; private set; }

		/// <summary>
		///   Gets the default value that is used if no value has been specified.
		/// </summary>
		public object DefaultValue { get; private set; }

		/// <summary>
		///   Gets a value indicating whether the parameter has a default value.
		/// </summary>
		public bool HasDefaultValue { get; private set; }

		/// <summary>
		///   Gets the description of the parameter.
		/// </summary>
		public string Description { get; private set; }

		/// <summary>
		///   Validates the given value.
		/// </summary>
		/// <param name="value">The value that should be validated.</param>
		[Pure]
		internal bool Validate(object value)
		{
			Assert.ArgumentNotNull(value);

			foreach (var validator in _validators)
			{
				if (validator.Validate(value))
					continue;

				Log.Error("'{0}' is not a valid value for parameter '{1}': {2}", value, Name, validator.ErrorMessage);
				return false;
			}

			return true;
		}
	}
}