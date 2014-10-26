namespace Tests.Parsing.Combinators
{
	using System;
	using NUnit.Framework;
	using Pegasus.Scripting.Parsing.Combinators;
	using Pegasus.UserInterface.Input;

	[TestFixture]
	public class ConfigurableInputParserTests : ParserTestsHelper<ConfigurableInput>
	{
		public ConfigurableInputParserTests()
			: base(new ConfigurableInputParser())
		{
		}

		[Test]
		public void Valid_KeyTrigger()
		{
			Success("[Key.A]", Key.A);
		}

		[Test]
		public void Valid_KeyTrigger_Modifiers()
		{
			Success("[Key.A+Control]", new ConfigurableInput(Key.A, KeyModifiers.Control));
		}

		[Test]
		public void Valid_KeyTrigger_Whitespace()
		{
			Success("[Key .   A]", Key.A);
		}

		[Test]
		public void Valid_KeyTrigger_Whitespace_Modifiers()
		{
			Success("[Key .   A  +   Shift]", new ConfigurableInput(Key.A, KeyModifiers.Shift));
		}

		[Test]
		public void Valid_MouseTrigger()
		{
			Success("[Mouse.Left]", MouseButton.Left);
		}

		[Test]
		public void Valid_MouseTrigger_Modifiers()
		{
			Success("[Mouse.Left+Shift+Control+Alt]",
				new ConfigurableInput(MouseButton.Left, KeyModifiers.Shift | KeyModifiers.Control | KeyModifiers.Alt));
		}

		[Test]
		public void Valid_MouseTrigger_Whitespace()
		{
			Success("[Mouse   .   Middle]", MouseButton.Middle);
		}

		[Test]
		public void Valid_MouseTrigger_Whitespace_Modifiers()
		{
			Success("[Mouse   .   Middle + Shift+ Control +Alt]",
				new ConfigurableInput(MouseButton.Middle, KeyModifiers.Shift | KeyModifiers.Control | KeyModifiers.Alt));
		}
	}
}