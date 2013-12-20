namespace Pegasus.AssetsCompiler.Assets.Attributes
{
	using System;

	/// <summary>
	///     When applied to an asset assembly, overrides the default compilation settings of an asset.
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public class IgnoreAttribute : Attribute
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="name">The name of the asset that should be ignored.</param>
		public IgnoreAttribute(string name)
		{
			Assert.ArgumentNotNullOrWhitespace(name);
			Name = name;
		}

		/// <summary>
		///     Gets the name of the asset that should be ignored.
		/// </summary>
		public string Name { get; private set; }
	}
}