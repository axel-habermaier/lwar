namespace Pegasus.Rendering.UserInterface
{
	using System;
	using System.Text;
	using Math;
	using Platform;
	using Platform.Memory;
	using Scripting;

	/// <summary>
	///     Manages statistics about the performance of the application and other information useful for debugging.
	/// </summary>
	public sealed class DebugOverlay : DisposableObject
	{
		/// <summary>
		///     The update frequency of the statistics in Hz.
		/// </summary>
		private const int UpdateFrequency = 30;

		/// <summary>
		///     The string builder that is used to construct the output.
		/// </summary>
		private readonly StringBuilder _builder = new StringBuilder();

		/// <summary>
		///     A weak reference to an object to which no strong reference exists. When the weak reference is no longer set to a
		///     valid instance of an object, it is an indication that a garbage collection has occurred.
		/// </summary>
		private readonly WeakReference _gcCheck = new WeakReference(new object());

		/// <summary>
		///     The label that is used to draw platform info and frame stats.
		/// </summary>
		private readonly Label _platformInfo;

		/// <summary>
		///     The approximate amount of garbage collections that have occurred since the application has been started.
		/// </summary>
		private int _garbageCollections;

		/// <summary>
		///     The timer that is used to periodically update the statistics.
		/// </summary>
		private Timer _timer = new Timer(1000.0 / UpdateFrequency);

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="font">The font that should be used for drawing.</param>
		internal DebugOverlay(Font font)
		{
			Assert.ArgumentNotNull(font);

			_platformInfo = new Label(font) { LineSpacing = 2, Alignment = TextAlignment.Bottom };
			_timer.Timeout += UpdateStatistics;
		}

		/// <summary>
		///     Gets the GPU frame time measurements.
		/// </summary>
		internal double GpuFrameTime { get; set; }

		/// <summary>
		///     Gets the CPU frame time measurements.
		/// </summary>
		internal double CpuFrameTime { get; set; }

		/// <summary>
		///     Updates the statistics.
		/// </summary>
		/// <param name="size">The size of the area that the statistics should be drawn on.</param>
		internal void Update(Size size)
		{
			if (!_gcCheck.IsAlive)
			{
				++_garbageCollections;
				_gcCheck.Target = new object();
			}

			const int padding = 5;
			_platformInfo.Area = new Rectangle(padding, padding, size.Width - 2 * padding, size.Height - 2 * padding);

			_timer.Update();
		}

		/// <summary>
		///     Updates the statistics.
		/// </summary>
		private void UpdateStatistics()
		{
			if (!Cvars.ShowDebugOverlay)
				return;

			_builder.Append("Platform:   ").Append(PlatformInfo.Platform).Append(" ").Append(IntPtr.Size * 8).Append("bit\n");
			_builder.Append("Debug Mode: ").Append(PlatformInfo.IsDebug.ToString().ToLower()).Append("\n");
			_builder.Append("Renderer:   ").Append(PlatformInfo.GraphicsApi).Append("\n");
			_builder.Append("# of GCs:   ").Append(_garbageCollections).Append("\n");
			_builder.Append("GPU Time:   ").Append(GpuFrameTime.ToString("F2")).Append("ms\n");
			_builder.Append("CPU Time:   ").Append(CpuFrameTime.ToString("F2")).Append("ms\n");

			_platformInfo.Text = _builder.ToString();
			_builder.Clear();
		}

		/// <summary>
		///     Draws the statistics.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the statistics.</param>
		internal void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Layer = Int32.MaxValue - 2;
			spriteBatch.WorldMatrix = Matrix.Identity;

			if (Cvars.ShowDebugOverlay)
				_platformInfo.Draw(spriteBatch);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_platformInfo.SafeDispose();
			_timer.Timeout -= UpdateStatistics;
		}
	}
}