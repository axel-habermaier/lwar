using System;

namespace Pegasus.AssetsCompiler.ShaderCompilation.MetaModel
{
	using System.Linq;
	using System.Reflection;
	using Framework;
	using Semantics;

	/// <summary>
	///   Represents a parameter of a shader.
	/// </summary>
	public class ShaderParameter
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="parameter">The parameter that represents the shader input or output.</param>
		public ShaderParameter(ParameterInfo parameter)
		{
			Assert.ArgumentNotNull(parameter, () => parameter);

			Name = parameter.Name;
			FullName = String.Format("{0}.{1}:{2}", parameter.Member.DeclaringType.FullName, parameter.Member.Name, parameter.Name);
			IsOutput = parameter.IsOut;

			var attributes = parameter.GetCustomAttributes<SemanticsAttribute>().ToArray();
			if (attributes.Length != 1)
			{
				Log.Error("Parameter '{0}' must have exactly one semantics attribute.", FullName);
				Semantics = new TexCoordsAttribute(0);
			}
			else
				Semantics = attributes[0];
		}

		/// <summary>
		///   Gets the name of the shader parameter.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		///   Gets the full name of the shader parameter.
		/// </summary>
		public string FullName { get; private set; }

		/// <summary>
		///   Gets the semantics of the shader parameter.
		/// </summary>
		public SemanticsAttribute Semantics { get; private set; }

		/// <summary>
		///   Gets a value indicating whether the parameter is a shader output.
		/// </summary>
		public bool IsOutput { get; private set; }

		/// <summary>
		///   Gets the type of the parameter.
		/// </summary>
		public TypeInfo Type { get; private set; }

		/// <summary>
		///   Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			return string.Format("Name: {0}, Semantics: {1}, IsOutput: {2}", Name, Semantics, IsOutput);
		}
	}
}