using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	/// <summary>
	///   Represents a data object that can be read by a shader.
	/// </summary>
	internal interface IShaderDataObject
	{
		/// <summary>
		///   Gets the name of the shader constant.
		/// </summary>
		string Name { get; }

		/// <summary>
		///   Gets the type of the texture.
		/// </summary>
		DataType Type { get; }
	}
}