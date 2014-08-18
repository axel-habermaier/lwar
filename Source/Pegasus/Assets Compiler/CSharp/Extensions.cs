namespace Pegasus.AssetsCompiler.CSharp
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using ICSharpCode.NRefactory;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.CSharp.Resolver;
	using ICSharpCode.NRefactory.Semantics;
	using ICSharpCode.NRefactory.TypeSystem;
	using Attribute = System.Attribute;
	using CSharpAttribute = ICSharpCode.NRefactory.CSharp.Attribute;

	/// <summary>
	///     Provides extension methods on NRefactory AST types.
	/// </summary>
	public static class Extensions
	{
		/// <summary>
		///     Checks whether the declared class is derived from the given base class.
		/// </summary>
		/// <typeparam name="T">The type of the base class.</typeparam>
		/// <param name="declaration">The class declaration that should be checked.</param>
		/// <param name="resolver">The resolver that should be used to resolve type information.</param>
		public static bool IsDerivedFrom<T>(this TypeDeclaration declaration, CSharpAstResolver resolver)
			where T : class
		{
			Assert.ArgumentNotNull(declaration);
			Assert.ArgumentNotNull(resolver);

			if (declaration.ClassType != ClassType.Class)
				return false;

			return declaration.BaseTypes
							  .Select(type => resolver.Resolve(type))
							  .OfType<TypeResolveResult>()
							  .Any(type => type.Type.FullName == typeof(T).FullName);
		}

		/// <summary>
		///     Gets all declared attributes of the given type.
		/// </summary>
		/// <typeparam name="T">The type of the attributes.</typeparam>
		/// <param name="attributes">The attributes that should be checked.</param>
		/// <param name="resolver">The resolver that should be used to resolve type information.</param>
		public static IEnumerable<CSharpAttribute> GetAttributes<T>(this AstNodeCollection<AttributeSection> attributes,
																	CSharpAstResolver resolver)
			where T : Attribute
		{
			Assert.ArgumentNotNull(attributes);
			Assert.ArgumentNotNull(resolver);

			return attributes
				.SelectMany(s => s.Attributes)
				.Where(a => resolver.Resolve(a).Type.FullName == typeof(T).FullName);
		}

		/// <summary>
		///     Gets the declared attribute of the given type.
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
		///     Checks whether there is at least one declared attribute of the given type.
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
		///     Gets the constant value of the expression.
		/// </summary>
		/// <param name="expression">The expression whose constant value should be returned.</param>
		/// <param name="resolver">The resolver that should be used to resolve type information.</param>
		public static object GetConstantValue(this Expression expression, CSharpAstResolver resolver)
		{
			Assert.ArgumentNotNull(expression);
			Assert.ArgumentNotNull(resolver);

			var resolved = resolver.Resolve(expression);
			if (resolved.IsCompileTimeConstant)
				return resolved.ConstantValue;

			return null;
		}

		/// <summary>
		///     Gets the constant values of the array expression.
		/// </summary>
		/// <param name="expression">The expression whose constant value should be returned.</param>
		/// <param name="resolver">The resolver that should be used to resolve type information.</param>
		public static object[] GetConstantValues(this Expression expression, CSharpAstResolver resolver)
		{
			Assert.ArgumentNotNull(expression);
			Assert.ArgumentNotNull(resolver);

			var resolved = resolver.Resolve(expression) as ArrayCreateResolveResult;
			if (resolved == null)
				return null;

			if (resolved.InitializerElements.All(element => element.IsCompileTimeConstant))
				return resolved.InitializerElements.Select(element => element.ConstantValue).ToArray();

			return null;
		}

		/// <summary>
		///     Gets the resolved type of the entity declaration.
		/// </summary>
		/// <param name="declaration">The entity declaration whose type should be returned.</param>
		/// <param name="resolver">The resolver that should be used to resolve type information.</param>
		public static IType ResolveType(this EntityDeclaration declaration, CSharpAstResolver resolver)
		{
			Assert.ArgumentNotNull(declaration);
			Assert.ArgumentNotNull(resolver);

			var resolved = resolver.Resolve(declaration.ReturnType);
			return resolved.Type;
		}

		/// <summary>
		///     Gets the resolved type of the parameter declaration.
		/// </summary>
		/// <param name="declaration">The parameter declaration whose type should be returned.</param>
		/// <param name="resolver">The resolver that should be used to resolve type information.</param>
		public static IType ResolveType(this ParameterDeclaration declaration, CSharpAstResolver resolver)
		{
			Assert.ArgumentNotNull(declaration);
			Assert.ArgumentNotNull(resolver);

			var resolved = resolver.Resolve(declaration.Type);
			return resolved.Type;
		}

		/// <summary>
		///     Generates a string that contains the given file name and the text locations. The string is formatted
		///     such that it can be used to report errors and warnings to Visual Studio.
		/// </summary>
		/// <param name="file">The name of the file for which the message should be raised.</param>
		/// <param name="begin">The beginning of the message location in the source file.</param>
		/// <param name="end">The end of the message location in the source file.</param>
		public static string ToLocationString(this string file, TextLocation begin, TextLocation end)
		{
			Assert.ArgumentNotNullOrWhitespace(file);

			string location;
			if (end.IsEmpty)
				location = String.Format("({0},{1})", begin.Line, begin.Column);
			else
				location = String.Format("({0},{1},{2},{3})", begin.Line, begin.Column, end.Line, end.Column);

			return String.Format("{0}{1}", file.Replace("/", "\\"), location);
		}

		/// <summary>
		///     Gets the documentation of the given node. Only documentation comments immediately preceding the given node are
		///     returned.
		/// </summary>
		/// <param name="node">The node whose documentation should be returned.</param>
		public static IEnumerable<string> GetDocumentation(this AstNode node)
		{
			var children = node.Children.ToArray();
			foreach (var child in children)
			{
				if (child is NewLineNode)
					continue;

				var comment = child as Comment;
				if (comment != null)
					yield return comment.Content;
				else
					yield break;
			}
		}

		/// <summary>
		///     Calls the AcceptVisitor method on all nodes.
		/// </summary>
		/// <param name="nodesCollection">The nodes on which the AcceptVisitor method should be called.</param>
		/// <param name="visitor">The visitor that should be passed to the AcceptVisitor method.</param>
		/// <param name="action">An action that should be invoked between visiting two nodes.</param>
		/// <param name="afterLast">Indicates whether the action should also be called after the last node has been visited.</param>
		public static void AcceptVisitor<T>(this AstNodeCollection<T> nodesCollection, IAstVisitor visitor, Action action = null,
											bool afterLast = false)
			where T : AstNode
		{
			nodesCollection.AcceptVisitor(visitor, _ =>
			{
				if (action != null)
					action();
			}, afterLast);
		}

		/// <summary>
		///     Calls the AcceptVisitor method on all nodes.
		/// </summary>
		/// <param name="nodesCollection">The nodes on which the AcceptVisitor method should be called.</param>
		/// <param name="visitor">The visitor that should be passed to the AcceptVisitor method.</param>
		/// <param name="action">
		///     An action that should be invoked between visiting two nodes. The node that has just been visited
		///     is passed as an argument.
		/// </param>
		/// <param name="afterLast">Indicates whether the action should also be called after the last node has been visited.</param>
		public static void AcceptVisitor<T>(this AstNodeCollection<T> nodesCollection, IAstVisitor visitor, Action<AstNode> action,
											bool afterLast = false)
			where T : AstNode
		{
			Assert.ArgumentNotNull(nodesCollection);
			Assert.ArgumentNotNull(visitor);

			var nodes = nodesCollection.ToArray();
			for (var i = 0; i < nodes.Length; ++i)
			{
				nodes[i].AcceptVisitor(visitor);

				if (action != null && ((!afterLast && i < nodes.Length - 1) || afterLast))
					action(nodes[i]);
			}
		}
	}
}