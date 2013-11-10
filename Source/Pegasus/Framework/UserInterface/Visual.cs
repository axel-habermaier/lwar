namespace Pegasus.Framework.UserInterface
{
	using System;
	using Math;
	using Rendering;

	/// <summary>
	///   Provides rendering support for the UI system, also including hit testing.
	/// </summary>
	public abstract class Visual : DependencyObject
	{
		/// <summary>
		///   Indicates whether the UI element is visible.
		/// </summary>
		public static readonly DependencyProperty<Visibility> VisibilityProperty =
			new DependencyProperty<Visibility>(defaultValue: Visibility.Visible, affectsMeasure: true);

		/// <summary>
		///   Gets or sets the offset value of the visual.
		/// </summary>
		protected internal Vector2d VisualOffset { get; protected set; }

		/// <summary>
		///   Gets the number of visual children for this visual.
		/// </summary>
		protected internal virtual int VisualChildrenCount
		{
			get { return 0; }
		}

		/// <summary>
		///   Indicates whether the UI element is visible.
		/// </summary>
		public Visibility Visibility
		{
			get { return GetValue(VisibilityProperty); }
			set { SetValue(VisibilityProperty, value); }
		}

		/// <summary>
		///   Gets the visual child at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the visual child that should be returned.</param>
		protected internal virtual Visual GetVisualChild(int index)
		{
			Assert.That(false, "This visual does not have any visual children.");
			return null;
		}

		internal void Draw(SpriteBatch spriteBatch)
		{
			if (Visibility != Visibility.Visible)
				return;

			OnDraw(spriteBatch);

			var count = VisualChildrenCount;
			for (var i = 0; i < count; ++i)
			{
				var child = GetVisualChild(i);
				child.VisualOffset += VisualOffset;
				child.Draw(spriteBatch);
			}
		}

		protected abstract void OnDraw(SpriteBatch spriteBatch);
	}
}