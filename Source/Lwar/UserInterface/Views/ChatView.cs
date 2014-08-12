namespace Lwar.UserInterface.Views
{
	using System;
	using Pegasus.Framework.UserInterface;

	partial class ChatView
	{
		/// <summary>
		///     Ensures that the keyboard focus is set to the chat message input text box whenever the view becomes visible.
		/// </summary>
		partial void OnLoaded()
		{
			AddChangedHandler(VisibilityProperty, OnVisibleChanged);
		}

		/// <summary>
		///     Sets the focus to the message input when the chat view becomes visible.
		/// </summary>
		private void OnVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs<Visibility> e)
		{
			if (e.NewValue == Visibility.Visible)
				_messageInput.Focus();
		}
	}
}