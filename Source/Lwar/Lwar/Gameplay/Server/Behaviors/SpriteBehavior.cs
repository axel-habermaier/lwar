namespace Lwar.Gameplay.Server.Behaviors
{
	using System;
	using Components;
	using Pegasus.Entities;
	using Pegasus.Platform.Graphics;
	using Pegasus.Platform.Memory;
	using Pegasus.Rendering;
	using Pegasus.Utilities;

	/// <summary>
	///     Draws entities with sprite components.
	/// </summary>
	public class SpriteBehavior : EntityBehavior<Transform, Sprite>
	{
		/// <summary>
		///     The sprite batch that is used to draw the entities.
		/// </summary>
		private readonly SpriteBatch _spriteBatch = new SpriteBatch();

		/// <summary>
		///     Draws all sprites in a single batch.
		/// </summary>
		/// <param name="renderOutput">The render output the sprites should be drawn to.</param>
		public void Draw(RenderOutput renderOutput)
		{
			Assert.ArgumentNotNull(renderOutput);

			Process();
			_spriteBatch.DrawBatch(renderOutput);
		}

		/// <summary>
		///     Processes the entities affected by the behavior.
		/// </summary>
		/// <param name="entities">The entities affected by the behavior.</param>
		/// <param name="transforms">The transform components of the affected entities.</param>
		/// <param name="sprites">The sprite components of the affected entities.</param>
		/// <param name="count">The number of entities that should be processed.</param>
		/// <remarks>
		///     All arrays have the same length. If a component is optional for an entity and the component is missing, a null
		///     value is placed in the array.
		/// </remarks>
		protected override void Process(Entity[] entities, Transform[] transforms, Sprite[] sprites, int count)
		{
			for (var i = 0; i < count; ++i)
				_spriteBatch.Draw(sprites[i].Texture, transforms[i].Position, Colors.White);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_spriteBatch.SafeDispose();
			base.OnDisposing();
		}
	}
}