using System;

namespace Pegasus.Framework.UserInterface.Controls
{
	using Rendering;

	/// <summary>
	///   A base class for all panel elements that position and arrange child UI elements.
	/// </summary>
	[ContentProperty("Children")]
	public abstract class Panel : UIElement
	{
		/// <summary>
		///   The collection of layouted children.
		/// </summary>
		private UIElementCollection _children;

		/// <summary>
		///   Gets an enumerator that can be used to enumerate all logical children of the panel.
		/// </summary>
		protected internal override UIElementCollection.Enumerator LogicalChildren
		{
			get
			{
				if (Children == null)
					return UIElementCollection.Enumerator.Empty;

				return Children.GetEnumerator();
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
		protected internal override int VisualChildrenCount
		{
			get { return _children == null ? 0 : _children.Count; }
		}

		/// <summary>
		///   Gets the visual child at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the visual child that should be returned.</param>
		protected internal override Visual GetVisualChild(int index)
		{
			Assert.NotNull(_children);
			Assert.ArgumentInRange(index, _children);

			return _children[index];
		}

		protected override void OnDraw(SpriteBatch spriteBatch)
		{
		}
	}
}