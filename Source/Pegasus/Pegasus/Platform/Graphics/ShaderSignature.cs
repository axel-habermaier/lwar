namespace Pegasus.Platform.Graphics
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Logging;
	using Memory;
	using Utilities;

	/// <summary>
	///     Represents a shader signature.
	/// </summary>
	public struct ShaderSignature
	{
		/// <summary>
		///     The list of known shader signatures.
		/// </summary>
		private static readonly List<ShaderSignature> Signatures = new List<ShaderSignature>();

		/// <summary>
		///     The compiled shader signature byte code.
		/// </summary>
		private byte[] _byteCode;

		/// <summary>
		///     The inputs of the shader.
		/// </summary>
		private ShaderInput[] _inputs;

		/// <summary>
		///     Gets the number of shader inputs.
		/// </summary>
		private int InputCount
		{
			get { return _inputs.Length; }
		}

		/// <summary>
		///     Gets a shader signature for the given vertex inputs.
		/// </summary>
		/// <param name="vertexBindings">The vertex inputs the shader signature should be retrieved for.</param>
		internal static byte[] GetShaderSignature(VertexBinding[] vertexBindings)
		{
			Assert.ArgumentNotNull(vertexBindings);

			// This might be slow, but this doesn't happen often anyway...
			var compatibleSignatures = (from signature in Signatures
										where signature._inputs.Length <= vertexBindings.Length
										let isCompatibleSignature = signature._inputs.All(input =>
											vertexBindings.Any(binding => input.Format == binding.Format && input.Semantics == binding.Semantics))
										where isCompatibleSignature
										select signature).ToArray();

			if (compatibleSignatures.Length == 0)
				Log.Die("No compatible shader signature could be found for the requested vertex input bindings.");

			return compatibleSignatures[0]._byteCode;
		}

		/// <summary>
		///     Loads the shader signatures from the given buffer.
		/// </summary>
		/// <param name="buffer">The buffer the shader signature should be loaded from.</param>
		public static void LoadSignatures(ref BufferReader buffer)
		{
			var count = buffer.ReadInt32();
			Assert.That(count <= GraphicsDevice.MaxVertexBindings, "Too many shader inputs.");

			for (var i = 0; i < count; ++i)
			{
				// Load the signature from the buffer
				var signature = new ShaderSignature { _inputs = new ShaderInput[buffer.ReadByte()] };

				for (var j = 0; j < signature.InputCount; ++j)
				{
					signature._inputs[j].Format = (VertexDataFormat)(buffer.ReadByte());
					signature._inputs[j].Semantics = (DataSemantics)(buffer.ReadByte());
				}

				// Check if we already know of such a signature; if so, ignore this one, otherwise, 
				// add it to the list of known signatures.
				// This might be slow, but this doesn't happen often anyway...
				var found = false;
				foreach (var knownSignature in Signatures.Where(knownSignature => knownSignature.InputCount == signature.InputCount))
				{
					found = true;
					for (var j = 0; j < signature.InputCount && found; ++j)
					{
						found &= signature._inputs[j].Format == knownSignature._inputs[j].Format &&
								 signature._inputs[j].Semantics == knownSignature._inputs[j].Semantics;
					}

					if (found)
						break;
				}

				if (!found)
				{
					signature._byteCode = buffer.ReadByteArray();
					Signatures.Add(signature);
				}
				else
					buffer.Skip(buffer.ReadInt32());
			}
		}
	}
}