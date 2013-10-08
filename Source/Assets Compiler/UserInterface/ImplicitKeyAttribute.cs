using System;

namespace Pegasus.AssetsCompiler.UserInterface
{
	/// <summary>
	///   When applied to a UI metaclass, denotes that the class provides an implicit key if no explicit key is set
	///   during the instantiation of the class within a dictionary.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	internal class ImplicitKeyAttribute : Attribute
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="property">The name of the property that should be used as the implicit key.</param>
		public ImplicitKeyAttribute(string property)
		{
			Assert.ArgumentNotNullOrWhitespace(property);
			Property = property;
		}

		/// <summary>
		///   Gets the name of the property that is used as the implicit key.
		/// </summary>
		public string Property { get; private set; }
	}
}