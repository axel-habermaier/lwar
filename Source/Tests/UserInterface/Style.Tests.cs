using System;

namespace Tests.UserInterface
{
	using FluentAssertions;
	using NUnit.Framework;
	using Pegasus.Framework.UserInterface;

	[TestFixture]
	public class StyleTests
	{
		[SetUp]
		public void Setup()
		{
			_control1 = new TestControl();
			_control2 = new TestControl();
		}

		private TestControl _control1;
		private TestControl _control2;

		[Test]
		public void Setters_BaseStyle_NoOverride()
		{
			const int value1 = 17;
			const int value2 = 42;

			var baseStyle = new Style();
			baseStyle.Setters.Add(new Setter<int>(TestControl.IntegerTestPropery1, value1));

			var style = new Style(baseStyle);
			style.Setters.Add(new Setter<int>(TestControl.IntegerTestPropery2, value2));

			_control1.Style = style;
			_control1.IntegerTest1.Should().Be(value1);
			_control1.IntegerTest2.Should().Be(value2);
		}

		[Test]
		public void Setters_BaseStyle_Override()
		{
			const int value1 = 17;
			const int value2 = 42;

			var baseStyle = new Style();
			baseStyle.Setters.Add(new Setter<int>(TestControl.IntegerTestPropery1, value1));

			var style = new Style(baseStyle);
			style.Setters.Add(new Setter<int>(TestControl.IntegerTestPropery1, value2));

			_control1.Style = style;
			_control1.IntegerTest1.Should().Be(value2);
			_control1.IntegerTest2.Should().Be(0);
		}

		[Test]
		public void Setters_BaseStyle_BaseStyle_Override()
		{
			const int value1 = 17;
			const int value2 = 42;
			const int value3 = 71;

			var baseStyle1 = new Style();
			baseStyle1.Setters.Add(new Setter<int>(TestControl.IntegerTestPropery1, value1));

			var baseStyle2 = new Style(baseStyle1);
			baseStyle2.Setters.Add(new Setter<int>(TestControl.IntegerTestPropery1, value2));

			var style = new Style(baseStyle2);
			style.Setters.Add(new Setter<int>(TestControl.IntegerTestPropery1, value3));

			_control1.Style = style;
			_control1.IntegerTest1.Should().Be(value3);
		}

		[Test]
		public void Setters_BaseStyle_BaseStyle_NoOverride()
		{
			const int value1 = 17;
			const int value2 = 42;

			var baseStyle1 = new Style();
			baseStyle1.Setters.Add(new Setter<int>(TestControl.IntegerTestPropery1, value1));

			var baseStyle2 = new Style(baseStyle1);
			baseStyle2.Setters.Add(new Setter<int>(TestControl.IntegerTestPropery2, value2));

			var style = new Style(baseStyle2);

			_control1.Style = style;
			_control1.IntegerTest1.Should().Be(value1);
			_control1.IntegerTest2.Should().Be(value2);
		}

		[Test]
		public void Setters_NoBaseStyle()
		{
			const int value = 17;

			var style = new Style();
			style.Setters.Add(new Setter<int>(TestControl.IntegerTestPropery1, value));

			_control1.Style = style;
			_control1.IntegerTest1.Should().Be(value);
		}

		[Test]
		public void Triggers_NoBaseStyle_NoSetters()
		{
			const int value = 17;

			var trigger = new Trigger<bool>(TestControl.BooleanTestProperty1, true);
			trigger.Setters.Add(new Setter<int>(TestControl.IntegerTestPropery1, value));

			var style = new Style();
			style.Triggers.Add(trigger);

			_control1.Style = style;
			_control1.IntegerTest1.Should().Be(0);

			_control1.BooleanTest1 = true;
			_control1.IntegerTest1.Should().Be(value);

			_control1.BooleanTest1 = false;
			_control1.IntegerTest1.Should().Be(0);
		}

		[Test]
		public void Triggers_NoBaseStyle_Setters_NoOverride()
		{
			const int value1 = 17;
			const int value2 = 42;

			var trigger = new Trigger<bool>(TestControl.BooleanTestProperty1, true);
			trigger.Setters.Add(new Setter<int>(TestControl.IntegerTestPropery1, value1));

			var style = new Style();
			style.Triggers.Add(trigger);
			style.Setters.Add(new Setter<int>(TestControl.IntegerTestPropery2, value2));

			_control1.Style = style;
			_control1.IntegerTest1.Should().Be(0);
			_control1.IntegerTest2.Should().Be(value2);

			_control1.BooleanTest1 = true;
			_control1.IntegerTest1.Should().Be(value1);
			_control1.IntegerTest2.Should().Be(value2);

			_control1.BooleanTest1 = false;
			_control1.IntegerTest1.Should().Be(0);
			_control1.IntegerTest2.Should().Be(value2);
		}

		[Test]
		public void Triggers_NoBaseStyle_Setters_Override()
		{
			const int value1 = 17;
			const int value2 = 42;

			var trigger = new Trigger<bool>(TestControl.BooleanTestProperty1, true);
			trigger.Setters.Add(new Setter<int>(TestControl.IntegerTestPropery1, value1));

			var style = new Style();
			style.Triggers.Add(trigger);
			style.Setters.Add(new Setter<int>(TestControl.IntegerTestPropery1, value2));

			_control1.Style = style;
			_control1.IntegerTest1.Should().Be(value2);

			_control1.BooleanTest1 = true;
			_control1.IntegerTest1.Should().Be(value1);

			_control1.BooleanTest1 = false;
			_control1.IntegerTest1.Should().Be(value2);
		}

		[Test]
		public void Triggers_BaseStyle_NoOverride()
		{
			const int value1 = 17;
			const int value2 = 42;

			var trigger1 = new Trigger<bool>(TestControl.BooleanTestProperty1, true);
			trigger1.Setters.Add(new Setter<int>(TestControl.IntegerTestPropery1, value1));

			var trigger2 = new Trigger<bool>(TestControl.BooleanTestProperty2, true);
			trigger2.Setters.Add(new Setter<int>(TestControl.IntegerTestPropery2, value2));

			var baseStyle = new Style();
			baseStyle.Triggers.Add(trigger1);

			var style = new Style(baseStyle);
			style.Triggers.Add(trigger2);

			_control1.Style = style;
			_control1.IntegerTest1.Should().Be(0);
			_control1.IntegerTest2.Should().Be(0);

			_control1.BooleanTest1 = true;
			_control1.IntegerTest1.Should().Be(value1);
			_control1.IntegerTest2.Should().Be(0);

			_control1.BooleanTest2 = true;
			_control1.IntegerTest1.Should().Be(value1);
			_control1.IntegerTest2.Should().Be(value2);

			_control1.BooleanTest1 = false;
			_control1.IntegerTest1.Should().Be(0);
			_control1.IntegerTest2.Should().Be(value2);

			_control1.BooleanTest2 = false;
			_control1.IntegerTest1.Should().Be(0);
			_control1.IntegerTest2.Should().Be(0);
		}

		[Test]
		public void Triggers_Ordering()
		{
			const int value1 = 17;
			const int value2 = 42;

			var trigger1 = new Trigger<bool>(TestControl.BooleanTestProperty1, true);
			trigger1.Setters.Add(new Setter<int>(TestControl.IntegerTestPropery1, value1));

			var trigger2 = new Trigger<bool>(TestControl.BooleanTestProperty2, true);
			trigger2.Setters.Add(new Setter<int>(TestControl.IntegerTestPropery1, value2));

			var style = new Style();
			style.Triggers.Add(trigger1);
			style.Triggers.Add(trigger2);

			_control1.Style = style;
			_control1.IntegerTest1.Should().Be(0);

			_control1.BooleanTest1 = true;
			_control1.IntegerTest1.Should().Be(value1);

			_control1.BooleanTest2 = true;
			_control1.IntegerTest1.Should().Be(value2);

			_control1.BooleanTest2 = false;
			_control1.IntegerTest1.Should().Be(value1);

			_control1.BooleanTest2 = true;
			_control1.IntegerTest1.Should().Be(value2);

			_control1.BooleanTest1 = false;
			_control1.IntegerTest1.Should().Be(value2);

			_control1.BooleanTest2 = false;
			_control1.IntegerTest1.Should().Be(0);
		}

		[Test]
		public void Triggers_BaseStyle_Override_Ordering()
		{
			const int value1 = 17;
			const int value2 = 42;

			var trigger1 = new Trigger<bool>(TestControl.BooleanTestProperty1, true);
			trigger1.Setters.Add(new Setter<int>(TestControl.IntegerTestPropery1, value1));

			var trigger2 = new Trigger<bool>(TestControl.BooleanTestProperty2, true);
			trigger2.Setters.Add(new Setter<int>(TestControl.IntegerTestPropery1, value2));

			var baseStyle = new Style();
			baseStyle.Triggers.Add(trigger1);

			var style = new Style(baseStyle);
			style.Triggers.Add(trigger2);

			_control1.Style = style;
			_control1.IntegerTest1.Should().Be(0);

			_control1.BooleanTest1 = true;
			_control1.IntegerTest1.Should().Be(value1);

			_control1.BooleanTest2 = true;
			_control1.IntegerTest1.Should().Be(value2);

			_control1.BooleanTest1 = false;
			_control1.IntegerTest1.Should().Be(value2);

			_control1.BooleanTest2 = false;
			_control1.IntegerTest1.Should().Be(0);
		}

		[Test]
		public void Triggers_ImmediatelyTriggered()
		{
			const int value = 17;

			var trigger = new Trigger<bool>(TestControl.BooleanTestProperty1, false);
			trigger.Setters.Add(new Setter<int>(TestControl.IntegerTestPropery1, value));

			var style = new Style();
			style.Triggers.Add(trigger);

			_control1.Style = style;
			_control1.IntegerTest1.Should().Be(value);
		}

		[Test]
		public void Triggers_SharedStyle()
		{
			const int value1 = 17;
			const int value2 = 42;

			var trigger = new Trigger<bool>(TestControl.BooleanTestProperty1, true);
			trigger.Setters.Add(new Setter<int>(TestControl.IntegerTestPropery1, value1));

			var style = new Style();
			style.Triggers.Add(trigger);
			style.Setters.Add(new Setter<int>(TestControl.IntegerTestPropery2, value2));

			_control1.Style = style;
			_control2.Style = style;

			_control1.IntegerTest1.Should().Be(0);
			_control2.IntegerTest1.Should().Be(0);
			_control1.IntegerTest2.Should().Be(value2);
			_control2.IntegerTest2.Should().Be(value2);

			_control2.BooleanTest1 = true;
			_control1.IntegerTest1.Should().Be(0);
			_control2.IntegerTest1.Should().Be(value1);
			_control1.IntegerTest2.Should().Be(value2);
			_control2.IntegerTest2.Should().Be(value2);

			_control1.BooleanTest1 = true;
			_control1.IntegerTest1.Should().Be(value1);
			_control2.IntegerTest1.Should().Be(value1);
			_control1.IntegerTest2.Should().Be(value2);
			_control2.IntegerTest2.Should().Be(value2);

			_control2.BooleanTest1 = false;
			_control1.IntegerTest1.Should().Be(value1);
			_control2.IntegerTest1.Should().Be(0);
			_control1.IntegerTest2.Should().Be(value2);
			_control2.IntegerTest2.Should().Be(value2);
		}

		[Test]
		public void Unset_Triggers_WhenTriggered()
		{
			const int value1 = 17;
			const int value2 = 42;

			var trigger1 = new Trigger<bool>(TestControl.BooleanTestProperty1, true);
			trigger1.Setters.Add(new Setter<int>(TestControl.IntegerTestPropery1, value1));

			var trigger2 = new Trigger<bool>(TestControl.BooleanTestProperty1, true);
			trigger2.Setters.Add(new Setter<int>(TestControl.IntegerTestPropery2, value2));

			var baseStyle = new Style();
			baseStyle.Triggers.Add(trigger1);

			var style = new Style(baseStyle);
			style.Triggers.Add(trigger2);

			_control1.Style = style;
			_control1.BooleanTest1 = true;
			_control1.IntegerTest1.Should().Be(value1);
			_control1.IntegerTest2.Should().Be(value2);

			_control1.Style = null;
			_control1.IntegerTest1.Should().Be(0);
			_control1.IntegerTest2.Should().Be(0);
		}

		[Test]
		public void Unset_Triggers_WhenNotTriggered()
		{
			const int value1 = 17;
			const int value2 = 42;

			var trigger1 = new Trigger<bool>(TestControl.BooleanTestProperty1, true);
			trigger1.Setters.Add(new Setter<int>(TestControl.IntegerTestPropery1, value1));

			var trigger2 = new Trigger<bool>(TestControl.BooleanTestProperty1, true);
			trigger2.Setters.Add(new Setter<int>(TestControl.IntegerTestPropery2, value2));

			var baseStyle = new Style();
			baseStyle.Triggers.Add(trigger1);

			var style = new Style(baseStyle);
			style.Triggers.Add(trigger2);

			_control1.Style = style;
			_control1.BooleanTest1 = true;
			_control1.IntegerTest1.Should().Be(value1);
			_control1.IntegerTest2.Should().Be(value2);

			_control1.BooleanTest1 = false;
			_control1.Style = null;
			_control1.IntegerTest1.Should().Be(0);
			_control1.IntegerTest2.Should().Be(0);
		}

		[Test]
		public void Setters_Ordering()
		{
			const int value1 = 17;
			const int value2 = 42;

			var style = new Style();
			style.Setters.Add(new Setter<int>(TestControl.IntegerTestPropery1, value1));
			style.Setters.Add(new Setter<int>(TestControl.IntegerTestPropery1, value2));

			_control1.Style = style;
			_control1.IntegerTest1.Should().Be(value2);
		}
	}
}
