using System;

namespace Pegasus.Rendering.UserInterface
{
	/// <summary>
	///   Represents the base class for a dependency property value.
	/// </summary>
	internal abstract class DependencyPropertyValue
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="propertyIndex">The index of the dependency property the value belongs to.</param>
		protected DependencyPropertyValue(int propertyIndex)
		{
			PropertyIndex = propertyIndex;
		}

		/// <summary>
		///   Gets the index of the dependency property the value belongs to.
		/// </summary>
		public int PropertyIndex { get; private set; }
	}
}