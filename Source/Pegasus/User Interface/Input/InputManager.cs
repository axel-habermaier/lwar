namespace Pegasus.UserInterface.Input
{
	using System;
	using System.Windows;

	public static class InputManager
	{
		public static readonly DependencyProperty CapturesInputProperty = DependencyProperty.RegisterAttached(
			"CapturesInput", typeof(bool), typeof(InputManager), new PropertyMetadata(default(bool)));

		public static readonly DependencyProperty AutoFocusProperty = DependencyProperty.RegisterAttached(
			"AutoFocus", typeof(bool), typeof(InputManager), new PropertyMetadata(default(bool)));

		public static void SetCapturesInput(DependencyObject element, bool value)
		{
			element.SetValue(CapturesInputProperty, value);
		}

		public static bool GetCapturesInput(DependencyObject element)
		{
			return (bool)element.GetValue(CapturesInputProperty);
		}

		public static void SetAutoFocus(DependencyObject element, bool value)
		{
			element.SetValue(AutoFocusProperty, value);
		}

		public static bool GetAutoFocus(DependencyObject element)
		{
			return (bool)element.GetValue(AutoFocusProperty);
		}
	}
}