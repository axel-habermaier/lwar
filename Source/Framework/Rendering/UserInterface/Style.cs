using System;

namespace Pegasus.Framework.Rendering.UserInterface
{
	/// <summary>
	///   Enables sharing of dependency property values between different dependency objects.
	/// </summary>
	public class Style
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public Style()
		{
			Setters = new SealableCollection<Setter>();
		}

		/// <summary>
		///   Gets the collection of setters that apply property values.
		/// </summary>
		public SealableCollection<Setter> Setters { get; private set; }

		/// <summary>
		/// Applies the style to the given dependency object and seals the style.
		/// </summary>
		/// <param name="obj">The dependency object the style should be applied to.</param>
		internal void Apply(DependencyObject obj)
		{
			Assert.ArgumentNotNull(obj);

			Setters.Seal();
			foreach (var setter in Setters)
				setter.Apply(obj);
		}
	}
}