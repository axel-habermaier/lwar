namespace Pegasus.Framework.UserInterface.ViewModels
{
	using System;
	using Controls;
	using Platform;
	using Scripting;

	/// <summary>
	///     Displays statistics about the performance of the application and other information useful for debugging.
	/// </summary>
	internal sealed class DebugOverlayViewModel : ViewModel
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
		///     The view of the debug overlay.
		/// </summary>
		private readonly DebugOverlayView _view;

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
			_view = new DebugOverlayView { DataContext = this };

			Application.Current.Window.LayoutRoot.Add(_view);
		}

		/// <summary>
		///     Sets the GPU frame time in milliseconds that is displayed by the debug overlay.
		/// </summary>
		internal double GpuFrameTime
		{
			set { _gpuFrameTime.AddMeasurement(value); }
		}

		/// <summary>
		///     Sets the CPU update time in milliseconds that is displayed by the debug overlay.
		/// </summary>
		internal double CpuUpdateTime
		{
			set { _cpuUpdateTime.AddMeasurement(value); }
		}

		/// <summary>
		///     Sets the CPU render time in milliseconds that is displayed by the debug overlay.
		/// </summary>
		internal double CpuRenderTime
		{
			set { _cpuRenderTime.AddMeasurement(value); }
		}

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
			get { return String.Format("{0} {1}bit", PlatformInfo.Platform, IntPtr.Size * 8); }
		}

		/// <summary>
		///     Gets a string indicating whether the application was built in debug mode.
		/// </summary>
		public bool DebugMode
		{
			get { return PlatformInfo.IsDebug; }
		}

		/// <summary>
		///     Gets a string describing the renderer back end that is currently used to render the application.
		/// </summary>
		public string Renderer
		{
			get { return PlatformInfo.GraphicsApi.ToString(); }
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
		}

		/// <summary>
		///     Gets or sets the averaged CPU render time.
		/// </summary>
		public double RenderTime
		{
			get { return _cpuRenderTime.Average; }
		}

		/// <summary>
		///     Updates the statistics.
		/// </summary>
		internal void Update()
		{
			if (!_gcCheck.IsAlive)
			{
				++_garbageCollections;
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

			NotifyPropertyChanged("GpuTime");
			NotifyPropertyChanged("CpuTime");
			NotifyPropertyChanged("UpdateTime");
			NotifyPropertyChanged("RenderTime");
		}

		/// <summary>
		///     Updates the visibility of the debug overlay.
		/// </summary>
		/// <param name="visibility">Indicates whether the debug overlay should be shown.</param>
		private void UpdateVisibility(bool visibility)
		{
			NotifyPropertyChanged("IsVisible");
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Cvars.ShowDebugOverlayCvar.Changed -= UpdateVisibility;
			_timer.Timeout -= UpdateViewModel;

			Application.Current.Window.LayoutRoot.Remove(_view);
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
					var count = _averageIsFilled ? _values.Length : _averageIndex;

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