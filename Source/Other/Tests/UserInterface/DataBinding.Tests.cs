namespace Tests.UserInterface
{
	using System;
	using System.Text;
	using FluentAssertions;
	using NUnit.Framework;
	using Pegasus.Framework;
	using Pegasus.Framework.UserInterface;
	using Pegasus.Framework.UserInterface.Controls;
	using Pegasus.Framework.UserInterface.Converters;

	[TestFixture]
	public class DataBindingTests
	{
		[SetUp]
		public void Setup()
		{
			_viewModel = new TestViewModel();
			_control = new TestControl();
		}

		private readonly Thickness _margin1 = new Thickness(2);
		private readonly Thickness _margin2 = new Thickness(4);
		private readonly Thickness _margin3 = new Thickness(8);
		private const double Width1 = 4;
		private const double Width2 = 66;
		private const double Width3 = 128;

		private TestViewModel _viewModel;
		private TestControl _control;

		private void Bind(object sourceObject, string property1, string property2 = null, string property3 = null)
		{
			_control.CreateDataBinding(sourceObject, UIElement.MarginProperty, BindingMode.OneWay, property1, property2, property3);
		}

		private void BindToViewModel(string property1, string property2 = null)
		{
			_control.DataContext = _viewModel;
			_control.CreateDataBinding(UIElement.MarginProperty, BindingMode.OneWay, property1, property2);
		}

		private void BindWidth(string property1, string property2 = null)
		{
			_control.CreateDataBinding(UIElement.WidthProperty, BindingMode.OneWay, property1, property2);
		}

		private class UntypedViewModelA : NotifyPropertyChanged
		{
			private object _value;

			public object Value
			{
				get { return _value; }
				set { ChangePropertyValue(ref _value, value); }
			}
		}

		private class UntypedViewModelB : NotifyPropertyChanged
		{
			private object _value;

			public object Value
			{
				get { return _value; }
				set { ChangePropertyValue(ref _value, value); }
			}
		}

		[Test]
		public void BindDirectlyToDataContext()
		{
			_control.DataContext = 333;
			_control.CreateDataBinding(TestControl.IntegerTestProperty1, BindingMode.OneWay);
			_control.IntegerTest1.Should().Be(333);
		}

		[Test]
		public void AutomaticallyInvokeToString()
		{
			_viewModel.Integer = 17;
			_control.CreateDataBinding(_viewModel, TestControl.StringTestProperty, BindingMode.OneWay, "Integer");
			_control.StringTest.Should().Be("17");

			var builder = new StringBuilder().Append("ABC");
			var sourceObject = new UntypedViewModelA { Value = builder };

			_control.CreateDataBinding(sourceObject, TestControl.StringTestProperty, BindingMode.OneWay, "Value");
			_control.StringTest.Should().Be(builder.ToString());
		}

		[Test]
		public void BindingMode_OneWay()
		{
			_viewModel.Integer = 17;
			_control.CreateDataBinding(_viewModel, TestControl.IntegerTestProperty1, BindingMode.OneWay, "Integer");

			_control.IntegerTest1.Should().Be(17);

			_viewModel.Integer = 8;
			_control.IntegerTest1.Should().Be(8);

			_control.IntegerTest1 = 33;
			_viewModel.Integer.Should().Be(8);
			_control.IntegerTest1.Should().Be(33);
		}

		[Test]
		public void BindingMode_OneWayToSource()
		{
			_viewModel.Integer = 17;
			_control.CreateDataBinding(_viewModel, TestControl.IntegerTestProperty1, BindingMode.OneWayToSource, "Integer");

			_control.IntegerTest1.Should().Be(0);

			_viewModel.Integer = 8;
			_control.IntegerTest1.Should().Be(0);

			_control.IntegerTest1 = 33;
			_viewModel.Integer.Should().Be(33);
			_control.IntegerTest1.Should().Be(33);
		}

		[Test]
		public void BindingMode_OneWayToSource_FirstObjectChanged()
		{
			_viewModel.InitializeRecursively(2);
			_viewModel.Model.Model.Integer = 17;
			_control.CreateDataBinding(_viewModel, TestControl.IntegerTestProperty1, BindingMode.OneWayToSource, "Model", "Model", "Integer");

			var model = new TestViewModel();
			model.InitializeRecursively(1);
			model.Model.Integer = 7777;
			_viewModel.Model = model;

			_control.IntegerTest1.Should().Be(0);

			_control.IntegerTest1 = 42;
			_viewModel.Model.Model.Integer.Should().Be(42);
			_control.IntegerTest1.Should().Be(42);

			model = new TestViewModel();
			model.InitializeRecursively(1);
			model.Model.Integer = 7777;
			_viewModel.Model = model;
			_viewModel.Model.Model.Integer.Should().Be(42);
			_control.IntegerTest1.Should().Be(42);
		}

		[Test]
		public void BindingMode_OneWayToSource_Property_Property()
		{
			_viewModel.InitializeRecursively(1);
			_viewModel.Model.Integer = 17;
			_control.CreateDataBinding(_viewModel, TestControl.IntegerTestProperty1, BindingMode.OneWayToSource, "Model", "Integer");

			_control.IntegerTest1.Should().Be(0);

			_viewModel.Model.Integer = 8;
			_control.IntegerTest1.Should().Be(0);

			_control.IntegerTest1 = 33;
			_viewModel.Model.Integer.Should().Be(33);
			_control.IntegerTest1.Should().Be(33);
		}

		[Test]
		public void BindingMode_OneWayToSource_Property_Property_Property()
		{
			_viewModel.InitializeRecursively(2);
			_viewModel.Model.Model.Integer = 17;
			_control.CreateDataBinding(_viewModel, TestControl.IntegerTestProperty1, BindingMode.OneWayToSource, "Model", "Model", "Integer");

			_control.IntegerTest1.Should().Be(0);

			_viewModel.Model.Model.Integer = 8;
			_control.IntegerTest1.Should().Be(0);

			_control.IntegerTest1 = 33;
			_viewModel.Model.Model.Integer.Should().Be(33);
			_control.IntegerTest1.Should().Be(33);
		}

		[Test]
		public void BindingMode_OneWayToSource_SecondObjectChanged()
		{
			_viewModel.InitializeRecursively(2);
			_viewModel.Model.Model.Integer = 17;
			_control.CreateDataBinding(_viewModel, TestControl.IntegerTestProperty1, BindingMode.OneWayToSource, "Model", "Model", "Integer");

			var model = new TestViewModel();
			model.Integer = 7777;
			_viewModel.Model.Model = model;

			_control.IntegerTest1.Should().Be(0);

			_control.IntegerTest1 = 42;
			_viewModel.Model.Model.Integer.Should().Be(42);
			_control.IntegerTest1.Should().Be(42);

			model = new TestViewModel();
			model.Integer = 7777;
			_viewModel.Model.Model = model;
			_viewModel.Model.Model.Integer.Should().Be(42);
			_control.IntegerTest1.Should().Be(42);
		}

		[Test]
		public void BindingMode_OneWay_Property_Property()
		{
			_viewModel.InitializeRecursively(1);
			_viewModel.Model.Integer = 17;
			_control.CreateDataBinding(_viewModel, TestControl.IntegerTestProperty1, BindingMode.OneWay, "Model", "Integer");

			_control.IntegerTest1.Should().Be(17);

			_viewModel.Model.Integer = 8;
			_control.IntegerTest1.Should().Be(8);

			_control.IntegerTest1 = 33;
			_viewModel.Model.Integer.Should().Be(8);
			_control.IntegerTest1.Should().Be(33);
		}

		[Test]
		public void BindingMode_OneWay_Property_Property_Property()
		{
			_viewModel.InitializeRecursively(2);
			_viewModel.Model.Model.Integer = 17;
			_control.CreateDataBinding(_viewModel, TestControl.IntegerTestProperty1, BindingMode.OneWay, "Model", "Model", "Integer");

			_control.IntegerTest1.Should().Be(17);

			_viewModel.Model.Model.Integer = 8;
			_control.IntegerTest1.Should().Be(8);

			_control.IntegerTest1 = 33;
			_viewModel.Model.Model.Integer.Should().Be(8);
			_control.IntegerTest1.Should().Be(33);
		}

		[Test]
		public void BindingMode_OneWay_SecondPropertyIsNull()
		{
			_control.IsAttachedToRoot = false;
			_control.CreateDataBinding(_viewModel, TestControl.DefaultStringTestProperty, BindingMode.OneWay, "Model", "String");
			_control.IsAttachedToRoot = true;

			_control.DefaultStringTest.Should().Be(TestControl.DefaultStringTestProperty.DefaultValue);

			_control = new TestControl();
			_viewModel.InitializeRecursively(1);
			_viewModel.Model.String = null;
			_control.IsAttachedToRoot = false;
			_control.CreateDataBinding(_viewModel, TestControl.DefaultStringTestProperty, BindingMode.OneWay, "Model", "String");
			_control.IsAttachedToRoot = true;

			_control.DefaultStringTest.Should().Be(TestControl.DefaultStringTestProperty.DefaultValue);

			_viewModel.Model.String = "A";
			_control.DefaultStringTest.Should().Be(_viewModel.Model.String);
		}

		[Test]
		public void BindingMode_OneWay_SourceIsNull()
		{
			_control.IsAttachedToRoot = false;
			_control.CreateDataBinding(TestControl.DefaultStringTestProperty, BindingMode.OneWay, "String");

			_control.IsAttachedToRoot = true;
			_control.DefaultStringTest.Should().Be(TestControl.DefaultStringTestProperty.DefaultValue);
		}

		[Test]
		public void BindingMode_TwoWay()
		{
			_viewModel.Integer = 17;
			_control.CreateDataBinding(_viewModel, TestControl.IntegerTestProperty1, BindingMode.TwoWay, "Integer");

			_control.IntegerTest1.Should().Be(17);

			_viewModel.Integer = 8;
			_control.IntegerTest1.Should().Be(8);

			_control.IntegerTest1 = 33;
			_viewModel.Integer.Should().Be(33);
			_control.IntegerTest1.Should().Be(33);

			_viewModel.Integer = 8;
			_control.IntegerTest1.Should().Be(8);
		}

		[Test]
		public void BindingMode_TwoWay_FirstObjectChanged()
		{
			_viewModel.InitializeRecursively(2);
			_viewModel.Model.Model.Integer = 17;
			_control.CreateDataBinding(_viewModel, TestControl.IntegerTestProperty1, BindingMode.TwoWay, "Model", "Model", "Integer");

			var model = new TestViewModel();
			model.InitializeRecursively(1);
			model.Model.Integer = 7777;
			_viewModel.Model = model;

			_control.IntegerTest1.Should().Be(7777);

			_control.IntegerTest1 = 42;
			_viewModel.Model.Model.Integer.Should().Be(42);
			_control.IntegerTest1.Should().Be(42);

			model.Model.Integer = 66;
			_control.IntegerTest1.Should().Be(66);
		}

		[Test]
		public void BindingMode_TwoWay_Property_Property()
		{
			_viewModel.InitializeRecursively(1);
			_viewModel.Model.Integer = 17;
			_control.CreateDataBinding(_viewModel, TestControl.IntegerTestProperty1, BindingMode.TwoWay, "Model", "Integer");

			_control.IntegerTest1.Should().Be(17);

			_viewModel.Model.Integer = 8;
			_control.IntegerTest1.Should().Be(8);

			_control.IntegerTest1 = 33;
			_viewModel.Model.Integer.Should().Be(33);
			_control.IntegerTest1.Should().Be(33);

			_viewModel.Model.Integer = 8;
			_control.IntegerTest1.Should().Be(8);
		}

		[Test]
		public void BindingMode_TwoWay_Property_Property_Property()
		{
			_viewModel.InitializeRecursively(2);
			_viewModel.Model.Model.Integer = 17;
			_control.CreateDataBinding(_viewModel, TestControl.IntegerTestProperty1, BindingMode.TwoWay, "Model", "Model", "Integer");

			_control.IntegerTest1.Should().Be(17);

			_viewModel.Model.Model.Integer = 8;
			_control.IntegerTest1.Should().Be(8);

			_control.IntegerTest1 = 33;
			_viewModel.Model.Model.Integer.Should().Be(33);
			_control.IntegerTest1.Should().Be(33);

			_viewModel.Model.Model.Integer = 8;
			_control.IntegerTest1.Should().Be(8);
		}

		[Test]
		public void BindingMode_TwoWay_SecondObjectChanged()
		{
			_viewModel.InitializeRecursively(2);
			_viewModel.Model.Model.Integer = 17;
			_control.CreateDataBinding(_viewModel, TestControl.IntegerTestProperty1, BindingMode.TwoWay, "Model", "Model", "Integer");

			var model = new TestViewModel();
			model.Integer = 7777;
			_viewModel.Model.Model = model;

			_control.IntegerTest1.Should().Be(7777);

			_control.IntegerTest1 = 42;
			_viewModel.Model.Model.Integer.Should().Be(42);
			_control.IntegerTest1.Should().Be(42);

			model.Integer = 66;
			_control.IntegerTest1.Should().Be(66);
		}

		[Test]
		public void DefaultBindingMode()
		{
			_control.CreateDataBinding(_viewModel, TestControl.BooleanTestProperty1, BindingMode.Default, "Bool");
			_viewModel.Bool = true;

			_control.BooleanTest1.Should().BeFalse();

			_viewModel.Bool = false;
			_control.BooleanTest1 = true;

			_viewModel.Bool.Should().BeTrue();
		}

		[Test]
		public void DoubleToString_ValueConversion_OneWay()
		{
			_viewModel.InitializeRecursively(1);
			_control.CreateDataBinding(_viewModel, TestControl.StringTestProperty, BindingMode.OneWay, "Width",
				converter: new DoubleToStringConverter());

			_control.StringTest.Should().Be("0");

			_viewModel.Width = 21.5;
			_control.StringTest.Should().Be("21.5");
		}

		[Test]
		public void DoubleToString_ValueConversion_OneWayToSource()
		{
			_viewModel.InitializeRecursively(1);
			_control.CreateDataBinding(_viewModel, TestControl.StringTestProperty, BindingMode.OneWayToSource, "Width",
				converter: new DoubleToStringConverter());

			_viewModel.Width.Should().Be(0);

			_control.StringTest = "21.5";
			_viewModel.Width.Should().Be(21.5);
		}

		[Test]
		public void DoubleToString_ValueConversion_TwoWay()
		{
			_viewModel.InitializeRecursively(1);
			_control.CreateDataBinding(_viewModel, TestControl.StringTestProperty, BindingMode.TwoWay, "Width",
				converter: new DoubleToStringConverter());

			_viewModel.Width.Should().Be(0);
			_control.StringTest.Should().Be("0");

			_control.StringTest = "21.5";
			_viewModel.Width.Should().Be(21.5);

			_viewModel.Width = 0.5;
			_control.StringTest.Should().Be("0.5");
		}

		[Test]
		public void OneWayToSource_FirstChangedToNull()
		{
			_viewModel.InitializeRecursively(2);
			_control.CreateDataBinding(_viewModel, TestControl.BooleanTestProperty1, BindingMode.OneWayToSource, "Model", "Model", "Bool");

			_control.BooleanTest1 = true;
			_viewModel.Model.Model.Bool.Should().BeTrue();

			_viewModel.Model = null;

			Action action = () => _control.BooleanTest1 = false;
			action.ShouldNotThrow();
		}

		[Test]
		public void OneWayToSource_NoSourceObject()
		{
			_control.CreateDataBinding(TestControl.BooleanTestProperty1, BindingMode.OneWayToSource, "Bool");

			Action action = () => _control.BooleanTest1 = true;
			action.ShouldNotThrow();
		}

		[Test]
		public void OneWayToSource_Property_NotSet()
		{
			_control.StringTest = "ABC";

			_control.CreateDataBinding(_viewModel, TestControl.StringTestProperty, BindingMode.OneWayToSource, "String");
			_viewModel.String.Should().Be("ABC");
		}

		[Test]
		public void OneWayToSource_Property_Property_FirstNotSet()
		{
			_control.CreateDataBinding(_viewModel, TestControl.BooleanTestProperty1, BindingMode.OneWayToSource, "Model", "Bool");

			Action action = () => _control.BooleanTest1 = true;
			action.ShouldNotThrow();
		}

		[Test]
		public void OneWayToSource_Property_Property_Property_FirstNotSet()
		{
			_control.CreateDataBinding(_viewModel, TestControl.BooleanTestProperty1, BindingMode.OneWayToSource, "Model", "Model", "Bool");

			Action action = () => _control.BooleanTest1 = true;
			action.ShouldNotThrow();
		}

		[Test]
		public void OneWayToSource_Property_Property_Property_SecondNotSet()
		{
			_viewModel.InitializeRecursively(1);
			_control.CreateDataBinding(_viewModel, TestControl.BooleanTestProperty1, BindingMode.OneWayToSource, "Model", "Model", "Bool");

			Action action = () => _control.BooleanTest1 = true;
			action.ShouldNotThrow();
		}

		[Test]
		public void OneWayToSource_SecondChangedToNull()
		{
			_viewModel.InitializeRecursively(2);
			_control.CreateDataBinding(_viewModel, TestControl.BooleanTestProperty1, BindingMode.OneWayToSource, "Model", "Model", "Bool");

			_control.BooleanTest1 = true;
			_viewModel.Model.Model.Bool.Should().BeTrue();

			_viewModel.Model.Model = null;

			Action action = () => _control.BooleanTest1 = false;
			action.ShouldNotThrow();
		}

		[Test]
		public void OneWayToSource_ViewModel_Property_NotSet()
		{
			_control.StringTest = "ABC";
			_control.DataContext = _viewModel;

			_control.CreateDataBinding(TestControl.StringTestProperty, BindingMode.OneWayToSource, "String");
			_viewModel.String.Should().Be("ABC");
		}

		[Test]
		public void Overwrite_OneWay()
		{
			_viewModel.String = "ABC";

			_control.CreateDataBinding(_viewModel, TestControl.StringTestProperty, BindingMode.OneWay, "String");
			_control.StringTest.Should().Be("ABC");

			_viewModel = new TestViewModel { String = "DEF" };

			_control.CreateDataBinding(_viewModel, TestControl.StringTestProperty, BindingMode.OneWay, "String");
			_control.StringTest.Should().Be("DEF");
		}

		[Test]
		public void Overwrite_OneWayToSource()
		{
			_control.StringTest = "ABC";

			_control.CreateDataBinding(_viewModel, TestControl.StringTestProperty, BindingMode.OneWayToSource, "String");
			_viewModel.String.Should().Be("ABC");

			_viewModel = new TestViewModel { String = "DEF" };

			_control.CreateDataBinding(_viewModel, TestControl.StringTestProperty, BindingMode.OneWayToSource, "String");
			_viewModel.String.Should().Be("ABC");
		}

		[Test]
		public void Overwrite_TwoWay()
		{
			_viewModel.String = "ABC";

			_control.CreateDataBinding(_viewModel, TestControl.StringTestProperty, BindingMode.TwoWay, "String");
			_control.StringTest.Should().Be("ABC");
			_viewModel.String.Should().Be("ABC");

			_viewModel = new TestViewModel { String = "DEF" };

			_control.CreateDataBinding(_viewModel, TestControl.StringTestProperty, BindingMode.TwoWay, "String");
			_control.StringTest.Should().Be("DEF");
			_viewModel.String.Should().Be("DEF");
		}

		[Test]
		public void Source_ChangeNotificationUnregisteredCorrectly()
		{
			_viewModel.InitializeRecursively(1);
			_viewModel.Model.Thickness = _margin1;

			Bind(_viewModel, "Model", "Thickness");
			_control.Margin.Should().Be(_margin1);

			var model = _viewModel.Model;
			_viewModel.Model = new TestViewModel { Thickness = _margin2 };
			_control.Margin.Should().Be(_margin2);

			model.Thickness = _margin3;
			_control.Margin.Should().Be(_margin2);
		}

		[Test]
		public void Source_Property()
		{
			_viewModel.Thickness = _margin1;
			Bind(_viewModel, "Thickness");

			_control.Margin.Should().Be(_margin1);
		}

		[Test]
		public void Source_Property_Changed()
		{
			Bind(_viewModel, "Thickness");
			_control.Margin.Should().Be(new Thickness());

			_viewModel.Thickness = _margin1;
			_control.Margin.Should().Be(_margin1);
		}

		[Test]
		public void Source_Property_Property()
		{
			_viewModel.InitializeRecursively(1);
			_viewModel.Model.Thickness = _margin1;

			Bind(_viewModel, "Model", "Thickness");

			_control.Margin.Should().Be(_margin1);
		}

		[Test]
		public void Source_Property_Property_Property()
		{
			_viewModel.InitializeRecursively(2);
			_viewModel.Model.Model.Thickness = _margin1;

			Bind(_viewModel, "Model", "Model", "Thickness");

			_control.Margin.Should().Be(_margin1);
		}

		[Test]
		public void Source_Property_Property_PropertyChanged()
		{
			_viewModel.InitializeRecursively(1);

			Bind(_viewModel, "Model", "Thickness");
			_control.Margin.Should().Be(new Thickness());

			_viewModel.Model = new TestViewModel { Thickness = _margin1 };
			_control.Margin.Should().Be(_margin1);
		}

		[Test]
		public void Source_Property_Property_Property_FirstPropertyChanged()
		{
			_viewModel.InitializeRecursively(2);

			Bind(_viewModel, "Model", "Model", "Thickness");
			_control.Margin.Should().Be(new Thickness());

			_viewModel.Model = new TestViewModel { Model = new TestViewModel { Thickness = _margin1 } };
			_control.Margin.Should().Be(_margin1);
		}

		[Test]
		public void Source_Property_Property_Property_SecondPropertyChanged()
		{
			_viewModel.InitializeRecursively(2);

			Bind(_viewModel, "Model", "Model", "Thickness");
			_control.Margin.Should().Be(new Thickness());

			_viewModel.Model.Model = new TestViewModel { Thickness = _margin1 };
			_control.Margin.Should().Be(_margin1);
		}

		[Test]
		public void Source_Property_Property_Property_ValueChanged()
		{
			_viewModel.InitializeRecursively(2);

			Bind(_viewModel, "Model", "Model", "Thickness");
			_control.Margin.Should().Be(new Thickness());

			_viewModel.Model.Model.Thickness = _margin1;
			_control.Margin.Should().Be(_margin1);
		}

		[Test]
		public void Source_Property_Property_ValueChanged()
		{
			_viewModel.InitializeRecursively(1);

			Bind(_viewModel, "Model", "Thickness");
			_control.Margin.Should().Be(new Thickness());

			_viewModel.Model.Thickness = _margin1;
			_control.Margin.Should().Be(_margin1);
		}

		[Test]
		public void UnsetBinding()
		{
			_control.DataContext = _viewModel;
			_viewModel.Width = Width1;

			BindWidth("Width");
			_control.Width.Should().Be(Width1);

			_control.Width = Width2;
			_control.Width.Should().Be(Width2);

			_viewModel.Width = Width3;
			_control.Width.Should().Be(Width2);
		}

		[Test]
		public void UpdateOnActivation()
		{
			_control.IsAttachedToRoot = false;
			_control.StringTest = "ABC";
			_control.BooleanTest1 = true;
			_control.Width = 17;

			_viewModel.String = "DEF";
			_viewModel.Bool = false;
			_viewModel.Width = 1;

			_control.CreateDataBinding(_viewModel, TestControl.StringTestProperty, BindingMode.OneWay, "String");
			_control.CreateDataBinding(_viewModel, TestControl.BooleanTestProperty1, BindingMode.OneWayToSource, "Bool");
			_control.CreateDataBinding(_viewModel, UIElement.WidthProperty, BindingMode.TwoWay, "Width");

			_control.StringTest.Should().Be("ABC");
			_control.BooleanTest1.Should().Be(true);
			_control.Width.Should().Be(17);

			_viewModel.String.Should().Be("DEF");
			_viewModel.Bool.Should().BeFalse();
			_viewModel.Width.Should().Be(1);

			_control.IsAttachedToRoot = true;

			_control.StringTest.Should().Be("DEF");
			_control.BooleanTest1.Should().Be(true);
			_control.Width.Should().Be(1);

			_viewModel.String.Should().Be("DEF");
			_viewModel.Bool.Should().BeTrue();
			_viewModel.Width.Should().Be(1);
		}

		[Test]
		public void ValueTypeBoxing_BothUntyped()
		{
			var obj = new object();
			var viewModel = new UntypedViewModelA { Value = 1 };

			_control.DataContext = viewModel;

			_control.CreateDataBinding(TestControl.ObjectTestProperty, BindingMode.TwoWay, "Value");
			_control.ObjectTest.Should().Be(1);

			_control.ObjectTest = 0.0m;
			viewModel.Value.Should().Be(0.0m);

			viewModel.Value = 17u;
			_control.ObjectTest.Should().Be(17u);

			viewModel.Value = obj;
			_control.ObjectTest.Should().Be(obj);

			viewModel.Value = 33;
			_control.ObjectTest = obj;
			viewModel.Value.Should().Be(obj);
		}

		[Test]
		public void ViewModel_ChangeType()
		{
			_control.DataContext = new UntypedViewModelA { Value = _margin1 };
			_control.CreateDataBinding(UIElement.MarginProperty, BindingMode.OneWay, "Value");

			_control.Margin.Should().Be(_margin1);

			var margin = new Thickness(65);
			_control.DataContext = new UntypedViewModelB { Value = margin };

			_control.Margin.Should().Be(margin);
		}

		[Test]
		public void ViewModel_ChangeType_AfterNull()
		{
			_control.DataContext = new UntypedViewModelA { Value = _margin1 };
			_control.CreateDataBinding(UIElement.MarginProperty, BindingMode.OneWay, "Value");

			_control.Margin.Should().Be(_margin1);

			_control.DataContext = null;
			_control.Margin.Should().Be(new Thickness());

			var margin = new Thickness(65);
			_control.DataContext = new UntypedViewModelB { Value = margin };

			_control.Margin.Should().Be(margin);
		}

		[Test]
		public void ViewModel_ChangeType_SecondProperty()
		{
			var builder = new StringBuilder();
			var viewModel = new UntypedViewModelA { Value = builder };

			_control.DataContext = viewModel;
			_control.CreateDataBinding(TestControl.ObjectTestProperty, BindingMode.OneWay, "Value");

			_control.ObjectTest.Should().Be(builder);

			viewModel.Value = "Test";
			_control.ObjectTest.Should().Be("Test");
		}

		[Test]
		public void ViewModel_ChangeType_Twice()
		{
			_control.DataContext = new UntypedViewModelA { Value = _margin1 };
			_control.CreateDataBinding(UIElement.MarginProperty, BindingMode.OneWay, "Value");

			_control.Margin.Should().Be(_margin1);

			var margin = new Thickness(65);
			_control.DataContext = new UntypedViewModelB { Value = margin };

			_control.Margin.Should().Be(margin);

			_control.DataContext = new UntypedViewModelA { Value = _margin1 };
			_control.Margin.Should().Be(_margin1);
		}

		[Test]
		public void ViewModel_NotSet()
		{
			_control.DataContext = null;
			BindWidth("Width");

			_control.Width.Should().Be(UIElement.WidthProperty.DefaultValue);
		}

		[Test]
		public void ViewModel_Property()
		{
			_viewModel.Thickness = _margin1;
			BindToViewModel("Thickness");

			_control.Margin.Should().Be(_margin1);
		}

		[Test]
		public void ViewModel_Property_ChangeType()
		{
			var viewModel = new UntypedViewModelA { Value = new UntypedViewModelA { Value = _margin1 } };
			_control.DataContext = viewModel;
			_control.CreateDataBinding(UIElement.MarginProperty, BindingMode.OneWay, "Value", "Value");

			_control.Margin.Should().Be(_margin1);

			var margin = new Thickness(65);
			viewModel.Value = new UntypedViewModelB { Value = margin };

			_control.Margin.Should().Be(margin);
		}

		[Test]
		public void ViewModel_Property_Changed()
		{
			BindToViewModel("Thickness");
			_control.Margin.Should().Be(new Thickness());

			_viewModel.Thickness = _margin1;
			_control.Margin.Should().Be(_margin1);
		}

		[Test]
		public void ViewModel_Property_Property()
		{
			_viewModel.InitializeRecursively(1);
			_viewModel.Model.Thickness = _margin1;

			BindToViewModel("Model", "Thickness");

			_control.Margin.Should().Be(_margin1);
		}

		[Test]
		public void ViewModel_Property_Property_NotSet()
		{
			_control.DataContext = _viewModel;
			BindWidth("Model", "Width");

			_control.Width.Should().Be(UIElement.WidthProperty.DefaultValue);
		}

		[Test]
		public void ViewModel_Property_Property_PropertyChanged()
		{
			_viewModel.InitializeRecursively(1);

			BindToViewModel("Model", "Thickness");
			_control.Margin.Should().Be(new Thickness());

			_viewModel.Model = new TestViewModel { Thickness = _margin1 };
			_control.Margin.Should().Be(_margin1);
		}

		[Test]
		public void ViewModel_Property_Property_RemoveProperty()
		{
			_viewModel.InitializeRecursively(1);
			_control.DataContext = _viewModel;
			_viewModel.Model.Width = Width1;

			BindWidth("Model", "Width");
			_control.Width.Should().Be(Width1);

			_viewModel.Model = null;
			_control.Width.Should().Be(UIElement.WidthProperty.DefaultValue);
		}

		[Test]
		public void ViewModel_Property_Property_RemoveViewModel()
		{
			_viewModel.InitializeRecursively(1);
			_control.DataContext = _viewModel;
			_viewModel.Model.Width = Width1;

			BindWidth("Model", "Width");
			_control.Width.Should().Be(Width1);

			_control.DataContext = null;
			_control.Width.Should().Be(UIElement.WidthProperty.DefaultValue);
		}

		[Test]
		public void ViewModel_Property_Property_ValueChanged()
		{
			_viewModel.InitializeRecursively(1);

			BindToViewModel("Model", "Thickness");
			_control.Margin.Should().Be(new Thickness());

			_viewModel.Model.Thickness = _margin1;
			_control.Margin.Should().Be(_margin1);
		}

		[Test]
		public void ViewModel_Property_Property_ViewModel_NotSet()
		{
			_control.DataContext = null;
			BindWidth("Model", "Width");

			_control.Width.Should().Be(UIElement.WidthProperty.DefaultValue);
		}

		[Test]
		public void ViewModel_Remove()
		{
			_control.DataContext = _viewModel;
			_viewModel.Width = Width1;

			BindWidth("Width");
			_control.Width.Should().Be(Width1);

			_control.DataContext = null;
			_control.Width.Should().Be(UIElement.WidthProperty.DefaultValue);
		}

		[Test]
		public void ChainedDataContextBinding_InheritedOverwrite_Later()
		{
			_viewModel.Width = 77;
			_viewModel.Model = new TestViewModel { Width = 33 };

			_control.Button3.CreateDataBinding(UIElement.WidthProperty, BindingMode.OneWay, "Width");
			_control.DataContext = _viewModel;
			_control.Button3.Width.Should().Be(77);

			_control.Canvas2.CreateDataBinding(UIElement.DataContextProperty, BindingMode.OneWay, "Model");
			_control.Button3.Width.Should().Be(33);
		}

		[Test]
		public void ChainedDataContextBinding_InheritedOverwrite_Immediately()
		{
			_viewModel.Width = 77;
			_viewModel.Model = new TestViewModel { Width = 33 };

			_control.Button3.CreateDataBinding(UIElement.WidthProperty, BindingMode.OneWay, "Width");
			_control.Canvas2.CreateDataBinding(UIElement.DataContextProperty, BindingMode.OneWay, "Model");
			_control.DataContext = _viewModel;

			_control.Button3.Width.Should().Be(33);
		}

		[Test]
		public void ChainedDataContextBinding_InheritedOverwrite_DifferentTypes()
		{
			_viewModel.Width = 77;
			_viewModel.Object = new UntypedViewModelA { Value = 33 };

			_control.Button3.CreateDataBinding(UIElement.WidthProperty, BindingMode.OneWay, "Value");
			_control.Canvas2.CreateDataBinding(UIElement.DataContextProperty, BindingMode.OneWay, "Object");
			_control.DataContext = _viewModel;

			_control.Button3.Width.Should().Be(33);
		}
	}
}