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
		///   The width of the UI element, measured in pixels.
		/// </summary>
		public static readonly DependencyProperty<double> WidthProperty = new DependencyProperty<double>();

		/// <summary>
		///   The height of the UI element, measured in pixels.
		/// </summary>
		public static readonly DependencyProperty<double> HeightProperty = new DependencyProperty<double>();

		/// <summary>
		///   The minimum width constraint of the UI element, measured in pixels.
		/// </summary>
		public static readonly DependencyProperty<double> MinWidthProperty = new DependencyProperty<double>();

		/// <summary>
		///   The minimum height constraint of the UI element, measured in pixels.
		/// </summary>
		public static readonly DependencyProperty<double> MinHeightProperty = new DependencyProperty<double>();

		/// <summary>
		///   The maximum width constraint of the UI element, measured in pixels.
		/// </summary>
		public static readonly DependencyProperty<double> MaxWidthProperty = new DependencyProperty<double>();

		/// <summary>
		///   The maximum height constraint of the UI element, measured in pixels.
		/// </summary>
		public static readonly DependencyProperty<double> MaxHeightProperty = new DependencyProperty<double>();

		/// <summary>
		///   The actual width of the UI element, measured in pixels, as determined by the layouting system.
		/// </summary>
		public static readonly DependencyProperty<double> ActualWidthProperty = new DependencyProperty<double>();

		/// <summary>
		///   The actual height of the UI element, measured in pixels, as determined by the layouting system.
		/// </summary>
		public static readonly DependencyProperty<double> ActualHeightProperty = new DependencyProperty<double>();

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
		///   Gets or sets the width of the UI element, measured in pixels.
		/// </summary>
		public double Width
		{
			get { return GetValue(WidthProperty); }
			set { SetValue(WidthProperty, value); }
		}

		/// <summary>
		///   Gets or sets the height of the UI element, measured in pixels.
		/// </summary>
		public double Height
		{
			get { return GetValue(HeightProperty); }
			set { SetValue(HeightProperty, value); }
		}

		/// <summary>
		///   Gets or sets the minimum width constraint of the UI element, measured in pixels.
		/// </summary>
		public double MinWidth
		{
			get { return GetValue(MinWidthProperty); }
			set { SetValue(MinWidthProperty, value); }
		}

		/// <summary>
		///   Gets or sets the minimum height constraint of the UI element, measured in pixels.
		/// </summary>
		public double MinHeight
		{
			get { return GetValue(MinHeightProperty); }
			set { SetValue(MinHeightProperty, value); }
		}

		/// <summary>
		///   Gets or sets the maximum width constraint of the UI element, measured in pixels.
		/// </summary>
		public double MaxWidth
		{
			get { return GetValue(MaxWidthProperty); }
			set { SetValue(MaxWidthProperty, value); }
		}

		/// <summary>
		///   Gets or sets the maximum height constraint of the UI element, measured in pixels.
		/// </summary>
		public double MaxHeight
		{
			get { return GetValue(MaxHeightProperty); }
			set { SetValue(MaxHeightProperty, value); }
		}

		/// <summary>
		///   Gets  the actual width of the UI element, measured in pixels, as determined by the layouting system.
		/// </summary>
		public double ActualWidth
		{
			get { return GetValue(ActualWidthProperty); }
		}

		/// <summary>
		///   Gets the actual height of the UI element, measured in pixels, as determined by the layouting system.
		/// </summary>
		public double ActualHeight
		{
			get { return GetValue(ActualHeightProperty); }
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
		///   Gets an enumerator that can be used to enumerate all logical children of the UI element.
		/// </summary>
		protected abstract UIElementCollection.Enumerator LogicalChildren { get; }

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

			foreach (var child in LogicalChildren)
				child.InvalidateResources();
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
		///   Attaches a resource binding to the dependency property. When the resource changes, the dependency
		///   property is updated accordingly.
		/// </summary>
		/// <typeparam name="T">The type of the value stored by the dependency property.</typeparam>
		/// <param name="property">The dependency property that the resource should be bound to.</param>
		/// <param name="binding">The binding that should be set.</param>
		public void SetResourceBinding<T>(DependencyProperty<T> property, ResourceBinding<T> binding)
		{
			Assert.ArgumentNotNull(property);
			Assert.ArgumentNotNull(binding);

			binding.Initialize(this, property);
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

		/// <summary>
		///   Changes the logical parent of the UI element.
		/// </summary>
		/// <param name="element">
		///   The new logical parent of the UI element. If null, the UI element is no longer part of the logical tree.
		/// </param>
		internal void ChangeLogicalParent(UIElement element)
		{
			if (element == Parent)
				return;

			Assert.That(element != this, "Detected a loop in the logical tree.");
			Assert.That(element == null || Parent == null, "The element is already attached to the logical tree.");

			Parent = element;

			if (element != null)
				InvalidateResources();
		}
	}
}