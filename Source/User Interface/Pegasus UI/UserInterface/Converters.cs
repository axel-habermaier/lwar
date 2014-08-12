namespace Pegasus.Framework.UserInterface
{
	using System;

	namespace Converters
	{
		using System.Globalization;
		using System.Windows.Data;
		using System.Windows.Markup;

		public abstract class ValueConverter<T> : MarkupExtension, IValueConverter
			where T : IValueConverter, new()
		{
			public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
			{
				return value;
			}

			public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			{
				return value;
			}

			public override object ProvideValue(IServiceProvider serviceProvider)
			{
				return new T();
			}
		}

		public class LogTypeToColorConverter : ValueConverter<LogTypeToColorConverter>
		{
		}

		public class FrameTimeToStringConverter : ValueConverter<FrameTimeToStringConverter>
		{
		}

		public class BooleanToStringConverter : ValueConverter<BooleanToStringConverter>
		{
		}

		public class ByteToStringConverter : ValueConverter<ByteToStringConverter>
		{
		}

		public class SByteToStringConverter : ValueConverter<SByteToStringConverter>
		{
		}

		public class CharToStringConverter : ValueConverter<CharToStringConverter>
		{
		}

		public class Int16ToStringConverter : ValueConverter<Int16ToStringConverter>
		{
		}

		public class UInt16ToStringConverter : ValueConverter<UInt16ToStringConverter>
		{
		}

		public class Int32ToStringConverter : ValueConverter<Int32ToStringConverter>
		{
		}

		public class UInt32ToStringConverter : ValueConverter<UInt32ToStringConverter>
		{
		}

		public class Int64ToStringConverter : ValueConverter<Int64ToStringConverter>
		{
		}

		public class UInt64ToStringConverter : ValueConverter<UInt64ToStringConverter>
		{
		}

		public class SingleToStringConverter : ValueConverter<SingleToStringConverter>
		{
			public string Format { get; set; }
		}

		public class DoubleToStringConverter : ValueConverter<DoubleToStringConverter>
		{
			public string Format { get; set; }
		}

		public class BooleanToVisibilityConverter : ValueConverter<BooleanToVisibilityConverter>
		{
		}
	}
}