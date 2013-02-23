using System;

namespace Lwar.Client.Rendering
{
	using Gameplay;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform.Assets;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Rendering;

	/// <summary>
	///   Represents the context in which rendering operations are performed.
	/// </summary>
	public class RenderContext : DisposableObject
	{
		/// <summary>
		///   The constant buffer slot for the world transform.
		/// </summary>
		private const int WorldTransformSlot = 1;

		/// <summary>
		///   The rasterizer state that is used to draw solid geometry.
		/// </summary>
		private readonly RasterizerState _solid;

		/// <summary>
		///   The rasterizer state that is used to draw in wireframe mode.
		/// </summary>
		private readonly RasterizerState _wireframe;

		/// <summary>
		///   The constant buffer that contains the world transform of the object that is currently being rendered.
		/// </summary>
		private readonly ConstantBuffer<Matrix> _worldTransform;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that the render context should use to draw the scene.</param>
		/// <param name="assets">The assets manager that should manage the assets of the render context.</param>
		public unsafe RenderContext(GraphicsDevice graphicsDevice, AssetsManager assets)
		{
			Assert.ArgumentNotNull(graphicsDevice, () => graphicsDevice);
			Assert.ArgumentNotNull(assets, () => assets);

			GraphicsDevice = graphicsDevice;
			Assets = assets;

			_worldTransform = new ConstantBuffer<Matrix>(graphicsDevice, (buffer, matrix) => buffer.Copy(&matrix));
			_solid = RasterizerState.CullCounterClockwise;
			_wireframe = new RasterizerState(graphicsDevice) { CullMode = CullMode.Back, FillMode = FillMode.Wireframe };

			PlanetRenderer = new PlanetRenderer(this);
			ShipRenderer = new ShipRenderer(this);
			BulletRenderer = new BulletRenderer(this);
		}

		/// <summary>
		///   Gets the graphics device that the render context uses to draw the scene.
		/// </summary>
		public GraphicsDevice GraphicsDevice { get; private set; }

		/// <summary>
		///   Gets the assets manager that manages the assets of the render context.
		/// </summary>
		public AssetsManager Assets { get; private set; }

		/// <summary>
		///   Gets or sets the camera that the render context uses to draw the scene.
		/// </summary>
		public Camera Camera { get; set; }

		/// <summary>
		///   Gets the renderer that is used to draw planets.
		/// </summary>
		public PlanetRenderer PlanetRenderer { get; private set; }

		/// <summary>
		///   Gets the renderer that is used to draw ships.
		/// </summary>
		public ShipRenderer ShipRenderer { get; private set; }

		/// <summary>
		///   Gets the renderer that is used to draw bullets.
		/// </summary>
		public BulletRenderer BulletRenderer { get; private set; }

		/// <summary>
		///   Marks the beginning of a frame.
		/// </summary>
		public void BeginFrame()
		{
			Assert.NotNull(Camera, "No camera has been set.");

			Camera.Bind();
			_worldTransform.Bind(WorldTransformSlot);

			if (LwarCvars.DrawWireframe.Value)
				_wireframe.Bind();
			else
				_solid.Bind();
		}

		/// <summary>
		///   Updates the world transformation constant buffer.
		/// </summary>
		/// <param name="entity">The entity for which the world transformation should be set.</param>
		public void UpdateWorldTransform(IEntity entity)
		{
			_worldTransform.Data = Matrix.CreateRotationY(entity.Rotation) *
								   Matrix.CreateTranslation(new Vector3(entity.Position.X, 0, entity.Position.Y));
			_worldTransform.Update();
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_worldTransform.SafeDispose();
			_wireframe.SafeDispose();
		}
	}
}