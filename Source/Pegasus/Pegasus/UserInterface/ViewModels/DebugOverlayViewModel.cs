﻿namespace Pegasus.UserInterface.ViewModels
{
	using System;
	using Platform;
	using Scripting;

	/// <summary>
	///     Displays statistics about the performance of the application and other information useful for debugging.
	/// </summary>
	internal sealed class DebugOverlayViewModel : DisposableNotifyPropertyChanged
	{
		/// <summary>
		///     The update frequency of the statistics in Hz.
		/// </summary>
		private const int UpdateFrequency = 30;

		/// <summary>
		///     The number of measurements that are used to calculate an average.
		/// </summary>
		private const int AverageSampleCount = 16;

		/// <summary>
		///     A weak reference to an object to which no strong reference exists. When the weak reference is no longer set to a
		///     valid instance of an object, it is an indication that a garbage collection has occurred.
		/// </summary>
		private readonly WeakReference _gcCheck = new WeakReference(new object());

		/// <summary>
		///     The total CPU frame time in milliseconds that is displayed by the debug overlay.
		/// </summary>
		private AveragedDouble _cpuFrameTime = new AveragedDouble(AverageSampleCount);

		/// <summary>
		///     The CPU render time in milliseconds that is displayed by the debug overlay.
		/// </summary>
		private AveragedDouble _cpuRenderTime = new AveragedDouble(AverageSampleCount);

		/// <summary>
		///     The CPU update time in milliseconds that is displayed by the debug overlay.
		/// </summary>
		private AveragedDouble _cpuUpdateTime = new AveragedDouble(AverageSampleCount);

		/// <summary>
		///     The approximate amount of garbage collections that have occurred since the application has been started.
		/// </summary>
		private int _garbageCollections;

		/// <summary>
		///     The GPU frame time in milliseconds that is displayed by the debug overlay.
		/// </summary>
		private AveragedDouble _gpuFrameTime = new AveragedDouble(AverageSampleCount);

		/// <summary>
		///     The particle render time in milliseconds that is displayed by the debug overlay.
		/// </summary>
		private AveragedDouble _particleRenderTime = new AveragedDouble(AverageSampleCount);

		/// <summary>
		///     The particle update time in milliseconds that is displayed by the debug overlay.
		/// </summary>
		private AveragedDouble _particleUpdateTime = new AveragedDouble(AverageSampleCount);

		/// <summary>
		///     The timer that is used to periodically update the statistics.
		/// </summary>
		private Timer _timer = new Timer(1000.0 / UpdateFrequency);

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		internal DebugOverlayViewModel()
		{
			Cvars.ShowDebugOverlayCvar.Changed += UpdateVisibility;
			UpdateVisibility(Cvars.ShowDebugOverlay);

			_timer.Timeout += UpdateViewModel;
		}

		/// <summary>
		///     Gets the number of particles that are currently being updated and drawn.
		/// </summary>
		public int ParticleCount { get; internal set; }

		/// <summary>
		///     Gets or sets the visibility of the debug overlay.
		/// </summary>
		public bool IsVisible
		{
			get { return Cvars.ShowDebugOverlay; }
		}

		/// <summary>
		///     Gets a string describing the platform the application is currently being executed on.
		/// </summary>
		public string Platform
		{
			get
			{
				return String.Format("{0} x{1}", PlatformInfo.Platform, Environment.Is64BitOperatingSystem ? "64" : "32");
			}
		}

		/// <summary>
		///     Gets a string indicating whether the build is 64 or 32 bits and whether the it is a debug build.
		/// </summary>
		public string Build
		{
			get
			{
				return String.Format("{0}bit, {1}", Environment.Is64BitProcess ? "64" : "32", PlatformInfo.IsDebug ? "debug" : "release");
			}
		}

		/// <summary>
		///     Gets a string describing the renderer back end that is currently used to render the application.
		/// </summary>
		public string Renderer
		{
			get { return Cvars.GraphicsApi.ToString(); }
		}

		/// <summary>
		///     Gets or sets the minimum number of garbage collections that have been performed since the start of the application.
		/// </summary>
		public int GarbageCollections
		{
			get { return _garbageCollections; }
			set { ChangePropertyValue(ref _garbageCollections, value); }
		}

		/// <summary>
		///     Gets or sets the averaged GPU frame time.
		/// </summary>
		public double GpuTime
		{
			get { return _gpuFrameTime.Average; }
			internal set { _gpuFrameTime.AddMeasurement(value); }
		}

		/// <summary>
		///     Gets or sets the averaged CPU frame time.
		/// </summary>
		public double CpuTime
		{
			get { return _cpuFrameTime.Average; }
		}

		/// <summary>
		///     Gets or sets the averaged CPU update time.
		/// </summary>
		public double UpdateTime
		{
			get { return _cpuUpdateTime.Average; }
			internal set { _cpuUpdateTime.AddMeasurement(value); }
		}

		/// <summary>
		///     Gets or sets the averaged CPU render time.
		/// </summary>
		public double RenderTime
		{
			get { return _cpuRenderTime.Average; }
			internal set { _cpuRenderTime.AddMeasurement(value); }
		}

		/// <summary>
		///     Gets or sets the averaged CPU update time.
		/// </summary>
		public double ParticleUpdateTime
		{
			get { return _particleUpdateTime.Average; }
			internal set { _particleUpdateTime.AddMeasurement(value); }
		}

		/// <summary>
		///     Gets or sets the averaged CPU render time.
		/// </summary>
		public double ParticleRenderTime
		{
			get { return _particleRenderTime.Average; }
			internal set { _particleRenderTime.AddMeasurement(value); }
		}

		/// <summary>
		///     Updates the statistics.
		/// </summary>
		internal void Update()
		{
			if (!_gcCheck.IsAlive)
			{
				++GarbageCollections;
				_gcCheck.Target = new object();
			}

			_cpuFrameTime.AddMeasurement(_cpuUpdateTime.LastValue + _cpuRenderTime.LastValue);
			_timer.Update();
		}

		/// <summary>
		///     Updates the view model.
		/// </summary>
		private void UpdateViewModel()
		{
			if (!Cvars.ShowDebugOverlay)
				return;

			OnPropertyChanged("GpuTime");
			OnPropertyChanged("CpuTime");
			OnPropertyChanged("UpdateTime");
			OnPropertyChanged("RenderTime");
			OnPropertyChanged("ParticleUpdateTime");
			OnPropertyChanged("ParticleRenderTime");
			OnPropertyChanged("ParticleCount");
		}

		/// <summary>
		///     Updates the visibility of the debug overlay.
		/// </summary>
		/// <param name="visibility">Indicates whether the debug overlay should be shown.</param>
		private void UpdateVisibility(bool visibility)
		{
			OnPropertyChanged("IsVisible");
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Cvars.ShowDebugOverlayCvar.Changed -= UpdateVisibility;
			_timer.Timeout -= UpdateViewModel;
		}

		/// <summary>
		///     Represents a measurement that is averaged over a certain number of samples.
		/// </summary>
		private struct AveragedDouble
		{
			/// <summary>
			///     The last couple of values for a more stable average.
			/// </summary>
			private readonly double[] _values;

			/// <summary>
			///     The current write index in the average array (circular writes).
			/// </summary>
			private int _averageIndex;

			/// <summary>
			///     A value indicating whether the entire average array has been filled at least once.
			/// </summary>
			private bool _averageIsFilled;

			/// <summary>
			///     The maximum supported value.
			/// </summary>
			private double _max;

			/// <summary>
			///     The minimum supported value.
			/// </summary>
			private double _min;

			/// <summary>
			///     Initializes a new instance.
			/// </summary>
			/// <param name="sampleCount">The number of samples for the computation of the average.</param>
			public AveragedDouble(int sampleCount)
				: this()
			{
				_values = new double[sampleCount];
				_max = Double.MinValue;
				_min = Double.MaxValue;
			}

			/// <summary>
			///     Gets the last value that has been measured.
			/// </summary>
			internal double LastValue { get; private set; }

			/// <summary>
			///     Gets the averaged value.
			/// </summary>
			public double Average
			{
				get
				{
					double average = 0;
					var count = _averageIsFilled ? _values.Length : Math.Max(_averageIndex, 1);

					for (var i = 0; i < count; ++i)
						average += _values[i];

					average /= count;
					return average;
				}
			}

			/// <summary>
			///     Adds the given measured value to the statistics.
			/// </summary>
			/// <param name="value">The value that should be added.</param>
			internal void AddMeasurement(double value)
			{
				LastValue = value;

				if (LastValue > _max)
					_max = LastValue;
				if (LastValue < _min)
					_min = LastValue;

				_values[_averageIndex] = LastValue;
				_averageIndex = (_averageIndex + 1) % _values.Length;

				if (_averageIndex == 0)
					_averageIsFilled = true;
			}
		}
	}
}