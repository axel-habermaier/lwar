namespace Pegasus.AssetsCompiler.Assets
{
	using System;
	using System.Xml.Linq;
	using Commands;
	using Utilities;

	/// <summary>
	///     Represents a C# shader effect that requires compilation.
	/// </summary>
	internal class EffectAsset : Asset
	{
		/// <summary>
		///     The runtime name of the asset.
		/// </summary>
		private string _runtimeName;

		/// <summary>
		///     The runtime type of the asset.
		/// </summary>
		private string _runtimeType;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="metadata">The metadata of the asset.</param>
		public EffectAsset(XElement metadata)
			: base(metadata, "File")
		{
		}

		/// <summary>
		///     Gets the type of the asset.
		/// </summary>
		public override byte AssetType
		{
			get { return 4; }
		}

		/// <summary>
		///     Gets the runtime type of the asset.
		/// </summary>
		public override string RuntimeType
		{
			get { return Configuration.RootNamespace + "." + RelativeDirectory.Replace("/", ".").Replace(" ", "_") + "." + RuntimeName; }
		}

		/// <summary>
		///     Sets the runtime info of the effect.
		/// </summary>
		/// <param name="name">The runtime name of the effect.</param>
		/// <param name="type">The runtime type of the effect.</param>
		public void SetRuntimeInfo(string name, string type)
		{
			Assert.ArgumentNotNullOrWhitespace(name);
			Assert.ArgumentNotNullOrWhitespace(type);

			_runtimeName = name;
			_runtimeType = type;
		}
	}
}