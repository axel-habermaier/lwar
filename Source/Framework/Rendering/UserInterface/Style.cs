using System;

namespace Pegasus.Framework.Rendering.UserInterface
{
	/// <summary>
	///   Enables sharing of dependency property values between different dependency objects.
	/// </summary>
	public class Style : ISealable
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
		///   Gets a value indicating whether the style is sealed and can no longer be modified.
		/// </summary>
		public bool IsSealed { get; private set; }

		/// <summary>
		///   Seals the object such that it can no longer be modified.
		/// </summary>
		void ISealable.Seal()
		{
			((ISealable)Setters).Seal();
			foreach (var setter in Setters)
				((ISealable)setter).Seal();
		}

		/// <summary>
		///   Applies the style to the given dependency object and seals the style.
		/// </summary>
		/// <param name="obj">The dependency object the style should be applied to.</param>
		internal void Apply(DependencyObject obj)
		{
			Assert.ArgumentNotNull(obj);

			((ISealable)this).Seal();
			foreach (var setter in Setters)
				setter.Apply(obj);
		}
	}
}