using System;

namespace Pegasus.Framework.Platform
{
	using System.Text;
	using Graphics;
	using Math;
	using Rendering;
	using Rendering.UserInterface;

	/// <summary>
	///   Manages statistics about the performance of the application.
	/// </summary>
	public class Statistics : DisposableObject
	{
		/// <summary>
		///   The string builder that is used to construct the output.
		/// </summary>
		private readonly StringBuilder _builder = new StringBuilder();

		/// <summary>
		///   A weak reference to an object to which no strong reference exists. When the weak reference is no longer set to a
		///   valid instance of an object, it is an indication that a garbage collection has occurred.
		/// </summary>
		private readonly WeakReference _gcCheck = new WeakReference(new object());

		/// <summary>
		///   The approximate amount of garbage collections that have occurred since the application has been started.
		/// </summary>
		private int _garbageCollections;

		/// <summary>
		///   The label that is used to draw the statistics.
		/// </summary>
		private Label _label;

		/// <summary>
		///   The sprite batch that is used for drawing.
		/// </summary>
		private SpriteBatch _spriteBatch;

		/// <summary>
		///   Gets the GPU frame time measurements.
		/// </summary>
		internal GpuProfiler GpuFrameTime { get; private set; }

		/// <summary>
		///   Gets the CPU frame time measurements.
		/// </summary>
		internal IMeasurement CpuFrameTime { get; private set; }

		/// <summary>
		///   Gets the input update time measurements.
		/// </summary>
		internal IMeasurement UpdateInput { get; private set; }

		/// <summary>
		///   Initializes the statistics.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used for drawing.</param>
		/// <param name="font">The font that should be used for drawing.</param>
		internal void Initialize(GraphicsDevice graphicsDevice, Font font)
		{
			Assert.ArgumentNotNull(graphicsDevice, () => graphicsDevice);
			Assert.ArgumentNotNull(font, () => font);

			_spriteBatch = new SpriteBatch(graphicsDevice);
			_label = new Label(font) { LineSpacing = 2, Alignment = TextAlignment.Bottom };

			GpuFrameTime = new GpuProfiler(graphicsDevice);
			CpuFrameTime = new AveragedValue();
			UpdateInput = new AveragedValue();
		}

		/// <summary>
		///   Sets the statistic's output size.
		/// </summary>
		/// <param name="size">The new size.</param>
		internal void Resize(Size size)
		{
			_spriteBatch.ProjectionMatrix = Matrix.OrthographicProjection(0, size.Width, size.Height, 0, 0, 1);
			_label.Area = new Rectangle(5, 5, size.Width, size.Height);
		}

		/// <summary>
		///   Draws the statistics.
		/// </summary>
		internal void Draw()
		{
			if (!_gcCheck.IsAlive)
			{
				++_garbageCollections;
				_gcCheck.Target = new object();
			}

			_builder.Clear();
			_builder.Append("Platform: ").Append(PlatformInfo.Platform).Append(" ").Append(IntPtr.Size * 8).Append("bit\n");
			_builder.Append("Debug Mode: ").Append(PlatformInfo.IsDebug).Append("\n");
			_builder.Append("Renderer: ").Append(PlatformInfo.GraphicsApi).Append("\n");
			_builder.Append("# of GCs: ").Append(_garbageCollections).Append("\n\n");

			WriteMeasurement(GpuFrameTime, "GPU Frame Time");
			WriteMeasurement(CpuFrameTime, "CPU Frame Time");

			_builder.Append("\n");

			WriteMeasurement(UpdateInput, "Update Input");
			WriteMeasurements(_builder);

			_label.Text = _builder.ToString();
			_label.Draw(_spriteBatch);
			_spriteBatch.DrawBatch();
		}

		/// <summary>
		///   Invoked when the measurements should be written to the given string builder.
		/// </summary>
		/// <param name="builder">The string builder that the measurements should be written to.</param>
		protected virtual void WriteMeasurements(StringBuilder builder)
		{
		}

		/// <summary>
		///   Writes the results of the given measurement to the string builder.
		/// </summary>
		/// <param name="measurement">The measurement that should be written to the string builder.</param>
		/// <param name="label">The label that describes the measurement.</param>
		protected void WriteMeasurement(IMeasurement measurement, string label)
		{
			_builder.Append(label).Append(": ");
			measurement.WriteResults(_builder);
			_builder.Append("\n");
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_spriteBatch.SafeDispose();
			GpuFrameTime.SafeDispose();
		}
	}
}