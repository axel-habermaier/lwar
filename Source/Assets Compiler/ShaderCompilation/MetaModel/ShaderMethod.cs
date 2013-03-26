using System;

namespace Pegasus.AssetsCompiler.ShaderCompilation.MetaModel
{
	using System.Linq;
	using System.Reflection;
	using Framework;
	using Framework.Platform.Graphics;
	using Semantics;

	/// <summary>
	///   Represents a C# method that is cross-compiled to GLSL or HLSL.
	/// </summary>
	public class ShaderMethod
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="method">The method that represents the shader.</param>
		public ShaderMethod(MethodInfo method)
		{
			Assert.ArgumentNotNull(method, () => method);
			Name = String.Format("{0}.{1}", method.DeclaringType.FullName, method.Name);

			if (method.GetCustomAttribute<VertexShaderAttribute>() != null)
				Type = ShaderType.VertexShader;
			else if (method.GetCustomAttribute<FragmentShaderAttribute>() != null)
				Type = ShaderType.FragmentShader;
			else
				Log.Error("'{0}': Unknown shader type.", Name);

			if (method.ReturnType != typeof(void))
				Log.Error("'{0}': Expected return type 'void'.");

			Inputs = GetParameters(method, p => !p.IsOut);
			Outputs = GetParameters(method, p => p.IsOut);

			switch (Type)
			{
				case ShaderType.VertexShader:
					if (Outputs.All(o => o.Semantics.GetType() != typeof(PositionAttribute)))
						Log.Error("Vertex shader '{0}' must declare an output parameter with the 'Position' semantics.", Name);
					break;
				case ShaderType.FragmentShader:
					if (Outputs.All(o => o.Semantics.GetType() != typeof(ColorAttribute)))
						Log.Error("Fragment shader '{0}' must declare an output parameter with the 'Color' semantics.", Name);
					break;
				default:
					Log.Die("Unsupported shader type.");
					break;
			}
		}

		/// <summary>
		///   Gets the type of the shader.
		/// </summary>
		public ShaderType Type { get; private set; }

		/// <summary>
		///   Gets the name of the shader.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		///   Gets the shader inputs.
		/// </summary>
		public ShaderParameter[] Inputs { get; private set; }

		/// <summary>
		///   Gets the shader outputs.
		/// </summary>
		public ShaderParameter[] Outputs { get; private set; }

		/// <summary>
		///   Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			return string.Format("Name: {0}, Type: {1}", Name, Type);
		}

		/// <summary>
		///   Gets the parameters defined by the given method.
		/// </summary>
		/// <param name="method">The method that represents the shader.</param>
		/// <param name="predicate">The predicate that should be used to determine whether the parameter should be returned.</param>
		private static ShaderParameter[] GetParameters(MethodInfo method, Func<ParameterInfo, bool> predicate)
		{
			return method
				.GetParameters()
				.Where(predicate)
				.Select(p => new ShaderParameter(p))
				.ToArray();
		}
	}
}