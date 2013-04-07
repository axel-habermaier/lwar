using System;

namespace Pegasus.Framework.Scripting.Parsing.Combinators
{
	using System.Collections.Generic;
	using System.Net;
	using BasicParsers;
	using Platform.Input;

	/// <summary>
	///   Parses the input based on a given type and returns the result as an instance of object.
	/// </summary>
	/// <typeparam name="TUserState">The type of the user state.</typeparam>
	public class TypeParser<TUserState> : CombinedParser<object, TUserState>
	{
		/// <summary>
		///   Parses an unquoted single-word string.
		/// </summary>
		private static readonly Parser<string, TUserState> UnquotedSingleWordString =
			String(c => !Char.IsWhiteSpace(c) && c != '"', c => !Char.IsWhiteSpace(c), "single-word string");

		/// <summary>
		///   Determines which parser should be used to parse an input of a given type.
		/// </summary>
		private static readonly Dictionary<Type, Parser<object, TUserState>> TypeParsers = new Dictionary
			<Type, Parser<object, TUserState>>
		{
			{ typeof(byte), AsObject(UInt8) },
			{ typeof(sbyte), AsObject(Int8) },
			{ typeof(short), AsObject(Int16) },
			{ typeof(ushort), AsObject(UInt16) },
			{ typeof(int), AsObject(Int32) },
			{ typeof(uint), AsObject(UInt32) },
			{ typeof(long), AsObject(Int64) },
			{ typeof(ulong), AsObject(UInt64) },
			{ typeof(double), AsObject(Float64) },
			{ typeof(float), AsObject(Float32) },
			{ typeof(string), AsObject(QuotedStringLiteral | UnquotedSingleWordString) },
			{ typeof(InputTrigger), AsObject(new InputTriggerParser<TUserState>()) },
			{ typeof(bool), AsObject(Boolean) },
			{ typeof(IPEndPoint), AsObject(new IPEndPointParser<TUserState>()) },
			{ typeof(IPAddress), AsObject(new IPAddressParser<TUserState>()) }
		};

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="type">The type that should be parsed.</param>
		public TypeParser(Type type)
		{
			Assert.ArgumentNotNull(type, () => type);
			Assert.That(TypeParsers.ContainsKey(type), "No parser has been registered for type '{0}'.", type.FullName);

			Parser = TypeParsers[type];
		}

		/// <summary>
		///   Returns the result of the given parser as an object instance.
		/// </summary>
		/// <typeparam name="T">The type of the parser's result.</typeparam>
		/// <param name="parser">The parser whose result should be returned as an object instance.</param>
		private static Parser<object, TUserState> AsObject<T>(Parser<T, TUserState> parser)
		{
			return parser.Apply(r => (object)r);
		}
	}
}