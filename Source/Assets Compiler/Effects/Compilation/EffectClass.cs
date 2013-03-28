using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using System.Linq;
	using Framework;
	using Framework.Platform.Graphics;
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

		/// <summary>
		///   Gets the vertex shaders defined by the effect.
		/// </summary>
		public ShaderMethod[] VertexShaders { get; private set; }

		/// <summary>
		///   Gets the fragment shaders defined by the effect.
		/// </summary>
		public ShaderMethod[] FragmentShaders { get; private set; }

		/// <summary>
		///   Gets the shader constants accessed by the effect.
		/// </summary>
		public ShaderConstant[] Constants { get; private set; }

		/// <summary>
		///   Gets the compile-time constant literals accessed by the effect.
		/// </summary>
		public ShaderLiteral[] Literals { get; private set; }

		/// <summary>
		///   Gets the texture objects accessed by the effect.
		/// </summary>
		public ShaderTexture[] Textures { get; private set; }

		/// <summary>
		///   Gets the constant buffers that are accessed by the effect.
		/// </summary>
		public ConstantBuffer[] ConstantBuffers { get; private set; }

		/// <summary>
		///   Gets the name of the effect.
		/// </summary>
		public string Name { get; private set; }

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
			Name = _type.GetFullName(context);

			GetShaderLiterals(context);
			GetShaderConstants(context);
			GetShaderTextures(context);
			GetShaderMethods(context);

			if (_type.TypeParameters.Any() || _type.Modifiers != Modifiers.Public)
				context.Error(_type,
							  "Effect '{0}' must be a public, non-static, non-partial, non-abstract, non-sealed class without any type arguments.",
							  Name);

			foreach (var property in _type.Descendants.OfType<PropertyDeclaration>())
				context.Error(property.NameToken, "Unexpected property '{0}' declared by effect '{1}'.", property.Name, Name);

			if (VertexShaders.Length == 0)
				context.Error(_type, "Effect '{0}' must declare at least one vertex shader.", Name);

			if (FragmentShaders.Length == 0)
				context.Error(_type, "Effect '{0}' must declare at least one fragment shader.", Name);
		}

		/// <summary>
		///   Gets the shader literals from the effect.
		/// </summary>
		/// <param name="context">The context of the compilation.</param>
		private void GetShaderLiterals(CompilationContext context)
		{
			var literals = from field in _type.Descendants.OfType<FieldDeclaration>()
						   where !field.HasAttribute<ShaderConstantAttribute>(context)
						   let dataType = field.GetDataType(context)
						   where dataType != DataType.Texture2D && dataType != DataType.CubeMap
						   from variable in field.Descendants.OfType<VariableInitializer>()
						   select new ShaderLiteral(field, variable);

			Literals = literals.ToArray();

			foreach (var literal in Literals)
				literal.Compile(context);
		}

		/// <summary>
		///   Gets the shader constants from the effect.
		/// </summary>
		/// <param name="context">The context of the compilation.</param>
		private void GetShaderConstants(CompilationContext context)
		{
			var constants = from field in _type.Descendants.OfType<FieldDeclaration>()
							let attribute = field.GetAttribute<ShaderConstantAttribute>(context)
							where attribute != null
							from variable in field.Descendants.OfType<VariableInitializer>()
							select new ShaderConstant(field, variable);

			Constants = constants.ToArray();

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
									   .OrderBy(buffer => buffer.Slot)
									   .ToArray();

			var defaultConstants = new[] { view, projection, viewProjection, viewportSize };
			Constants = Constants.Union(defaultConstants).ToArray();
		}

		/// <summary>
		///   Gets the shader textures from the effect.
		/// </summary>
		/// <param name="context">The context of the compilation.</param>
		private void GetShaderTextures(CompilationContext context)
		{
			var slot = 0;
			var textures = from field in _type.Descendants.OfType<FieldDeclaration>()
						   let dataType = field.GetDataType(context)
						   where dataType == DataType.Texture2D || dataType == DataType.CubeMap
						   from variable in field.Descendants.OfType<VariableInitializer>()
						   select new ShaderTexture(field, variable, slot++);

			Textures = textures.ToArray();

			foreach (var texture in Textures)
				texture.Compile(context);
		}

		/// <summary>
		///   Gets the shader methods from the effect.
		/// </summary>
		/// <param name="context">The context of the compilation.</param>
		private void GetShaderMethods(CompilationContext context)
		{
			var methods = from method in _type.Descendants.OfType<MethodDeclaration>()
						  let isVertexShader = method.HasAttribute<VertexShaderAttribute>(context)
						  let isFragmentShader = method.HasAttribute<FragmentShaderAttribute>(context)
						  let shaderType = isVertexShader ? ShaderType.VertexShader : ShaderType.FragmentShader
						  where isVertexShader || isFragmentShader
						  select new { Declaration = method, Type = shaderType, HasUnknownType = isVertexShader && isFragmentShader };

			var shaderMethods = methods.Select(method =>
				{
					if (method.HasUnknownType)
						context.Error(method.Declaration, "Shader method '{0}' cannot be both a vertex shader and a fragment shader.",
									  method.Declaration.Name);

					var shaderMethod = new ShaderMethod(method.Declaration, method.Type);
					shaderMethod.Compile(context);
					return shaderMethod;
				}).ToArray();

			VertexShaders = shaderMethods.Where(shader => shader.Type == ShaderType.VertexShader).ToArray();
			FragmentShaders = shaderMethods.Where(shader => shader.Type == ShaderType.FragmentShader).ToArray();
		}
	}
}