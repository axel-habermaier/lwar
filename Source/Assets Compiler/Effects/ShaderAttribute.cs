namespace Pegasus.AssetsCompiler.Effects
{
	using System;
	using Platform.Graphics;

	/// <summary>
	///     When applied to a method, indicates that the method should be cross-compiled into a shader.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public abstract class ShaderAttribute : Attribute
	{
		/// <summary>
		///     Gets the type of the shader.
		/// </summary>
		internal abstract ShaderType ShaderType { get; }
	}
}