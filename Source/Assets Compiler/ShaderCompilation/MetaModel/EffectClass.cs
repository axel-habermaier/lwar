using System;

namespace Pegasus.AssetsCompiler.ShaderCompilation.MetaModel
{
	using System.Linq;
	using System.Reflection;
	using Framework;

	/// <summary>
	///   Represents a C# class that contains cross-compiled shader code and shader constants.
	/// </summary>
	public class EffectClass
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="effectType">The type of the class that represents the effect.</param>
		public EffectClass(TypeInfo effectType)
		{
			Assert.ArgumentNotNull(effectType, () => effectType);

			Name = effectType.FullName;
			VertexShaders = GetShaders<VertexShaderAttribute>(effectType);
			FragmentShaders = GetShaders<FragmentShaderAttribute>(effectType);

			if (VertexShaders.Length == 0)
				Log.Error("Effect '{0}' must declare at least one vertex shader.", Name);

			if (FragmentShaders.Length == 0)
				Log.Error("Effect '{0}' must declare at least one fragment shader.", Name);
		}

		/// <summary>
		///   Gets the vertex shaders defined by the effect.
		/// </summary>
		public ShaderMethod[] VertexShaders { get; private set; }

		/// <summary>
		///   Gets the fragment shaders defined by the effect.
		/// </summary>
		public ShaderMethod[] FragmentShaders { get; private set; }

		/// <summary>
		///   Gets the name of the effect.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		///   Gets the shaders defined by the given type.
		/// </summary>
		/// <typeparam name="TAttribute">Determines which kind of shader should be returned.</typeparam>
		/// <param name="effectType">The type of the class that should be searched.</param>
		private static ShaderMethod[] GetShaders<TAttribute>(TypeInfo effectType)
			where TAttribute : Attribute
		{
			return effectType
				.DeclaredMethods
				.Where(m => m.GetCustomAttribute<TAttribute>() != null)
				.Select(m => new ShaderMethod(m))
				.ToArray();
		}

		/// <summary>
		///   Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			return string.Format("Name: {0}", Name);
		}
	}
}