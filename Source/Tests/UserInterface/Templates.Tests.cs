using System;

namespace Tests.UserInterface
{
	using FluentAssertions;
	using NUnit.Framework;
	using Pegasus.Framework.UserInterface;
	using Pegasus.Framework.UserInterface.Controls;

	[TestFixture]
	public class Templates
	{
		[SetUp]
		public void Setup()
		{
			_presenter1 = new ContentPresenter();
			_presenter2 = new ContentPresenter();

			_template1 = button =>
			{
				var thicknessBinding = new TemplateBinding<Thickness>(button, UIElement.MarginProperty);
				_presenter1.SetBinding(UIElement.MarginProperty, thicknessBinding);

				var contentBinding = new TemplateBinding<object>(button, ContentControl.ContentProperty);
				_presenter1.SetBinding(ContentPresenter.ContentProperty, contentBinding);

				return _presenter1;
			};

			_template2 = button =>
			{
				var binding = new TemplateBinding<Thickness>(button, UIElement.MarginProperty);
				_presenter2.SetBinding(UIElement.MarginProperty, binding);

				return _presenter2;
			};
		}

		private ContentPresenter _presenter1;
		private ContentPresenter _presenter2;

		private ControlTemplate _template1;
		private ControlTemplate _template2;

		private readonly Thickness _thickness1 = new Thickness(2);
		private readonly Thickness _thickness2 = new Thickness(4);

		[Test]
		public void LogicalParentOfTemplate()
		{
			var button = new Button { Template = _template1 };
			var control = new UserControl { Content = button };

			button.Parent.Should().Be(control);
		}

		[Test]
		public void LogicalParentOfTextContent_SetTemplateBeforeContent()
		{
			var text = new TextBlock();
			var button = new Button { Template = _template1, Content = text };
			var control = new UserControl { Content = button };

			button.Parent.Should().Be(control);
			text.Parent.Should().Be(button);
		}

		[Test]
		public void LogicalParentOfTextContent_SetContentBeforeTemplate()
		{
			var text = new TextBlock();
			var button = new Button { Content = text, Template = _template1 };
			var control = new UserControl { Content = button };

			button.Parent.Should().Be(control);
			text.Parent.Should().Be(button);
		}

		[Test]
		public void LogicalParentOfTextContent_ChangeButtonParent()
		{
			var text = new TextBlock();
			var button = new Button { Content = text, Template = _template1 };
			var control1 = new UserControl { Content = button };
			var control2 = new UserControl();

			button.Parent.Should().Be(control1);
			text.Parent.Should().Be(button);

			control1.Content = null;

			button.Parent.Should().Be(null);
			text.Parent.Should().Be(button);

			control2.Content = button;

			button.Parent.Should().Be(control2);
			text.Parent.Should().Be(button);
		}

		[Test]
		public void TemplateBinding_Set()
		{
			var button = new Button { Template = _template1 };
			button.GetVisualChild(0).Should().Be(_presenter1);

			button.Template = _template2;
			button.GetVisualChild(0).Should().Be(_presenter2);
		}

		[Test]
		public void TemplateBinding_SetValue()
		{
			var button = new Button { Margin = _thickness1, Template = _template1 };
			((ContentPresenter)button.GetVisualChild(0)).Margin.Should().Be(_thickness1);
		}

		[Test]
		public void TemplateBinding_ChangeValue()
		{
			var button = new Button { Margin = _thickness1, Template = _template1 };
			_presenter1.Margin.Should().Be(_thickness1);

			button.Margin = _thickness2;
			((ContentPresenter)button.GetVisualChild(0)).Margin.Should().Be(_thickness2);
		}

		[Test]
		public void VisualChildrenOfTemplatedControl()
		{
			var button = new Button { Template = _template1 };

			button.VisualChildrenCount.Should().Be(1);
			button.GetVisualChild(0).Should().Be(_presenter1);
		}

		[Test]
		public void LogicalChildrenOfTemplatedControl()
		{
			var button = new Button { Template = _template1 };

			var count = 0;
			foreach (var child in button.LogicalChildren)
			{
				++count;
				child.Should().Be(_presenter1);
			}
			count.Should().Be(1);
		}
	}
}