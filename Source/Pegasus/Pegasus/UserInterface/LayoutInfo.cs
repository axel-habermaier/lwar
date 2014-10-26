namespace Pegasus.UserInterface
{
	using System;
	using Utilities;

	/// <summary>
	///     Caches layouting information of an UI element.
	/// </summary>
	internal struct LayoutInfo
	{
		/// <summary>
		///     Initializes a new instance.
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
			HasExplicitWidth = !Single.IsNaN(Width);
			HasExplicitHeight = !Single.IsNaN(Height);
		}

		/// <summary>
		///     Gets a value indicating whether the UI element has an explicit width set.
		/// </summary>
		public bool HasExplicitWidth { get; private set; }

		/// <summary>
		///     Gets a value indicating whether the UI element has an explicit height set.
		/// </summary>
		public bool HasExplicitHeight { get; private set; }

		/// <summary>
		///     Gets the horizontal alignment of the UI element.
		/// </summary>
		public HorizontalAlignment HorizontalAlignment { get; private set; }

		/// <summary>
		///     Gets the vertical alignment of the UI element.
		/// </summary>
		public VerticalAlignment VerticalAlignment { get; private set; }

		/// <summary>
		///     Gets the width of the UI element.
		/// </summary>
		public float Width { get; private set; }

		/// <summary>
		///     Gets the minimum width of the UI element.
		/// </summary>
		public float MinWidth { get; private set; }

		/// <summary>
		///     Gets the maximum width of the UI element.
		/// </summary>
		public float MaxWidth { get; private set; }

		/// <summary>
		///     Gets the height of the UI element.
		/// </summary>
		public float Height { get; private set; }

		/// <summary>
		///     Gets the minimum height of the UI element.
		/// </summary>
		public float MinHeight { get; private set; }

		/// <summary>
		///     Gets the maximum height of the UI element.
		/// </summary>
		public float MaxHeight { get; private set; }

		/// <summary>
		///     Gets the margin of the UI element.
		/// </summary>
		public Thickness Margin { get; private set; }
	}
}