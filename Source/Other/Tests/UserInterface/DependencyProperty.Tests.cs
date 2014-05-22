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
		public void PreserveLocalValue()
		{
			var control = new TestControl();
			control.SetAnimatedValue(UIElement.WidthProperty, 1);
			control.SetValue(UIElement.WidthProperty, 2);

			control.Width.Should().Be(1);

			control.UnsetAnimatedValue(UIElement.WidthProperty);
			control.Width.Should().Be(2);
		}

		[Test]
		public void PreserveStyleValues()
		{
			var control = new TestControl();
			control.SetAnimatedValue(UIElement.WidthProperty, 1);
			control.SetStyleTriggeredValue(UIElement.WidthProperty, 3);
			control.SetStyleValue(UIElement.WidthProperty, 4);

			control.Width.Should().Be(1);

			control.UnsetAnimatedValue(UIElement.WidthProperty);
			control.Width.Should().Be(3);

			control.UnsetStyleTriggeredValue(UIElement.WidthProperty);
			control.Width.Should().Be(4);
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

		[Test]
		public void BoundValue()
		{
			var viewModel = new TestViewModel { String = "Test123" };
			var control = new TestControl();

			control.CreateDataBinding(viewModel, TestControl.DefaultStringTestProperty, BindingMode.OneWay, "String");
			control.DefaultStringTest.Should().Be(viewModel.String);

			control = new TestControl { IsAttachedToRoot = false };
			control.CreateDataBinding(viewModel, TestControl.DefaultStringTestProperty, BindingMode.OneWay, "String");

			control.IsAttachedToRoot = true;
			control.DefaultStringTest.Should().Be(viewModel.String);
		}

		[Test]
		public void NullBoundValue()
		{
			var control = new TestControl() { IsAttachedToRoot = false };
			control.CreateDataBinding(TestControl.DefaultStringTestProperty, BindingMode.OneWay, "String");

			control.IsAttachedToRoot = true;
			control.DefaultStringTest.Should().Be(TestControl.DefaultStringTestProperty.DefaultValue);

			var viewModel = new TestViewModel();
			control = new TestControl() { IsAttachedToRoot = false };
			control.CreateDataBinding(viewModel, TestControl.DefaultStringTestProperty, BindingMode.OneWay, "Model", "String");

			control.IsAttachedToRoot = true;
			control.DefaultStringTest.Should().Be(TestControl.DefaultStringTestProperty.DefaultValue);

			control = new TestControl() { IsAttachedToRoot = false };
			control.CreateDataBinding(TestControl.DefaultStringTestProperty, BindingMode.OneWay, "Model", "String");

			control.IsAttachedToRoot = true;
			control.DefaultStringTest.Should().Be(TestControl.DefaultStringTestProperty.DefaultValue);
		}
	}
}