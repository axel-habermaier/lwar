namespace Pegasus.Platform.Graphics
{
	using System;
	using System.Diagnostics;
	using System.Runtime.InteropServices;
	using Memory;
	using Utilities;

	/// <summary>
	///     Base class for all objects belonging to a graphics device.
	/// </summary>
	public abstract unsafe class GraphicsObject : DisposableObject
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device this instance belongs to.</param>
		protected GraphicsObject(GraphicsDevice graphicsDevice)
		{
			Assert.ArgumentNotNull(graphicsDevice);

			GraphicsDevice = graphicsDevice;
			DeviceInterface = graphicsDevice.DeviceInterface;
		}

		/// <summary>
		///     Gets or sets the underlying buffer object.
		/// </summary>
		protected internal void* NativeObject { get; protected set; }

		/// <summary>
		///     Gets the graphics device the graphics object belongs to.
		/// </summary>
		protected GraphicsDevice GraphicsDevice { get; private set; }

		/// <summary>
		///     Gets the device state of the graphics device this graphics object belongs to.
		/// </summary>
		protected DeviceState DeviceState
		{
			get { return GraphicsDevice.State; }
		}

		/// <summary>
		///     Gets the native device interface associated with the graphics object.
		/// </summary>
		protected DeviceInterface* DeviceInterface { get; private set; }

		/// <summary>
		///     Gets the function that should be used to set the debug name of the native object.
		/// </summary>
		protected virtual SetNameDelegate SetNameFunction
		{
			get { return null; }
		}

		/// <summary>
		///     Sets the name of the graphics object. This method is only available in debug builds.
		/// </summary>
		[Conditional("DEBUG"), StringFormatMethod("name")]
		public void SetName(string name, params object[] arguments)
		{
#if DEBUG
			_name = String.Format(name, arguments);
			SetDescription(_name);

			if (!String.IsNullOrWhiteSpace(_name))
				SetObjectName();
#endif
		}

		/// <summary>
		///     Ensures the current name is set on the native object after the native object has been recreated.
		/// </summary>
		[Conditional("DEBUG")]
		protected void SetName()
		{
#if DEBUG
			if (!String.IsNullOrWhiteSpace(_name))
				SetObjectName();
#endif
		}

		/// <summary>
		///     Represents a function that sets the debug name of a graphics object.
		/// </summary>
		/// <param name="obj">The object whose debug name should be set.</param>
		/// <param name="name">The debug name of the object that should be set.</param>
		protected delegate void SetNameDelegate(void* obj, void* name);

#if DEBUG
		/// <summary>
		///     The name of the graphics object. This field is only available in debug builds.
		/// </summary>
		private string _name;

		/// <summary>
		///     Sets the debug name of the graphics object.
		/// </summary>
		private void SetObjectName()
		{
			if (NativeObject == null || SetNameFunction == null)
				return;

			if (String.IsNullOrWhiteSpace(_name))
				SetNameFunction(NativeObject, null);
			else
			{
				var name = Marshal.StringToHGlobalAnsi(_name);
				SetNameFunction(NativeObject, name.ToPointer());
				Marshal.FreeHGlobal(name);
			}
		}

		/// <summary>
		///     Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			return String.Format("{0} '{1}'", GetType().Name, _name);
		}
#endif
	}
}