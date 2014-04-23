namespace Pegasus.AssetsCompiler.Assets
{
	using System;
	using Pegasus.Assets;
	using Platform.Graphics;

	/// <summary>
	///     Represents a fragment shader
	/// </summary>
	public class FragmentShaderAsset : ShaderAsset
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="effect">The name of the effect the shader belongs to.</param>
		/// <param name="name">The name of the shader method.</param>
		public FragmentShaderAsset(string effect, string name)
			: base(effect, name, ShaderType.FragmentShader)
		{
		}

		/// <summary>
		///     Gets the type of the asset.
		/// </summary>
		public override AssetType AssetType
		{
			get { return AssetType.FragmentShader; }
		}

		/// <summary>
		///     The identifier type that should be used for the asset when generating the asset identifier list. If null is
		///     returned, no asset identifier is generated for this asset instance.
		/// </summary>
		public override string IdentifierType
		{
			get { return "Pegasus.Platform.Graphics.FragmentShader"; }
		}
	}
}