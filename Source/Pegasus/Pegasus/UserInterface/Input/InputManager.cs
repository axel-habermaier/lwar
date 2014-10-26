namespace Pegasus.UserInterface.Input
{
	using System;
	using Utilities;

	/// <summary>
	///     Provides means to automatically focus an UI element whenever it becomes visible and allows
	///     UI elements to easily capture all input.
	/// </summary>
	public static class InputManager
	{
		/// <summary>
		///     Indicates whether an UI element captures all keyboard and mouse input.
		/// </summary>
		public static readonly DependencyProperty<bool> CapturesInputProperty = new DependencyProperty<bool>();

		/// <summary>
		///     Indicates whether an UI element should automatically receive the keyboard focus when it becomes visible.
		/// </summary>
		public static readonly DependencyProperty<bool> AutoFocusProperty = new DependencyProperty<bool>();

		/// <summary>
		///     Initializes the type.
		/// </summary>
		static InputManager()
		{
			CapturesInputProperty.Changed += OnCapturesInputChanged;
			AutoFocusProperty.Changed += OnAutoFocusChanged;

			UIElement.KeyUpEvent.Raised += HandleInputEvent;
			UIElement.KeyDownEvent.Raised += HandleInputEvent;
			UIElement.MouseDownEvent.Raised += HandleInputEvent;
			UIElement.MouseUpEvent.Raised += HandleInputEvent;
			UIElement.MouseWheelEvent.Raised += HandleInputEvent;
			UIElement.MouseEnterEvent.Raised += HandleInputEvent;
			UIElement.MouseLeaveEvent.Raised += HandleInputEvent;
			UIElement.MouseMoveEvent.Raised += HandleInputEvent;
		}

		/// <summary>
		///     Marks the input event as handled.
		/// </summary>
		private static void HandleInputEvent(object sender, RoutedEventArgs e)
		{
			var element = sender as UIElement;
			if (element != null && GetCapturesInput(element))
				e.Handled = true;
		}

		/// <summary>
		///     Ensures that an UI element that should capture all input is hit test visible and focusable and ensures that the UI
		///     element is focused whenever it becomes visible.
		/// </summary>
		private static void OnCapturesInputChanged(DependencyObject obj, DependencyPropertyChangedEventArgs<bool> args)
		{
			var element = obj as UIElement;
			if (element == null)
				return;

			if (args.NewValue)
				element.IsVisibleChanged += OnIsVisibleChanged;
			else
				element.IsVisibleChanged -= OnIsVisibleChanged;

			// We don't reset these properties when the UI element should no longer capture all input. This is
			// wrong, technically, but on the other hand this is a very unlikely situation.
			element.Focusable = true;
			element.IsHitTestVisible = true;
		}

		/// <summary>
		///     Ensures that an auto focused UI element receives the focus whenever it becomes visible.
		/// </summary>
		private static void OnAutoFocusChanged(DependencyObject obj, DependencyPropertyChangedEventArgs<bool> args)
		{
			var element = obj as UIElement;
			if (element == null)
				return;

			if (args.NewValue)
				element.IsVisibleChanged += OnIsVisibleChanged;
			else
				element.IsVisibleChanged -= OnIsVisibleChanged;

			// We don't reset the Focusable properties when the UI element should no longer be auto focused. This is
			// wrong, technically, but on the other hand this is a very unlikely situation.
			element.Focusable = true;
		}

		/// <summary>
		///     Focuses the auto focused UI element when it becomes visible.
		/// </summary>
		private static void OnIsVisibleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs<bool> args)
		{
			var element = obj as UIElement;
			if (element == null || !args.NewValue)
				return;

			Assert.NotNull(element.ParentWindow, "Visible element has no parent window.");
			element.ParentWindow.Keyboard.FocusedElement = element;
		}

		/// <summary>
		///     Gets a value indicating whether the given UI element captures all keyboard and mouse input.
		/// </summary>
		/// <param name="element">The UI element that should be checked.</param>
		public static bool GetCapturesInput(UIElement element)
		{
			Assert.ArgumentNotNull(element);
			return element.GetValue(CapturesInputProperty);
		}

		/// <summary>
		///     Sets a value indicating whether the given UI element should capture all keyboard and mouse input.
		/// </summary>
		/// <param name="element">The UI element that should capture all input.</param>
		/// <param name="capturesInput">A value indicating whether all input should be captured.</param>
		public static void SetCapturesInput(UIElement element, bool capturesInput)
		{
			Assert.ArgumentNotNull(element);
			element.SetValue(CapturesInputProperty, capturesInput);
		}

		/// <summary>
		///     Gets a value indicating whether the UI element automatically receives the keyboard focus when it becomes visible.
		/// </summary>
		/// <param name="element">The UI element the auto focus state should be returned for.</param>
		public static bool GetAutoFocus(UIElement element)
		{
			Assert.ArgumentNotNull(element);
			return element.GetValue(AutoFocusProperty);
		}

		/// <summary>
		///     Sets a value indicating whether the UI element should automatically receive the keyboard focus when it becomes visible.
		/// </summary>
		/// <param name="element">The UI element the auto focus state should be set for.</param>
		/// <param name="autoFocus">
		///     A value indicating whether the UI element should automatically receive the keyboard focus when it becomes visible.
		/// </param>
		public static void SetAutoFocus(UIElement element, bool autoFocus)
		{
			Assert.ArgumentNotNull(element);
			element.SetValue(AutoFocusProperty, autoFocus);
		}
	}
}