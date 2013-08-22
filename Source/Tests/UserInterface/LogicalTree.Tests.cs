namespace Tests.UserInterface
{
	using FluentAssertions;
	using NUnit.Framework;

	[TestFixture]
	public class LogicalTreeTests
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
		public void Canvas()
		{
			_control1.Canvas1.Parent.Should().Be(_control1);
			_control1.Canvas1.Children.Should().ContainInOrder(_control1.Button1, _control1.Button2, _control1.Canvas2);
		}

		[Test]
		public void Button()
		{
			_control1.Button1.Parent.Should().Be(_control1.Canvas1);
			_control1.Button2.Parent.Should().Be(_control1.Canvas1);
			_control1.Button3.Parent.Should().Be(_control1.Canvas2);
		}

		[Test]
		public void TreeRemove()
		{
			_control1.Canvas1.Children.Remove(_control1.Button2);

			_control1.Canvas1.Children.Should().ContainInOrder(_control1.Button1, _control1.Canvas2);
			_control1.Button2.Parent.Should().BeNull();
		}

		[Test]
		public void TreeChange()
		{
			_control1.Canvas1.Children.Remove(_control1.Button2);
			_control2.Canvas1.Children.Add(_control1.Button2);

			_control1.Button2.Parent.Should().Be(_control2.Canvas1);
		}
	}
}
