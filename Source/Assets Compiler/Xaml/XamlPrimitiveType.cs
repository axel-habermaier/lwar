using System;

namespace Pegasus.AssetsCompiler.Xaml
{
	/// <summary>
	///     Represents a built-in primitive Xaml type such as int, string, or double.
	/// </summary>
	internal class XamlPrimitiveType : IXamlType
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="T:System.Object" /> class.
		/// </summary>
		public XamlPrimitiveType(string name)
		{
			Assert.ArgumentNotNullOrWhitespace(name);
			Name = name;
		}

		/// <summary>
		///     Gets the namespace the type belongs to.
		/// </summary>
		public string Namespace
		{
			get { return ""; }
		}

		/// <summary>
		///     Gets the name of the type.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		///     Gets the full name of the type.
		/// </summary>
		public string FullName
		{
			get { return Name; }
		}
	}
}