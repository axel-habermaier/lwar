namespace Pegasus.UserInterface.Input
{
	using System;
	using System.ComponentModel;
	using System.Globalization;
	using System.Windows;
	using System.Windows.Media;

	[TypeConverter(typeof(CursorTypeConverter))]
	public class Cursor
	{
		public static readonly DependencyProperty CursorProperty = DependencyProperty.RegisterAttached(
			"Cursor", typeof(Cursor), typeof(Cursor), new PropertyMetadata(default(Cursor)));

		public Point HotSpot { get; set; }
		public string Texture { get; set; }
		public Color Color { get; set; }

		public static void SetCursor(DependencyObject element, Cursor value)
		{
			element.SetValue(CursorProperty, value);
		}

		public static Cursor GetCursor(DependencyObject element)
		{
			return (Cursor)element.GetValue(CursorProperty);
		}

		private class CursorTypeConverter : TypeConverter
		{
			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
			{
				return true;
			}

			public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
			{
				return new Cursor();
			}
		}
	}
}