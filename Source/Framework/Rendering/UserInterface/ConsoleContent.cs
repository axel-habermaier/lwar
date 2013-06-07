using System;

namespace Pegasus.Framework.Rendering.UserInterface
{
	using Math;
	using Platform.Graphics;
	using Platform.Memory;

	/// <summary>
	///   The displayed content of the console.
	/// </summary>
	internal struct ConsoleContent
	{
		/// <summary>
		///   The maximum number of labels that the console can display. If all labels are used and another label is
		///   added, the oldest labels is removed.
		/// </summary>
		private const int MaxLabels = 2048;

		/// <summary>
		///   The spacing between the individual lines.
		/// </summary>
		private const int LineSpacing = 2;

		/// <summary>
		///   Number of lines to scroll with each scroll command.
		/// </summary>
		private const int ScrollSpeed = 10;

		/// <summary>
		///   The labels that the console displays.
		/// </summary>
		private readonly Label[] _labels;

		/// <summary>
		///   The line height of the font that is used to draw the content.
		/// </summary>
		private readonly int _lineHeight;

		/// <summary>
		///   The area of the rows.
		/// </summary>
		private Rectangle _area;

		/// <summary>
		///   The number of rows that are currently displayed.
		/// </summary>
		private int _numLabels;

		/// <summary>
		///   The current scroll offset (in pixels), used to scroll through the rows.
		/// </summary>
		private int _scrollOffset;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="font">The font that should be used to draw the content.</param>
		public ConsoleContent(Font font)
			: this()
		{
			Assert.ArgumentNotNull(font);

			_lineHeight = font.LineHeight;
			_labels = new Label[MaxLabels];

			for (var i = 0; i < MaxLabels; ++i)
				_labels[i] = new Label(font, String.Empty) { LineSpacing = LineSpacing };
		}

		/// <summary>
		/// Disposes the console content.
		/// </summary>
		public void Dispose()
		{
			_labels.SafeDisposeAll();
		}

		/// <summary>
		///   Gets the total height of all labels.
		/// </summary>
		private int TotalLabelHeight
		{
			get
			{
				if (_numLabels == 0)
					return 0;

				// Return the delta between the oldest label's top value and the newest label's bottom
				// value. When the top label is overwritten after a buffer overflow, its top value won't be
				// set to 0 in order to avoid repositioning all lines.
				return _labels[_numLabels - 1].ActualArea.Bottom - _labels[0].ActualArea.Top;
			}
		}

		/// <summary>
		///   Draws the content.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used for drawing.</param>
		public void Draw(SpriteBatch spriteBatch)
		{
			if (_numLabels == 0)
				return;

			spriteBatch.UseScissorTest = true;
			spriteBatch.ScissorArea = _area;

			// We draw the labels aligned to the bottom of the content area. Therefore, we apply an offset
			// to all label positions using the sprite batch's world matrix (this is a lot faster than 
			// really changing the positions of all labels).
			// We start by defining the content area's lower edge as the origin, subtracting the oldest label's
			// top value. Consequently, the oldest label's top edge corresponds to the content area's bottom edge.
			// We then move up the origin by the total label height, such that the newest label's lower edge 
			// is perfectly aligned with the content area's lower edge. And last, we add the current scroll offset.
			var offsetY = _area.Bottom - _labels[0].ActualArea.Top - TotalLabelHeight + _scrollOffset;
			spriteBatch.WorldMatrix = Matrix.CreateTranslation(_area.Left, offsetY, 0);

			// Draw the labels, but only those that are at least partially visible
			for (var i = 0; i < _numLabels; ++i)
			{
				var topIsInside = _labels[i].ActualArea.Top + offsetY < _area.Bottom;
				var bottomIsInside = _labels[i].ActualArea.Bottom + offsetY > _area.Top;

				if (topIsInside && bottomIsInside)
					_labels[i].Draw(spriteBatch);
			}
		}

		/// <summary>
		///   Resizes the content's area.
		/// </summary>
		/// <param name="area">The new content area.</param>
		public void Resize(Rectangle area)
		{
			_area = area;

			// Ensure that all labels have the correct width
			foreach (var label in _labels)
				label.Area = new Rectangle(0, 0, _area.Width, 0);

			// The width of the labels might have caused a label to be split over more or less lines,
			// so the height might have changed. We therefore have to reposition all labels.
			UpdateLabelPositions();
		}

		/// <summary>
		///   Adds a the given string to the console's content.
		/// </summary>
		/// <param name="message">The message that should be displayed.</param>
		/// <param name="color">The color that should be used to display the message.</param>
		public void Add(string message, Color color)
		{
			// If all labels are used, remove the oldest one by shifting the entire array up one
			// index and add the oldest label to the end of the array (in order to re-use the label instance for 
			// the new message); this is not terribly efficient, however, it's good enough for now because it 
			// only copies MaxLabels * ReferenceSize bytes instead of relayouting all lines.
			// After the copy operations, the oldest label's y value will have increased. We have to take that
			// into account when drawing and scrolling. However, this is a lot faster than repositioning all labels
			// after the copy operations.
			if (_numLabels >= MaxLabels)
			{
				var oldest = _labels[0];
				Array.Copy(_labels, 1, _labels, 0, MaxLabels - 1);
				--_numLabels;
				_labels[_numLabels] = oldest;
			}

			_labels[_numLabels].Text = message;
			_labels[_numLabels].Color = color;

			// Position the label right below the previous one
			var area = new Rectangle(0, 0, _area.Width, 0);
			if (_numLabels != 0)
				area = area.Offset(0, _labels[_numLabels - 1].ActualArea.Bottom + LineSpacing);

			_labels[_numLabels].Area = area;
			++_numLabels;
		}

		/// <summary>
		///   Clears all content.
		/// </summary>
		public void Clear()
		{
			_numLabels = 0;
			_scrollOffset = 0;
		}

		/// <summary>
		///   Scrolls up a couple of lines.
		/// </summary>
		public void ScrollUp()
		{
			Scroll(ScrollSpeed);
		}

		/// <summary>
		///   Scrolls up a couple of lines.
		/// </summary>
		public void ScrollDown()
		{
			Scroll(-ScrollSpeed);
		}

		/// <summary>
		///   Scrolls the content by the given number of lines.
		/// </summary>
		/// <param name="scrollLines">The number of lines to scroll.</param>
		private void Scroll(int scrollLines)
		{
			if (_numLabels == 0)
				return;

			var totalLineHeight = _lineHeight + LineSpacing;
			var deltaInPixels = scrollLines * totalLineHeight;
			_scrollOffset += deltaInPixels;

			// Do not scroll down any further if we've already reached the newest line
			if (_scrollOffset + deltaInPixels < 0)
				ScrollToBottom();

			// Do not scroll up any further if we would not even see the oldest line
			if (_scrollOffset >= TotalLabelHeight)
				ScrollToTop();
		}

		/// <summary>
		///   Scrolls to the top of the row area.
		/// </summary>
		public void ScrollToTop()
		{
			if (_numLabels == 0)
				return;

			_scrollOffset = TotalLabelHeight - _lineHeight;
		}

		/// <summary>
		///   Scrolls to the bottom of the row area.
		/// </summary>
		public void ScrollToBottom()
		{
			_scrollOffset = 0;
		}

		/// <summary>
		///   Updates the positions of all labels.
		/// </summary>
		private void UpdateLabelPositions()
		{
			var offsetY = 0;
			for (var i = 0; i < _numLabels; ++i)
			{
				_labels[i].Area = new Rectangle(0, offsetY, _area.Width, 0);
				offsetY += _labels[i].ActualArea.Height + LineSpacing;
			}

			// Ensure that at least the oldest line is visible; that might not be the case after a resize if the
			// width increased and some labels are split over fewer lines
			Scroll(0);
		}
	}
}