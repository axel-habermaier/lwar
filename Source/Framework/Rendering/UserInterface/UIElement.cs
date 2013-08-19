using System;

namespace Pegasus.Framework.Rendering.UserInterface
{
	using Platform.Assets;
	using Platform.Graphics;

	/// <summary>
	///   Provides layouting, input, and other base functionality for all UI elements.
	/// </summary>
	public abstract class UIElement : Visual
	{
		/// <summary>
		///   The view model of the UI element.
		/// </summary>
		public static readonly DependencyProperty<ViewModel> ViewModelProperty = new DependencyProperty<ViewModel>();

		/// <summary>
		///   The foreground color of the UI element.
		/// </summary>
		public static readonly DependencyProperty<Color> ForegroundProperty = new DependencyProperty<Color>(new Color(255, 255, 255, 255));

		/// <summary>
		///   The style of the UI element.
		/// </summary>
		public static readonly DependencyProperty<Style> StyleProperty = new DependencyProperty<Style>();

		/// <summary>
		///   The font used for text rendering by the UI element.
		/// </summary>
		public static readonly DependencyProperty<AssetIdentifier<Font>> FontProperty = new DependencyProperty<AssetIdentifier<Font>>();

		/// <summary>
		///   The resources used by the UI element.
		/// </summary>
		internal static readonly DependencyProperty<ResourceDictionary> ResourcesProperty = new DependencyProperty<ResourceDictionary>();

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		protected UIElement()
		{
			AddChangeHandler(StyleProperty, OnStyleChanged);
		}

		/// <summary>
		///   Gets or sets the view model of the UI element.
		/// </summary>
		public ViewModel ViewModel
		{
			get { return GetValue(ViewModelProperty); }
			set { SetValue(ViewModelProperty, value); }
		}

		/// <summary>
		///   Gets or sets the foreground color of the UI element.
		/// </summary>
		public Color Foreground
		{
			get { return GetValue(ForegroundProperty); }
			set { SetValue(ForegroundProperty, value); }
		}

		/// <summary>
		///   Gets or sets the style of the UI element.
		/// </summary>
		public Style Style
		{
			get { return GetValue(StyleProperty); }
			set
			{
				Assert.ArgumentNotNull(value);
				SetValue(StyleProperty, value);
			}
		}

		/// <summary>
		///   Gets or sets the font used for text rendering by the UI element.
		/// </summary>
		public AssetIdentifier<Font> Font
		{
			get { return GetValue(FontProperty); }
			set { SetValue(FontProperty, value); }
		}

		/// <summary>
		///   Gets or sets the resources used by the UI element.
		/// </summary>
		public ResourceDictionary Resources
		{
			get
			{
				var resources = GetValue(ResourcesProperty);

				if (resources == null)
				{
					resources = new ResourceDictionary();
					SetValue(ResourcesProperty, resources);
				}

				return resources;
			}
			set
			{
				if (value == null)
					value = new ResourceDictionary();

				SetValue(ResourcesProperty, value);
			}
		}

		/// <summary>
		///   Applies a style change to the UI element.
		/// </summary>
		private void OnStyleChanged(DependencyObject obj, DependencyProperty property)
		{
			Assert.NotNull(Style);
			Style.Apply(this);
		}

		/// <summary>
		///   Assigns a dynamic resource reference to the given dependency property. When the resource changes, the dependency
		///   property is updated accordingly.
		/// </summary>
		/// <typeparam name="T">The type of the value stored by the dependency property.</typeparam>
		/// <param name="property">The dependency property that should be set to the resource.</param>
		/// <param name="key">The key of the resource that should be bound to the dependency property.</param>
		public void SetResourceReference<T>(DependencyProperty<T> property, string key)
		{
			Assert.ArgumentNotNull(property);
			Assert.ArgumentNotNullOrWhitespace(key);

			new ResourceBinding<T>(this, property, key);
		}

		/// <summary>
		/// Searches the tree for a resource with the given key.
		/// </summary>
		/// <param name="key">The key of the resource that should be returned.</param>
		/// <param name="resource">Returns the resource with the specified key, if it is found.</param>
		internal bool TryFindResource(string key, out object resource)
		{
			Assert.ArgumentNotNullOrWhitespace(key);

			// If the key is in our resource dictionary, return the resource
			var resources = GetValue(ResourcesProperty);
			if (resources != null && resources.TryGetValue(key, out resource))
				return true;

			// Otherwise, check the logical parent
			if (LogicalParent != null)
				return LogicalParent.TryFindResource(key, out resource);

			// If there is no logical parent, there is no resource with the given key
			resource = null;
			return false;
		}

		/// <summary>
		/// Gets or sets the logical parent of the UI element.
		/// </summary>
		internal UIElement LogicalParent { get; set; }
	}
}