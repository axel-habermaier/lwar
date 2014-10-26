namespace Pegasus.Scripting
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using Parsing;
	using Parsing.BasicParsers;
	using Parsing.Combinators;
	using Platform.Logging;
	using UserInterface.Controls;
	using UserInterface.Input;
	using Utilities;

	/// <summary>
	///     Manages the types that can be used for command parameters and cvar values. All enumerations types as well as most C#
	///     built-in types and common .NET and Pegasus framework types are supported automatically. Enumeration types can also be
	///     registered, in which case they overwrite the defaults.
	/// </summary>
	public static class TypeRegistry
	{
		/// <summary>
		///     Stores the registered type information.
		/// </summary>
		private static readonly Dictionary<Type, TypeInfo> RegisteredTypes = new Dictionary<Type, TypeInfo>();

		/// <summary>
		///     Initializes the type and registers all built-in types.
		/// </summary>
		static TypeRegistry()
		{
			var unquotedSingleWordString = new StringParser(c => !Char.IsWhiteSpace(c) && c != '"', c => !Char.IsWhiteSpace(c), "string");
			var stringParser = new QuotedStringParser() | unquotedSingleWordString;

			// Register the C# built-in types (except for char, for which there is no obvious use-case)
			Register(Parsers.Boolean, "Boolean", b => b ? "true" : "false", "true", "false", "0", "1", "on", "off");
			Register(Parsers.UInt8, "8-bit unsigned integer", null, "0", "17");
			Register(Parsers.Int8, "8-bit signed integer", null, "-17", "0", "17");
			Register(Parsers.UInt16, "16-bit unsigned integer", null, "0", "17");
			Register(Parsers.Int16, "16-bit signed integer", null, "-17", "0", "17");
			Register(Parsers.UInt32, "32-bit unsigned integer", null, "0", "17");
			Register(Parsers.Int32, "32-bit signed integer", null, "-17", "0", "17");
			Register(Parsers.UInt64, "64-bit unsigned integer", null, "0", "17");
			Register(Parsers.Int64, "64-bit signed integer", null, "-17", "0", "17");
			Register(Parsers.Float32, "32-bit floating point number", f => f.ToString("F"), "-17.1", "0.0", "17");
			Register(Parsers.Float64, "64-bit floating point number", d => d.ToString("F"), "-17.1", "0.0", "17");
			Register(stringParser, "string", null, "\"\"", "word", "\"multiple words\"", "\"escaped quote: \\\"\"");

			// Register default Pegasus framework types
			Register(new IPAddressParser(), "IPv4 or IPv6 address", null, "::1", "127.0.0.1");
			Register(new IPEndPointParser(), "IPv4 or IPv6 address with optional port", null, "::1", "[::1]:8081", "127.0.0.1", "127.0.0.1:8081");
			Register(new EnumerationLiteralParser<WindowMode>(ignoreCase: true), null, null);
			Register(new Vector2Parser(), null, v => String.Format("{0};{1}", v.X, v.Y), "0;0", "-10.0;10.5");
			Register(new SizeParser(), null, s => String.Format("{0}x{1}", s.Width, s.Height), "0x0", "-10x10.5", "1920x1200");
			Register(new EnumerationLiteralParser<Key>(false), "Keyboard key name", null, "A", "B", "LeftControl", "Return", "F1");
			Register(new EnumerationLiteralParser<MouseButton>(false), "Mouse button", null, "Left", "Right", "Middle", "XButton1", "XButton2");
			Register(new InputTriggerParser(), null, i => String.Format("[{0}]", i), "[Key(Return,WentDown)]", "[Key(A,Pressed)]",
				"[Key(Left,Repeated)]", "[Mouse(Left,Pressed) | Mouse(Right,Pressed)]");
			Register(new ConfigurableInputParser(), null, null, "Key.A+Control", "Mouse.Left+Alt", "Mouse.XButton1+Shift+Alt");
		}

		/// <summary>
		///     Registers the given type.
		/// </summary>
		/// <typeparam name="T">The type that should be registered.</typeparam>
		/// <param name="parser">A parser that parses an input string into the given type.</param>
		/// <param name="description">A more user-friendly description of the type. If null, the C# type name is used.</param>
		/// <param name="toString">Converts a value of the type to a string. If null, object.ToString() is used.</param>
		/// <param name="examples">
		///     A list of example values for the type that can be parsed by the parser. Can only contain zero elements for
		///     enumeration types, where a list of all enumeration literals can be generated automatically.
		/// </param>
		public static void Register<T>(Parser<T> parser, string description, Func<T, string> toString, params string[] examples)
		{
			Assert.ArgumentNotNull(parser);
			Assert.ArgumentNotNull(examples);
			Assert.ArgumentSatisfies(description == null || !String.IsNullOrWhiteSpace(description), "The description cannot be empty.");
			Assert.ArgumentSatisfies(examples.Length > 0 || typeof(T).GetTypeInfo().IsEnum, "The examples can be empty for enumeration types only.");
			Assert.That(!RegisteredTypes.ContainsKey(typeof(T)), "The type has already been registered.");

			description = description ?? typeof(T).Name;

			if (examples.Length == 0)
				examples = Enum.GetNames(typeof(T));

			Func<object, string> objToString;
			if (toString == null)
				objToString = o => o.ToString();
			else
				objToString = o => toString((T)o);

			var objParser = parser.Apply(value => (object)value);
			RegisteredTypes.Add(typeof(T), new TypeInfo(objParser, description, examples, objToString));
		}

		/// <summary>
		///     Gets a parser for the given type.
		/// </summary>
		/// <typeparam name="T">The type for which the parser should be returned.</typeparam>
		internal static Parser<object> GetParser<T>()
		{
			return GetParser(typeof(T));
		}

		/// <summary>
		///     Gets a parser for the given type.
		/// </summary>
		/// <param name="type">The type for which the parser should be returned.</param>
		internal static Parser<object> GetParser(Type type)
		{
			Assert.ArgumentNotNull(type);

			TypeInfo info;
			if (RegisteredTypes.TryGetValue(type, out info))
				return info.Parser;

			if (type.GetTypeInfo().IsEnum)
				return new EnumerationLiteralParser<object>(type, false);

			Log.Die("An attempt was made to get a parser for an unregistered type.");
			return null;
		}

		/// <summary>
		///     Gets the user-friendly description of the given type.
		/// </summary>
		/// <typeparam name="T">The type for which the user-friendly description should be returned.</typeparam>
		internal static string GetDescription<T>()
		{
			return GetDescription(typeof(T));
		}

		/// <summary>
		///     Gets the user-friendly description of the given type.
		/// </summary>
		/// <param name="type">The type for which the user-friendly description should be returned.</param>
		internal static string GetDescription(Type type)
		{
			Assert.ArgumentNotNull(type);

			TypeInfo info;
			if (RegisteredTypes.TryGetValue(type, out info))
				return info.Description;

			if (type.GetTypeInfo().IsEnum)
				return type.Name;

			Log.Die("An attempt was made to get the description of an unregistered type.");
			return null;
		}

		/// <summary>
		///     Gets the examples for the given type.
		/// </summary>
		/// <typeparam name="T">The type for which the examples should be returned.</typeparam>
		internal static IEnumerable<string> GetExamples<T>()
		{
			return GetExamples(typeof(T));
		}

		/// <summary>
		///     Gets the examples for the given type.
		/// </summary>
		/// <param name="type">The type for which the examples should be returned.</param>
		internal static IEnumerable<string> GetExamples(Type type)
		{
			Assert.ArgumentNotNull(type);

			TypeInfo info;
			if (RegisteredTypes.TryGetValue(type, out info))
				return info.Examples;

			if (type.GetTypeInfo().IsEnum)
				return Enum.GetNames(type);

			Log.Die("An attempt was made to get examples for an unregistered type.");
			return null;
		}

		/// <summary>
		///     Gets the string representation of the given value.
		/// </summary>
		/// <param name="value">The value for which the string representation should be returned.</param>
		internal static string ToString(object value)
		{
			Assert.ArgumentNotNull(value);

			TypeInfo info;
			if (RegisteredTypes.TryGetValue(value.GetType(), out info))
				return info.ToDisplayString(value);

			if (value.GetType().GetTypeInfo().IsEnum)
				return value.ToString();

			// Try all base types of the value, as typically only the base type of a hierarchy is registered
			var baseType = value.GetType().GetTypeInfo().BaseType;
			while (baseType != null && baseType != typeof(object))
			{
				if (RegisteredTypes.TryGetValue(baseType, out info))
					return info.ToDisplayString(value);

				baseType = baseType.GetTypeInfo().BaseType;
			}

			Log.Die("An attempt was made to get the string representation of a value of an unregistered type.");
			return null;
		}

		/// <summary>
		///     Stores the information about a registered type.
		/// </summary>
		private struct TypeInfo
		{
			/// <summary>
			///     Initializes a new instance.
			/// </summary>
			/// <param name="parser">The parser for the type.</param>
			/// <param name="description">A user-friendly description of the type.</param>
			/// <param name="examples">A list of example values of the type.</param>
			/// <param name="toDisplayString">Converts a value of the type to a string.</param>
			public TypeInfo(Parser<object> parser, string description, string[] examples, Func<object, string> toDisplayString)
				: this()
			{
				Assert.ArgumentNotNullOrWhitespace(description);
				Assert.ArgumentNotNull(examples);
				Assert.ArgumentSatisfies(examples.Length > 0, "There must be at least one example.");
				Assert.ArgumentNotNull(toDisplayString);

				Parser = parser;
				Description = description;
				Examples = examples;
				ToDisplayString = toDisplayString;
			}

			/// <summary>
			///     Gets the parser for the type.
			/// </summary>
			public Parser<object> Parser { get; private set; }

			/// <summary>
			///     Gets the user-friendly description of the type.
			/// </summary>
			public string Description { get; private set; }

			/// <summary>
			///     Gets the examples of the type.
			/// </summary>
			public string[] Examples { get; private set; }

			/// <summary>
			///     Gets a function that converts an instance of the type into a string.
			/// </summary>
			public Func<object, string> ToDisplayString { get; private set; }
		}
	}
}