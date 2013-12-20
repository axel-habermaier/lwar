namespace Pegasus.Framework.UserInterface
{
	using System;

	/// <summary>
	///   Caches layouting information of an UI element.
	/// </summary>
	internal struct LayoutInfo
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="element">The UI element the layout info should be cached for.</param>
		public LayoutInfo(UIElement element)
			: this()
		{
			Assert.ArgumentNotNull(element);

			HorizontalAlignment = element.HorizontalAlignment;
			VerticalAlignment = element.VerticalAlignment;
			Width = element.Width;
			Height = element.Height;
			MinWidth = element.MinWidth;
			MinHeight = element.MinHeight;
			MaxWidth = element.MaxWidth;
			MaxHeight = element.MaxHeight;
			Margin = element.Margin;
		}

		/// <summary>
		///   Gets the horizontal alignment of the UI element.
		/// </summary>
		public HorizontalAlignment HorizontalAlignment { get; private set; }

		/// <summary>
		///   Gets the vertical alignment of the UI element.
		/// </summary>
		public VerticalAlignment VerticalAlignment { get; private set; }

		/// <summary>
		///   Gets the width of the UI element.
		/// </summary>
		public double Width { get; private set; }

		/// <summary>
		///   Gets the minimum width of the UI element.
		/// </summary>
		public double MinWidth { get; private set; }

		/// <summary>
		///   Gets the maximum width of the UI element.
		/// </summary>
		public double MaxWidth { get; private set; }

		/// <summary>
		///   Gets the height of the UI element.
		/// </summary>
		public double Height { get; private set; }

		/// <summary>
		///   Gets the minimum height of the UI element.
		/// </summary>
		public double MinHeight { get; private set; }

		/// <summary>
		///   Gets the maximum height of the UI element.
		/// </summary>
		public double MaxHeight { get; private set; }

		/// <summary>
		///   Gets the margin of the UI element.
		/// </summary>
		public Thickness Margin { get; private set; }
	}
}