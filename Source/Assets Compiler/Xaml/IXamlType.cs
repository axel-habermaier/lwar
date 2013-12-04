using System;

namespace Pegasus.AssetsCompiler.Xaml
{
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
	}
}