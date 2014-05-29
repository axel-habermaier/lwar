namespace Tests.UserInterface
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using FluentAssertions;
	using NUnit.Framework;
	using Pegasus.Framework.UserInterface;
	using Pegasus.Framework.UserInterface.Controls;

	[TestFixture]
	public class ItemsControlTests
	{
		[SetUp]
		public void Setup()
		{
			_control = new ItemsControl
			{
				IsAttachedToRoot = true,
				ItemTemplate = () =>
				{
					var control = new ItemControl();
					control.CreateDataBinding(ItemControl.ValueProperty, BindingMode.OneWay);
					return control;
				}
			};
		}

		private class ItemControl : Control
		{
			public static readonly DependencyProperty<int> ValueProperty = new DependencyProperty<int>();

			public int Value
			{
				get { return GetValue(ValueProperty); }
				set { SetValue(ValueProperty, value); }
			}
		}

		private ItemsControl _control;

		private IEnumerable<int> Values
		{
			get
			{
				var children = ((Panel)_control.GetVisualChild(0)).Children;
				return children.Select(child => ((ItemControl)child).Value);
			}
		}

		[Test]
		public void ShouldHaveChildren()
		{
			var items = new[] { 1 };
			_control.ItemsSource = items;
			Values.Should().Equal(items);

			items = new[] { 1, 2, 3, 4 };
			_control.ItemsSource = items;
			Values.Should().Equal(items);

			items = new[] { 3, 4 };
			_control.ItemsSource = items;
			Values.Should().Equal(items);
		}

		[Test]
		public void ShouldHaveNoChildrenByDefault()
		{
			Values.Should().HaveCount(0);
		}
	}
}