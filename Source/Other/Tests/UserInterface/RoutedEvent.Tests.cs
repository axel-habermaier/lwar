namespace Tests.UserInterface
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using FluentAssertions;
	using NUnit.Framework;
	using Pegasus.Framework.UserInterface;
	using Pegasus.Framework.UserInterface.Controls;
	using Pegasus.Framework.UserInterface.Input;
	using Pegasus.Math;

	[TestFixture]
	public class RoutedEventTests
	{
		[SetUp]
		public void Setup()
		{
			_stackPanel = new StackPanel { IsAttachedToRoot = true };
			_textBlock = new TextBlock();
			_button = new Button { Content = _textBlock };
			_stackPanel.Add(new Border());
			_stackPanel.Add(_button);
			_stackPanel.Add(new Border());
			_actualEvents = new List<UIElement>();
		}

		private void RecordEvent<T>(object sender, T args)
			where T : RoutedEventArgs
		{
			_actualEvents.Add((UIElement)sender);
		}

		private void RegisterHandlers<T>(UIElement uiElement, RoutedEvent<T> routedEvent)
			where T : RoutedEventArgs
		{
			uiElement.AddHandler(routedEvent, RecordEvent);
			for (var i = 0; i < uiElement.VisualChildrenCount; ++i)
				RegisterHandlers(uiElement.GetVisualChild(i), routedEvent);
		}

		private void TestEvent<T>(UIElement uiElement, RoutedEvent<T> routedEvent, T args, params UIElement[] expectedEvents)
			where T : RoutedEventArgs
		{
			RegisterHandlers(_stackPanel, routedEvent);
			uiElement.RaiseEvent(routedEvent, args);

			var actual = _actualEvents.Select(element => element.GetType().FullName);
			var expected = expectedEvents.Select(element => element.GetType().FullName);
			actual.Should().Equal(expected);
		}

		private StackPanel _stackPanel;
		private TextBlock _textBlock;
		private Button _button;
		private List<UIElement> _actualEvents;

		[Test]
		public void BubblingEvent_AllLevels()
		{
			TestEvent(_textBlock, UIElement.KeyUpEvent, KeyEventArgs.Create(Key.A, 0, new InputState()),
				_textBlock, _button, _stackPanel);
		}

		[Test]
		public void BubblingEvent_OneLevel()
		{
			TestEvent(_stackPanel, UIElement.KeyUpEvent, KeyEventArgs.Create(Key.A, 0, new InputState()),
				_stackPanel);
		}

		[Test]
		public void BubblingEvent_TwoLevels()
		{
			TestEvent(_button, UIElement.KeyDownEvent, KeyEventArgs.Create(Key.A, 0, new InputState()),
				_button, _stackPanel);
		}

		[Test]
		public void DirectEvent_Bottom()
		{
			TestEvent(_textBlock, UIElement.MouseEnterEvent, MouseEventArgs.Create(new Vector2i(), new InputState[] { }),
				_textBlock);
		}

		[Test]
		public void DirectEvent_Middle()
		{
			TestEvent(_button, UIElement.MouseEnterEvent, MouseEventArgs.Create(new Vector2i(), new InputState[] { }),
				_button);
		}

		[Test]
		public void DirectEvent_Top()
		{
			TestEvent(_stackPanel, UIElement.MouseEnterEvent, MouseEventArgs.Create(new Vector2i(), new InputState[] { }),
				_stackPanel);
		}

		[Test]
		public void TunnelingEvent_AllLevels()
		{
			TestEvent(_textBlock, UIElement.PreviewKeyUpEvent, KeyEventArgs.Create(Key.A, 0, new InputState()),
				_stackPanel, _button, _textBlock);
		}

		[Test]
		public void TunnelingEvent_OneLevel()
		{
			TestEvent(_stackPanel, UIElement.PreviewKeyUpEvent, KeyEventArgs.Create(Key.A, 0, new InputState()),
				_stackPanel);
		}

		[Test]
		public void TunnelingEvent_TwoLevels()
		{
			TestEvent(_button, UIElement.PreviewKeyDownEvent, KeyEventArgs.Create(Key.A, 0, new InputState()),
				_stackPanel, _button);
		}
	}
}