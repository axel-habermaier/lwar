namespace Lwar.Rendering.Renderers
{
	using System;
	using Assets;
	using Gameplay.Entities;
	using Pegasus.Assets;
	using Pegasus.Platform.Graphics;
	using Pegasus.Rendering;

	/// <summary>
	///     Renders ships into a 3D scene.
	/// </summary>
	public class ShipRenderer : Renderer<Ship>
	{
		/// <summary>
		///     The texture that is used to draw the ship.
		/// </summary>
		private Texture2D _texture;

		/// <summary>
		///     Loads the required assets of the renderer.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used for drawing.</param>
		/// <param name="assets">The assets manager that should be used to load all required assets.</param>
		public override void Load(GraphicsDevice graphicsDevice, AssetsManager assets)
		{
			_texture = assets.Load(Textures.Ship);
		}

		/// <summary>
		///     Draws all registered 2D elements.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the 2D elements.</param>
		public override void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.BlendState = BlendState.Premultiplied;
			spriteBatch.DepthStencilState = DepthStencilState.DepthDisabled;

			foreach (var ship in Elements)
				spriteBatch.Draw(ship.Position, _texture.Size, _texture, Color.White, -ship.Rotation);
		}

		///// <summary>
		/////     Draws the user interface elements.
		///// </summary>
		///// <param name="spriteBatch">The sprite batch that should be used to draw the user interface.</param>
		///// <param name="camera">The camera that is used to draw the scene.</param>
		//public override void DrawUserInterface(SpriteBatch spriteBatch, GameCamera camera)
		//{
		//	// Don't show the names when the game camera is inactive
		//	if (!camera.IsActive)
		//		return;

		//	spriteBatch.WorldMatrix = Matrix.Identity;
		//	var viewport = camera.Viewport;

		//	// Update the positions of all names
		//	var i = 0;
		//	foreach (var ship in Elements)
		//	{
		//		var name = _names[i];
		//		name.Update();

		//		// Determine the screen-space height of the ship
		//		var topLeft = camera.ToScreenCoodinates(new Vector2(ship.Position.X + _texture.Width / 2.0f,
		//			ship.Position.Y - _texture.Height / 2.0f));
		//		var bottomRight = camera.ToScreenCoodinates(new Vector2(ship.Position.X - _texture.Width / 2.0f,
		//			ship.Position.Y + _texture.Height / 2.0f));

		//		var width = bottomRight.X - topLeft.X;
		//		var height = bottomRight.Y - topLeft.Y;

		//		// Position the text centered below the ship
		//		var position = new Vector2(topLeft.X + width / 2.0f - name.Area.Width / 2.0f, viewport.Height - bottomRight.Y + height);
		//		name.Area.Left = (int)position.X;
		//		name.Area.Top = (int)position.Y;

		//		// If the ship is outside the viewport, calculate the intersection of the line connecting the player to the current player
		//		// and the viewport and show the name there
		//		if (name.Area.Left < viewport.Left || name.Area.Top < viewport.Top ||
		//			name.Area.Right > viewport.Right || name.Area.Bottom > viewport.Bottom)
		//		{
		//			// The line segments defining the viewport; make the line segments slightly larger to account for rounding errors
		//			var left = new LineSegment(new Vector2(0, -1), new Vector2(0, viewport.Height + 1));
		//			var top = new LineSegment(new Vector2(-1, 0), new Vector2(viewport.Width + 1, 0));
		//			var right = new LineSegment(new Vector2(viewport.Width, -1), new Vector2(viewport.Width, viewport.Height + 1));
		//			var bottom = new LineSegment(new Vector2(-1, viewport.Height), new Vector2(viewport.Width + 1, viewport.Height));

		//			// The line segment from the center of the viewport to the ship's screen space position to 
		//			var line = new LineSegment(new Vector2(viewport.Width / 2.0f, viewport.Height / 2.0f),
		//				new Vector2(name.Area.Left, name.Area.Top) + new Vector2(name.Area.Width / 2.0f, name.Area.Height / 2.0f));

		//			Vector2 intersection;
		//			if (line.Intersects(left, out intersection) || line.Intersects(top, out intersection) ||
		//				line.Intersects(right, out intersection) || line.Intersects(bottom, out intersection))
		//			{
		//				position = intersection;
		//			}

		//			// Clamp to the viewport
		//			position.X = MathUtils.Clamp(position.X, 0, viewport.Width - name.Area.Width);
		//			position.Y = MathUtils.Clamp(position.Y, 0, viewport.Height - _font.LineHeight);

		//			name.Area.Left = (int)position.X;
		//			name.Area.Top = (int)position.Y;
		//		}

		//		// Store the updated values (a copy has to be made as we cannot modify the return value of the lists indexing operator directly)
		//		_names[i] = name;

		//		++i;
		//	}

		//	// Draw player names and health bars
		//	i = 0;
		//	foreach (var ship in Elements)
		//	{
		//		var name = _names[i];

		//		// Draw the health bar
		//		var center = name.Area.Position + new Vector2i(name.Area.Width / 2, name.Area.Height / 2);

		//		const int healthBarLength = 60;
		//		var positionY = center.Y - 10;

		//		var start = new Vector2(center.X - healthBarLength / 2, positionY);
		//		var middle = new Vector2(start.X + ship.Health / 100.0f * healthBarLength, positionY);
		//		var end = new Vector2(center.X + healthBarLength / 2, positionY);

		//		if ((middle - end).Length > 1)
		//			spriteBatch.DrawLine(end, middle, new Color(255, 0, 0, 255), 5);
		//		if ((middle - start).Length > 1)
		//			spriteBatch.DrawLine(start, middle, new Color(0, 255, 0, 255), 5);

		//		// Draw the name
		//		spriteBatch.DrawText(_font, name.Name, Color.White, name.Area.Position);
		//		++i;
		//	}
		//}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposingCore()
		{
		}
	}
}