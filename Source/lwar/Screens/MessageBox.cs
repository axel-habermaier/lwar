namespace Lwar.Screens
{
	using System;
	using Assets;
	using Pegasus;
	using Pegasus.Math;
	using Pegasus.Platform.Graphics;
	using Pegasus.Platform.Input;
	using Pegasus.Platform.Logging;
	using Pegasus.Platform.Memory;
	using Pegasus.Rendering;
	using Pegasus.Rendering.UserInterface;

	/// <summary>
	///     Displays a message to the user.
	/// </summary>
	public class MessageBox : Screen
	{
		/// <summary>
		///     The input that, when triggered, closes the message box.
		/// </summary>
		private readonly LogicalInput _continue;

		/// <summary>
		///     The frame defining the out edges of the message box.
		/// </summary>
		private readonly Frame _frame = new Frame { FrameColor = new Color(0xFF333333), Margin = 10 };

		/// <summary>
		///     The message that is displayed to the user.
		/// </summary>
		private readonly string _message;

		/// <summary>
		///     The label that draws the message to the screen.
		/// </summary>
		private Label _messageLabel;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="entry">The entry that should be displayed to the user.</param>
		private MessageBox(LogEntry entry)
		{
			_message = entry.Message;
			_continue = new LogicalInput(Key.Space.WentDown(), InputLayers.Game);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			InputDevice.Remove(_continue);
			_messageLabel.SafeDispose();
		}

		/// <summary>
		///     Initializes the screen.
		/// </summary>
		public override void Initialize()
		{
			_messageLabel = new Label(Assets.Load(Fonts.LiberationMono11))
			{
				Text = _message + "\n\nPress [Space] to continue...",
				Alignment = TextAlignment.Middle | TextAlignment.Centered
			};

			InputDevice.Add(_continue);
		}

		/// <summary>
		///     Updates the screen.
		/// </summary>
		/// <param name="topmost">Indicates whether the app screen is the topmost one.</param>
		public override void Update(bool topmost)
		{
			_messageLabel.Area = new Rectangle(0, 0, (int)Window.ActualWidth, (int)Window.ActualHeight);
			_frame.ContentArea = _messageLabel.ActualArea;

			if (topmost && _continue.IsTriggered)
				ScreenManager.Remove(this);
		}

		/// <summary>
		///     Draws the user interface elements of the app screen.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the user interface.</param>
		public override void DrawUserInterface(SpriteBatch spriteBatch)
		{
			_frame.Draw(spriteBatch);
			_messageLabel.Draw(spriteBatch);
		}

		/// <summary>
		///     Shows a message box with the given message, optionally removing the current state from the state manager.
		/// </summary>
		/// <param name="screen">The screen that should be considered to be the parent of the message box.</param>
		/// <param name="type">The type of the message that should be shown.</param>
		/// <param name="message">The message that should be displayed by the message box.</param>
		/// <param name="removeState">Indicates whether the current screen should be removed from the state manager.</param>
		public static void Show(Screen screen, LogType type, string message, bool removeState = false)
		{
			Assert.ArgumentNotNull(screen);
			Assert.InRange(type);
			Assert.ArgumentNotNullOrWhitespace(message);

			var entry = new LogEntry(type, message);
			entry.RaiseLogEvent();

			screen.ScreenManager.Add(new MessageBox(entry));

			if (removeState)
				screen.ScreenManager.Remove(screen);
		}
	}
}