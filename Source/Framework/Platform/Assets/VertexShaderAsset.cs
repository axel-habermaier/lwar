﻿using System;

namespace Pegasus.Framework.Platform.Assets
{
	using Graphics;

	/// <summary>
	///   Represents a vertex shader asset.
	/// </summary>
	internal sealed class VertexShaderAsset : CompiledAsset
	{
		/// <summary>
		///   The vertex shader that is managed by this asset instance.
		/// </summary>
		internal VertexShader Shader { get; private set; }

		/// <summary>
		///   Gets the friendly name of the asset.
		/// </summary>
		internal override string FriendlyName
		{
			get { return "Vertex Shader"; }
		}

		/// <summary>
		///   Loads or reloads the asset using the given asset reader.
		/// </summary>
		/// <param name="assetReader">The asset reader that should be used to load the asset.</param>
		internal override void Load(AssetReader assetReader)
		{
			if (Shader == null)
				Shader = new VertexShader(GraphicsDevice);
			
			Shader.Reinitialize(assetReader.Data);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Shader.SafeDispose();
		}
	}
}