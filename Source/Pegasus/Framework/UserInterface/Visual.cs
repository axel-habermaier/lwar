using System;

namespace Pegasus.Framework.UserInterface
{
	using Math;
	using Rendering;

	/// <summary>
	///   Provides rendering support for the UI system, also including hit testing.
	/// </summary>
	public abstract class Visual : DependencyObject
	{
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
		///   Gets the visual child at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the visual child that should be returned.</param>
		protected internal virtual Visual GetVisualChild(int index)
		{
			Assert.That(false, "This visual does not have any visual children.");
			return null;
		}

		/// <summary>
		///   Applies the given offset to the visual's offset and to all of its visual children.
		/// </summary>
		/// <param name="offset">The offset that should be applied to the visual's offset.</param>
		//internal void ApplyVisualOffset(Vector2d offset)
		//{
		//	VisualOffset += offset;

		//	var count = VisualChildrenCount;
		//	for (var i = 0; i < count; ++i)
		//		GetVisualChild(i).ApplyVisualOffset(VisualOffset);
		//}

		internal void Draw(SpriteBatch spriteBatch)
		{
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