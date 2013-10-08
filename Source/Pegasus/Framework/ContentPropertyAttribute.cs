using System;

namespace Pegasus.Framework
{
	/// <summary>
	///   When applied to a class, denotes the property that should be treated as the element's content property.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class ContentPropertyAttribute : Attribute
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="name">The name of the content property.</param>
		public ContentPropertyAttribute(string name)
		{
			Assert.ArgumentNotNullOrWhitespace(name);
			Name = name;
		}

		/// <summary>
		///   Gets the name of the content property.
		/// </summary>
		public string Name { get; private set; }
	}
}