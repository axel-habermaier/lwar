using System;

namespace Pegasus.AssetsCompiler.Effects.Ast
{
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.PatternMatching;

	/// <summary>
	///   Creates an abstract syntax tree for the shader code from the syntax tree of a C# method.
	/// </summary>
	internal partial class AstCreator : IAstVisitor<ShaderAstNode>
	{
		public ShaderAstNode VisitAttribute(Attribute attribute)
		{
			throw new NotImplementedException();
		}

		public ShaderAstNode VisitAttributeSection(AttributeSection attributeSection)
		{
			throw new NotImplementedException();
		}

		public ShaderAstNode VisitDelegateDeclaration(DelegateDeclaration delegateDeclaration)
		{
			throw new NotImplementedException();
		}

		public ShaderAstNode VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration)
		{
			throw new NotImplementedException();
		}

		public ShaderAstNode VisitTypeDeclaration(TypeDeclaration typeDeclaration)
		{
			throw new NotImplementedException();
		}

		public ShaderAstNode VisitUsingAliasDeclaration(UsingAliasDeclaration usingAliasDeclaration)
		{
			throw new NotImplementedException();
		}

		public ShaderAstNode VisitUsingDeclaration(UsingDeclaration usingDeclaration)
		{
			throw new NotImplementedException();
		}

		public ShaderAstNode VisitPrimitiveType(PrimitiveType primitiveType)
		{
			throw new NotImplementedException();
		}

		public ShaderAstNode VisitComment(Comment comment)
		{
			throw new NotImplementedException();
		}

		public ShaderAstNode VisitWhitespace(WhitespaceNode whitespaceNode)
		{
			throw new NotImplementedException();
		}

		public ShaderAstNode VisitText(TextNode textNode)
		{
			throw new NotImplementedException();
		}

		public ShaderAstNode VisitAccessor(Accessor accessor)
		{
			throw new NotImplementedException();
		}

		public ShaderAstNode VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration)
		{
			throw new NotImplementedException();
		}

		public ShaderAstNode VisitConstructorInitializer(ConstructorInitializer constructorInitializer)
		{
			throw new NotImplementedException();
		}

		public ShaderAstNode VisitDestructorDeclaration(DestructorDeclaration destructorDeclaration)
		{
			throw new NotImplementedException();
		}

		public ShaderAstNode VisitEnumMemberDeclaration(EnumMemberDeclaration enumMemberDeclaration)
		{
			throw new NotImplementedException();
		}

		public ShaderAstNode VisitEventDeclaration(EventDeclaration eventDeclaration)
		{
			throw new NotImplementedException();
		}

		public ShaderAstNode VisitCustomEventDeclaration(CustomEventDeclaration customEventDeclaration)
		{
			throw new NotImplementedException();
		}

		public ShaderAstNode VisitFieldDeclaration(FieldDeclaration fieldDeclaration)
		{
			throw new NotImplementedException();
		}

		public ShaderAstNode VisitIndexerDeclaration(IndexerDeclaration indexerDeclaration)
		{
			throw new NotImplementedException();
		}

		public ShaderAstNode VisitMethodDeclaration(MethodDeclaration methodDeclaration)
		{
			throw new NotImplementedException();
		}

		public ShaderAstNode VisitOperatorDeclaration(OperatorDeclaration operatorDeclaration)
		{
			throw new NotImplementedException();
		}

		public ShaderAstNode VisitParameterDeclaration(ParameterDeclaration parameterDeclaration)
		{
			throw new NotImplementedException();
		}

		public ShaderAstNode VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration)
		{
			throw new NotImplementedException();
		}

		public ShaderAstNode VisitVariableInitializer(VariableInitializer variableInitializer)
		{
			throw new NotImplementedException();
		}

		public ShaderAstNode VisitFixedFieldDeclaration(FixedFieldDeclaration fixedFieldDeclaration)
		{
			throw new NotImplementedException();
		}

		public ShaderAstNode VisitFixedVariableInitializer(FixedVariableInitializer fixedVariableInitializer)
		{
			throw new NotImplementedException();
		}

		public ShaderAstNode VisitSyntaxTree(SyntaxTree syntaxTree)
		{
			throw new NotImplementedException();
		}

		public ShaderAstNode VisitSimpleType(SimpleType simpleType)
		{
			throw new NotImplementedException();
		}

		public ShaderAstNode VisitMemberType(MemberType memberType)
		{
			throw new NotImplementedException();
		}

		public ShaderAstNode VisitComposedType(ComposedType composedType)
		{
			throw new NotImplementedException();
		}

		public ShaderAstNode VisitNewLine(NewLineNode newLineNode)
		{
			throw new NotImplementedException();
		}

		public ShaderAstNode VisitDocumentationReference(DocumentationReference documentationReference)
		{
			throw new NotImplementedException();
		}

		public ShaderAstNode VisitTypeParameterDeclaration(TypeParameterDeclaration typeParameterDeclaration)
		{
			throw new NotImplementedException();
		}

		public ShaderAstNode VisitConstraint(Constraint constraint)
		{
			throw new NotImplementedException();
		}

		public ShaderAstNode VisitCSharpTokenNode(CSharpTokenNode cSharpTokenNode)
		{
			throw new NotImplementedException();
		}

		public ShaderAstNode VisitIdentifier(Identifier identifier)
		{
			throw new NotImplementedException();
		}

		public ShaderAstNode VisitPatternPlaceholder(AstNode placeholder, Pattern pattern)
		{
			throw new NotImplementedException();
		}

		public ShaderAstNode VisitExternAliasDeclaration(ExternAliasDeclaration externAliasDeclaration)
		{
			throw new NotImplementedException();
		}
	}
}