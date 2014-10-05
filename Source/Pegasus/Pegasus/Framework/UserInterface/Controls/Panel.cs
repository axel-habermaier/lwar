﻿namespace Pegasus.Framework.UserInterface.Controls
{
	using System;
	using System.Collections.Generic;
	using Platform.Graphics;

	/// <summary>
	///     A base class for all panel elements that position and arrange child UI elements.
	/// </summary>
	public abstract class Panel : UIElement, IScrollAware
	{
		/// <summary>
		///     Represents the order on the z-plane in which an element appears. Elements with higher z indices are drawn above ones
		///     with lower indices.
		/// </summary>
		public static readonly DependencyProperty<int> ZIndexProperty =
			new DependencyProperty<int>(defaultValue: 0, affectsRender: true);

		/// <summary>
		///     A lookup table in which the child elements are sorted by z index.
		/// </summary>
		private readonly List<int> _zLookup = new List<int>(16);

		/// <summary>
		///     The collection of layouted children.
		/// </summary>
		private UIElementCollection _children;

		/// <summary>
		///     A value indicating whether the child UI elements must be sorted by z-index again before drawing.
		/// </summary>
		private bool _zOrderDirty;

		/// <summary>
		///     Initializes the type.
		/// </summary>
		static Panel()
		{
			ZIndexProperty.Changed += OnZIndexChanged;
		}

		/// <summary>
		///     Gets or sets a value indicating that the panel is a host for items generated by a control such as items control.
		/// </summary>
		public bool IsItemsHost { get; set; }

		/// <summary>
		///     Gets an enumerator that can be used to enumerate all logical children of the panel.
		/// </summary>
		protected internal override sealed Enumerator<UIElement> LogicalChildren
		{
			get
			{
				if (_children == null)
					return Enumerator<UIElement>.Empty;

				return _children.GetEnumerator();
			}
		}

		/// <summary>
		///     Gets the collection of child UI elements managed by this layout.
		/// </summary>
		public UIElementCollection Children
		{
			get { return _children ?? (_children = new UIElementCollection(this, this)); }
		}

		/// <summary>
		///     Gets the number of visual children for this visual.
		/// </summary>
		protected internal override sealed int VisualChildrenCount
		{
			get { return _children == null ? 0 : _children.Count; }
		}

		/// <summary>
		///     Gets or sets the scroll handler that handles this scrolling aware UI element.
		/// </summary>
		public IScrollHandler ScrollHandler { get; set; }

		/// <summary>
		///     Adds the given UI element to the panel.
		/// </summary>
		/// <param name="element">The UI element that should be added.</param>
		public void Add(UIElement element)
		{
			Assert.ArgumentNotNull(element);
			Children.Add(element);
		}

		/// <summary>
		///     Removes the given UI element from the panel.
		/// </summary>
		/// <param name="element">The UI element that should be removed.</param>
		public bool Remove(UIElement element)
		{
			Assert.ArgumentNotNull(element);
			return Children.Remove(element);
		}

		/// <summary>
		///     Removes all child UI elements from the panel.
		/// </summary>
		public void Clear()
		{
			Children.Clear();
		}

		/// <summary>
		///     Sets the z-order dirty flag if a panel's child element's z-index property has changed.
		/// </summary>
		private static void OnZIndexChanged(DependencyObject obj, DependencyPropertyChangedEventArgs<int> args)
		{
			var element = obj as UIElement;
			if (element == null)
				return;

			var panel = element.LogicalParent as Panel;
			if (panel == null)
				return;

			panel._zOrderDirty = true;
		}

		/// <summary>
		///     Invoked when the visual children of the UI element have changed.
		/// </summary>
		protected internal override void OnVisualChildrenChanged()
		{
			_zOrderDirty = true;
		}

		/// <summary>
		///     Gets the z-index of the given dependency object.
		/// </summary>
		public static int GetZIndex(DependencyObject obj)
		{
			Assert.ArgumentNotNull(obj);
			return obj.GetValue(ZIndexProperty);
		}

		/// <summary>
		///     Sets the z-index of the given dependency object.
		/// </summary>
		public static void SetZIndex(DependencyObject obj, int zIndex)
		{
			Assert.ArgumentNotNull(obj);
			obj.SetValue(ZIndexProperty, zIndex);
		}

		/// <summary>
		///     Gets the visual child at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the visual child that should be returned.</param>
		protected internal override sealed UIElement GetVisualChild(int index)
		{
			Assert.NotNull(_children);
			Assert.ArgumentInRange(index, _children);

			if (_zOrderDirty)
				SortByZIndex();

			return _children[_zLookup[index]];
		}

		/// <summary>
		///     Sorts the child UI elements by their z-indices. Insertion sort is used as typically the number of
		///     child UI elements is rather low and they are mostly sorted anyway.
		/// </summary>
		private void SortByZIndex()
		{
			_zLookup.Clear();
			for (var i = 0; i < _children.Count; ++i)
				_zLookup.Add(i);

			for (var i = 1; i < _children.Count; ++i)
			{
				var item = _children[i];
				var itemZIndex = GetZIndex(item);

				// Shift down the items until we find the index where the current item should be inserted.
				var j = i;
				for (; j > 0 && itemZIndex < GetZIndex(_children[_zLookup[j - 1]]); --j)
					_zLookup[j] = _zLookup[j - 1];

				_zLookup[j] = i;
			}

			_zOrderDirty = false;
		}

		/// <summary>
		///     Draws the child UI elements of the current UI element using the given sprite batch.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the UI element's children.</param>
		protected override void DrawChildren(SpriteBatch spriteBatch)
		{
			if (ScrollHandler != null)
			{
				// We draw the children on a higher layer to avoid draw ordering issues.
				++spriteBatch.Layer;

				// Only draw the children that are actually visible
				var count = VisualChildrenCount;
				var area = ScrollHandler.ScrollArea;
				for (var i = 0; i < count; ++i)
				{
					var child = GetVisualChild(i);

					var topIsInside = child.VisualOffset.Y <= area.Bottom;
					var bottomIsInside = child.VisualOffset.Y + child.ActualHeight >= area.Top;
					var leftIsInside = child.VisualOffset.X <= area.Right;
					var rightIsInside = child.VisualOffset.X + child.ActualWidth >= area.Left;

					if (topIsInside && bottomIsInside && leftIsInside && rightIsInside)
						child.Draw(spriteBatch);
				}

				--spriteBatch.Layer;
			}
			else
				base.DrawChildren(spriteBatch);
		}
	}
}