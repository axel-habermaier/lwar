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
			AddChangedHandler(VisibilityProperty, (o, e) =>
			{
				if (e.NewValue == Visibility.Visible)
					_messageInput.Focus();
			});
		}
	}
}