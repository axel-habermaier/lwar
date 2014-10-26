namespace Tests.UserInterface
{
	using System;
	using FluentAssertions;
	using NUnit.Framework;
	using Pegasus.UserInterface;
	using Pegasus.UserInterface.Controls;

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
		private static readonly Style ExplicitButtonStyle = new Style();

		private const string Key = "Margin";

		private readonly Thickness _thickness1 = new Thickness(2);
		private readonly Thickness _thickness2 = new Thickness(4);
		private readonly Thickness _thickness3 = new Thickness(8);

		private const float Width = 4;

		private static void AddBinding(UIElement element)
		{
			element.CreateResourceBinding(Key, UIElement.MarginProperty);
		}

		private static void AddWidthBinding(UIElement element)
		{
			element.CreateResourceBinding(Key, UIElement.WidthProperty);
		}

		private static void SetImplicitStyle1(UIElement element)
		{
			element.Resources.AddOrReplace(typeof(Button), ImplicitButtonStyle1);
		}

		private static void SetImplicitStyle2(UIElement element)
		{
			element.Resources.AddOrReplace(typeof(Button), ImplicitButtonStyle2);
		}

		[Test]
		public void ChangeTree_ResourceDefined()
		{
			_control1.Resources.Add(Key, _thickness1);
			_control2.Resources.Add(Key, _thickness2);

			AddBinding(_control1.Button3);
			_control1.Button3.Margin.Should().Be(_thickness1);

			_control1.Canvas2.Children.Remove(_control1.Button3);
			_control2.Canvas1.Children.Add(_control1.Button3);
			_control1.Button3.Margin.Should().Be(_thickness2);
		}

		[Test]
		public void ChangeTree_ResourceNotDefined()
		{
			_control1.Resources.Add(Key, Width);

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
			_control1.Resources.Clear();

			_control1.Button1.Style.Should().Be(null);
			_control1.Button2.Style.Should().Be(null);
			_control1.Button3.Style.Should().Be(null);
		}

		[Test]
		public void ImplicitStyle_ChangeResourceDictionary_SameElement()
		{
			SetImplicitStyle1(_control1.Button2);
			_control1.Button2.Resources.Clear();

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
		public void ImplicitStyle_OverriddenByExplicitStyle_ExplicitSetFirst()
		{
			_control1.Button1.Style = ExplicitButtonStyle;
			SetImplicitStyle1(_control1);

			_control1.Button1.Style.Should().Be(ExplicitButtonStyle);
		}

		[Test]
		public void ImplicitStyle_OverriddenByExplicitStyle_ImplicitSetFirst()
		{
			SetImplicitStyle1(_control1);
			_control1.Button1.Style = ExplicitButtonStyle;

			_control1.Button1.Style.Should().Be(ExplicitButtonStyle);
		}

		[Test]
		public void ImplicitStyle_Reparent_BothImplicit()
		{
			var button = new Button();
			var userControl1 = new UserControl { IsAttachedToRoot = true };
			var userControl2 = new UserControl { IsAttachedToRoot = true };

			SetImplicitStyle1(userControl1);
			SetImplicitStyle2(userControl2);

			userControl1.Content = button;
			button.Style.Should().Be(ImplicitButtonStyle1);

			userControl1.Content = null;
			userControl2.Content = button;
			button.Style.Should().Be(ImplicitButtonStyle2);
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
			_control1.Resources.Add(Key, _thickness1);
			AddBinding(_control1);

			_control1.Margin.Should().Be(_thickness1);
		}

		[Test]
		public void OnElementWithResourceDefined_ChangeResource()
		{
			_control1.Resources.Add(Key, _thickness1);
			AddBinding(_control1);
			_control1.Margin.Should().Be(_thickness1);

			_control1.Resources.AddOrReplace(Key, _thickness2);
			_control1.Margin.Should().Be(_thickness2);
		}

		[Test]
		public void OnElementWithResourceDefined_ChangeResourceDictionary()
		{
			_control1.Resources.Add(Key, _thickness1);
			AddBinding(_control1);
			_control1.Margin.Should().Be(_thickness1);

			_control1.Resources.Clear();
			_control1.Resources.Add(Key, _thickness2);
			_control1.Margin.Should().Be(_thickness2);
		}

		[Test]
		public void OnElementWithResourceDefined_RemoveResource()
		{
			_control1.Resources.Add(Key, _thickness1);
			AddBinding(_control1);
			_control1.Margin.Should().Be(_thickness1);

			_control1.Resources.Clear();
			_control1.Margin.Should().Be(new Thickness());
		}

		[Test]
		public void ResourceDefinedByParent()
		{
			_control1.Resources.Add(Key, _thickness1);
			AddBinding(_control1.Button3);

			_control1.Button3.Margin.Should().Be(_thickness1);
		}

		[Test]
		public void ResourceDefinedByParent_ChangeResource()
		{
			_control1.Resources.Add(Key, _thickness1);
			AddBinding(_control1.Button3);
			_control1.Button3.Margin.Should().Be(_thickness1);

			_control1.Resources.AddOrReplace(Key, _thickness2);
			_control1.Button3.Margin.Should().Be(_thickness2);
		}

		[Test]
		public void ResourceDefinedByParent_ChangeResourceDictionary()
		{
			_control1.Resources.Add(Key, _thickness1);
			AddBinding(_control1.Button3);
			_control1.Button3.Margin.Should().Be(_thickness1);

			_control1.Resources.Clear();
			_control1.Resources.Add(Key, _thickness2);
			_control1.Button3.Margin.Should().Be(_thickness2);
		}

		[Test]
		public void ResourceDefinedByParent_RemoveResource()
		{
			_control1.Resources.Add(Key, _thickness1);
			AddBinding(_control1.Button3);
			_control1.Button3.Margin.Should().Be(_thickness1);

			_control1.Resources.Clear();
			_control1.Button3.Margin.Should().Be(new Thickness());
		}

		[Test]
		public void ResourceOverridden()
		{
			_control1.Resources.Add(Key, _thickness1);
			_control1.Canvas1.Resources.Add(Key, _thickness2);
			AddBinding(_control1.Button3);
			_control1.Button3.Margin.Should().Be(_thickness2);
		}

		[Test]
		public void ResourceOverridden_ChangeOverridingResourceDictionary()
		{
			_control1.Resources.Add(Key, _thickness3);
			_control1.Canvas1.Resources.Add(Key, _thickness1);
			AddBinding(_control1.Button3);
			_control1.Button3.Margin.Should().Be(_thickness1);

			_control1.Canvas1.Resources.Clear();
			_control1.Canvas1.Resources.Add(Key, _thickness2);
			_control1.Button3.Margin.Should().Be(_thickness2);
		}

		[Test]
		public void ResourceOverridden_ChangeResource()
		{
			_control1.Resources.Add(Key, _thickness1);
			AddBinding(_control1.Button3);
			_control1.Button3.Margin.Should().Be(_thickness1);

			_control1.Canvas1.Resources.Add(Key, _thickness2);
			_control1.Button3.Margin.Should().Be(_thickness2);
		}

		[Test]
		public void ResourceOverridden_RemoveOverridingResourceDictionary()
		{
			_control1.Resources.Add(Key, _thickness3);
			_control1.Canvas1.Resources.Add(Key, _thickness1);
			AddBinding(_control1.Button3);
			_control1.Button3.Margin.Should().Be(_thickness1);

			_control1.Canvas1.Resources.Clear();
			_control1.Button3.Margin.Should().Be(_thickness3);
		}

		[Test]
		public void UnsetBinding()
		{
			_control1.Resources.Add(Key, _thickness1);
			AddBinding(_control1);

			_control1.Margin.Should().Be(_thickness1);

			_control1.Margin = _thickness2;
			_control1.Margin.Should().Be(_thickness2);

			_control1.Resources.AddOrReplace(Key, _thickness3);
			_control1.Margin.Should().Be(_thickness2);
		}
	}
}