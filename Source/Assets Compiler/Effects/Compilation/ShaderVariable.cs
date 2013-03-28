using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	/// <summary>
	///   Represents a local variable of a shader.
	/// </summary>
	internal class ShaderVariable
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="name">The name of the variable.</param>
		/// <param name="type">The type of the variable.</param>
		public ShaderVariable(string name, DataType type)
		{
			Name = name;
			Type = type;
		}

		/// <summary>
		///   Gets the name of the variable.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		///   Gets the type of the variable.
		/// </summary>
		public DataType Type { get; private set; }
	}
}