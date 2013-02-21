using System;

namespace Lwar.Client.Rendering
{
	using Gameplay;
	using Pegasus.Framework;
	using Pegasus.Framework.Platform.Graphics;

	/// <summary>
	///   Renders bullets into a 3D scene.
	/// </summary>
	public struct BulletRenderer
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
		public BulletRenderer(RenderContext context)
		{
			Assert.ArgumentNotNull(context, () => context);
			_context = context;
		}

		/// <summary>
		///   Draws the bullet.
		/// </summary>
		/// <param name="bullet">The bullet that should be drawn.</param>
		public void Draw(Bullet bullet)
		{
			Assert.ArgumentNotNull(bullet, () => bullet);

			_context.UpdateWorldTransform(bullet);
			VertexShader.Bind();
			FragmentShader.Bind();
			SamplerState.PointWrap.Bind(0);

			Ship.Texture.Bind(0);
			bullet.Model.Draw();
		}
	}
}