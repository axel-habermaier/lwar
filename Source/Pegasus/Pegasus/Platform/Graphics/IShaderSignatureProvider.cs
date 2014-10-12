namespace Pegasus.Platform.Graphics
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	///     Provides shader signatures.
	/// </summary>
	public interface IShaderSignatureProvider
	{
		/// <summary>
		///     Gets the provided shader signatures.
		/// </summary>
		IEnumerable<ShaderSignature> GetShaderSignatures();
	}
}