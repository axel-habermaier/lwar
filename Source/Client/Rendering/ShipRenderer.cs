using System;

namespace Lwar.Client.Rendering
{
	using Gameplay;
	using Pegasus.Framework;
	using Pegasus.Framework.Platform.Graphics;

	/// <summary>
	///   Renders ships into a 3D scene.
	/// </summary>
	public struct ShipRenderer
	{
		/// <summary>
		///   The vertex shader that is used to draw the ships.
		/// </summary>
		[Asset("Shaders/QuadVS")]
		public static VertexShader VertexShader;

		/// <summary>
		///   The fragment shader that is used to draw the ships.
		/// </summary>
		[Asset("Shaders/QuadFS")]
		public static FragmentShader FragmentShader;

		/// <summary>
		///   The context in which the ships are rendered.
		/// </summary>
		private readonly RenderContext _context;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="context">The context in which the ships should be rendered.</param>
		public ShipRenderer(RenderContext context)
		{
			Assert.ArgumentNotNull(context, () => context);
			_context = context;
		}

		/// <summary>
		///   Draws the ship.
		/// </summary>
		/// <param name="ship">The ship that should be drawn.</param>
		public void Draw(Ship ship)
		{
			Assert.ArgumentNotNull(ship, () => ship);

			_context.UpdateWorldTransform(ship);
			VertexShader.Bind();
			FragmentShader.Bind();
			
			Ship.Texture.Bind(0);
			ship.Model.Draw();
		}
	}
}