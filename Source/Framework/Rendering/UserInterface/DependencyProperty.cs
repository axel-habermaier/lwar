using System;

namespace Pegasus.Framework.Rendering.UserInterface
{
	/// <summary>
	///   Represents an untyped property that has multiple sources (such as data bindings, style setters, animation,
	///   etc.).
	/// </summary>
	public abstract class DependencyProperty
	{
		/// <summary>
		///   The number of dependency properties that have been registered throughout the lifetime of the application.
		/// </summary>
		private static int _propertyCount;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		protected DependencyProperty()
		{
			Index = _propertyCount++;
		}

		/// <summary>
		///   The index of the dependency property that remains unchanged and unique throughout the lifetime of the application.
		/// </summary>
		internal int Index { get; private set; }
	}
}