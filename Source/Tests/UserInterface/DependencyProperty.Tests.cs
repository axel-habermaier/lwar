namespace Tests.UserInterface
{
	using System;
	using FluentAssertions;
	using NUnit.Framework;
	using Pegasus.Framework.UserInterface;

	[TestFixture]
	public class DependencyPropertyTests
	{
		[Test]
		public void DefaultValue_NonStandardDefault()
		{
			// This check is just to be sure that we're actually testing a dependency property with a non-standard default value
			UIElement.WidthProperty.DefaultValue.Should().Be(Double.NaN);

			var control = new TestControl();
			control.Width.Should().Be(Double.NaN);
		}

		[Test]
		public void DefaultValue_StandardDefault()
		{
			// This check is just to be sure that we're actually testing a dependency property with a standard default value
			UIElement.ViewModelProperty.DefaultValue.Should().BeNull();

			var control = new TestControl();
			control.ViewModel.Should().BeNull();
		}

		[Test]
		public void SetValue()
		{
			const HorizontalAlignment horizontalAlignment = HorizontalAlignment.Right;
			const VerticalAlignment verticalAlignment = VerticalAlignment.Center;
			const double width = 0.5;

			var margin = new Thickness(2);
			var viewModel = new TestViewModel();

			var control = new TestControl
			{
				HorizontalAlignment = horizontalAlignment,
				VerticalAlignment = verticalAlignment,
				Width = width,
				Margin = margin,
				ViewModel = viewModel
			};

			control.HorizontalAlignment.Should().Be(horizontalAlignment);
			control.VerticalAlignment.Should().Be(verticalAlignment);
			control.Width.Should().Be(width);
			control.Margin.Should().Be(margin);
			control.ViewModel.Should().Be(viewModel);
		}
	}
}