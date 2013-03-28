using System;

namespace Pegasus.AssetsCompiler.Effects.Ast
{
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using ICSharpCode.NRefactory.CSharp;

	/// <summary>
	///   Provides extension methods for shader and C# AST nodes.
	/// </summary>
	internal static class Extensions
	{
		/// <summary>
		///   Visits all C# nodes in the source list and returns the generated shader AST nodes as an array.
		/// </summary>
		/// <typeparam name="TShaderNode">The type of the shader nodes that should be returned.</typeparam>
		/// <param name="nodes">The list of C# nodes that should be visited.</param>
		/// <param name="visitor">The visitor that should be used.</param>
		[DebuggerHidden]
		public static TShaderNode[] Visit<TShaderNode>(this IEnumerable<AstNode> nodes, IAstVisitor<IAstNode> visitor)
			where TShaderNode : IAstNode
		{
			return nodes.Select(initializer => initializer.AcceptVisitor(visitor))
						.Cast<TShaderNode>()
						.ToArray();
		}

		/// <summary>
		///   Visits the C# node and returns the generated shader AST node.
		/// </summary>
		/// <typeparam name="TShaderNode">The type of the shader node that should be returned.</typeparam>
		/// <param name="node">The C# node that should be visited.</param>
		/// <param name="visitor">The visitor that should be used.</param>
		[DebuggerHidden]
		public static TShaderNode Visit<TShaderNode>(this AstNode node, IAstVisitor<IAstNode> visitor)
			where TShaderNode : IAstNode
		{
			return (TShaderNode)node.AcceptVisitor(visitor);
		}
	}
}