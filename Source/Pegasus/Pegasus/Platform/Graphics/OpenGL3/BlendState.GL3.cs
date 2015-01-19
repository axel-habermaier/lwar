namespace Pegasus.Platform.Graphics.OpenGL3
{
	using System;
	using Bindings;
	using Interface;

	/// <summary>
	///     Describes an OpenGL3-based blend state of the output merger pipeline stage.
	/// </summary>
	internal class BlendStateGL3 : GraphicsObjectGL3, IBlendState
	{
		/// <summary>
		///     The description of the blend state.
		/// </summary>
		private readonly BlendDescription _desc;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="description">A description of the blend state that should be created.</param>
		public BlendStateGL3(GraphicsDeviceGL3 graphicsDevice, ref BlendDescription description)
			: base(graphicsDevice)
		{
			_desc = description;
		}

		/// <summary>
		///     Sets the state on the pipeline.
		/// </summary>
		public void Bind()
		{
			if (_desc.BlendEnabled)
			{
				GraphicsDevice.EnableBlend(true);
				GraphicsDevice.SetBlendEquation(_desc.BlendOperation.Map(), _desc.BlendOperationAlpha.Map());

				GraphicsDevice.SetBlendFuncs(_desc.SourceBlend.Map(),
					_desc.DestinationBlend.Map(),
					_desc.SourceBlendAlpha.Map(),
					_desc.DestinationBlendAlpha.Map());

				GraphicsDevice.SetColorMask(
					((_desc.WriteMask & ColorWriteChannels.Alpha) == ColorWriteChannels.Alpha),
					((_desc.WriteMask & ColorWriteChannels.Red) == ColorWriteChannels.Red),
					((_desc.WriteMask & ColorWriteChannels.Green) == ColorWriteChannels.Green),
					((_desc.WriteMask & ColorWriteChannels.Blue) == ColorWriteChannels.Blue));
			}
			else
				GraphicsDevice.EnableBlend(false);
		}

		/// <summary>
		///     Sets the debug name of the state.
		/// </summary>
		/// <param name="name">The debug name of the state.</param>
		public void SetName(string name)
		{
			// Not supported by OpenGL
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			// Nothing to do here
		}
	}
}