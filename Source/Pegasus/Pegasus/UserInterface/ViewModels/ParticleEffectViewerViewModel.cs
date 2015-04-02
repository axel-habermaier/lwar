namespace Pegasus.UserInterface.ViewModels
{
	using System;
	using System.Linq;
	using Input;
	using Platform;
	using Platform.Memory;
	using Rendering;
	using Rendering.Particles;
	using Utilities;
	using Views;

	/// <summary>
	///     The view model for the particle effects viewer.
	/// </summary>
	internal class ParticleEffectViewerViewModel : DisposableNotifyPropertyChanged
	{
		/// <summary>
		///     The view of the particle effect viewer;
		/// </summary>
		private readonly ParticleEffectViewerView _view;

		/// <summary>
		///     The asset bundles loaded by the viewer.
		/// </summary>
		private AssetBundle[] _assetBundles;

		/// <summary>
		///     The debug camera that is used to draw the particle effect preview.
		/// </summary>
		private DebugCamera _camera;

		/// <summary>
		///     The clock used for time measurements.
		/// </summary>
		private Clock _clock;

		/// <summary>
		///     Indicates that input is captured and the preview camera is controlled.
		/// </summary>
		private bool _inputCaptured;

		/// <summary>
		///     The input device that is used to control the camera.
		/// </summary>
		private LogicalInputDevice _inputDevice;

		/// <summary>
		///     The particle effect that is previewed.
		/// </summary>
		private ParticleEffect _particleEffect = new ParticleEffect();

		/// <summary>
		///     Indicates whether relative mouse mode was enabled when the viewer was opened.
		/// </summary>
		private bool _relativeMouseMode;

		/// <summary>
		///     We're using a separate render context for the particle effect viewer as we have to load and unload all asset bundles.
		///     This way, we are guaranteed not to interfere with the usual application logic.
		/// </summary>
		private RenderContext _renderContext;

		/// <summary>
		///     The particle effect template that is currently selected.
		/// </summary>
		private ParticleEffectTemplate _selectedTemplate;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public ParticleEffectViewerViewModel()
		{
			_view = new ParticleEffectViewerView();
			_inputDevice = new LogicalInputDevice(_view, usePreviewEvents: true);
			_renderContext = new RenderContext(Application.Current.GraphicsDevice);
			_relativeMouseMode = Mouse.RelativeMouseMode;
			_camera = new DebugCamera(Application.Current.GraphicsDevice, _inputDevice) { MoveSpeed = 250, IsActive = false };

			// Load all asset bundles
			_assetBundles = AssemblyCache.CreateInstancesOfType<AssetBundle>(_renderContext).ToArray();
			foreach (var bundle in _assetBundles)
				bundle.Load();

			// Load all templates
			var templates = AssemblyCache.CreateInstancesOfType<ParticleEffectTemplate>(_renderContext);
			ParticleTemplates = templates.OrderBy(t => t.DisplayName).ToArray();
			
			Mouse.RelativeMouseMode = false;
			MessageBox.CloseAll();

			RenderOutput = new RenderOutput(_renderContext) { Camera = _camera };
			ResetCamera();

			_view.DataContext = this;
			Application.Current.Window.LayoutRoot.Add(_view);
		}

		/// <summary>
		///     Gets or sets the render output the particle effect viewer is rendered to.
		/// </summary>
		public RenderOutput RenderOutput { get; set; }

		/// <summary>
		///     Gets the particle effects defined by the application.
		/// </summary>
		public ParticleEffectTemplate[] ParticleTemplates { get; private set; }

		/// <summary>
		///     Gets or sets the selected particle effect template.
		/// </summary>
		public ParticleEffectTemplate SelectedTemplate
		{
			get { return _selectedTemplate; }
			set
			{
				ChangePropertyValue(ref _selectedTemplate, value);

				if (SelectedTemplate != null)
					_particleEffect.Template = SelectedTemplate;
			}
		}

		/// <summary>
		///     Resets the particle effect.
		/// </summary>
		public void ResetEffect()
		{
			_particleEffect.Reset();
		}

		/// <summary>
		///     Resets the camera to the start position and orientation.
		/// </summary>
		public void ResetCamera()
		{
			_camera.Reset();
		}

		/// <summary>
		///     Draws a preview of the selected particle effect.
		/// </summary>
		public void DrawParticlePreview()
		{
			if (RenderOutput.RenderTarget == null)
				return;

			_camera.Viewport = RenderOutput.Viewport;

			RenderOutput.ClearColor(Colors.Black);
			RenderOutput.ClearDepth();

			if (_inputCaptured)
			{
				_inputDevice.Update();
				_camera.Update();
			}

			if (_particleEffect.IsCompleted)
				_particleEffect.Reset();

			var elapsedSeconds = (float)_clock.Seconds;
			_clock.Reset();

			_particleEffect.Update(elapsedSeconds);
			_particleEffect.Draw(RenderOutput);
		}

		/// <summary>
		///     Starts capturing input, moving the camera.
		/// </summary>
		public void CaptureInput()
		{
			if (_inputCaptured)
				return;

			_inputCaptured = true;
			Mouse.RelativeMouseMode = true;
			_camera.IsActive = true;
		}

		/// <summary>
		///     Stops capturing input, no longer moving the camera.
		/// </summary>
		public void ReleaseInput()
		{
			if (!_inputCaptured)
				return;

			_inputCaptured = false;
			Mouse.RelativeMouseMode = false;
			_camera.IsActive = false;
		}

		/// <summary>
		///     Closes the particle effect viewer.
		/// </summary>
		public void Close()
		{
			if (!IsDisposing)
				Mouse.RelativeMouseMode = _relativeMouseMode;

			if (Application.Current.Window.LayoutRoot.Children.Contains(_view))
				Application.Current.Window.LayoutRoot.Remove(_view);

			ParticleTemplates.SafeDisposeAll();
			_camera.SafeDispose();

			_inputDevice.SafeDispose();
			_particleEffect.SafeDispose();
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_assetBundles.SafeDisposeAll();
			_renderContext.SafeDispose();

			Close();
		}
	}
}