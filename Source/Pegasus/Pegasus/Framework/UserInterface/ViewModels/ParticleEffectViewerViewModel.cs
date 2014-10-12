namespace Pegasus.Framework.UserInterface.ViewModels
{
	using System;
	using System.Linq;
	using Input;
	using Platform;
	using Platform.Graphics;
	using Platform.Memory;
	using Rendering;
	using Rendering.Particles;
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
		///     The clock used for time measurements.
		/// </summary>
		private Clock _clock;

		/// <summary>
		///     A value other than zero indicates that input is captured and the preview camera is controlled.
		/// </summary>
		private int _inputCaptured;

		/// <summary>
		///     The input device that is used to control the camera.
		/// </summary>
		private LogicalInputDevice _inputDevice;

		/// <summary>
		///     Indicates whether the mouse was captured when the viewer was opened.
		/// </summary>
		private bool _mouseWasCaptured;

		/// <summary>
		///     The particle effect that is previewed.
		/// </summary>
		private ParticleEffect _particleEffect = new ParticleEffect();

		/// <summary>
		///     The particle effect template that is currently selected.
		/// </summary>
		private ParticleEffectTemplate _selectedTemplate;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public ParticleEffectViewerViewModel()
		{
			var templates = from assembly in AppDomain.CurrentDomain.GetAssemblies()
							from type in assembly.GetTypes()
							where type.IsClass && !type.IsAbstract && typeof(ParticleEffectTemplate).IsAssignableFrom(type)
							let template = (ParticleEffectTemplate)Activator.CreateInstance(type)
							orderby template.DisplayName
							select template;

			_mouseWasCaptured = Application.Current.Window.MouseCaptured;
			_view = new ParticleEffectViewerView();
			_inputDevice = new LogicalInputDevice(_view, usePreviewEvents: true);
			_view.DataContext = this;

			ParticleTemplates = templates.ToArray();
			Camera = new DebugCamera(Application.Current.GraphicsDevice, _inputDevice) { MoveSpeed = 250 };

			foreach (var template in ParticleTemplates)
				template.Load(Application.Current.GraphicsDevice, Application.Current.Assets);

			Application.Current.Window.LayoutRoot.Add(_view);
			Application.Current.Window.MouseCaptured = false;

			ResetCamera();
		}

		/// <summary>
		///     Gets the debug camera that is used to draw the particle effect preview.
		/// </summary>
		public DebugCamera Camera { get; private set; }

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
			Camera.Reset();
		}

		/// <summary>
		///     Draws a preview of the selected particle effect.
		/// </summary>
		/// <param name="renderOutput">The render output the preview should be drawn to.</param>
		public void OnDraw(RenderOutput renderOutput)
		{
			renderOutput.ClearColor(Colors.Black);
			renderOutput.ClearDepth();

			if (_inputCaptured != 0)
			{
				_inputDevice.Update();
				Camera.Update();
			}

			if (_particleEffect.IsCompleted)
				_particleEffect.Reset();

			var elapsedSeconds = (float)_clock.Seconds;
			_clock.Reset();

			_particleEffect.Update(elapsedSeconds);
			_particleEffect.Draw(renderOutput);

			Camera.IsActive = _inputCaptured != 0;
		}

		/// <summary>
		///     Starts capturing input, moving the camera.
		/// </summary>
		public void CaptureInput()
		{
			if (_inputCaptured++ == 0)
				Application.Current.Window.MouseCaptured = true;
		}

		/// <summary>
		///     Stops capturing input, no longer moving the camera.
		/// </summary>
		public void ReleaseInput()
		{
			if (--_inputCaptured == 0)
				Application.Current.Window.MouseCaptured = false;
		}

		/// <summary>
		///     Closes the particle effect viewer.
		/// </summary>
		public void Close()
		{
			Application.Current.Window.LayoutRoot.Remove(_view);
			Application.Current.Window.MouseCaptured = _mouseWasCaptured;
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			if (Application.Current.Window.LayoutRoot.Children.Contains(_view))
				Close();

			ParticleTemplates.SafeDisposeAll();
			Camera.SafeDispose();

			_inputDevice.SafeDispose();
			_particleEffect.SafeDispose();
		}
	}
}