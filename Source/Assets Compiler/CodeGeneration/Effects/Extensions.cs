namespace Pegasus.AssetsCompiler.CodeGeneration.Effects
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using AssetsCompiler.Effects;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.CSharp.Resolver;
	using ICSharpCode.NRefactory.TypeSystem;
	using Platform.Graphics;
	using CSharpAttribute = ICSharpCode.NRefactory.CSharp.Attribute;
	using CubeMap = AssetsCompiler.Effects.CubeMap;
	using Texture2D = AssetsCompiler.Effects.Texture2D;

	/// <summary>
	///   Provides extension methods on NRefactory AST types.
	/// </summary>
	internal static class Extensions
	{
		/// <summary>
		///   Resolves the intrinsic that the given invocation expression represents.
		/// </summary>
		/// <param name="invocationExpression">The invocation expression that should be resolved.</param>
		/// <param name="resolver">The resolver that should be used to resolve type information.</param>
		public static Intrinsic ResolveIntrinsic(this InvocationExpression invocationExpression, CSharpAstResolver resolver)
		{
			var resolvedTarget = (MethodGroupResolveResult)resolver.Resolve(invocationExpression.Target);
			var resolvedArguments = invocationExpression.Arguments.Select(argument => resolver.Resolve(argument)).ToArray();

			var invokedMethod = resolvedTarget.PerformOverloadResolution(resolver.Compilation, resolvedArguments).BestCandidate;

			var type = Type.GetType(invokedMethod.DeclaringType.FullName);
			if (type == null)
				return Intrinsic.Unknown;

			var flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
			var method = type.GetMethods(flags).Where(m => m.Name == invokedMethod.Name)
							 .Where(m =>
							 {
								 var declaredParameters = m.GetParameters();
								 var invokedParameters = invokedMethod.Parameters;

								 if (declaredParameters.Length != invokedParameters.Count)
									 return false;

								 for (var i = 0; i < declaredParameters.Length; ++i)
								 {
									 if (declaredParameters[i].Name != invokedParameters[i].Name)
										 return false;

									 if (declaredParameters[i].ParameterType.FullName != invokedParameters[i].Type.FullName)
										 return false;
								 }

								 return true;
							 })
							 .Single();

			var mapsTo = method.GetCustomAttributes(typeof(MapsToAttribute), true).OfType<MapsToAttribute>().FirstOrDefault();
			if (mapsTo == null)
				return Intrinsic.Unknown;

			return mapsTo.Intrinsic;
		}

		/// <summary>
		///   Converts the given type to its corresponding data type value.
		/// </summary>
		/// <param name="type">The type that should be converted.</param>
		public static DataType ToDataType(this IType type)
		{
			Assert.ArgumentNotNull(type);

			var typeName = type.FullName;
			if (type.FullName.EndsWith("[]"))
				typeName = typeName.Substring(0, typeName.Length - 2);

			if (typeName == typeof(bool).FullName)
				return DataType.Boolean;

			if (typeName == typeof(int).FullName)
				return DataType.Integer;

			if (typeName == typeof(float).FullName)
				return DataType.Float;

			if (typeName == typeof(Vector2).FullName)
				return DataType.Vector2;

			if (typeName == typeof(Vector3).FullName)
				return DataType.Vector3;

			if (typeName == typeof(Vector4).FullName)
				return DataType.Vector4;

			if (typeName == typeof(Matrix).FullName)
				return DataType.Matrix;

			if (typeName == typeof(Texture2D).FullName)
				return DataType.Texture2D;

			if (typeName == typeof(CubeMap).FullName)
				return DataType.CubeMap;

			if (typeName == typeof(Matrix).FullName)
				return DataType.Matrix;

			return DataType.Unknown;
		}

		/// <summary>
		///   Converts the semantics into its display string.
		/// </summary>
		/// <param name="semantics">The semantics that should be converted.</param>
		public static string ToDisplayString(this DataSemantics semantics)
		{
			var semanticsString = semantics.ToString();
			var lastCharacter = semanticsString[semanticsString.Length - 1];

			if (Char.IsDigit(lastCharacter))
				return String.Format("{0}({1})", semanticsString.Substring(0, semanticsString.Length - 1), lastCharacter);

			return semanticsString;
		}

		/// <summary>
		///   Checks whether the semantics represents one of the color semantics, regardless of the semantic index.
		/// </summary>
		/// <param name="semantics">The semantics that should be checked.</param>
		public static bool IsColor(this DataSemantics semantics)
		{
			return semantics == DataSemantics.Color0 ||
				   semantics == DataSemantics.Color1 ||
				   semantics == DataSemantics.Color2 ||
				   semantics == DataSemantics.Color3;
		}

		/// <summary>
		///   Gets the semantics attributes that are applied to the parameter.
		/// </summary>
		/// <param name="parameter">The parameter whose semantics attributes should be returned.</param>
		/// <param name="resolver">The resolver that should be used to resolve type information.</param>
		public static IEnumerable<CSharpAttribute> GetSemantics(this ParameterDeclaration parameter, CSharpAstResolver resolver)
		{
			var attributes = new[]
			{
				parameter.Attributes.GetAttribute<PositionAttribute>(resolver),
				parameter.Attributes.GetAttribute<NormalAttribute>(resolver),
				parameter.Attributes.GetAttribute<TexCoordsAttribute>(resolver),
				parameter.Attributes.GetAttribute<ColorAttribute>(resolver)
			};

			return attributes.Where(attribute => attribute != null);
		}

		/// <summary>
		///   Gets the semantics attributes that are applied to the parameter.
		/// </summary>
		/// <param name="parameter">The parameter whose semantics attributes should be returned.</param>
		/// <param name="resolver">The resolver that should be used to resolve type information.</param>
		public static IEnumerable<SemanticsAttribute> GetSemanticsAttributes(this ParameterDeclaration parameter,
																			 CSharpAstResolver resolver)
		{
			foreach (var semantics in parameter.GetSemantics(resolver))
				yield return semantics.ToSemanticsAttribute(resolver);
		}

		/// <summary>
		///   Gets the corresponding semantics attribute instance from the C# attribute.
		/// </summary>
		/// <param name="attribute">The attribute that should be converted.</param>
		/// <param name="resolver">The resolver that should be used to resolve type information.</param>
		public static SemanticsAttribute ToSemanticsAttribute(this CSharpAttribute attribute, CSharpAstResolver resolver)
		{
			var index = 0;
			if (attribute.HasArgumentList)
			{
				var resolved = resolver.Resolve(attribute.Arguments.Single());
				if (resolved.ConstantValue == null)
					index = -1;
				else
					index = (int)resolved.ConstantValue;
			}

			var type = resolver.Resolve(attribute).Type.FullName;

			if (type == typeof(PositionAttribute).FullName)
				return new PositionAttribute();

			if (type == typeof(NormalAttribute).FullName)
				return new NormalAttribute();

			if (type == typeof(TexCoordsAttribute).FullName)
				return new TexCoordsAttribute(index);

			if (type == typeof(ColorAttribute).FullName)
				return new ColorAttribute(index);

			return null;
		}
	}
}