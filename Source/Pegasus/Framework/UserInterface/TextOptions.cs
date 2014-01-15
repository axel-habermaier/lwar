namespace Pegasus.Framework.UserInterface
{
	using System;

	/// <summary>
	///     Defines attached properties that affect the way text is rendered.
	/// </summary>
	public static class TextOptions
	{
		/// <summary>
		///     The text rendering mode of an UI element.
		/// </summary>
		public static readonly DependencyProperty<TextRenderingMode> TextRenderingModeProperty =
			new DependencyProperty<TextRenderingMode>(defaultValue: TextRenderingMode.ClearType, affectsRender: true, inherits: true);

		/// <summary>
		///     Gets the text rendering mode of the given UI element.
		/// </summary>
		/// <param name="element">The element whose the text rendering mode property should be returned.</param>
		public static TextRenderingMode GetTextRenderingMode(UIElement element)
		{
			Assert.ArgumentNotNull(element);
			return element.GetValue(TextRenderingModeProperty);
		}

		/// <summary>
		///     Sets the text rendering mode of the given UI element.
		/// </summary>
		/// <param name="element">The element whose the text rendering mode property should be set.</param>
		/// <param name="mode">The text rendering mode that should be set.</param>
		public static void SetTextRenderingMode(UIElement element, TextRenderingMode mode)
		{
			Assert.ArgumentNotNull(element);
			Assert.ArgumentInRange(mode);

			element.SetValue(TextRenderingModeProperty, mode);
		}
	}
}