namespace Pegasus.Platform.Graphics
{
	using System;
	using System.Diagnostics;
	using Memory;
	using Utilities;

	/// <summary>
	///     Base class for all objects belonging to a graphics device.
	/// </summary>
	public abstract class GraphicsObject : DisposableObject
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device this instance belongs to.</param>
		protected GraphicsObject(GraphicsDevice graphicsDevice)
		{
			Assert.ArgumentNotNull(graphicsDevice);
			GraphicsDevice = graphicsDevice;
		}

		/// <summary>
		///     Gets the graphics device this instance belongs to.
		/// </summary>
		protected GraphicsDevice GraphicsDevice { get; private set; }

		/// <summary>
		///     Sets the name of the graphics object. This method is only available in debug builds.
		/// </summary>
		[Conditional("DEBUG"), StringFormatMethod("name")]
		public void SetName(string name, params object[] arguments)
		{
#if DEBUG
			Name = String.Format(name, arguments);
			SetDescription(Name);

			if (!String.IsNullOrWhiteSpace(Name))
				OnRenamed(Name);
#endif
		}

		/// <summary>
		///     Ensures the current name is set on the native object after the native object has been recreated.
		/// </summary>
		[Conditional("DEBUG")]
		protected void SetName()
		{
#if DEBUG
			if (!String.IsNullOrWhiteSpace(Name))
				OnRenamed(Name);
#endif
		}

		/// <summary>
		///     Invoked after the name of the graphics object has changed. This method is only invoked in debug builds.
		/// </summary>
		/// <param name="name">The new name of the graphics object.</param>
		protected virtual void OnRenamed(string name)
		{
		}

#if DEBUG
		/// <summary>
		///     Gets the name of the graphics object. This property is only available in debug builds.
		/// </summary>
		protected string Name { get; private set; }

		/// <summary>
		///     Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			return String.Format("{0} '{1}'", GetType().Name, Name);
		}
#endif
	}
}