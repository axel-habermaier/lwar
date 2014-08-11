namespace Pegasus.Framework.UserInterface.Input
{
	using System;
	using System.Windows;

	public static class Keyboard
	{
		public static readonly DependencyProperty HandlesAllInputProperty = DependencyProperty.RegisterAttached(
			"HandlesAllInput", typeof(bool), typeof(Keyboard));

		public static void SetHandlesAllInput(UIElement element, bool value)
		{
			element.SetValue(HandlesAllInputProperty, value);
		}

		public static bool GetHandlesAllInput(UIElement element)
		{
			return (bool)element.GetValue(HandlesAllInputProperty);
		}
	}
}