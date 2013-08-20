using System;

namespace Pegasus.Platform.Graphics
{
	using System.Diagnostics;
	using Memory;

	/// <summary>
	///   Base class for all objects belong to a graphics device.
	/// </summary>
	public abstract class GraphicsObject : DisposableObject
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device this instance belongs to.</param>
		protected GraphicsObject(GraphicsDevice graphicsDevice)
		{
			Assert.ArgumentNotNull(graphicsDevice);
			GraphicsDevice = graphicsDevice;

			SetName("Unnamed");
		}

		/// <summary>
		///   Gets the graphics device this instance belongs to.
		/// </summary>
		protected GraphicsDevice GraphicsDevice { get; private set; }

		/// <summary>
		///   Sets the name of the graphics object. This method is only available in debug builds.
		/// </summary>
		[Conditional("DEBUG")]
		public void SetName(string name)
		{
#if DEBUG
			Assert.ArgumentNotNullOrWhitespace(name);
			Name = name;
			OnRenamed();
			SetDescription(name);
#endif
		}

#if DEBUG

		/// <summary>
		///   Gets the name of the graphics object. This property is only available in debug builds.
		/// </summary>
		protected string Name { get; private set; }

		/// <summary>
		///   Invoked after the name of the graphics object has changed. This method is only available in debug builds.
		/// </summary>
		protected virtual void OnRenamed()
		{
		}

		/// <summary>
		///   Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			return String.Format("{0} '{1}'", GetType().Name, Name);
		}
#endif
	}
}