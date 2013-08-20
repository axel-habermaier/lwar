using System;

namespace Pegasus.Framework.UserInterface.Controls
{
	/// <summary>
	///   A base class for all layouts that position and arrange child UI elements.
	/// </summary>
	public class Layout : UIElement
	{
		private UIElementCollection _children;

		/// <summary>
		///   Gets an enumerator that can be used to enumerate all logical children of the panel.
		/// </summary>
		protected override UIElementCollection.Enumerator LogicalChildren
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
	}
}