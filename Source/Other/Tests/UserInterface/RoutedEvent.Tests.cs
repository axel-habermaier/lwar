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
			_actualInstanceEvents = new List<UIElement>();
			_actualClassEvents = new List<UIElement>();
			_handlingElement = null;
		}

		private void RecordInstanceEvent<T>(object sender, T args)
			where T : RoutedEventArgs
		{
			_actualInstanceEvents.Add((UIElement)sender);

			if (sender == _handlingElement)
				args.Handled = true;
		}

		private void RecordClassEvent<T>(object sender, T args)
			where T : RoutedEventArgs
		{
			_actualClassEvents.Add((UIElement)sender);

			if (sender == _handlingElement)
				args.Handled = true;
		}

		private void RegisterHandlers<T>(UIElement uiElement, RoutedEvent<T> routedEvent)
			where T : RoutedEventArgs
		{
			uiElement.AddHandler(routedEvent, RecordInstanceEvent);
			for (var i = 0; i < uiElement.VisualChildrenCount; ++i)
				RegisterHandlers(uiElement.GetVisualChild(i), routedEvent);
		}

		private void TestInstanceHandlers<T>(UIElement uiElement, RoutedEvent<T> routedEvent, T args, params UIElement[] expectedEvents)
			where T : RoutedEventArgs
		{
			RegisterHandlers(_stackPanel, routedEvent);
			uiElement.RaiseEvent(routedEvent, args);

			var actual = _actualInstanceEvents.Select(element => element.GetType().FullName);
			var expected = expectedEvents.Select(element => element.GetType().FullName);
			actual.Should().Equal(expected);
		}

		private void TestClassHandler<T>(UIElement uiElement, RoutedEvent<T> routedEvent, T args, params UIElement[] expectedEvents)
			where T : RoutedEventArgs
		{
			try
			{
				routedEvent.Raised += RecordClassEvent;
				uiElement.RaiseEvent(routedEvent, args);

				var actual = _actualClassEvents.Select(element => element.GetType().FullName);
				var expected = expectedEvents.Select(element => element.GetType().FullName);
				actual.Should().Equal(expected);
			}
			finally
			{
				routedEvent.Raised -= RecordClassEvent;
			}
		}

		private StackPanel _stackPanel;
		private TextBlock _textBlock;
		private Button _button;
		private UIElement _handlingElement;
		private List<UIElement> _actualInstanceEvents;
		private List<UIElement> _actualClassEvents;

		[Test]
		public void BubblingEvent_ClassHandlers_AllLevels()
		{
			TestClassHandler(_textBlock, UIElement.KeyUpEvent, KeyEventArgs.Create(new Keyboard(), Key.A, 0, new InputState()),
				_textBlock, _button, _stackPanel);
		}

		[Test]
		public void BubblingEvent_ClassHandlers_Handled()
		{
			_handlingElement = _textBlock;
			TestClassHandler(_textBlock, UIElement.KeyUpEvent, KeyEventArgs.Create(new Keyboard(), Key.A, 0, new InputState()), _textBlock);
		}

		[Test]
		public void BubblingEvent_ClassHandlers_OneLevel()
		{
			TestClassHandler(_stackPanel, UIElement.KeyUpEvent, KeyEventArgs.Create(new Keyboard(), Key.A, 0, new InputState()),
				_stackPanel);
		}

		[Test]
		public void BubblingEvent_ClassHandlers_TwoLevels()
		{
			TestClassHandler(_button, UIElement.KeyDownEvent, KeyEventArgs.Create(new Keyboard(), Key.A, 0, new InputState()),
				_button, _stackPanel);
		}

		[Test]
		public void BubblingEvent_InstanceHandlers_AllLevels()
		{
			TestInstanceHandlers(_textBlock, UIElement.KeyUpEvent, KeyEventArgs.Create(new Keyboard(), Key.A, 0, new InputState()),
				_textBlock, _button, _stackPanel);
		}

		[Test]
		public void BubblingEvent_InstanceHandlers_Handled()
		{
			_handlingElement = _textBlock;
			TestInstanceHandlers(_textBlock, UIElement.KeyUpEvent, KeyEventArgs.Create(new Keyboard(), Key.A, 0, new InputState()), _textBlock);
		}

		[Test]
		public void BubblingEvent_InstanceHandlers_OneLevel()
		{
			TestInstanceHandlers(_stackPanel, UIElement.KeyUpEvent, KeyEventArgs.Create(new Keyboard(), Key.A, 0, new InputState()),
				_stackPanel);
		}

		[Test]
		public void BubblingEvent_InstanceHandlers_TwoLevels()
		{
			TestInstanceHandlers(_button, UIElement.KeyDownEvent, KeyEventArgs.Create(new Keyboard(), Key.A, 0, new InputState()),
				_button, _stackPanel);
		}

		[Test]
		public void DirectEvent_ClassHandler_InstanceHandlers_Middle()
		{
			TestClassHandler(_button, UIElement.MouseEnterEvent, MouseEventArgs.Create(new Mouse(), new Vector2i(), new InputState[] { }),
				_button);
		}

		[Test]
		public void DirectEvent_ClassHandler_InstanceHandlers_Top()
		{
			TestClassHandler(_stackPanel, UIElement.MouseEnterEvent, MouseEventArgs.Create(new Mouse(), new Vector2i(), new InputState[] { }),
				_stackPanel);
		}

		[Test]
		public void DirectEvent_ClassHandlers_Bottom()
		{
			TestClassHandler(_textBlock, UIElement.MouseEnterEvent, MouseEventArgs.Create(new Mouse(), new Vector2i(), new InputState[] { }),
				_textBlock);
		}

		[Test]
		public void DirectEvent_InstanceHandlers_Bottom()
		{
			TestInstanceHandlers(_textBlock, UIElement.MouseEnterEvent, MouseEventArgs.Create(new Mouse(), new Vector2i(), new InputState[] { }),
				_textBlock);
		}

		[Test]
		public void DirectEvent_InstanceHandlers_Middle()
		{
			TestInstanceHandlers(_button, UIElement.MouseEnterEvent, MouseEventArgs.Create(new Mouse(), new Vector2i(), new InputState[] { }),
				_button);
		}

		[Test]
		public void DirectEvent_InstanceHandlers_Top()
		{
			TestInstanceHandlers(_stackPanel, UIElement.MouseEnterEvent, MouseEventArgs.Create(new Mouse(), new Vector2i(), new InputState[] { }),
				_stackPanel);
		}

		[Test]
		public void TunnelingEvent_ClassHandlers_AllLevels()
		{
			TestClassHandler(_textBlock, UIElement.PreviewKeyUpEvent, KeyEventArgs.Create(new Keyboard(), Key.A, 0, new InputState()),
				_stackPanel, _button, _textBlock);
		}

		[Test]
		public void TunnelingEvent_ClassHandlers_Handled()
		{
			_handlingElement = _stackPanel;
			TestClassHandler(_textBlock, UIElement.PreviewKeyUpEvent, KeyEventArgs.Create(new Keyboard(), Key.A, 0, new InputState()), _stackPanel);
		}

		[Test]
		public void TunnelingEvent_ClassHandlers_OneLevel()
		{
			TestClassHandler(_stackPanel, UIElement.PreviewKeyUpEvent, KeyEventArgs.Create(new Keyboard(), Key.A, 0, new InputState()),
				_stackPanel);
		}

		[Test]
		public void TunnelingEvent_ClassHandlers_TwoLevels()
		{
			TestClassHandler(_button, UIElement.PreviewKeyDownEvent, KeyEventArgs.Create(new Keyboard(), Key.A, 0, new InputState()),
				_stackPanel, _button);
		}

		[Test]
		public void TunnelingEvent_InstanceHandlers_AllLevels()
		{
			TestInstanceHandlers(_textBlock, UIElement.PreviewKeyUpEvent, KeyEventArgs.Create(new Keyboard(), Key.A, 0, new InputState()),
				_stackPanel, _button, _textBlock);
		}

		[Test]
		public void TunnelingEvent_InstanceHandlers_Handled()
		{
			_handlingElement = _stackPanel;
			TestInstanceHandlers(_textBlock, UIElement.PreviewKeyUpEvent, KeyEventArgs.Create(new Keyboard(), Key.A, 0, new InputState()),
				_stackPanel);
		}

		[Test]
		public void TunnelingEvent_InstanceHandlers_OneLevel()
		{
			TestInstanceHandlers(_stackPanel, UIElement.PreviewKeyUpEvent, KeyEventArgs.Create(new Keyboard(), Key.A, 0, new InputState()),
				_stackPanel);
		}

		[Test]
		public void TunnelingEvent_InstanceHandlers_TwoLevels()
		{
			TestInstanceHandlers(_button, UIElement.PreviewKeyDownEvent, KeyEventArgs.Create(new Keyboard(), Key.A, 0, new InputState()),
				_stackPanel, _button);
		}
	}
}