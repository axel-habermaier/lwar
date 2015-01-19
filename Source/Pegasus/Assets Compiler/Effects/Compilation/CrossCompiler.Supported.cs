namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using System;
	using System.Linq;
	using CSharp;
	using ICSharpCode.NRefactory.CSharp;

	internal partial class CrossCompiler
	{
		public virtual void VisitAssignmentExpression(AssignmentExpression assignmentExpression)
		{
			assignmentExpression.Left.AcceptVisitor(this);
			Writer.Append(" {0} ", GetToken(assignmentExpression.Operator));
			assignmentExpression.Right.AcceptVisitor(this);
		}

		public virtual void VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression)
		{
			Writer.Append("(");
			binaryOperatorExpression.Left.AcceptVisitor(this);
			Writer.Append(" {0} ", GetToken(binaryOperatorExpression.Operator));
			binaryOperatorExpression.Right.AcceptVisitor(this);
			Writer.Append(")");
		}

		public virtual void VisitBlockStatement(BlockStatement blockStatement)
		{
			Writer.AppendBlockStatement(() => blockStatement.Statements.AcceptVisitor(this, node =>
			{
				if (!OmitTerminatingSemicolon(node))
					Writer.AppendLine(";");
			}, true));
		}

		public virtual void VisitBreakStatement(BreakStatement breakStatement)
		{
			Writer.Append("break");
		}

		public virtual void VisitContinueStatement(ContinueStatement continueStatement)
		{
			Writer.Append("continue");
		}

		public virtual void VisitDoWhileStatement(DoWhileStatement doWhileStatement)
		{
			Writer.AppendLine("do");
			VisitStatementBlock(doWhileStatement.EmbeddedStatement);
			Writer.Append("while (");
			doWhileStatement.Condition.AcceptVisitor(this);
			Writer.Append(")");
		}

		public virtual void VisitParenthesizedExpression(ParenthesizedExpression parenthesizedExpression)
		{
			Writer.Append("(");
			parenthesizedExpression.Expression.AcceptVisitor(this);
			Writer.Append(")");
		}

		public virtual void VisitExpressionStatement(ExpressionStatement expressionStatement)
		{
			expressionStatement.Expression.AcceptVisitor(this);
		}

		public virtual void VisitIdentifierExpression(IdentifierExpression identifierExpression)
		{
			Writer.Append(Escape(identifierExpression.Identifier));
		}

		public virtual void VisitForStatement(ForStatement forStatement)
		{
			Writer.Append("for (");

			forStatement.Initializers.AcceptVisitor(this, () => Writer.Append(", "));
			Writer.Append("; ");

			forStatement.Condition.AcceptVisitor(this);
			Writer.Append("; ");

			forStatement.Iterators.AcceptVisitor(this, () => Writer.Append(", "));
			Writer.Append(")");

			VisitStatementBlock(forStatement.EmbeddedStatement);
		}

		public virtual void VisitIfElseStatement(IfElseStatement ifElseStatement)
		{
			Writer.Append("if (");
			ifElseStatement.Condition.AcceptVisitor(this);
			Writer.AppendLine(")");
			VisitStatementBlock(ifElseStatement.TrueStatement);

			if (ifElseStatement.FalseStatement.IsNull)
				return;

			Writer.Append("else ");
			VisitStatementBlock(ifElseStatement.FalseStatement);
		}

		public virtual void VisitConditionalExpression(ConditionalExpression conditionalExpression)
		{
			conditionalExpression.Condition.AcceptVisitor(this);
			Writer.Append(" ? ");
			conditionalExpression.TrueExpression.AcceptVisitor(this);
			Writer.Append(" : ");
			conditionalExpression.FalseExpression.AcceptVisitor(this);
		}

		public virtual void VisitIndexerExpression(IndexerExpression indexerExpression)
		{
			var resolved = Resolver.Resolve(indexerExpression.Target);
			var type = resolved.Type.ToDataType();

			indexerExpression.Target.AcceptVisitor(this);

			if (type == DataType.Matrix)
			{
				Expression firstExpression, secondExpression;
				GetMatrixIndices(indexerExpression.Arguments, out firstExpression, out secondExpression);

				Writer.Append("[");
				firstExpression.AcceptVisitor(this);
				Writer.Append("][");
				secondExpression.AcceptVisitor(this);
				Writer.Append("]");
			}
			else
			{
				Writer.Append("[");
				indexerExpression.Arguments.Single().AcceptVisitor(this);
				Writer.Append("]");
			}
		}

		public virtual void VisitEmptyStatement(EmptyStatement emptyStatement)
		{
			// Nothing to do here
		}

		public virtual void VisitPrimitiveExpression(PrimitiveExpression primitiveExpression)
		{
			Writer.Append(primitiveExpression.Value.ToString().ToLower());
		}

		public virtual void VisitInvocationExpression(InvocationExpression invocationExpression)
		{
			var intrinsic = invocationExpression.ResolveIntrinsic(Resolver);
			if (intrinsic != Intrinsic.Unknown)
			{
				VisitIntrinsicExpression(invocationExpression);
				return;
			}

			invocationExpression.Target.AcceptVisitor(this);
			Writer.Append("(");
			invocationExpression.Arguments.AcceptVisitor(this, () => Writer.Append(", "));
			Writer.Append(")");
		}

		public virtual void VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression)
		{
			var resolved = Resolver.Resolve(memberReferenceExpression.Target);

			var vectorName = typeof(Vector2).FullName;
			vectorName = vectorName.Substring(0, vectorName.Length - 1);

			if (resolved.Type.FullName.StartsWith(vectorName) && memberReferenceExpression.MemberName == "Length")
			{
				Writer.Append("length(");
				memberReferenceExpression.Target.AcceptVisitor(this);
				Writer.Append(")");
			}
			else
			{
				memberReferenceExpression.Target.AcceptVisitor(this);
				Writer.Append(".");
				Writer.Append(memberReferenceExpression.MemberName);
			}
		}

		public virtual void VisitObjectCreateExpression(ObjectCreateExpression objectCreateExpression)
		{
			var resolved = Resolver.Resolve(objectCreateExpression);
			var dataType = resolved.Type.ToDataType();
			var isVector = dataType == DataType.Vector2 || dataType == DataType.Vector3 || dataType == DataType.Vector4;
			var constructFromSingleArgument = objectCreateExpression.Arguments.Count == 1;

			if (isVector && constructFromSingleArgument)
			{
				Writer.Append("{0}(", ToShaderType(resolved.Type.ToDataType()));
				objectCreateExpression.Arguments.First().AcceptVisitor(this);
				Writer.Append(", ");
				objectCreateExpression.Arguments.First().AcceptVisitor(this);

				if (dataType == DataType.Vector3 || dataType == DataType.Vector4)
				{
					Writer.Append(", ");
					objectCreateExpression.Arguments.First().AcceptVisitor(this);
				}

				if (dataType == DataType.Vector4)
				{
					Writer.Append(", ");
					objectCreateExpression.Arguments.First().AcceptVisitor(this);
				}

				Writer.Append(")");
				return;
			}

			Writer.Append("{0}(", ToShaderType(resolved.Type.ToDataType()));
			objectCreateExpression.Arguments.AcceptVisitor(this, () => Writer.Append(", "));
			Writer.Append(")");
		}

		public virtual void VisitReturnStatement(ReturnStatement returnStatement)
		{
			Writer.Append("return ");
			returnStatement.Expression.AcceptVisitor(this);
		}

		public virtual void VisitUnaryOperatorExpression(UnaryOperatorExpression unaryOperatorExpression)
		{
			Writer.Append("(");
			if (unaryOperatorExpression.Operator == UnaryOperatorType.PostDecrement ||
				unaryOperatorExpression.Operator == UnaryOperatorType.PostIncrement)
			{
				unaryOperatorExpression.Expression.AcceptVisitor(this);
				Writer.Append(GetToken(unaryOperatorExpression.Operator));
			}
			else
			{
				Writer.Append(GetToken(unaryOperatorExpression.Operator));
				unaryOperatorExpression.Expression.AcceptVisitor(this);
			}
			Writer.Append(")");
		}

		public virtual void VisitVariableDeclarationStatement(VariableDeclarationStatement variableDeclarationStatement)
		{
			var resolved = Resolver.Resolve(variableDeclarationStatement.Variables.First());
			Writer.Append("{0} ", ToShaderType(resolved.Type.ToDataType()));

			variableDeclarationStatement.Variables.AcceptVisitor(this, () => Writer.Append(", "));
		}

		public virtual void VisitVariableInitializer(VariableInitializer variableInitializer)
		{
			Writer.Append(Escape(variableInitializer.Name));

			if (variableInitializer.Initializer.IsNull)
				return;

			Writer.Append(" = ");
			variableInitializer.Initializer.AcceptVisitor(this);
		}

		public virtual void VisitWhileStatement(WhileStatement whileStatement)
		{
			Writer.Append("while (");
			whileStatement.Condition.AcceptVisitor(this);
			Writer.AppendLine(")");
			VisitStatementBlock(whileStatement.EmbeddedStatement);
		}

		protected virtual void VisitIntrinsicExpression(InvocationExpression invocationExpression)
		{
			var intrinsic = invocationExpression.ResolveIntrinsic(Resolver);
			Writer.Append("{0}(", GetToken(intrinsic));
			invocationExpression.Arguments.AcceptVisitor(this, () => Writer.Append(", "));
			Writer.Append(")");
		}
	}
}