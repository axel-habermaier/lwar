namespace Pegasus.Framework.UserInterface.Controls
{
	using System;
	using Math;
	using Platform.Graphics;
	using Rendering;

	/// <summary>
	///   A base class for all panel elements that position and arrange child UI elements.
	/// </summary>
	[ContentProperty("Children")]
	public abstract class Panel : UIElement
	{
		/// <summary>
		///   Represents the order on the z-plane in which an element appears. Elements with higher z indices are drawn above ones
		///   with lower indices.
		/// </summary>
		public static readonly DependencyProperty<int> ZIndexProperty =
			new DependencyProperty<int>(defaultValue: 0, affectsRender: true);

		/// <summary>
		///   The collection of layouted children.
		/// </summary>
		private UIElementCollection _children;

		/// <summary>
		///   Gets an enumerator that can be used to enumerate all logical children of the panel.
		/// </summary>
		protected sealed internal override UIElementCollection.Enumerator LogicalChildren
		{
			get
			{
				if (_children == null)
					return UIElementCollection.Enumerator.Empty;

				return _children.GetEnumerator();
			}
		}

		/// <summary>
		///   Gets the collection of child UI elements managed by this layout.
		/// </summary>
		public UIElementCollection Children
		{
			get { return _children ?? (_children = new UIElementCollection(this)); }
		}

		/// <summary>
		///   Gets the number of visual children for this visual.
		/// </summary>
		protected sealed internal override int VisualChildrenCount
		{
			get { return _children == null ? 0 : _children.Count; }
		}

		/// <summary>
		///   Gets the z-index of the given dependency object.
		/// </summary>
		public static int GetZIndex(DependencyObject obj)
		{
			Assert.ArgumentNotNull(obj);
			return obj.GetValue(ZIndexProperty);
		}

		/// <summary>
		///   Sets the z-index of the given dependency object.
		/// </summary>
		public static void SetZIndex(DependencyObject obj, int zIndex)
		{
			Assert.ArgumentNotNull(obj);
			obj.SetValue(ZIndexProperty, zIndex);
		}

		/// <summary>
		///   Gets the visual child at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the visual child that should be returned.</param>
		protected sealed internal override UIElement GetVisualChild(int index)
		{
			Assert.NotNull(_children);
			Assert.ArgumentInRange(index, _children);

			return _children[index];
		}

		protected override void OnDraw(SpriteBatch spriteBatch)
		{
			var width = (int)Math.Round(ActualWidth);
			var height = (int)Math.Round(ActualHeight);
			var x = (int)Math.Round(VisualOffset.X);
			var y = (int)Math.Round(VisualOffset.Y);

			spriteBatch.Draw(new Rectangle(x, y, width, height), Texture2D.White, Background);
		}
	}
}