﻿using System;

namespace Lwar.Client.Screens
{
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Rendering;

	/// <summary>
	///   Draws a frame around arbitrary content.
	/// </summary>
	public class Frame
	{
		/// <summary>
		///   The color of the frame.
		/// </summary>
		public Color FrameColor = new Color(32, 32, 32, 64);

		/// <summary>
		///   The margin of the frame, i.e., the distance between the outer border of the frame and its content.
		/// </summary>
		public int Margin = 5;

		/// <summary>
		///   Gets or sets the area of the frame.
		/// </summary>
		public Rectangle ContentArea { get; set; }

		/// <summary>
		///   Draws the frame.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used for drawing.</param>
		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(ContentArea.Enlarge(Margin), Texture2D.White, FrameColor);
		}
	}
}