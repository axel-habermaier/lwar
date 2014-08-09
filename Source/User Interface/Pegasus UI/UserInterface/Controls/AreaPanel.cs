namespace Pegasus.Framework.UserInterface.Controls
{
	using System;
	using System.Windows;
	using System.Windows.Controls;

	public class AreaPanel : Panel
	{

		protected override Size MeasureOverride(Size availableSize)
		{
			foreach (UIElement child in Children)
				child.Measure(availableSize);

			return availableSize;
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			foreach (UIElement child in Children)
				child.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));

			return finalSize;
		}
	}
}