using System;

namespace Tests.UserInterface
{
	using System.Linq.Expressions;
	using FluentAssertions;
	using NUnit.Framework;
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

		private void Bind(object sourceObject, Expression<Func<object, Thickness>> sourceExpression)
		{
			_control.CreateDataBinding(sourceObject, sourceExpression, UIElement.MarginProperty);
		}

		private void BindToViewModel(Expression<Func<object, Thickness>> sourceExpression)
		{
			_control.ViewModel = _viewModel;
			_control.CreateDataBinding(sourceExpression, UIElement.MarginProperty);
		}

		private void BindWidth(Expression<Func<object, double>> sourceExpression)
		{
			_control.CreateDataBinding(sourceExpression, UIElement.WidthProperty);
		}

		[Test]
		public void Source_Property()
		{
			_viewModel.Thickness = _margin;
			Bind(_viewModel, vm => ((TestViewModel)vm).Thickness);

			_control.Margin.Should().Be(_margin);
		}

		[Test]
		public void Source_Property_Changed()
		{
			Bind(_viewModel, vm => ((TestViewModel)vm).Thickness);
			_control.Margin.Should().Be(new Thickness());

			_viewModel.Thickness = _margin;
			_control.Margin.Should().Be(_margin);
		}

		[Test]
		public void Source_Property_Property()
		{
			_viewModel.InitializeRecursively(1);
			_viewModel.Model.Thickness = _margin;

			Bind(_viewModel, vm => ((TestViewModel)vm).Model.Thickness);

			_control.Margin.Should().Be(_margin);
		}

		[Test]
		public void Source_Property_Property_Property()
		{
			_viewModel.InitializeRecursively(2);
			_viewModel.Model.Model.Thickness = _margin;

			Bind(_viewModel, vm => ((TestViewModel)vm).Model.Model.Thickness);

			_control.Margin.Should().Be(_margin);
		}

		[Test]
		public void Source_Property_Property_PropertyChanged()
		{
			_viewModel.InitializeRecursively(1);

			Bind(_viewModel, vm => ((TestViewModel)vm).Model.Thickness);
			_control.Margin.Should().Be(new Thickness());

			_viewModel.Model = new TestViewModel { Thickness = _margin };
			_control.Margin.Should().Be(_margin);
		}

		[Test]
		public void Source_Property_Property_Property_FirstPropertyChanged()
		{
			_viewModel.InitializeRecursively(2);

			Bind(_viewModel, vm => ((TestViewModel)vm).Model.Model.Thickness);
			_control.Margin.Should().Be(new Thickness());

			_viewModel.Model = new TestViewModel { Model = new TestViewModel { Thickness = _margin } };
			_control.Margin.Should().Be(_margin);
		}

		[Test]
		public void Source_Property_Property_Property_SecondPropertyChanged()
		{
			_viewModel.InitializeRecursively(2);

			Bind(_viewModel, vm => ((TestViewModel)vm).Model.Model.Thickness);
			_control.Margin.Should().Be(new Thickness());

			_viewModel.Model.Model = new TestViewModel { Thickness = _margin };
			_control.Margin.Should().Be(_margin);
		}

		[Test]
		public void Source_Property_Property_Property_ValueChanged()
		{
			_viewModel.InitializeRecursively(2);

			Bind(_viewModel, vm => ((TestViewModel)vm).Model.Model.Thickness);
			_control.Margin.Should().Be(new Thickness());

			_viewModel.Model.Model.Thickness = _margin;
			_control.Margin.Should().Be(_margin);
		}

		[Test]
		public void Source_Property_Property_ValueChanged()
		{
			_viewModel.InitializeRecursively(1);

			Bind(_viewModel, vm => ((TestViewModel)vm).Model.Thickness);
			_control.Margin.Should().Be(new Thickness());

			_viewModel.Model.Thickness = _margin;
			_control.Margin.Should().Be(_margin);
		}

		[Test]
		public void ViewModel_Property()
		{
			_viewModel.Thickness = _margin;
			BindToViewModel(vm => ((TestViewModel)vm).Thickness);

			_control.Margin.Should().Be(_margin);
		}

		[Test]
		public void ViewModel_Property_Changed()
		{
			BindToViewModel(vm => ((TestViewModel)vm).Thickness);
			_control.Margin.Should().Be(new Thickness());

			_viewModel.Thickness = _margin;
			_control.Margin.Should().Be(_margin);
		}

		[Test]
		public void ViewModel_Property_Property()
		{
			_viewModel.InitializeRecursively(1);
			_viewModel.Model.Thickness = _margin;

			BindToViewModel(vm => ((TestViewModel)vm).Model.Thickness);

			_control.Margin.Should().Be(_margin);
		}

		[Test]
		public void ViewModel_Property_Property_PropertyChanged()
		{
			_viewModel.InitializeRecursively(1);

			BindToViewModel(vm => ((TestViewModel)vm).Model.Thickness);
			_control.Margin.Should().Be(new Thickness());

			_viewModel.Model = new TestViewModel { Thickness = _margin };
			_control.Margin.Should().Be(_margin);
		}

		[Test]
		public void ViewModel_Property_Property_ValueChanged()
		{
			_viewModel.InitializeRecursively(1);

			BindToViewModel(vm => ((TestViewModel)vm).Model.Thickness);
			_control.Margin.Should().Be(new Thickness());

			_viewModel.Model.Thickness = _margin;
			_control.Margin.Should().Be(_margin);
		}

		[Test]
		public void ViewModel_NotSet()
		{
			_control.ViewModel = null;
			BindWidth(vm => ((TestViewModel)vm).Width);

			_control.Width.Should().Be(UIElement.WidthProperty.DefaultValue);
		}

		[Test]
		public void ViewModel_Remove()
		{
			_control.ViewModel = _viewModel;
			_viewModel.Width = Width;

			BindWidth(vm => ((TestViewModel)vm).Width);
			_control.Width.Should().Be(Width);

			_control.ViewModel = null;
			_control.Width.Should().Be(UIElement.WidthProperty.DefaultValue);
		}

		[Test]
		public void ViewModel_Property_Property_ViewModel_NotSet()
		{
			_control.ViewModel = null;
			BindWidth(vm => ((TestViewModel)vm).Model.Width);

			_control.Width.Should().Be(UIElement.WidthProperty.DefaultValue);
		}

		[Test]
		public void ViewModel_Property_Property_NotSet()
		{
			_control.ViewModel = _viewModel;
			BindWidth(vm => ((TestViewModel)vm).Model.Width);

			_control.Width.Should().Be(UIElement.WidthProperty.DefaultValue);
		}

		[Test]
		public void ViewModel_Property_Property_RemoveViewModel()
		{
			_viewModel.InitializeRecursively(1);
			_control.ViewModel = _viewModel;
			_viewModel.Model.Width = Width;

			BindWidth(vm => ((TestViewModel)vm).Model.Width);
			_control.Width.Should().Be(Width);

			_control.ViewModel = null;
			_control.Width.Should().Be(UIElement.WidthProperty.DefaultValue);
		}

		[Test]
		public void ViewModel_Property_Property_RemoveProperty()
		{
			_viewModel.InitializeRecursively(1);
			_control.ViewModel = _viewModel;
			_viewModel.Model.Width = Width;

			BindWidth(vm => ((TestViewModel)vm).Model.Width);
			_control.Width.Should().Be(Width);

			_viewModel.Model = null;
			_control.Width.Should().Be(UIElement.WidthProperty.DefaultValue);
		}
	}
}