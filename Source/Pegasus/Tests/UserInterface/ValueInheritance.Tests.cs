namespace Tests.UserInterface
{
	using System;
	using FluentAssertions;
	using NUnit.Framework;
	using Pegasus.Framework.UserInterface;

	[TestFixture]
	public class ValueInheritanceTests
	{
		[SetUp]
		public void Setup()
		{
			_viewModel1 = new TestViewModel();
			_viewModel2 = new TestViewModel();
			_control1 = new TestControl();
			_control2 = new TestControl();

			// This is just to ensure that the maring property actually not inherits its value
			UIElement.MarginProperty.Inherits.Should().BeFalse();

			// This is just to ensure that the view model property actually inherits its value
			UIElement.DataContextProperty.Inherits.Should().BeTrue();
		}

		private TestViewModel _viewModel1;
		private TestViewModel _viewModel2;

		private TestControl _control1;
		private TestControl _control2;

		[Test]
		public void FindResource_RootElement()
		{
			var resource = new object();
			_control1.Resources.Add("r", resource);

			object foundResource;
			_control1.Button3.TryFindResource("r", out foundResource).Should().BeTrue();
			foundResource.Should().Be(resource);
		}

		[Test]
		public void FindResource_RootElement_Overriden()
		{
			var resource = new object();
			_control1.Resources.Add("r", new object());
			_control1.Canvas1.Resources.Add("r", resource);

			object foundResource;
			_control1.Button3.TryFindResource("r", out foundResource).Should().BeTrue();
			foundResource.Should().Be(resource);
		}

		[Test]
		public void FindResource_SameElement()
		{
			var resource = new object();
			_control1.Button3.Resources.Add("r", resource);

			object foundResource;
			_control1.Button3.TryFindResource("r", out foundResource).Should().BeTrue();
			foundResource.Should().Be(resource);
		}

		[Test]
		public void FindResource_Unknown()
		{
			object foundResource;
			_control1.Button3.TryFindResource("r", out foundResource).Should().BeFalse();
		}

		[Test]
		public void InheritedProperty_Inherits()
		{
			_control1.DataContext = _viewModel1;

			_control1.Canvas1.DataContext.Should().Be(_viewModel1);
			_control1.Button1.DataContext.Should().Be(_viewModel1);
			_control1.Button2.DataContext.Should().Be(_viewModel1);
		}

		[Test]
		public void InheritedProperty_Inherits_Overrides()
		{
			_control1.DataContext = _viewModel1;
			_control1.Canvas2.DataContext = _viewModel2;

			_control1.Canvas1.DataContext.Should().Be(_viewModel1);
			_control1.Button1.DataContext.Should().Be(_viewModel1);
			_control1.Button2.DataContext.Should().Be(_viewModel1);
			_control1.Canvas2.DataContext.Should().Be(_viewModel2);
			_control1.Button3.DataContext.Should().Be(_viewModel2);
		}

		[Test]
		public void Inherits_TreeChange()
		{
			_control1.DataContext = _viewModel1;
			_control2.DataContext = _viewModel2;

			_control1.Button3.DataContext.Should().Be(_viewModel1);
			_control1.Canvas2.Children.Remove(_control1.Button3);
			_control2.Canvas2.Children.Add(_control1.Button3);

			_control1.Button3.DataContext.Should().Be(_viewModel2);
		}

		[Test]
		public void Inherits_TreeRemove()
		{
			_control1.DataContext = _viewModel1;

			_control1.Button3.DataContext.Should().Be(_viewModel1);
			_control1.Canvas2.Children.Remove(_control1.Button3);

			_control1.Button3.DataContext.Should().Be(UIElement.DataContextProperty.DefaultValue);
		}

		[Test]
		public void NonInheritedProperty()
		{
			_control1.Margin = new Thickness(5);

			_control1.Canvas1.Margin.Should().Be(new Thickness());
			_control1.Canvas2.Margin.Should().Be(new Thickness());
			_control1.Button1.Margin.Should().Be(new Thickness());
			_control1.Button2.Margin.Should().Be(new Thickness());
			_control1.Button3.Margin.Should().Be(new Thickness());
		}
	}
}