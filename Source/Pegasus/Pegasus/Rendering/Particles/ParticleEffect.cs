namespace Pegasus.Rendering.Particles
{
	using System;
	using System.Collections.Generic;
	using Math;
	using Platform;
	using Platform.Graphics;
	using Platform.Memory;
	using Scripting;

	/// <summary>
	///     Represents a particle effect consisting of one or more particle emitters.
	/// </summary>
	public sealed class ParticleEffect : DisposableObject
	{
		/// <summary>
		///     The particle effect template the particle effect is initialized from.
		/// </summary>
		private ParticleEffectTemplate _template;

		/// <summary>
		///     The world matrix used to render the particles.
		/// </summary>
		private Matrix _worldMatrix;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public ParticleEffect()
		{
			Emitters = new List<Emitter>();
		}

		/// <summary>
		///     Gets or sets the emitters of the particle effect.
		/// </summary>
		public List<Emitter> Emitters { get; set; }

		/// <summary>
		///     Gets or sets the world matrix used to render the particles.
		/// </summary>
		public Matrix WorldMatrix
		{
			get
			{
				Assert.NotDisposed(this);
				return _worldMatrix;
			}
			set
			{
				Assert.NotNull(Emitters);
				Assert.NotDisposed(this);

				_worldMatrix = value;

				foreach (var emitter in Emitters)
				{
					Assert.NotNull(emitter.Renderer, "The world matrix cannot be set as long as an emitter has no renderer.");
					emitter.Renderer.ChangeWorldMatrix(ref _worldMatrix);
				}
			}
		}

		/// <summary>
		///     Gets a value indicating whether the particle effect is completed, i.e., there are no living particles and no new
		///     particles will be emitted.
		/// </summary>
		public bool IsCompleted
		{
			get
			{
				Assert.NotNull(Emitters);
				Assert.NotDisposed(this);

				// Not using Linq for GC reasons, as it is expected that 
				// this property is called multiple times per frame.
				foreach (var emitter in Emitters)
				{
					if (!emitter.IsCompleted)
						return false;
				}

				return true;
			}
		}

		/// <summary>
		///     Gets or sets the template the particle effect is initialized from.
		/// </summary>
		public ParticleEffectTemplate Template
		{
			get { return _template; }
			set
			{
				Assert.ArgumentNotNull(value);
				Assert.NotDisposed(this);

				if (_template == value)
					return;

				// Only register the event handler if we haven't done so before
				if (_template == null)
					Commands.OnReloadAssets += InitializeFromTemplate;

				_template = value;
				InitializeFromTemplate();
			}
		}

		/// <summary>
		///     Initializes the particle effect from its template.
		/// </summary>
		private void InitializeFromTemplate()
		{
			Assert.NotNull(_template, "The particle effect is not created from a template.");
			Assert.NotDisposed(_template);

			Emitters.SafeDisposeAll();
			_template.Initialize(this);
		}

		/// <summary>
		///     Resets the particle effect, restarting it.
		/// </summary>
		public void Reset()
		{
			Assert.NotDisposed(this);

			if (Emitters == null)
				return;

			foreach (var emitter in Emitters)
				emitter.Reset();
		}

		/// <summary>
		///     Updates the particle effect.
		/// </summary>
		/// <param name="elapsedSeconds">The number of seconds that have elapsed since the last update.</param>
		public unsafe void Update(float elapsedSeconds)
		{
			Assert.NotDisposed(this);

			if (Emitters == null)
				return;

			double updateTime;
			using (TimeMeasurement.Measure(&updateTime))
			{
				foreach (var emitter in Emitters)
				{
					emitter.Update(elapsedSeconds);
					ParticleStatistics.ParticleCount += emitter.ParticleCount;
				}
			}

			ParticleStatistics.UpdateTime += updateTime;
		}

		/// <summary>
		///     Draws the particle effect to the given render output.
		/// </summary>
		/// <param name="renderOutput">The render output the particle effect should be drawn to.</param>
		public unsafe void Draw(RenderOutput renderOutput)
		{
			Assert.ArgumentNotNull(renderOutput);
			Assert.NotDisposed(this);

			if (Emitters == null)
				return;

			double drawTime;
			using (TimeMeasurement.Measure(&drawTime))
			{
				foreach (var emitter in Emitters)
					emitter.Draw(renderOutput);
			}

			ParticleStatistics.DrawTime += drawTime;
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Emitters.SafeDisposeAll();

			if (_template != null)
				Commands.OnReloadAssets -= InitializeFromTemplate;
		}
	}
}