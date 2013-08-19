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
		///   Applies a style change to the UI element.
		/// </summary>
		private void OnStyleChanged(DependencyObject obj, DependencyProperty property)
		{
			Assert.NotNull(Style);
			Style.Apply(this);
		}
	}
}