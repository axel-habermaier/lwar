using System;

namespace Tests.UserInterface
{
	using FluentAssertions;
	using NUnit.Framework;
	using Pegasus.Framework.UserInterface;
	using Pegasus.Framework.UserInterface.Controls;

	[TestFixture]
	public class ResourceBindingTests
	{
		[SetUp]
		public void Setup()
		{
			_control1 = new TestControl();
			_control2 = new TestControl();

			// Sanity check
			UIElement.WidthProperty.DefaultValue.Should().NotBe(0);
		}

		private TestControl _control1;
		private TestControl _control2;

		private static readonly Style ImplicitButtonStyle1 = new Style();
		private static readonly Style ImplicitButtonStyle2 = new Style();

		private const string Key = "Margin";

		private readonly Thickness _thickness1 = new Thickness(4);
		private readonly Thickness _thickness2 = new Thickness(4);
		private readonly Thickness _thickness3 = new Thickness(8);

		private const double Width = 4;

		private static void AddBinding(UIElement element)
		{
			var binding = new ResourceBinding<Thickness>(Key);
			element.SetBinding(UIElement.MarginProperty, binding);
		}

		private static void AddWidthBinding(UIElement element)
		{
			var binding = new ResourceBinding<double>(Key);
			element.SetBinding(UIElement.WidthProperty, binding);
		}

		private static void SetImplicitStyle1(UIElement element)
		{
			element.Resources[typeof(Button)] = ImplicitButtonStyle1;
		}

		private static void SetImplicitStyle2(UIElement element)
		{
			element.Resources[typeof(Button)] = ImplicitButtonStyle2;
		}

		[Test]
		public void ChangeTree_ResourceDefined()
		{
			_control1.Resources[Key] = _thickness1;
			_control2.Resources[Key] = _thickness2;

			AddBinding(_control1.Button3);
			_control1.Button3.Margin.Should().Be(_thickness1);

			_control1.Canvas2.Children.Remove(_control1.Button3);
			_control2.Canvas1.Children.Add(_control1.Button3);
			_control1.Button3.Margin.Should().Be(_thickness2);
		}

		[Test]
		public void ChangeTree_ResourceNotDefined()
		{
			_control1.Resources[Key] = Width;

			AddWidthBinding(_control1.Button3);
			_control1.Button3.Width.Should().Be(Width);

			_control1.Canvas2.Children.Remove(_control1.Button3);
			_control2.Canvas1.Children.Add(_control1.Button3);
			_control1.Button3.Width.Should().Be(UIElement.WidthProperty.DefaultValue);
		}

		[Test]
		public void ImplicitStyle_ChangeResourceDictionary_Parent()
		{
			SetImplicitStyle1(_control1);
			_control1.Resources = new ResourceDictionary();

			_control1.Button1.Style.Should().Be(null);
			_control1.Button2.Style.Should().Be(null);
			_control1.Button3.Style.Should().Be(null);
		}

		[Test]
		public void ImplicitStyle_ChangeResourceDictionary_SameElement()
		{
			SetImplicitStyle1(_control1.Button2);
			_control1.Button2.Resources = new ResourceDictionary();

			_control1.Button1.Style.Should().Be(null);
			_control1.Button2.Style.Should().Be(null);
			_control1.Button3.Style.Should().Be(null);
		}

		[Test]
		public void ImplicitStyle_ChangeResource_Parent()
		{
			SetImplicitStyle1(_control1);
			SetImplicitStyle2(_control1);

			_control1.Button1.Style.Should().Be(ImplicitButtonStyle2);
			_control1.Button2.Style.Should().Be(ImplicitButtonStyle2);
			_control1.Button3.Style.Should().Be(ImplicitButtonStyle2);
		}

		[Test]
		public void ImplicitStyle_ChangeResource_SameElement()
		{
			SetImplicitStyle1(_control1.Button2);
			SetImplicitStyle2(_control1.Button2);

			_control1.Button1.Style.Should().Be(null);
			_control1.Button2.Style.Should().Be(ImplicitButtonStyle2);
			_control1.Button3.Style.Should().Be(null);
		}

		[Test]
		public void ImplicitStyle_Find_Parent()
		{
			SetImplicitStyle1(_control1);

			_control1.Button1.Style.Should().Be(ImplicitButtonStyle1);
			_control1.Button2.Style.Should().Be(ImplicitButtonStyle1);
			_control1.Button3.Style.Should().Be(ImplicitButtonStyle1);
		}

		[Test]
		public void ImplicitStyle_Find_SameElement()
		{
			SetImplicitStyle1(_control1.Button2);

			_control1.Button1.Style.Should().Be(null);
			_control1.Button2.Style.Should().Be(ImplicitButtonStyle1);
			_control1.Button3.Style.Should().Be(null);
		}

		[Test]
		public void NoValue()
		{
			AddWidthBinding(_control1);
			_control1.Width.Should().Be(UIElement.WidthProperty.DefaultValue);
		}

		[Test]
		public void OnElementWithResourceDefined()
		{
			_control1.Resources[Key] = _thickness1;
			AddBinding(_control1);

			_control1.Margin.Should().Be(_thickness1);
		}

		[Test]
		public void OnElementWithResourceDefined_ChangeResource()
		{
			_control1.Resources[Key] = _thickness1;
			AddBinding(_control1);
			_control1.Margin.Should().Be(_thickness1);

			_control1.Resources[Key] = _thickness2;
			_control1.Margin.Should().Be(_thickness2);
		}

		[Test]
		public void OnElementWithResourceDefined_ChangeResourceDictionary()
		{
			_control1.Resources[Key] = _thickness1;
			AddBinding(_control1);
			_control1.Margin.Should().Be(_thickness1);

			var r = new ResourceDictionary();
			r[Key] = _thickness2;
			_control1.Resources = r;
			_control1.Margin.Should().Be(_thickness2);
		}

		[Test]
		public void OnElementWithResourceDefined_RemoveResource()
		{
			_control1.Resources[Key] = _thickness1;
			AddBinding(_control1);
			_control1.Margin.Should().Be(_thickness1);

			_control1.Resources = new ResourceDictionary();
			_control1.Margin.Should().Be(new Thickness());
		}

		[Test]
		public void ResourceDefinedByParent()
		{
			_control1.Resources[Key] = _thickness1;
			AddBinding(_control1.Button3);

			_control1.Button3.Margin.Should().Be(_thickness1);
		}

		[Test]
		public void ResourceDefinedByParent_ChangeResource()
		{
			_control1.Resources[Key] = _thickness1;
			AddBinding(_control1.Button3);
			_control1.Button3.Margin.Should().Be(_thickness1);

			_control1.Resources[Key] = _thickness2;
			_control1.Button3.Margin.Should().Be(_thickness2);
		}

		[Test]
		public void ResourceDefinedByParent_ChangeResourceDictionary()
		{
			_control1.Resources[Key] = _thickness1;
			AddBinding(_control1.Button3);
			_control1.Button3.Margin.Should().Be(_thickness1);

			var r = new ResourceDictionary();
			r[Key] = _thickness2;
			_control1.Resources = r;
			_control1.Button3.Margin.Should().Be(_thickness2);
		}

		[Test]
		public void ResourceDefinedByParent_RemoveResource()
		{
			_control1.Resources[Key] = _thickness1;
			AddBinding(_control1.Button3);
			_control1.Button3.Margin.Should().Be(_thickness1);

			_control1.Resources = new ResourceDictionary();
			_control1.Button3.Margin.Should().Be(new Thickness());
		}

		[Test]
		public void ResourceOverriden()
		{
			_control1.Resources[Key] = _thickness1;
			_control1.Canvas1.Resources[Key] = _thickness2;
			AddBinding(_control1.Button3);
			_control1.Button3.Margin.Should().Be(_thickness2);
		}

		[Test]
		public void ResourceOverriden_ChangeOverridingResourceDictionary()
		{
			_control1.Resources[Key] = _thickness3;
			_control1.Canvas1.Resources[Key] = _thickness1;
			AddBinding(_control1.Button3);
			_control1.Button3.Margin.Should().Be(_thickness1);

			var r = new ResourceDictionary();
			r[Key] = _thickness2;
			_control1.Canvas1.Resources = r;
			_control1.Button3.Margin.Should().Be(_thickness2);
		}

		[Test]
		public void ResourceOverriden_ChangeResource()
		{
			_control1.Resources[Key] = _thickness1;
			AddBinding(_control1.Button3);
			_control1.Button3.Margin.Should().Be(_thickness1);

			_control1.Canvas1.Resources[Key] = _thickness2;
			_control1.Button3.Margin.Should().Be(_thickness2);
		}

		[Test]
		public void ResourceOverriden_RemoveOverridingResourceDictionary()
		{
			_control1.Resources[Key] = _thickness3;
			_control1.Canvas1.Resources[Key] = _thickness1;
			AddBinding(_control1.Button3);
			_control1.Button3.Margin.Should().Be(_thickness1);

			_control1.Canvas1.Resources = null;
			_control1.Button3.Margin.Should().Be(_thickness3);
		}
	}
}