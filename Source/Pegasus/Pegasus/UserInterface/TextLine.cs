namespace Pegasus.UserInterface
{
	using System;

	/// <summary>
	///     Represents a line of text.
	/// </summary>
	internal struct TextLine
	{
		/// <summary>
		///     Gets the with of the text line.
		/// </summary>
		public float Width { get; private set; }

		/// <summary>
		///     Gets the index of the first character of the text line. A value of -1 indicates that the index has not yet
		///     been determined.
		/// </summary>
		public int FirstCharacter { get; private set; }

		/// <summary>
		///     Gets the index of the last character of the text line. A value of -1 indicates that the index has not yet
		///     been determined.
		/// </summary>
		public int LastCharacter { get; private set; }

		/// <summary>
		///     Gets a value indicating whether the line is invalid.
		/// </summary>
		public bool IsInvalid
		{
			get { return FirstCharacter == -1 || LastCharacter == -1 || FirstCharacter >= LastCharacter; }
		}

		/// <summary>
		///     Creates a new instance.
		/// </summary>
		public static TextLine Create()
		{
			return new TextLine
			{
				FirstCharacter = -1,
				LastCharacter = -1
			};
		}

		/// <summary>
		///     Appends the given sequence to the line.
		/// </summary>
		/// <param name="sequence">The sequence that should be appended.</param>
		/// <param name="width">The new width of the line.</param>
		public void Append(TextSequence sequence, float width)
		{
			if (FirstCharacter == -1)
				FirstCharacter = sequence.FirstCharacter;

			LastCharacter = sequence.LastCharacter;
			Width = width;
		}
	}
}