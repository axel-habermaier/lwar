using System;

namespace Lwar.Screens
{
	using Gameplay;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Platform.Memory;
	using Pegasus.Framework.Rendering;
	using Pegasus.Framework.Rendering.UserInterface;

	/// <summary>
	///   Displays event messages on the screen.
	/// </summary>
	public class EventMessageDisplay : DisposableObject
	{
		/// <summary>
		///   The margin between the event messages and the screen.
		/// </summary>
		private const int Margin = 10;

		/// <summary>
		///   The spacing between two lines.
		/// </summary>
		private const int LineSpacing = 4;

		/// <summary>
		///   The messages that are displayed on the screen.
		/// </summary>
		private readonly Label[] _messages = new Label[EventMessageList.Capacity];

		/// <summary>
		///   The number of messages that are displayed on the screen.
		/// </summary>
		private int _count;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="assets">The assets manager that should be used to load required assets.</param>
		public EventMessageDisplay(AssetsManager assets)
		{
			Assert.ArgumentNotNull(assets);

			var font = assets.LoadFont("Fonts/Liberation Mono 12");
			for (var i = 0; i < _messages.Length; ++i)
				_messages[i] = new Label(font) { LineSpacing = LineSpacing };
		}

		/// <summary>
		///   Updates the event message display layout.
		/// </summary>
		/// <param name="messages">The event messages that should be displayed.</param>
		/// <param name="size">The size of the display area.</param>
		public void Update(EventMessageList messages, Size size)
		{
			Assert.ArgumentNotNull(messages);

			_count = messages.Count;
			var offset = new Vector2i(Margin, Margin);

			for (var i = 0; i < _count; ++i)
			{
				_messages[i].Text = messages[i].DisplayString;
				_messages[i].Area = new Rectangle(offset, size.Width - 2 * Margin, 0);

				offset.Y = _messages[i].ActualArea.Bottom;
			}
		}

		/// <summary>
		///   Draws the active event messages.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used for drawing.</param>
		public void Draw(SpriteBatch spriteBatch)
		{
			Assert.ArgumentNotNull(spriteBatch);

			for (var i = 0; i < _count; ++i)
				_messages[i].Draw(spriteBatch);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_messages.SafeDisposeAll();
		}
	}
}