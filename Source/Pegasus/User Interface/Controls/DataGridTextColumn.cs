namespace Pegasus.UserInterface.Controls
{
	using System;
	using System.Windows;
	using System.Windows.Controls;

	public class DataGridTextColumn : DataGridColumn
	{
		public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.Register(
			"PropertyName", typeof(string), typeof(DataGridTextColumn), new PropertyMetadata(default(string)));

		public string PropertyName
		{
			get { return (string)GetValue(PropertyNameProperty); }
			set { SetValue(PropertyNameProperty, value); }
		}

		protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
		{
			return null;
		}

		protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
		{
			return null;
		}
	}
}