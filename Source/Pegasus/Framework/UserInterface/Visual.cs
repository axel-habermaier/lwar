using System;

namespace Pegasus.Framework.UserInterface
{
	using Math;
	using Rendering;
	using Rendering.UserInterface;

	/// <summary>
	///   Provides rendering support for the UI system, also including hit testing.
	/// </summary>
	public abstract class Visual : DependencyObject
	{
		/// <summary>
		///   Gets or sets the offset value of the visual.
		/// </summary>
		protected internal Vector2d VisualOffset { get; protected set; }

		public abstract void Draw(SpriteBatch spriteBatch, Font font);
	}
}