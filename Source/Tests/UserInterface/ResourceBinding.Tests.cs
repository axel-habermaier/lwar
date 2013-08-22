using System;

namespace Tests.UserInterface
{
	using FluentAssertions;
	using NUnit.Framework;
	using Pegasus.Framework.UserInterface;

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

		private const string Key = "Margin";

		private readonly Thickness _thickness1 = new Thickness(4);
		private readonly Thickness _thickness2 = new Thickness(4);
		private readonly Thickness _thickness3 = new Thickness(8);

		private const double Width = 4;

		private static void AddBinding(UIElement control)
		{
			var binding = new ResourceBinding<Thickness>(Key);
			control.SetResourceBinding(UIElement.MarginProperty, binding);
		}

		private static void AddWidthBinding(UIElement control)
		{
			var binding = new ResourceBinding<double>(Key);
			control.SetResourceBinding(UIElement.WidthProperty, binding);
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
		public void OnElementWithResourceDefined_RemoveResource()
		{
			_control1.Resources[Key] = _thickness1;
			AddBinding(_control1);
			_control1.Margin.Should().Be(_thickness1);

			_control1.Resources = new ResourceDictionary();
			_control1.Margin.Should().Be(new Thickness());
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
		public void ResourceDefinedByParent_RemoveResource()
		{
			_control1.Resources[Key] = _thickness1;
			AddBinding(_control1.Button3);
			_control1.Button3.Margin.Should().Be(_thickness1);

			_control1.Resources = new ResourceDictionary();
			_control1.Button3.Margin.Should().Be(new Thickness());
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
		public void ResourceOverriden()
		{
			_control1.Resources[Key] = _thickness1;
			_control1.Canvas1.Resources[Key] = _thickness2;
			AddBinding(_control1.Button3);
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
		public void ResourceOverriden_RemoveOverridingResourceDictionary()
		{
			_control1.Resources[Key] = _thickness3;
			_control1.Canvas1.Resources[Key] = _thickness1;
			AddBinding(_control1.Button3);
			_control1.Button3.Margin.Should().Be(_thickness1);

			_control1.Canvas1.Resources = null;
			_control1.Button3.Margin.Should().Be(_thickness3);
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
	}
}