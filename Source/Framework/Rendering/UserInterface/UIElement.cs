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
		private static readonly DependencyProperty<ResourceDictionary> ResourcesProperty = new DependencyProperty<ResourceDictionary>();

		/// <summary>
		///   Indicates whether the mouse is currently hovering the UI element.
		/// </summary>
		public static readonly DependencyProperty<bool> IsMouseOverProperty = new DependencyProperty<bool>();

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		protected UIElement()
		{
			AddChangedHandler(StyleProperty, OnStyleChanged);
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

					AttachResourcesEventHandlers();
				}

				return resources;
			}
			set
			{
				DetachResourcesEventHandlers();

				if (value == null)
					value = new ResourceDictionary();

				SetValue(ResourcesProperty, value);
				AttachResourcesEventHandlers();

				InvalidateResources();
			}
		}

		/// <summary>
		///   Indicates whether the mouse is currently hovering the UI element.
		/// </summary>
		public bool IsMouseOver
		{
			get { return GetValue(IsMouseOverProperty); }
			set { SetValue(IsMouseOverProperty, value); }
		}

		/// <summary>
		///   Gets the logical parent of the UI element.
		/// </summary>
		public UIElement Parent { get; internal set; }

		/// <summary>
		///   Attaches the resources change event handlers.
		/// </summary>
		private void AttachResourcesEventHandlers()
		{
			var resources = GetValue(ResourcesProperty);
			Assert.NotNull(resources);

			resources.ResourceChanged += ResourceChanged;
		}

		/// <summary>
		///   Detaches the resources change event handlers.
		/// </summary>
		private void DetachResourcesEventHandlers()
		{
			var resources = GetValue(ResourcesProperty);
			if (resources == null)
				return;

			resources.ResourceChanged -= ResourceChanged;
		}

		/// <summary>
		///   Raised when a change to a resource dictionary in this UI element or one of its ancestors has occurred.
		/// </summary>
		internal event Action ResourcesInvalidated;

		/// <summary>
		///   Invoked when a resource within the resource dictionary has been replaced, invalidating all resources for this UI
		///   element and all of its children.
		/// </summary>
		private void ResourceChanged(ResourceDictionary resources, string key)
		{
			InvalidateResources();
		}

		/// <summary>
		///   Raises the resources invalidated event for this UI element and all of its children.
		/// </summary>
		private void InvalidateResources()
		{
			if (ResourcesInvalidated != null)
				ResourcesInvalidated();

			// TODO: Invalidate children
		}

		/// <summary>
		///   Applies a style change to the UI element.
		/// </summary>
		private void OnStyleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs<Style> property)
		{
			Assert.NotNull(property.NewValue);

			if (property.OldValue != null)
				property.OldValue.Unset(this);

			property.NewValue.Seal();
			property.NewValue.Apply(this);
		}

		/// <summary>
		///   Assigns a dynamic resource reference to the given dependency property. When the resource changes, the dependency
		///   property is updated accordingly.
		/// </summary>
		/// <typeparam name="T">The type of the value stored by the dependency property.</typeparam>
		/// <param name="property">The dependency property that the resource should be bound to.</param>
		/// <param name="key">The key of the resource that should be bound to the dependency property.</param>
		public void SetResourceReference<T>(DependencyProperty<T> property, string key)
		{
			Assert.ArgumentNotNull(property);
			Assert.ArgumentNotNullOrWhitespace(key);

			new ResourceBinding<T>(this, property, key);
		}

		/// <summary>
		///   Searches the tree for a resource with the given key.
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
			if (Parent != null)
				return Parent.TryFindResource(key, out resource);

			// If there is no logical parent, there is no resource with the given key
			resource = null;
			return false;
		}

		protected void AddLogicalChild(UIElement element)
		{
			
		}
	}
}