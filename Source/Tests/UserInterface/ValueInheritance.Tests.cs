using System;

namespace Tests.UserInterface
{
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
			UIElement.ViewModelProperty.Inherits.Should().BeTrue();
		}

		private TestViewModel _viewModel1;
		private TestViewModel _viewModel2;

		private TestControl _control1;
		private TestControl _control2;

		[Test]
		public void InheritedProperty_Inherits()
		{
			_control1.ViewModel = _viewModel1;

			_control1.Canvas1.ViewModel.Should().Be(_viewModel1);
			_control1.Button1.ViewModel.Should().Be(_viewModel1);
			_control1.Button2.ViewModel.Should().Be(_viewModel1);
		}

		[Test]
		public void InheritedProperty_Inherits_Overrides()
		{
			_control1.ViewModel = _viewModel1;
			_control1.Canvas2.ViewModel = _viewModel2;

			_control1.Canvas1.ViewModel.Should().Be(_viewModel1);
			_control1.Button1.ViewModel.Should().Be(_viewModel1);
			_control1.Button2.ViewModel.Should().Be(_viewModel1);
			_control1.Canvas2.ViewModel.Should().Be(_viewModel2);
			_control1.Button3.ViewModel.Should().Be(_viewModel2);
		}

		[Test]
		public void Inherits_TreeRemove()
		{
			_control1.ViewModel = _viewModel1;

			_control1.Button3.ViewModel.Should().Be(_viewModel1);
			_control1.Canvas2.Children.Remove(_control1.Button3);

			_control1.Button3.ViewModel.Should().Be(UIElement.ViewModelProperty.DefaultValue);
		}

		[Test]
		public void Inherits_TreeChange()
		{
			_control1.ViewModel = _viewModel1;
			_control2.ViewModel = _viewModel2;

			_control1.Button3.ViewModel.Should().Be(_viewModel1);
			_control1.Canvas2.Children.Remove(_control1.Button3);
			_control2.Canvas2.Children.Add(_control1.Button3);

			_control1.Button3.ViewModel.Should().Be(_viewModel2);
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