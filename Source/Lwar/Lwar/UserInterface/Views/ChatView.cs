namespace Lwar.UserInterface.Views
{
	using System;
	using Pegasus.Framework.UserInterface;
	using Pegasus.Framework.UserInterface.Input;
	using Pegasus.Math;

	partial class ChatView
	{
		/// <summary>
		///     Ensures that the keyboard focus is set to the chat message input text box whenever the view becomes visible.
		/// </summary>
		partial void OnLoaded()
		{
			AddChangedHandler(VisibilityProperty, OnVisibleChanged);
			Cursor.SetCursor(this, Cursor.Arrow);
		}

		/// <summary>
		///     Sets the focus to the message input when the chat view becomes visible.
		/// </summary>
		private void OnVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs<Visibility> e)
		{
			if (e.NewValue == Visibility.Visible)
				_messageInput.Focus();
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
			// The chat view above everything else.
			return true;
		}
	}
}