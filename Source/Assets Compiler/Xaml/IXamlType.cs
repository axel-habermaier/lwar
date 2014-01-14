namespace Pegasus.AssetsCompiler.Xaml
{
	using System;

	/// <summary>
	///     Represents a type that can be referenced in a Xaml file.
	/// </summary>
	internal interface IXamlType
	{
		/// <summary>
		///     Gets the namespace the type belongs to.
		/// </summary>
		string Namespace { get; }

		/// <summary>
		///     Gets the name of the type.
		/// </summary>
		string Name { get; }

		/// <summary>
		///     Gets the full name of the type.
		/// </summary>
		string FullName { get; }

		/// <summary>
		///     Gets a value indicating whether the type is a list type.
		/// </summary>
		bool IsList { get; }

		/// <summary>
		///     Gets a value indicating whether the type is a dictionary type.
		/// </summary>
		bool IsDictionary { get; }
	}
}