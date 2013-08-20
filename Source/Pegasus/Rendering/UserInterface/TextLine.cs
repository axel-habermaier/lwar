using System;

namespace Pegasus.Rendering.UserInterface
{
	using Math;

	/// <summary>
	///   Represents a line of text.
	/// </summary>
	internal struct TextLine
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="left">The position of the line's left edge.</param>
		/// <param name="top">The position of the line's top edge.</param>
		/// <param name="lineHeight">The height of the line.</param>
		public TextLine(int left, int top, int lineHeight)
			: this()
		{
			Area = new Rectangle(left, top, 0, lineHeight);
			FirstCharacter = -1;
			LastCharacter = -1;
		}

		/// <summary>
		///   Gets the area occupied by the text line.
		/// </summary>
		public Rectangle Area { get; private set; }

		/// <summary>
		///   Gets the index of the first character of the text line. A value of -1 indicates that the index has not yet
		///   been determined.
		/// </summary>
		public int FirstCharacter { get; private set; }

		/// <summary>
		///   Gets the index of the last character of the text line. A value of -1 indicates that the index has not yet
		///   been determined.
		/// </summary>
		public int LastCharacter { get; private set; }

		/// <summary>
		///   Gets a value indicating whether the line is invalid.
		/// </summary>
		public bool IsInvalid
		{
			get { return FirstCharacter == -1 || LastCharacter == -1 || FirstCharacter >= LastCharacter; }
		}

		/// <summary>
		///   Appends the given sequence to the line.
		/// </summary>
		/// <param name="sequence">The sequence that should be appended.</param>
		/// <param name="width">The new width of the line.</param>
		public void Append(TextSequence sequence, int width)
		{
			if (FirstCharacter == -1)
				FirstCharacter = sequence.FirstCharacter;

			LastCharacter = sequence.LastCharacter;
			Area = new Rectangle(Area.Left, Area.Top, width, Area.Height);
		}

		/// <summary>
		///   Adds the given offsets to the position of the line.
		/// </summary>
		/// <param name="offset">The offset that should be applied to the line's position.</param>
		public void Offset(Vector2i offset)
		{
			Area = Area.Offset(offset);
		}
	}
}