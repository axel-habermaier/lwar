using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using System.Collections.Generic;
	using System.Linq;
	using Framework;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.Semantics;
	using ICSharpCode.NRefactory.TypeSystem;
	using Math;
	using Attribute = System.Attribute;
	using CSharpAttribute = ICSharpCode.NRefactory.CSharp.Attribute;

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
		/// <param name="context">The context of the compilation.</param>
		public static bool IsDerivedFrom<T>(this TypeDeclaration declaration, CompilationContext context)
			where T : class
		{
			Assert.ArgumentNotNull(declaration, () => declaration);
			Assert.ArgumentNotNull(context, () => context);

			if (declaration.ClassType != ClassType.Class)
				return false;

			var resolvedDeclaration = context.Resolve<TypeResolveResult>(declaration);
			return resolvedDeclaration.Type.DirectBaseTypes.Any(b => b.FullName == typeof(T).FullName);
		}

		/// <summary>
		///   Gets all declared attributes of the given type.
		/// </summary>
		/// <typeparam name="T">The type of the attributes.</typeparam>
		/// <param name="attributes">The attributes that should be checked.</param>
		/// <param name="context">The context of the compilation.</param>
		public static IEnumerable<CSharpAttribute> GetAttributes<T>(this AstNodeCollection<AttributeSection> attributes,
																	CompilationContext context)
			where T : Attribute
		{
			Assert.ArgumentNotNull(attributes, () => attributes);
			Assert.ArgumentNotNull(context, () => context);

			return attributes
				.SelectMany(s => s.Attributes)
				.Where(a => context.Resolve(a).Type.FullName == typeof(T).FullName);
		}

		/// <summary>
		///   Gets all declared attributes of the given type.
		/// </summary>
		/// <typeparam name="T">The type of the attributes.</typeparam>
		/// <param name="attributes">The attributes that should be checked.</param>
		/// <param name="context">The context of the compilation.</param>
		public static CSharpAttribute GetAttribute<T>(this AstNodeCollection<AttributeSection> attributes,
													  CompilationContext context)
			where T : Attribute
		{
			return attributes.GetAttributes<T>(context).SingleOrDefault();
		}

		/// <summary>
		///   Gets all declared attributes of the given type.
		/// </summary>
		/// <typeparam name="T">The type of the attributes.</typeparam>
		/// <param name="declaration">The entity declaration that should be checked.</param>
		/// <param name="context">The context of the compilation.</param>
		public static IEnumerable<CSharpAttribute> GetAttributes<T>(this EntityDeclaration declaration, CompilationContext context)
			where T : Attribute
		{
			Assert.ArgumentNotNull(declaration, () => declaration);
			return declaration.Attributes.GetAttributes<T>(context);
		}

		/// <summary>
		///   Gets all declared attributes of the given type.
		/// </summary>
		/// <typeparam name="T">The type of the attributes.</typeparam>
		/// <param name="declaration">The entity declaration that should be checked.</param>
		/// <param name="context">The context of the compilation.</param>
		public static CSharpAttribute GetAttribute<T>(this EntityDeclaration declaration, CompilationContext context)
			where T : Attribute
		{
			return declaration.GetAttributes<T>(context).SingleOrDefault();
		}

		/// <summary>
		///   Checks whether the declared entity has at least one attribute of the given type.
		/// </summary>
		/// <typeparam name="T">The type of the attribute.</typeparam>
		/// <param name="declaration">The entity declaration that should be checked.</param>
		/// <param name="context">The context of the compilation.</param>
		public static bool HasAttribute<T>(this EntityDeclaration declaration, CompilationContext context)
			where T : Attribute
		{
			return declaration.GetAttributes<T>(context).Any();
		}

		/// <summary>
		///   Gets the full name of the entity declaration.
		/// </summary>
		/// <param name="declaration">The entity declaration whose full name should be returned.</param>
		/// <param name="context">The context of the compilation.</param>
		public static string GetFullName(this EntityDeclaration declaration, CompilationContext context)
		{
			Assert.ArgumentNotNull(declaration, () => declaration);
			Assert.ArgumentNotNull(context, () => context);

			var resolved = context.Resolve<TypeResolveResult>(declaration);
			return resolved.Type.FullName;
		}

		/// <summary>
		///   Gets the constant value of the expression.
		/// </summary>
		/// <typeparam name="T">The type of the constant value that should be returned.</typeparam>
		/// <param name="expression">The expression whose constant value should be returned.</param>
		/// <param name="context">The context of the compilation.</param>
		public static T GetConstantValue<T>(this Expression expression, CompilationContext context)
		{
			Assert.ArgumentNotNull(expression, () => expression);
			Assert.ArgumentNotNull(context, () => context);

			var resolved = context.Resolve(expression);
			if (resolved.IsCompileTimeConstant)
				return (T)resolved.ConstantValue;

			return default(T);
		}

		/// <summary>
		///   Gets the constant values of the array expression.
		/// </summary>
		/// <param name="expression">The expression whose constant value should be returned.</param>
		/// <param name="context">The context of the compilation.</param>
		public static object[] GetConstantValues(this Expression expression, CompilationContext context)
		{
			Assert.ArgumentNotNull(expression, () => expression);
			Assert.ArgumentNotNull(context, () => context);

			var resolved = context.Resolve<ArrayCreateResolveResult>(expression);
			if (resolved.InitializerElements.All(element => element.IsCompileTimeConstant))
				return resolved.InitializerElements.Select(element => element.ConstantValue).ToArray();

			return null;
		}

		/// <summary>
		///   Gets the type of an entity declaration.
		/// </summary>
		/// <param name="declaration">The entity declaration whose type should be returned.</param>
		/// <param name="context">The context of the compilation.</param>
		public static IType GetType(this EntityDeclaration declaration, CompilationContext context)
		{
			Assert.ArgumentNotNull(declaration, () => declaration);
			Assert.ArgumentNotNull(context, () => context);

			var resolved = context.Resolve(declaration.ReturnType);
			return resolved.Type;
		}

		/// <summary>
		///   Gets the data type of an entity declaration.
		/// </summary>
		/// <param name="declaration">The entity declaration whose type should be returned.</param>
		/// <param name="context">The context of the compilation.</param>
		public static DataType GetDataType(this EntityDeclaration declaration, CompilationContext context)
		{
			return declaration.GetType(context).ToDataType();
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
	}
}