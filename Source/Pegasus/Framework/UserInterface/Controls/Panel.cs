namespace Pegasus.Framework.UserInterface.Controls
{
	using System;
	using Math;
	using Platform.Graphics;
	using Rendering;

	/// <summary>
	///   A base class for all panel elements that position and arrange child UI elements.
	/// </summary>
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
		///   A value indicating whether the child UI elements must be sorted by z-index again before drawing.
		/// </summary>
		private bool _zOrderDirty;

		/// <summary>
		///   Initializes the type.
		/// </summary>
		static Panel()
		{
			ZIndexProperty.Changed += OnZIndexChanged;
		}

		/// <summary>
		///   Gets an enumerator that can be used to enumerate all logical children of the panel.
		/// </summary>
		protected internal override sealed UIElementCollection.Enumerator LogicalChildren
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
		protected internal override sealed int VisualChildrenCount
		{
			get { return _children == null ? 0 : _children.Count; }
		}

		/// <summary>
		///   Sets the z-order dirty flag if a panel's child element's z-index property has changed.
		/// </summary>
		private static void OnZIndexChanged(DependencyObject obj, DependencyPropertyChangedEventArgs<int> args)
		{
			var uiElement = obj as UIElement;
			if (uiElement == null)
				return;

			var panel = uiElement.Parent as Panel;
			if (panel == null)
				return;

			panel._zOrderDirty = true;
		}

		/// <summary>
		///   Invoked when the visual children of the UI element have changed.
		/// </summary>
		protected internal override void OnVisualChildrenChanged()
		{
			_zOrderDirty = true;
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
		protected internal override sealed UIElement GetVisualChild(int index)
		{
			Assert.NotNull(_children);
			Assert.ArgumentInRange(index, _children);

			if (_zOrderDirty)
				SortByZIndex();

			return _children[index];
		}

		/// <summary>
		///   Sorts the child UI elements by their z-indices. Insertion sort is used as typically the number of
		///   child UI elements is rather low and they are mostly sorted anyway.
		/// </summary>
		private void SortByZIndex()
		{
			for (var i = 1; i < _children.Count; ++i)
			{
				var item = _children[i];
				var itemZIndex = GetZIndex(item);

				// Shift down the items until we find the index where the current item should be inserted.
				var j = i;
				for (; j > 0 && itemZIndex < GetZIndex(_children[j - 1]); --j)
					_children.ReplaceItemWithoutNotifications(j, _children[j - 1]);

				_children.ReplaceItemWithoutNotifications(j, item);
			}

			_zOrderDirty = false;
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