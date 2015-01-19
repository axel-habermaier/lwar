namespace Tests.UserInterface
{
	using System;
	using FluentAssertions;
	using NUnit.Framework;
	using Pegasus.UserInterface;
	using Pegasus.UserInterface.Controls;
	using Pegasus.UserInterface.Input;
	using Pegasus.Utilities;

	[TestFixture]
	public class RoutedEventBindingTests
	{
		[SetUp]
		public void Setup()
		{
			_parameterMethodInvoked = false;
			_parameterlessMethodInvoked = false;
			_sender = null;
			_args = null;
		}

		private bool _parameterlessMethodInvoked;
		private bool _parameterMethodInvoked;
		private object _sender;
		private RoutedEventArgs _args;

		[UsedImplicitly]
		public void OnEvent()
		{
			_parameterlessMethodInvoked = true;
		}

		[UsedImplicitly]
		public void OnEventParameters(object sender, RoutedEventArgs args)
		{
			_parameterMethodInvoked = true;
			_sender = sender;
			_args = args;
		}

		[UsedImplicitly]
		public void OnEventParameters(object sender, MouseEventArgs args)
		{
			// Just declared to ensure that the correct overload is called
		}

		[UsedImplicitly]
		public void OnEventParameters(object sender, TextChangedEventArgs args)
		{
			_parameterMethodInvoked = true;
			_sender = sender;
			_args = args;
		}

		[Test]
		public void BindMethodWithParameters()
		{
			var button = new Button { DataContext = this, IsAttachedToRoot = true };
			button.CreateEventBinding(Button.ClickEvent, "OnEventParameters");

			button.RaiseEvent(Button.ClickEvent, RoutedEventArgs.Default);
			_parameterMethodInvoked.Should().BeTrue();
			_parameterlessMethodInvoked.Should().BeFalse();
			_sender.Should().Be(button);
			_args.Should().Be(RoutedEventArgs.Default);
		}

		[Test]
		public void BindMethodWithParameters_TextChangedEvent()
		{
			var textBox = new TextBox { DataContext = this, IsAttachedToRoot = true };
			textBox.CreateEventBinding(TextBox.TextChangedEvent, "OnEventParameters");

			textBox.RaiseEvent(TextBox.TextChangedEvent, TextChangedEventArgs.Create("test"));
			_parameterMethodInvoked.Should().BeTrue();
			_parameterlessMethodInvoked.Should().BeFalse();
			_sender.Should().Be(textBox);

			var args = (TextChangedEventArgs)_args;
			args.Text.Should().Be("test");
		}

		[Test]
		public void BindMethodWithoutParameters()
		{
			var button = new Button { DataContext = this, IsAttachedToRoot = true };
			button.CreateEventBinding(Button.ClickEvent, "OnEvent");

			button.RaiseEvent(Button.ClickEvent, RoutedEventArgs.Default);
			_parameterlessMethodInvoked.Should().BeTrue();
			_parameterMethodInvoked.Should().BeFalse();
		}

		[Test]
		public void Binding_BecomesAttachedToRoot()
		{
			var button = new Button { DataContext = this };
			button.CreateEventBinding(Button.ClickEvent, "OnEventParameters");

			button.RaiseEvent(Button.ClickEvent, RoutedEventArgs.Default);
			_parameterMethodInvoked.Should().BeFalse();
			_parameterlessMethodInvoked.Should().BeFalse();

			button.IsAttachedToRoot = true;

			button.RaiseEvent(Button.ClickEvent, RoutedEventArgs.Default);
			_parameterMethodInvoked.Should().BeTrue();
			_parameterlessMethodInvoked.Should().BeFalse();
		}

		[Test]
		public void Binding_NoLongerAttachedToRoot()
		{
			var button = new Button { DataContext = this, IsAttachedToRoot = true };
			button.CreateEventBinding(Button.ClickEvent, "OnEventParameters");

			button.IsAttachedToRoot = false;

			button.RaiseEvent(Button.ClickEvent, RoutedEventArgs.Default);
			_parameterMethodInvoked.Should().BeFalse();
			_parameterlessMethodInvoked.Should().BeFalse();
		}

		[Test]
		public void Binding_NotAttachedToRoot()
		{
			var button = new Button { DataContext = this };
			button.CreateEventBinding(Button.ClickEvent, "OnEventParameters");

			button.RaiseEvent(Button.ClickEvent, RoutedEventArgs.Default);
			_parameterMethodInvoked.Should().BeFalse();
			_parameterlessMethodInvoked.Should().BeFalse();
		}

		[Test]
		public void DataContextChanged()
		{
			var button = new Button { DataContext = this, IsAttachedToRoot = true };
			button.CreateEventBinding(Button.ClickEvent, "OnEvent");

			button.RaiseEvent(Button.ClickEvent, RoutedEventArgs.Default);
			_parameterlessMethodInvoked.Should().BeTrue();
			_parameterlessMethodInvoked = false;

			var dataContext = new RoutedEventBindingTests();
			button.DataContext = dataContext;

			button.RaiseEvent(Button.ClickEvent, RoutedEventArgs.Default);
			_parameterlessMethodInvoked.Should().BeFalse();
			dataContext._parameterlessMethodInvoked.Should().BeTrue();
			dataContext._parameterMethodInvoked.Should().BeFalse();
		}

		[Test]
		public void DataContextSetAfterBinding()
		{
			var button = new Button { IsAttachedToRoot = true };
			button.CreateEventBinding(Button.ClickEvent, "OnEvent");

			button.DataContext = this;
			button.RaiseEvent(Button.ClickEvent, RoutedEventArgs.Default);
			_parameterlessMethodInvoked.Should().BeTrue();
			_parameterMethodInvoked.Should().BeFalse();
		}

		[Test]
		public void DataContextSetAfterFirstInvocation()
		{
			var button = new Button { IsAttachedToRoot = true };
			button.CreateEventBinding(Button.ClickEvent, "OnEvent");

			button.RaiseEvent(Button.ClickEvent, RoutedEventArgs.Default);
			_parameterlessMethodInvoked.Should().BeFalse();
			_parameterMethodInvoked.Should().BeFalse();

			button.DataContext = this;

			button.RaiseEvent(Button.ClickEvent, RoutedEventArgs.Default);
			_parameterlessMethodInvoked.Should().BeTrue();
			_parameterMethodInvoked.Should().BeFalse();
		}

		[Test]
		public void NoDataContext()
		{
			var button = new Button { IsAttachedToRoot = true };
			button.CreateEventBinding(Button.ClickEvent, "OnEvent");

			button.RaiseEvent(Button.ClickEvent, RoutedEventArgs.Default);
			_parameterlessMethodInvoked.Should().BeFalse();
			_parameterMethodInvoked.Should().BeFalse();
		}
	}
}