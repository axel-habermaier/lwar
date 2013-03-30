using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using System.Collections.Generic;
	using System.Linq;
	using Framework;
	using Framework.Platform.Graphics;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.CSharp.Resolver;
	using ICSharpCode.NRefactory.Semantics;
	using ICSharpCode.NRefactory.TypeSystem;
	using Math;
	using Semantics;
	using Attribute = System.Attribute;
	using CSharpAttribute = ICSharpCode.NRefactory.CSharp.Attribute;
	using CubeMap = Effects.CubeMap;
	using Texture2D = Effects.Texture2D;

	/// <summary>
	///   Provides extension methods on NRefactory AST types.
	/// </summary>
	internal static class Extensions
	{
		/// <summary>
		///   Checks whether the declared class is derived from the given base class.
		/// </summary>
		/// <typeparam name="T">The type of the base class.</typeparam>
		/// <param name="declaration">The class declaration that should be checked.</param>
		/// <param name="resolver">The resolver that should be used to resolve type information.</param>
		public static bool IsDerivedFrom<T>(this TypeDeclaration declaration, CSharpAstResolver resolver)
			where T : class
		{
			Assert.ArgumentNotNull(declaration, () => declaration);
			Assert.ArgumentNotNull(resolver, () => resolver);

			if (declaration.ClassType != ClassType.Class)
				return false;

			return declaration.BaseTypes
							  .Select(type => resolver.Resolve(type))
							  .OfType<TypeResolveResult>()
							  .Any(type => type.Type.FullName == typeof(T).FullName);
		}

		/// <summary>
		///   Gets all declared attributes of the given type.
		/// </summary>
		/// <typeparam name="T">The type of the attributes.</typeparam>
		/// <param name="attributes">The attributes that should be checked.</param>
		/// <param name="resolver">The resolver that should be used to resolve type information.</param>
		public static IEnumerable<CSharpAttribute> GetAttributes<T>(this AstNodeCollection<AttributeSection> attributes,
																	CSharpAstResolver resolver)
			where T : Attribute
		{
			Assert.ArgumentNotNull(attributes, () => attributes);
			Assert.ArgumentNotNull(resolver, () => resolver);

			return attributes
				.SelectMany(s => s.Attributes)
				.Where(a => resolver.Resolve(a).Type.FullName == typeof(T).FullName);
		}

		/// <summary>
		///   Gets the declared attribute of the given type.
		/// </summary>
		/// <typeparam name="T">The type of the attributes.</typeparam>
		/// <param name="attributes">The attributes that should be checked.</param>
		/// <param name="resolver">The resolver that should be used to resolve type information.</param>
		public static CSharpAttribute GetAttribute<T>(this AstNodeCollection<AttributeSection> attributes,
													  CSharpAstResolver resolver)
			where T : Attribute
		{
			return attributes.GetAttributes<T>(resolver).SingleOrDefault();
		}

		/// <summary>
		///   Checks whether there is at least one declared attribute of the given type.
		/// </summary>
		/// <typeparam name="T">The type of the attribute.</typeparam>
		/// <param name="attributes">The attributes that should be checked.</param>
		/// <param name="resolver">The resolver that should be used to resolve type information.</param>
		public static bool Contain<T>(this AstNodeCollection<AttributeSection> attributes, CSharpAstResolver resolver)
			where T : Attribute
		{
			return attributes.GetAttributes<T>(resolver).Any();
		}

		/// <summary>
		///   Gets the full name of the entity declaration.
		/// </summary>
		/// <param name="declaration">The entity declaration whose full name should be returned.</param>
		/// <param name="resolver">The resolver that should be used to resolve type information.</param>
		public static string GetFullName(this EntityDeclaration declaration, CSharpAstResolver resolver)
		{
			Assert.ArgumentNotNull(declaration, () => declaration);
			Assert.ArgumentNotNull(resolver, () => resolver);

			var resolved = (TypeResolveResult)resolver.Resolve(declaration);
			return resolved.Type.FullName;
		}

		/// <summary>
		///   Gets the constant value of the expression.
		/// </summary>
		/// <param name="expression">The expression whose constant value should be returned.</param>
		/// <param name="resolver">The resolver that should be used to resolve type information.</param>
		public static object GetConstantValue(this Expression expression, CSharpAstResolver resolver)
		{
			Assert.ArgumentNotNull(expression, () => expression);
			Assert.ArgumentNotNull(resolver, () => resolver);

			var resolved = resolver.Resolve(expression);
			if (resolved.IsCompileTimeConstant)
				return resolved.ConstantValue;

			return null;
		}

		/// <summary>
		///   Gets the constant values of the array expression.
		/// </summary>
		/// <param name="expression">The expression whose constant value should be returned.</param>
		/// <param name="resolver">The resolver that should be used to resolve type information.</param>
		public static object[] GetConstantValues(this Expression expression, CSharpAstResolver resolver)
		{
			Assert.ArgumentNotNull(expression, () => expression);
			Assert.ArgumentNotNull(resolver, () => resolver);

			var resolved = resolver.Resolve(expression) as ArrayCreateResolveResult;
			if (resolved == null)
				return null;

			if (resolved.InitializerElements.All(element => element.IsCompileTimeConstant))
				return resolved.InitializerElements.Select(element => element.ConstantValue).ToArray();

			return null;
		}

		/// <summary>
		///   Gets the resolved type of the entity declaration.
		/// </summary>
		/// <param name="declaration">The entity declaration whose type should be returned.</param>
		/// <param name="resolver">The resolver that should be used to resolve type information.</param>
		public static IType ResolveType(this EntityDeclaration declaration, CSharpAstResolver resolver)
		{
			Assert.ArgumentNotNull(declaration, () => declaration);
			Assert.ArgumentNotNull(resolver, () => resolver);

			var resolved = resolver.Resolve(declaration.ReturnType);
			return resolved.Type;
		}

		/// <summary>
		///   Gets the resolved type of the parameter declaration.
		/// </summary>
		/// <param name="declaration">The parameter declaration whose type should be returned.</param>
		/// <param name="resolver">The resolver that should be used to resolve type information.</param>
		public static IType ResolveType(this ParameterDeclaration declaration, CSharpAstResolver resolver)
		{
			Assert.ArgumentNotNull(declaration, () => declaration);
			Assert.ArgumentNotNull(resolver, () => resolver);

			var resolved = resolver.Resolve(declaration.Type);
			return resolved.Type;
		}

		/// <summary>
		///   Converts the given type to its corresponding data type value.
		/// </summary>
		/// <param name="type">The type that should be converted.</param>
		public static DataType ToDataType(this IType type)
		{
			Assert.ArgumentNotNull(type, () => type);

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

		/// <summary>
		///   Calls the AcceptVisitor method on all nodes.
		/// </summary>
		/// <param name="nodesCollection">The nodes on which the AcceptVisitor method should be called.</param>
		/// <param name="visitor">The visitor that should be passed to the AcceptVisitor method.</param>
		/// <param name="action">
		///   An action that should be invoked between visiting two nodes; the action is not invoked after the
		///   last node has been visited.
		/// </param>
		public static void AcceptVisitor<T>(this AstNodeCollection<T> nodesCollection, IAstVisitor visitor, Action action = null)
			where T : AstNode
		{
			Assert.ArgumentNotNull(nodesCollection, () => nodesCollection);
			Assert.ArgumentNotNull(visitor, () => visitor);

			var nodes = nodesCollection.ToArray();
			for (var i = 0; i < nodes.Length; ++i)
			{
				nodes[i].AcceptVisitor(visitor);

				if (i < nodes.Length - 1 && action != null)
					action();
			}
		}
	}
}