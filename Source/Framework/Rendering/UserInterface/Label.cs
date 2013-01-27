using System;

namespace Pegasus.Framework.Rendering.UserInterface
{
	using Platform.Graphics;
	using Math;
	using Rendering;

    /// <summary>
    ///   Represents a text label.
    /// </summary>
    public sealed class Label
    {
        /// <summary>
        ///   The layout of the label's text.
        /// </summary>
        private TextLayout _layout;

        /// <summary>
        ///   The renderer for the label's text.
        /// </summary>
        private TextRenderer _textRenderer;

        /// <summary>
        ///   Initializes a new instance.
        /// </summary>
        /// <param name="font">The font that should be used for drawing the text.</param>
        public Label(Font font)
            : this(font, String.Empty)
        {
        }

        /// <summary>
        ///   Initializes a new instance.
        /// </summary>
        /// <param name="font">The font that should be used for drawing the text.</param>
        /// <param name="text">The text that should be displayed.</param>
        public Label(Font font, string text)
        {
            Assert.ArgumentNotNull(font, () => font);
            Assert.ArgumentNotNull(text, () => text);

            _layout = new TextLayout(font, text);
            _layout.LayoutChanged += () => _textRenderer.RebuildCache(Font, Text, _layout.LayoutData);

            Color = Color.White;

            // Use a large value for the default area; however, don't use Int32.MaxValue because of
            // integer arithmetic overflowing problems
            Area = new Rectangle(0, 0, Int16.MaxValue, Int16.MaxValue);
        }

        /// <summary>
        ///   Gets or sets the label's text.
        /// </summary>
        public string Text
        {
            get { return _layout.Text; }
            set { _layout.Text = value; }
        }

        /// <summary>
        ///   Gets or sets the label's area.
        /// </summary>
        public Rectangle Area
        {
            get { return _layout.DesiredArea; }
            set { _layout.DesiredArea = value; }
        }

        /// <summary>
        ///   Gets or sets the amount of spacing between lines.
        /// </summary>
        public int LineSpacing
        {
            get { return _layout.LineSpacing; }
            set { _layout.LineSpacing = value; }
        }

        /// <summary>
        ///   Gets or sets the font that is used to draw the label's text.
        /// </summary>
        public Font Font
        {
            get { return _layout.Font; }
            set { _layout.Font = value; }
        }

        /// <summary>
        ///   Gets or sets the text color.
        /// </summary>
        public Color Color
        {
            get { return _textRenderer.Color; }
            set { _textRenderer.Color = value; }
        }

        /// <summary>
        ///   Gets or sets the alignment of the label's text.
        /// </summary>
        public TextAlignment Alignment
        {
            get { return _layout.Alignment; }
            set { _layout.Alignment = value; }
        }

        /// <summary>
        ///   Gets the actual text rendering area. Usually, the actual area is smaller than the desired size.
        ///   If any words overlap, however, the actual area is bigger.
        /// </summary>
        public Rectangle ActualArea
        {
            get
            {
                // Ensure that the layout is up to date
                _layout.UpdateLayout();
                return _layout.ActualArea;
            }
        }

        /// <summary>
        ///   Draws the label using the given sprite batch.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch that should be used to draw the label.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            // Ensure that the layout is up to date
            _layout.UpdateLayout();

            _layout.DrawDebugVisualizations(spriteBatch);
            _textRenderer.DrawCached(spriteBatch, _layout.Font.Texture);
        }
    }
}