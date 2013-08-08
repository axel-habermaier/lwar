using System;

namespace Lwar.Rendering.Renderers
{
	using Gameplay.Actors;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Platform.Logging;
	using Pegasus.Framework.Rendering;

	/// <summary>
	///   Renders explosion effects into a 3D scene.
	/// </summary>
	public class ExplosionRenderer : Renderer<Explosion>
	{
		/// <summary>
		///   Gets the number of frames stored in the texture per dimension.
		/// </summary>
		private const int FrameCount = 8;

		/// <summary>
		///   The texture that is used to draw the explosion.
		/// </summary>
		private Texture2D _texture;

		/// <summary>
		///   Initializes the renderer.
		/// </summary>
		protected override void Initialize()
		{
			_texture = Assets.LoadTexture2D("Textures/Explosion");
		}

		/// <summary>
		///   Draws all registered 2D elements.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the 2D elements.</param>
		public override void Draw(SpriteBatch spriteBatch)
		{
			var numFrames = FrameCount * FrameCount;
			var frameSize = new SizeF(1.0f / FrameCount, 1.0f / FrameCount);

			foreach (var explosion in Elements)
			{
				var currentFrame = (int)(numFrames * (1.0f - explosion.TimeToLive));
				var frameX = currentFrame % FrameCount;
				var frameY = currentFrame / FrameCount;
				
				var size = new SizeF((float)_texture.Width / FrameCount * 2, (float)_texture.Height / FrameCount * 2);
				var position = new Vector2(explosion.Transform.Position2D.X - size.Width / 2.0f, explosion.Transform.Position2D.Y - size.Height / 2.0f);
				var texCoords = new RectangleF(frameX * frameSize.Width, frameY * frameSize.Height, frameSize.Width, frameSize.Height);

				spriteBatch.BlendState = BlendState.Premultiplied;
				spriteBatch.Draw(new RectangleF(position, size), _texture, Color.White, texCoords);
			}
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposingCore()
		{
			// Nothing to do here
		}
	}
}