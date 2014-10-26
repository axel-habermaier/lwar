namespace Tests.Parsing.Combinators
{
	using System;
	using NUnit.Framework;
	using Pegasus.Scripting.Parsing.Combinators;
	using Pegasus.UserInterface.Input;

	[TestFixture]
	public class InputTriggerParserTests : ParserTestsHelper<InputTrigger>
	{
		public InputTriggerParserTests()
			: base(new InputTriggerParser())
		{
		}

		private void TestTrigger(InputTrigger trigger)
		{
			Success(String.Format("[{0}]", trigger), trigger);
		}

		[Test]
		public void Valid_Alias()
		{
			TestTrigger(MouseButton.Left.WentDown() | Key.X.IsRepeated());
		}

		[Test]
		public void Valid_Chord()
		{
			TestTrigger(MouseButton.Left.WentDown() & Key.X.IsRepeated());
		}

		[Test]
		public void Valid_ChordOnce()
		{
			TestTrigger(MouseButton.Left.WentDown() + Key.X.IsRepeated());
		}

		[Test]
		public void Valid_KeyTrigger()
		{
			Success("[Key(Up, WentDown)]", Key.Up.WentDown());
		}

		[Test]
		public void Valid_KeyTrigger_Whitespace()
		{
			Success("[ Key ( A , Pressed ) ]", Key.A.IsPressed());
		}

		[Test]
		public void Valid_LeftAssociativity_Alias()
		{
			Success("[Key(X, Repeated) | Key(Y, Repeated) | Key(Z, Repeated)]",
				Key.X.IsRepeated() | Key.Y.IsRepeated() | Key.Z.IsRepeated());
		}

		[Test]
		public void Valid_LeftAssociativity_Chord()
		{
			Success("[Key(X, Repeated) & Key(Y, Repeated) & Key(Z, Repeated)]",
				Key.X.IsRepeated() & Key.Y.IsRepeated() & Key.Z.IsRepeated());
		}

		[Test]
		public void Valid_LeftAssociativity_ChordOnce()
		{
			Success("[Key(X, Repeated) + Key(Y, Repeated) + Key(Z, Repeated)]",
				Key.X.IsRepeated() + Key.Y.IsRepeated() + Key.Z.IsRepeated());
		}

		[Test]
		public void Valid_MouseTrigger()
		{
			Success("[Mouse(Left, WentDown)]", MouseButton.Left.WentDown());
		}

		[Test]
		public void Valid_MouseTrigger_Whitespace()
		{
			Success("[ Mouse ( Right , WentUp ) ]", MouseButton.Right.WentUp());
		}

		[Test]
		public void Valid_Nested()
		{
			TestTrigger(MouseButton.Left.WentUp() | Key.A.IsPressed() + Key.B.IsRepeated() & MouseButton.Right.IsPressed() |
						Key.X.WentUp() & Key.Q.WentDown() + MouseButton.XButton1.IsPressed());
		}

		[Test]
		public void Valid_Parens()
		{
			Success("[(Key(X, Repeated) | Key(Y, Repeated)) & Key(Z, Repeated)]",
				(Key.X.IsRepeated() | Key.Y.IsRepeated()) & Key.Z.IsRepeated());
		}

		[Test]
		public void Valid_Precedence_AliasLowerThanChord1()
		{
			Success("[Key(X, Repeated) | Key(Y, Repeated) & Key(Z, Repeated)]",
				Key.X.IsRepeated() | Key.Y.IsRepeated() & Key.Z.IsRepeated());
		}

		[Test]
		public void Valid_Precedence_AliasLowerThanChord2()
		{
			Success("[Key(X, Repeated) & Key(Y, Repeated) | Key(Z, Repeated)]",
				Key.X.IsRepeated() & Key.Y.IsRepeated() | Key.Z.IsRepeated());
		}

		[Test]
		public void Valid_Precedence_AliasLowerThanChordOnce1()
		{
			Success("[Key(X, Repeated) | Key(Y, Repeated) & Key(Z, Repeated)]",
				Key.X.IsRepeated() | Key.Y.IsRepeated() & Key.Z.IsRepeated());
		}

		[Test]
		public void Valid_Precedence_AliasLowerThanChordOnce2()
		{
			Success("[Key(X, Repeated) & Key(Y, Repeated) | Key(Z, Repeated)]",
				Key.X.IsRepeated() & Key.Y.IsRepeated() | Key.Z.IsRepeated());
		}

		[Test]
		public void Valid_Precedence_ChordLowerThanChordOnce1()
		{
			Success("[Key(X, Repeated) + Key(Y, Repeated) & Key(Z, Repeated)]",
				Key.X.IsRepeated() + Key.Y.IsRepeated() & Key.Z.IsRepeated());
		}

		[Test]
		public void Valid_Precedence_ChordLowerThanChordOnce2()
		{
			Success("[Key(X, Repeated) & Key(Y, Repeated) + Key(Z, Repeated)]",
				Key.X.IsRepeated() & Key.Y.IsRepeated() + Key.Z.IsRepeated());
		}
	}
}