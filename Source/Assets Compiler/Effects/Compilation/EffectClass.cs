using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using System.Linq;
	using Framework;
	using ICSharpCode.NRefactory.CSharp;

	/// <summary>
	///   Represents a C# class that contains cross-compiled shader code and shader constants.
	/// </summary>
	internal class EffectClass
	{
		/// <summary>
		///   The declaration of the class that represents the effect.
		/// </summary>
		private readonly TypeDeclaration _type;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="type">The declaration of the class that represents the effect.</param>
		public EffectClass(TypeDeclaration type)
		{
			Assert.ArgumentNotNull(type, () => type);
			_type = type;
		}

		///// <summary>
		/////   Initializes a new instance.
		///// </summary>
		///// <param name="effectType">The type of the class that represents the effect.</param>
		//public EffectClass(TypeInfo effectType)
		//{
		//	Assert.ArgumentNotNull(effectType, () => effectType);

		//	Name = effectType.FullName;
		//	VertexShaders = GetShaders<VertexShaderAttribute>(effectType);
		//	FragmentShaders = GetShaders<FragmentShaderAttribute>(effectType);

		//	if (VertexShaders.Length == 0)
		//		Log.Error("Effect '{0}' must declare at least one vertex shader.", Name);

		//	if (FragmentShaders.Length == 0)
		//		Log.Error("Effect '{0}' must declare at least one fragment shader.", Name);

		//	Constants = effectType
		//		.DeclaredFields
		//		.Select(f => new ShaderConstant(f))
		//		.ToArray();

		//	var slot = 0;
		//	foreach (var texture in Constants.Where(c => c.IsTexture2D || c.IsCubeMap))
		//		texture.Slot = slot++;

		//	foreach (var property in effectType.DeclaredProperties)
		//		Log.Error("Unexpected property '{1}' declared by effect '{0}'.", Name, property.Name);

		//	var view = new ShaderConstant("View", typeof(Matrix));
		//	var projection = new ShaderConstant("Projection", typeof(Matrix));
		//	var viewProjection = new ShaderConstant("ViewProjection", typeof(Matrix));
		//	var viewportSize = new ShaderConstant("ViewportSize", typeof(Vector2));

		//	var constantBuffers = new List<ConstantBuffer>
		//	{
		//		new ConstantBuffer("CameraConstants", 0, new[] { view, projection, viewProjection }),
		//		new ConstantBuffer("ViewportConstants", 1, new[] { viewportSize })
		//	};

		//	var count = constantBuffers.Count;
		//	foreach (var group in Constants.Where(c => c.IsConstantBufferMember).GroupBy(c => c.ChangeFrequency))
		//		constantBuffers.Add(new ConstantBuffer(count++, group.ToArray()));

		//	ConstantBuffers = constantBuffers.ToArray();
		//}

		/// <summary>
		///   Gets the vertex shaders defined by the effect.
		/// </summary>
		public ShaderMethod[] VertexShaders { get; private set; }

		/// <summary>
		///   Gets the fragment shaders defined by the effect.
		/// </summary>
		public ShaderMethod[] FragmentShaders { get; private set; }

		/// <summary>
		///   Gets the constants accessed by the effect.
		/// </summary>
		public ShaderConstant[] Constants { get; private set; }

		/// <summary>
		///   Gets the constant buffers that are accessed by the effect.
		/// </summary>
		public ConstantBuffer[] ConstantBuffers { get; private set; }

		/// <summary>
		///   Gets the name of the effect.
		/// </summary>
		public string Name { get; private set; }

		///// <summary>
		/////   Gets the shaders defined by the given type.
		///// </summary>
		///// <typeparam name="TAttribute">Determines which kind of shader should be returned.</typeparam>
		///// <param name="effectType">The type of the class that should be searched.</param>
		//private static ShaderMethod[] GetShaders<TAttribute>(TypeInfo effectType)
		//	where TAttribute : Attribute
		//{
		//	return effectType
		//		.DeclaredMethods
		//		.Where(m => m.GetCustomAttribute<TAttribute>() != null)
		//		.Select(m => new ShaderMethod(m))
		//		.ToArray();
		//}

		/// <summary>
		///   Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			return string.Format("Name: {0}", Name);
		}

		/// <summary>
		///   Compiles the effect.
		/// </summary>
		/// <param name="context">The context of the compilation.</param>
		public void Compile(CompilationContext context)
		{
			GetShaderLiterals(context);
			GetShaderConstants(context);
		}

		/// <summary>
		///   Gets the shader literals from the effect.
		/// </summary>
		/// <param name="context">The context of the compilation.</param>
		private void GetShaderLiterals(CompilationContext context)
		{
		}

		/// <summary>
		///   Gets the shader constants from the effect.
		/// </summary>
		/// <param name="context">The context of the compilation.</param>
		private void GetShaderConstants(CompilationContext context)
		{
			var shaderConstants = from field in _type.Descendants.OfType<FieldDeclaration>()
								  let attribute = field.GetAttribute<ShaderConstantAttribute>(context)
								  where attribute != null
								  from variable in field.Descendants.OfType<VariableInitializer>()
								  select new ShaderConstant(field, variable);

			Constants = shaderConstants.ToArray();

			foreach (var constant in Constants)
				constant.Compile(context);

			var view = new ShaderConstant("View", DataType.Matrix);
			var projection = new ShaderConstant("Projection", DataType.Matrix);
			var viewProjection = new ShaderConstant("ViewProjection", DataType.Matrix);
			var viewportSize = new ShaderConstant("ViewportSize", DataType.Vector2);

			var constantBuffers = new[]
			{
				new ConstantBuffer(0, new[] { view, projection, viewProjection }, true),
				new ConstantBuffer(1, new[] { viewportSize }, true)
			};

			var count = constantBuffers.Length;
			ConstantBuffers = Constants.GroupBy(constant => constant.ChangeFrequency)
									   .Select(group => new ConstantBuffer(count++, group.ToArray()))
									   .Union(constantBuffers)
									   .ToArray();

			var defaultConstants = new[] { view, projection, viewProjection, viewportSize };
			Constants = Constants.Union(defaultConstants).ToArray();
		}
	}
}