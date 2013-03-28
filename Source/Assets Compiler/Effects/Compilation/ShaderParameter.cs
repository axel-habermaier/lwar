using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using System.Linq;
	using Framework.Platform.Graphics;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.TypeSystem;
	using Semantics;

	/// <summary>
	///   Represents a parameter of a shader.
	/// </summary>
	internal class ShaderParameter : ShaderDataObject<ParameterDeclaration>
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="parameter">The declaration of the method parameter that represents the shader parameter.</param>
		public ShaderParameter(ParameterDeclaration parameter)
			: base(parameter)
		{
		}

		/// <summary>
		///   Gets the semantics of the shader parameter.
		/// </summary>
		public DataSemantics Semantics { get; private set; }

		/// <summary>
		///   Gets a value indicating whether the parameter is a shader output.
		/// </summary>
		public bool IsOutput { get; private set; }

		/// <summary>
		///   Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			return String.Format("[{2}] {0} : {1} (IsOutput: {3})", Name, Type, Semantics, IsOutput);
		}

		/// <summary>
		///   Compiles the shader parameter.
		/// </summary>
		/// <param name="context">The context of the compilation.</param>
		public void Compile(CompilationContext context)
		{
			Name = Declaration.Name;
			context.ValidateIdentifier(Declaration.NameToken);

			var type = context.Resolve(Declaration).Type;
			Type = type.ToDataType();
			GetSemantics(context);

			if (Type == DataType.Unknown)
				context.Error(Declaration, "Parameter '{0}' is declared with unknown or unsupported data type '{1}'.", Name,
							  type.FullName);

			if (type.Kind == TypeKind.Array)
				context.Error(Declaration, "Parameter '{0}' cannot be an array.", Name);

			switch (Declaration.ParameterModifier)
			{
				case ParameterModifier.None:
					break;
				case ParameterModifier.Ref:
					context.Error(Declaration, "Parameter '{0}' cannot be declared with modifier 'ref'.", Name);
					break;
				case ParameterModifier.Out:
					IsOutput = true;
					break;
				case ParameterModifier.Params:
					context.Error(Declaration, "Parameter '{0}' cannot be declared with modifier 'params'.", Name);
					break;
				case ParameterModifier.This:
					context.Error(Declaration, "Parameter '{0}' cannot be declared with modifier 'this'.", Name);
					break;
				default:
					throw new InvalidOperationException("Unknown parameter modifier.");
			}
		}

		/// <summary>
		///   Gets the semantics of the parameter.
		/// </summary>
		/// <param name="context">The context of the compilation.</param>
		private void GetSemantics(CompilationContext context)
		{
			var position = Declaration.Attributes.GetAttribute<PositionAttribute>(context);
			var normal = Declaration.Attributes.GetAttribute<NormalAttribute>(context);
			var texCoords = Declaration.Attributes.GetAttribute<TexCoordsAttribute>(context);
			var color = Declaration.Attributes.GetAttribute<ColorAttribute>(context);

			var semanticsCount = new[] { position, normal, color, texCoords }.Count(attribute => attribute != null);
			if (semanticsCount > 1)
				context.Error(Declaration, "Parameter '{0}' cannot have multiple semantics.", Name);
			if (semanticsCount == 0)
				context.Error(Declaration, "Parameter '{0}' is missing a semantics declaration.", Name);

			if (position != null)
				Semantics = DataSemantics.Position;

			if (normal != null)
				Semantics = DataSemantics.Normal;

			if (texCoords != null)
			{
				var index = GetSemanticIndex(context, texCoords);
				Semantics = DataSemantics.TexCoords0 + index;
			}

			if (color != null)
			{
				var index = GetSemanticIndex(context, color);
				Semantics = DataSemantics.Color0 + index;
			}
		}

		/// <summary>
		///   Gets the semantic index of the given attribute.
		/// </summary>
		/// <param name="context">The context of the compilation.</param>
		/// <param name="attribute">The attribute whose semantic index specification should be returned.</param>
		private int GetSemanticIndex(CompilationContext context, Attribute attribute)
		{
			if (!attribute.HasArgumentList)
				return 0;

			var resolved = context.Resolve(attribute.Arguments.Single());
			var index = (int)resolved.ConstantValue;

			if (index < 0 || index > 3)
			{
				context.Error(Declaration, "Semantic index of parameter '{0}' must be between 0 and 3.", Name);
				return 0;
			}

			return index;
		}
	}
}