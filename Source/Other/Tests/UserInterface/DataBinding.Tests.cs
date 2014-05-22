﻿namespace Tests.UserInterface
{
	using System;
	using FluentAssertions;
	using NUnit.Framework;
	using Pegasus.Framework.UserInterface;
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
		private readonly Thickness _margin4 = new Thickness(16);
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
			_control.ViewModel = _viewModel;
			_control.CreateDataBinding(UIElement.MarginProperty, BindingMode.OneWay, property1, property2);
		}

		private void BindWidth(string property1, string property2 = null)
		{
			_control.CreateDataBinding(UIElement.WidthProperty, BindingMode.OneWay, property1, property2);
		}

		private class UntypedViewModelA : ViewModel
		{
			private object _value;

			public object Value
			{
				get { return _value; }
				set { ChangePropertyValue(ref _value, value); }
			}
		}

		private class UntypedViewModelB : ViewModel
		{
			private object _value;

			public object Value
			{
				get { return _value; }
				set { ChangePropertyValue(ref _value, value); }
			}
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
			_control.ViewModel = _viewModel;
			_viewModel.Width = Width1;

			BindWidth("Width");
			_control.Width.Should().Be(Width1);

			_control.Width = Width2;
			_control.Width.Should().Be(Width2);

			_viewModel.Width = Width3;
			_control.Width.Should().Be(Width2);
		}

		[Test]
		public void ViewModel_ChangeType()
		{
			_control.ViewModel = new UntypedViewModelA { Value = _margin1 };
			_control.CreateDataBinding(UIElement.MarginProperty, BindingMode.OneWay, "Value");

			_control.Margin.Should().Be(_margin1);

			var margin = new Thickness(65);
			_control.ViewModel = new UntypedViewModelB { Value = margin };

			_control.Margin.Should().Be(margin);
		}

		[Test]
		public void ViewModel_ChangeType_AfterNull()
		{
			_control.ViewModel = new UntypedViewModelA { Value = _margin1 };
			_control.CreateDataBinding(UIElement.MarginProperty, BindingMode.OneWay, "Value");

			_control.Margin.Should().Be(_margin1);

			_control.ViewModel = null;
			_control.Margin.Should().Be(new Thickness());

			var margin = new Thickness(65);
			_control.ViewModel = new UntypedViewModelB { Value = margin };

			_control.Margin.Should().Be(margin);
		}

		[Test]
		public void ViewModel_ChangeType_Twice()
		{
			_control.ViewModel = new UntypedViewModelA { Value = _margin1 };
			_control.CreateDataBinding(UIElement.MarginProperty, BindingMode.OneWay, "Value");

			_control.Margin.Should().Be(_margin1);

			var margin = new Thickness(65);
			_control.ViewModel = new UntypedViewModelB { Value = margin };

			_control.Margin.Should().Be(margin);

			_control.ViewModel = new UntypedViewModelA { Value = _margin1 };
			_control.Margin.Should().Be(_margin1);
		}

		[Test]
		public void ViewModel_NotSet()
		{
			_control.ViewModel = null;
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
			_control.ViewModel = viewModel;
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
			_control.ViewModel = _viewModel;
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
			_control.ViewModel = _viewModel;
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
			_control.ViewModel = _viewModel;
			_viewModel.Model.Width = Width1;

			BindWidth("Model", "Width");
			_control.Width.Should().Be(Width1);

			_control.ViewModel = null;
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
			_control.ViewModel = null;
			BindWidth("Model", "Width");

			_control.Width.Should().Be(UIElement.WidthProperty.DefaultValue);
		}

		[Test]
		public void ViewModel_Remove()
		{
			_control.ViewModel = _viewModel;
			_viewModel.Width = Width1;

			BindWidth("Width");
			_control.Width.Should().Be(Width1);

			_control.ViewModel = null;
			_control.Width.Should().Be(UIElement.WidthProperty.DefaultValue);
		}
	}
}