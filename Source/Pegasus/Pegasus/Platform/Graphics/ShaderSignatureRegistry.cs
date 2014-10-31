namespace Pegasus.Platform.Graphics
{
	using System;
	using System.Linq;
	using Logging;
	using Utilities;

	/// <summary>
	///     Provides access to all shader signatures used throughout the application.
	/// </summary>
	public static class ShaderSignatureRegistry
	{
		/// <summary>
		///     The registered shader signatures.
		/// </summary>
		private static readonly ShaderSignature[] ShaderSignatures;

		/// <summary>
		///     Initializes the type.
		/// </summary>
		static ShaderSignatureRegistry()
		{
			var providers = from assembly in AppDomain.CurrentDomain.GetAssemblies()
							from type in assembly.GetTypes()
							where type.IsClass && !type.IsAbstract && typeof(IShaderSignatureProvider).IsAssignableFrom(type)
							select (IShaderSignatureProvider)Activator.CreateInstance(type);

			ShaderSignatures = providers.SelectMany(provider => provider.GetShaderSignatures()).ToArray();
		}

		/// <summary>
		///     Gets a shader signature for the given vertex inputs.
		/// </summary>
		/// <param name="vertexInputBindings">The vertex inputs the shader signature should be retrieved for.</param>
		public static byte[] GetShaderSignature(VertexInputBinding[] vertexInputBindings)
		{
			Assert.ArgumentNotNull(vertexInputBindings);

			// This might be slow, but this doesn't happen often anyway...
			var compatibleSignatures = (from signature in ShaderSignatures
										where signature.Inputs.Length <= vertexInputBindings.Length
										let isCompatibleSignature = signature.Inputs.All(input =>
											vertexInputBindings.Any(binding => input.Format == binding.Format && input.Semantics == binding.Semantics))
										where isCompatibleSignature
										select signature).ToArray();

			if (compatibleSignatures.Length == 0)
				Log.Die("No compatible shader signature could be found for the requested vertex input bindings.");

			return compatibleSignatures[0].ByteCode;
		}
	}
}