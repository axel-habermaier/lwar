﻿namespace Pegasus.Framework.UserInterface.Views
{
	using System;
	using Input;
	using Math;
	using Platform;

	partial class ConsoleView
	{
		/// <summary>
		///     Invoked once the UI element and all of its children have been fully loaded.
		/// </summary>
		partial void OnLoaded()
		{
			AddChangedHandler(ActualHeightProperty, OnHeightChanged);
			AddChangedHandler(VisibilityProperty, OnVisibleChanged);
			InputBindings.Add(new ScanCodeBinding(PlatformInfo.ConsoleKey, "Hide", triggerOnRepeat: false));
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
				_prompt.Focus();
		}

		/// <summary>
		///     Performs a detailed hit test for the given position. The position is guaranteed to lie within the UI element's
		///     bounds. This method should be overridden to implement special hit testing logic that is more precise than a
		///     simple bounding box check.
		/// </summary>
		/// <param name="position">The position that should be checked for a hit.</param>
		/// <returns>Returns true if the UI element is hit; false, otherwise.</returns>
		protected override bool HitTestCore(Vector2d position)
		{
			// The console lies above everything else.
			return true;
		}
	}
}