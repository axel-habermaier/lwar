namespace Tests.UserInterface
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using FluentAssertions;
	using NUnit.Framework;
	using Pegasus.Framework;
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
		public void ObservableChange_Add()
		{
			var items = new ObservableCollection<int> { 1, 2, 3 };
			_control.ItemsSource = items;
			Values.Should().Equal(items);

			items.Add(56);
			Values.Should().Equal(items);

			items.Insert(0, 22);
			Values.Should().Equal(items);

			items.Insert(3, 22);
			Values.Should().Equal(items);
		}

		[Test]
		public void ObservableChange_Remove()
		{
			var items = new ObservableCollection<int> { 1, 2, 3, 6, 234, 7 };
			_control.ItemsSource = items;
			Values.Should().Equal(items);

			items.Remove(1);
			Values.Should().Equal(items);

			items.Remove(7);
			Values.Should().Equal(items);

			items.Remove(3);
			Values.Should().Equal(items);
		}

		[Test]
		public void ObservableChange_RemoveAt()
		{
			var items = new ObservableCollection<int> { 1, 2, 3, 6, 234, 7 };
			_control.ItemsSource = items;
			Values.Should().Equal(items);

			items.RemoveAt(items.Count - 1);
			Values.Should().Equal(items);

			items.Remove(0);
			Values.Should().Equal(items);

			items.Remove(3);
			Values.Should().Equal(items);
		}

		[Test]
		public void ObservableChange_Replace()
		{
			var items = new ObservableCollection<int> { 1, 2, 3, 6, 234, 7 };
			_control.ItemsSource = items;
			Values.Should().Equal(items);

			items[0] = 22;
			Values.Should().Equal(items);

			items[items.Count - 1] = 2134;
			Values.Should().Equal(items);

			items[4] = 7482;
			Values.Should().Equal(items);
		}

		[Test]
		public void ObservableChanged_Clear()
		{
			var items = new ObservableCollection<int> { 1, 2, 3, 6, 234, 7 };
			_control.ItemsSource = items;
			Values.Should().Equal(items);

			items.Clear();
			Values.Should().HaveCount(0);
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