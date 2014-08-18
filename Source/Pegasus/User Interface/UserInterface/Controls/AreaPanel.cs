namespace Pegasus.Framework.UserInterface.Controls
{
	using System;
	using System.Windows;
	using System.Windows.Controls;

	public class AreaPanel : Panel
	{
		protected override Size MeasureOverride(Size availableSize)
		{
			var size = new Size(0, 0);
			foreach (UIElement child in Children)
			{
				child.Measure(availableSize);

				if (child.DesiredSize.Width > size.Width)
					size.Width = child.DesiredSize.Width;

				if (child.DesiredSize.Height > size.Height)
					size.Height = child.DesiredSize.Height;
			}

			return size;
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			foreach (UIElement child in Children)
				child.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));

			return finalSize;
		}
	}
}