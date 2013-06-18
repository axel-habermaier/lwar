using System;

namespace Lwar.Assets.Templates.Compilation
{
	using Pegasus.AssetsCompiler.Assets;
	using Pegasus.AssetsCompiler.Assets.Attributes;
	using Pegasus.Framework;

	/// <summary>
	///   When applied to an asset assembly, uses the TemplateCompiler to process the C# file.
	/// </summary>
	public class TemplatesAttribute : AssetAttribute
	{
		/// <summary>
		///   The name of the asset.
		/// </summary>
		private readonly string _name;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="name">The name of the asset.</param>
		public TemplatesAttribute(string name)
		{
			Assert.ArgumentNotNullOrWhitespace(name);
			_name = name;
		}

		/// <summary>
		///   Gets the asset that should be compiled.
		/// </summary>
		public override Asset Asset
		{
			get { return new TemplateAsset(_name); }
		}
	}
}