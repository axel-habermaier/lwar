using System;

namespace Tests.UserInterface
{
	using FluentAssertions;
	using NUnit.Framework;
	using Pegasus.Framework;
	using Pegasus.Framework.UserInterface;

	[TestFixture]
	public class DataBindingTests
	{
		[SetUp]
		public void Setup()
		{
			_viewModel = new TestViewModel();
			_control = new TestControl();
		}

		private readonly Thickness _margin = new Thickness(2);
		private const double Width = 4;

		private TestViewModel _viewModel;
		private TestControl _control;

		private void Bind(object sourceObject, string sourceExpression)
		{
			_control.CreateDataBinding(sourceObject, sourceExpression, UIElement.MarginProperty);
		}

		private void BindToViewModel(string sourceExpression)
		{
			_control.ViewModel = _viewModel;
			_control.CreateDataBinding(sourceExpression, UIElement.MarginProperty);
		}

		private void BindWidth(string sourceExpression)
		{
			_control.CreateDataBinding(sourceExpression, UIElement.WidthProperty);
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
		public void Source_Property()
		{
			_viewModel.Thickness = _margin;
			Bind(_viewModel, "Thickness");

			_control.Margin.Should().Be(_margin);
		}

		[Test]
		public void Source_Property_Changed()
		{
			Bind(_viewModel, "Thickness");
			_control.Margin.Should().Be(new Thickness());

			_viewModel.Thickness = _margin;
			_control.Margin.Should().Be(_margin);
		}

		[Test]
		public void Source_Property_Property()
		{
			_viewModel.InitializeRecursively(1);
			_viewModel.Model.Thickness = _margin;

			Bind(_viewModel, "Model.Thickness");

			_control.Margin.Should().Be(_margin);
		}

		[Test]
		public void Source_Property_Property_Property()
		{
			_viewModel.InitializeRecursively(2);
			_viewModel.Model.Model.Thickness = _margin;

			Bind(_viewModel, "Model.Model.Thickness");

			_control.Margin.Should().Be(_margin);
		}

		[Test]
		public void Source_Property_Property_PropertyChanged()
		{
			_viewModel.InitializeRecursively(1);

			Bind(_viewModel, "Model.Thickness");
			_control.Margin.Should().Be(new Thickness());

			_viewModel.Model = new TestViewModel { Thickness = _margin };
			_control.Margin.Should().Be(_margin);
		}

		[Test]
		public void Source_Property_Property_Property_FirstPropertyChanged()
		{
			_viewModel.InitializeRecursively(2);

			Bind(_viewModel, "Model.Model.Thickness");
			_control.Margin.Should().Be(new Thickness());

			_viewModel.Model = new TestViewModel { Model = new TestViewModel { Thickness = _margin } };
			_control.Margin.Should().Be(_margin);
		}

		[Test]
		public void Source_Property_Property_Property_SecondPropertyChanged()
		{
			_viewModel.InitializeRecursively(2);

			Bind(_viewModel, "Model.Model.Thickness");
			_control.Margin.Should().Be(new Thickness());

			_viewModel.Model.Model = new TestViewModel { Thickness = _margin };
			_control.Margin.Should().Be(_margin);
		}

		[Test]
		public void Source_Property_Property_Property_ValueChanged()
		{
			_viewModel.InitializeRecursively(2);

			Bind(_viewModel, "Model.Model.Thickness");
			_control.Margin.Should().Be(new Thickness());

			_viewModel.Model.Model.Thickness = _margin;
			_control.Margin.Should().Be(_margin);
		}

		[Test]
		public void Source_Property_Property_ValueChanged()
		{
			_viewModel.InitializeRecursively(1);

			Bind(_viewModel, "Model.Thickness");
			_control.Margin.Should().Be(new Thickness());

			_viewModel.Model.Thickness = _margin;
			_control.Margin.Should().Be(_margin);
		}

		[Test]
		public void ViewModel_ChangeType()
		{
			_control.ViewModel = new UntypedViewModelA { Value = _margin };
			_control.CreateDataBinding("Value", UIElement.MarginProperty);

			_control.Margin.Should().Be(_margin);

			var margin = new Thickness(65);
			_control.ViewModel = new UntypedViewModelB { Value = margin };

			_control.Margin.Should().Be(margin);
		}

		[Test]
		public void ViewModel_ChangeType_Twice()
		{
			_control.ViewModel = new UntypedViewModelA { Value = _margin };
			_control.CreateDataBinding("Value", UIElement.MarginProperty);

			_control.Margin.Should().Be(_margin);

			var margin = new Thickness(65);
			_control.ViewModel = new UntypedViewModelB { Value = margin };

			_control.Margin.Should().Be(margin);

			_control.ViewModel = new UntypedViewModelA { Value = _margin };
			_control.Margin.Should().Be(_margin);
		}

		[Test]
		public void ViewModel_ChangeType_AfterNull()
		{
			_control.ViewModel = new UntypedViewModelA { Value = _margin };
			_control.CreateDataBinding("Value", UIElement.MarginProperty);

			_control.Margin.Should().Be(_margin);

			_control.ViewModel = null;
			_control.Margin.Should().Be(new Thickness());

			var margin = new Thickness(65);
			_control.ViewModel = new UntypedViewModelB { Value = margin };

			_control.Margin.Should().Be(margin);
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
			_viewModel.Thickness = _margin;
			BindToViewModel("Thickness");

			_control.Margin.Should().Be(_margin);
		}

		[Test]
		public void ViewModel_Property_ChangeType()
		{
			var viewModel = new UntypedViewModelA { Value = new UntypedViewModelA { Value = _margin } };
			_control.ViewModel = viewModel;
			_control.CreateDataBinding("Value.Value", UIElement.MarginProperty);

			_control.Margin.Should().Be(_margin);

			var margin = new Thickness(65);
			viewModel.Value = new UntypedViewModelB { Value = margin };

			_control.Margin.Should().Be(margin);
		}

		[Test]
		public void ViewModel_Property_Changed()
		{
			BindToViewModel("Thickness");
			_control.Margin.Should().Be(new Thickness());

			_viewModel.Thickness = _margin;
			_control.Margin.Should().Be(_margin);
		}

		[Test]
		public void ViewModel_Property_Property()
		{
			_viewModel.InitializeRecursively(1);
			_viewModel.Model.Thickness = _margin;

			BindToViewModel("Model.Thickness");

			_control.Margin.Should().Be(_margin);
		}

		[Test]
		public void ViewModel_Property_Property_NotSet()
		{
			_control.ViewModel = _viewModel;
			BindWidth("Model.Width");

			_control.Width.Should().Be(UIElement.WidthProperty.DefaultValue);
		}

		[Test]
		public void ViewModel_Property_Property_PropertyChanged()
		{
			_viewModel.InitializeRecursively(1);

			BindToViewModel("Model.Thickness");
			_control.Margin.Should().Be(new Thickness());

			_viewModel.Model = new TestViewModel { Thickness = _margin };
			_control.Margin.Should().Be(_margin);
		}

		[Test]
		public void ViewModel_Property_Property_RemoveProperty()
		{
			_viewModel.InitializeRecursively(1);
			_control.ViewModel = _viewModel;
			_viewModel.Model.Width = Width;

			BindWidth("Model.Width");
			_control.Width.Should().Be(Width);

			_viewModel.Model = null;
			_control.Width.Should().Be(UIElement.WidthProperty.DefaultValue);
		}

		[Test]
		public void ViewModel_Property_Property_RemoveViewModel()
		{
			_viewModel.InitializeRecursively(1);
			_control.ViewModel = _viewModel;
			_viewModel.Model.Width = Width;

			BindWidth("Model.Width");
			_control.Width.Should().Be(Width);

			_control.ViewModel = null;
			_control.Width.Should().Be(UIElement.WidthProperty.DefaultValue);
		}

		[Test]
		public void ViewModel_Property_Property_ValueChanged()
		{
			_viewModel.InitializeRecursively(1);

			BindToViewModel("Model.Thickness");
			_control.Margin.Should().Be(new Thickness());

			_viewModel.Model.Thickness = _margin;
			_control.Margin.Should().Be(_margin);
		}

		[Test]
		public void ViewModel_Property_Property_ViewModel_NotSet()
		{
			_control.ViewModel = null;
			BindWidth("Model.Width");

			_control.Width.Should().Be(UIElement.WidthProperty.DefaultValue);
		}

		[Test]
		public void ViewModel_Remove()
		{
			_control.ViewModel = _viewModel;
			_viewModel.Width = Width;

			BindWidth("Width");
			_control.Width.Should().Be(Width);

			_control.ViewModel = null;
			_control.Width.Should().Be(UIElement.WidthProperty.DefaultValue);
		}
	}
}