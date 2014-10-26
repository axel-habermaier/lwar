namespace Pegasus.AssetsCompiler.Xaml
{
	using System;
	using Utilities;

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

			Namespace = String.Empty;
			Name = name;
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="T:System.Object" /> class.
		/// </summary>
		public XamlPrimitiveType(string declaringNamespace, string name)
		{
			Assert.ArgumentNotNullOrWhitespace(declaringNamespace);
			Assert.ArgumentNotNullOrWhitespace(name);

			Namespace = declaringNamespace;
			Name = name;
		}

		/// <summary>
		///     Gets the namespace the type belongs to.
		/// </summary>
		public string Namespace { get; private set; }

		/// <summary>
		///     Gets the name of the type.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		///     Gets the full name of the type.
		/// </summary>
		public string FullName
		{
			get
			{
				if (String.IsNullOrWhiteSpace(Namespace))
					return Name;

				return String.Format("{0}.{1}", Namespace, Name);
			}
		}

		/// <summary>
		///     Gets a value indicating whether the type is a list type.
		/// </summary>
		public bool IsList
		{
			get { return false; }
		}

		/// <summary>
		///     Gets a value indicating whether the type is a dictionary type.
		/// </summary>
		public bool IsDictionary
		{
			get { return false; }
		}
	}
}