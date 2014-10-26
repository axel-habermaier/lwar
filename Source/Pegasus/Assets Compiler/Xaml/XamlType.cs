namespace Pegasus.AssetsCompiler.Xaml
{
	using System;
	using System.Linq;
	using System.Xml.Linq;
	using Utilities;

	/// <summary>
	///     Stores information about a type that can be referenced in a Xaml file.
	/// </summary>
	internal abstract class XamlType : IXamlType
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="file">The file the xaml type was defined in.</param>
		/// <param name="element">The element that defined the type.</param>
		protected XamlType(XamlTypeInfoFile file, XElement element)
		{
			Assert.ArgumentNotNull(file);
			Assert.ArgumentNotNull(element);

			File = file;
			Element = element;
		}

		/// <summary>
		///     Gets the file the xaml type was defined in.
		/// </summary>
		public XamlTypeInfoFile File { get; private set; }

		/// <summary>
		///     Gets the element that defined the type.
		/// </summary>
		public XElement Element { get; private set; }

		/// <summary>
		///     Gets the namespace the type belongs to.
		/// </summary>
		public string Namespace { get; protected set; }

		/// <summary>
		///     Gets the name of the type.
		/// </summary>
		public string Name { get; protected set; }

		/// <summary>
		///     Gets a value indicating whether the type is a list type.
		/// </summary>
		public bool IsList { get; protected set; }

		/// <summary>
		///     Gets a value indicating whether the type is a dictionary type.
		/// </summary>
		public bool IsDictionary { get; protected set; }

		/// <summary>
		///     Gets the full name of the type.
		/// </summary>
		public string FullName
		{
			get { return String.Format("{0}.{1}", Namespace, Name); }
			protected set
			{
				Assert.ArgumentNotNullOrWhitespace(value);
				var split = value.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

				Name = split[split.Length - 1];
				Namespace = String.Join(".", split.Take(split.Length - 1));
			}
		}
	}
}