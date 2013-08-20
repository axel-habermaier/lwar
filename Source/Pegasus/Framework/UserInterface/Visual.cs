using System;

namespace Pegasus.Framework.UserInterface
{
	using Rendering;
	using Rendering.UserInterface;

	/// <summary>
	///   Provides rendering support for the UI system, also including hit testing.
	/// </summary>
	public abstract class Visual : DependencyObject
	{
		public abstract void Draw(SpriteBatch spriteBatch, Font font);
	}
}