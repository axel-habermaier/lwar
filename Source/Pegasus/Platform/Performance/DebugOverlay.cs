namespace Pegasus.Platform.Performance
{
	using System;
	using System.Text;
	using Graphics;
	using Math;
	using Memory;
	using Rendering;
	using Rendering.UserInterface;
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
		///     The timer that is used to periodically update the statistics.
		/// </summary>
		private readonly Timer _timer = Timer.Create(1000.0 / UpdateFrequency);

		/// <summary>
		///     The approximate amount of garbage collections that have occurred since the application has been started.
		/// </summary>
		private int _garbageCollections;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used for drawing.</param>
		/// <param name="font">The font that should be used for drawing.</param>
		internal DebugOverlay(GraphicsDevice graphicsDevice, Font font)
		{
			Assert.ArgumentNotNull(graphicsDevice);
			Assert.ArgumentNotNull(font);

			_platformInfo = new Label(font) { LineSpacing = 2, Alignment = TextAlignment.Bottom };
			_timer.Timeout += UpdateStatistics;

			GpuFrameTime = new AveragedDouble("ms", 32);
			CpuFrameTime = new TimingMeasurement();
		}

		/// <summary>
		///     Gets the GPU frame time measurements.
		/// </summary>
		internal AveragedDouble GpuFrameTime { get; private set; }

		/// <summary>
		///     Gets the CPU frame time measurements.
		/// </summary>
		internal IMeasurement CpuFrameTime { get; private set; }

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
			_builder.Append("GPU Time:   ");
			GpuFrameTime.WriteResults(_builder);
			WriteMeasurement(CpuFrameTime, "\nCPU Time:   ");

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
		///     Writes the results of the given measurement to the string builder.
		/// </summary>
		/// <param name="measurement">The measurement that should be written to the string builder.</param>
		/// <param name="label">The label that describes the measurement.</param>
		private void WriteMeasurement(IMeasurement measurement, string label)
		{
			_builder.Append(label);
			measurement.WriteResults(_builder);
			_builder.Append("\n");
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_platformInfo.SafeDispose();

			_timer.Timeout -= UpdateStatistics;
			_timer.SafeDispose();
		}
	}
}