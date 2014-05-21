namespace Pegasus.Framework.UserInterface
{
	using System;

	namespace Converters
	{
		using System.Globalization;
		using System.Windows.Data;

		public abstract class ValueConverter : IValueConverter
		{
			public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
			{
				return value;
			}

			public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			{
				return value;
			}
		}

		public class BooleanToStringConverter : ValueConverter
		{
		}

		public class ByteToStringConverter : ValueConverter
		{
		}

		public class SByteToStringConverter : ValueConverter
		{
		}

		public class CharToStringConverter : ValueConverter
		{
		}

		public class Int16ToStringConverter : ValueConverter
		{
		}

		public class UInt16ToStringConverter : ValueConverter
		{
		}

		public class Int32ToStringConverter : ValueConverter
		{
		}

		public class UInt32ToStringConverter : ValueConverter
		{
		}

		public class Int64ToStringConverter : ValueConverter
		{
		}

		public class UInt64ToStringConverter : ValueConverter
		{
		}

		public class SingleToStringConverter : ValueConverter
		{
			public string Format { get; set; }
		}

		public class DoubleToStringConverter : ValueConverter
		{
			public string Format { get; set; }
		}

		public class BooleanToVisibilityConverter : ValueConverter
		{
		}
	}
}