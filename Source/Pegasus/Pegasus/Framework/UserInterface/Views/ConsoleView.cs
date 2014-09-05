namespace Pegasus.Framework.UserInterface.Views
{
	using System;

	partial class ConsoleView
	{
		/// <summary>
		///     Indicates whether the mouse was captured before the console was shown.
		/// </summary>
		private bool _mouseCaptured;

		/// <summary>
		///     Invoked once the UI element and all of its children have been fully loaded.
		/// </summary>
		partial void OnLoaded()
		{
			AddChangedHandler(ActualHeightProperty, OnHeightChanged);
			AddChangedHandler(VisibilityProperty, OnVisibleChanged);
			_layoutRoot.Height = 0; // Reduces flickering when the console is opened for the first time
		}

		/// <summary>
		///     Sets the height of the console's layout root to half of the height of the console itself.
		/// </summary>
		private void OnHeightChanged(DependencyObject obj, DependencyPropertyChangedEventArgs<double> args)
		{
			_layoutRoot.Height = args.NewValue / 2;
		}

		/// <summary>
		///     Sets the focus to the console input when the console becomes visible.
		/// </summary>
		private void OnVisibleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs<Visibility> args)
		{
			if (args.NewValue == Visibility.Visible)
			{
				_mouseCaptured = ParentWindow.MouseCaptured;
				ParentWindow.MouseCaptured = false;
			}
			else
				ParentWindow.MouseCaptured = _mouseCaptured;
		}
	}
}