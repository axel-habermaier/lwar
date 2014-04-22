namespace Pegasus.AssetsCompiler.Assets
{
	using System;
	using Platform.Assets;
	using Platform.Graphics;

	/// <summary>
	///     Represents a vertex shader
	/// </summary>
	public class VertexShaderAsset : ShaderAsset
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="effect">The name of the effect the shader belongs to.</param>
		/// <param name="name">The name of the shader method.</param>
		public VertexShaderAsset(string effect, string name)
			: base(effect, name, ShaderType.VertexShader)
		{
		}

		/// <summary>
		///     Gets the type of the asset.
		/// </summary>
		public override AssetType AssetType
		{
			get { return AssetType.VertexShader; }
		}

		/// <summary>
		///     The identifier type that should be used for the asset when generating the asset identifier list. If null is
		///     returned, no asset identifier is generated for this asset instance.
		/// </summary>
		public override string IdentifierType
		{
			get { return "Pegasus.Platform.Graphics.VertexShader"; }
		}
	}
}