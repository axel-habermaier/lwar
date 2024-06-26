﻿namespace Pegasus.Rendering.Particles.Emitteres
{
	using System;
	using System.Diagnostics;
	using Math;
	using Modifiers;
	using Platform.Graphics;
	using Platform.Memory;
	using Utilities;

	/// <summary>
	///     Emits, updates, and removes particles of a particle effect, with all particles sharing the same properties and
	///     modifiers.
	/// </summary>
	public abstract class Emitter : DisposableObject
	{
		/// <summary>
		///     The number of times per second that the emitter searches for dead particles and removes them.
		/// </summary>
		private const int RemovalRate = 30;

		/// <summary>
		///     The maximum number of live particles supported by the emitter.
		/// </summary>
		private int _capacity;

		/// <summary>
		///     The particles managed by the emitter.
		/// </summary>
		private ParticleCollection _particles;

		/// <summary>
		///     The renderer that is used to render the particles of the emitter.
		/// </summary>
		private ParticleRenderer _renderer;

		/// <summary>
		///     The number of seconds since the last particle was emitted.
		/// </summary>
		private float _secondsSinceLastEmit;

		/// <summary>
		///     The number of seconds since the emitter removed dead particles.
		/// </summary>
		private float _secondsSinceLastRemoval;

		/// <summary>
		///     The total seconds since the emitter emitted was first updated.
		/// </summary>
		private double _totalSeconds;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		protected Emitter()
		{
			Duration = Single.PositiveInfinity;
			InitialScale = new Range<float>(1);
		}

		/// <summary>
		///     Gets or sets the modifiers affecting the particles of the emitter.
		/// </summary>
		public ModifierCollection Modifiers { get; set; }

		/// <summary>
		///     Gets or sets the maximum number of live particles supported by the emitter.
		/// </summary>
		public int Capacity
		{
			get { return _capacity; }
			set
			{
				if (_capacity == value)
					return;

				Assert.InRange(value, 1, Int32.MaxValue);

				_capacity = value;
				_particles.SafeDispose();
				_particles = new ParticleCollection(_capacity);
				ParticleCount = Math.Min(ParticleCount, _capacity);

				if (Renderer != null)
					Renderer.Capacity = _capacity;
			}
		}

		/// <summary>
		///     Gets or sets the renderer that is used to render the particles of the emitter.
		/// </summary>
		public ParticleRenderer Renderer
		{
			get { return _renderer; }
			set
			{
				Assert.ArgumentNotNull(value);

				if (_renderer == value)
					return;

				if (_renderer != null)
					_renderer.IsUsed = false;

				_renderer = value;
				_renderer.IsUsed = true;
				_renderer.Capacity = _capacity;
			}
		}

		/// <summary>
		///     Gets or sets the amount of time in seconds that the emitter emits new particles. Single.PositiveInfinity means that
		///     emitting never stops.
		/// </summary>
		public float Duration { get; set; }

		/// <summary>
		///     Gets or sets the number of particles that are emitted per second.
		/// </summary>
		public int EmissionRate { get; set; }

		/// <summary>
		///     Gets or sets the range of the initial particle colors.
		/// </summary>
		public Range<Color> InitialColor { get; set; }

		/// <summary>
		///     Gets or sets the range of the initial particle scales.
		/// </summary>
		public Range<float> InitialScale { get; set; }

		/// <summary>
		///     Gets or sets the range of the initial particle life time.
		/// </summary>
		public Range<float> InitialLifetime { get; set; }

		/// <summary>
		///     Gets or sets the range of the initial particle speeds.
		/// </summary>
		public Range<float> InitialSpeed { get; set; }

		/// <summary>
		///     Gets a value indicating whether the emitter is completed, i.e., there are no living particles and no new particles will
		///     be emitted.
		/// </summary>
		public bool IsCompleted
		{
			get { return ParticleCount == 0 && _totalSeconds > Duration; }
		}

		/// <summary>
		///     Gets the number of active particles.
		/// </summary>
		internal int ParticleCount { get; private set; }

		/// <summary>
		///     Resets the particle emitter, restarting it.
		/// </summary>
		internal void Reset()
		{
			_totalSeconds = 0;
			ParticleCount = 0;
			_secondsSinceLastEmit = 0;
			_secondsSinceLastRemoval = 0;

			if (Modifiers != null)
				Modifiers.ResetState();
		}

		/// <summary>
		///     Updates the particles already emitted by the emitter, removing dead ones. Emits new particles, if necessary and
		///     appropriate.
		/// </summary>
		/// <param name="elapsedSeconds">The number of seconds that have elapsed since the last update.</param>
		internal void Update(float elapsedSeconds)
		{
			Validate();
			_totalSeconds += elapsedSeconds;

			RemoveParticles(elapsedSeconds);
			UpdateParticles(elapsedSeconds);
			EmitParticles(elapsedSeconds);

			if (Modifiers != null && ParticleCount != 0)
				Modifiers.Execute(_particles, ParticleCount, elapsedSeconds);
		}

		/// <summary>
		///     Draws the particles of the emitter to the given render output.
		/// </summary>
		/// <param name="renderOutput">The render output the particles should be drawn to.</param>
		internal void Draw(RenderOutput renderOutput)
		{
			Assert.ArgumentNotNull(renderOutput);
			Assert.NotNull(Renderer);

			Renderer.Draw(renderOutput, _particles, ParticleCount);
		}

		/// <summary>
		///     Removes all dead particles.
		/// </summary>
		/// <param name="elapsedSeconds">The number of seconds that have elapsed since the last update.</param>
		private unsafe void RemoveParticles(float elapsedSeconds)
		{
			// We don't want to search for dead particles during each update for performance reasons.
			_secondsSinceLastRemoval += elapsedSeconds;
			if (_secondsSinceLastRemoval < 1.0f / RemovalRate)
				return;

			_secondsSinceLastRemoval = 0;
			for (var i = 0; i < ParticleCount; ++i)
			{
				if (_particles.Lifetimes[i] > 0)
					continue;

				_particles.Copy(source: ParticleCount - 1, target: i);
				--ParticleCount;
			}
		}

		/// <summary>
		///     Emits new particles, if necessary and appropriate.
		/// </summary>
		/// <param name="elapsedSeconds">The number of seconds that have elapsed since the last update.</param>
		private unsafe void EmitParticles(float elapsedSeconds)
		{
			if (_totalSeconds > Duration)
				return;

			_secondsSinceLastEmit += elapsedSeconds;
			var count = (int)(EmissionRate * _secondsSinceLastEmit);
			count = Math.Min(count, _particles.Capacity - ParticleCount);

			if (count <= 0)
				return;

			var lifetimes = _particles.Lifetimes + ParticleCount;
			var initialLifetimes = _particles.InitialLifetimes + ParticleCount;
			var age = _particles.Age + ParticleCount;
			var positions = _particles.Positions + ParticleCount * 3;
			var velocities = _particles.Velocities + ParticleCount * 3;
			var colors = _particles.Colors + ParticleCount * 4;
			var scales = _particles.Scales + ParticleCount;

			_secondsSinceLastEmit = 0;
			ParticleCount += count;

			InitializeParticles(positions, velocities, count);

			while (count-- > 0)
			{
				*initialLifetimes = RandomValues.NextSingle(InitialLifetime.LowerBound, InitialLifetime.UpperBound);
				*lifetimes = *initialLifetimes;
				*age = 1;
				*scales = RandomValues.NextSingle(InitialScale.LowerBound, InitialScale.UpperBound);

				colors[0] = RandomValues.NextByte(InitialColor.LowerBound.Red, InitialColor.UpperBound.Red);
				colors[1] = RandomValues.NextByte(InitialColor.LowerBound.Green, InitialColor.UpperBound.Green);
				colors[2] = RandomValues.NextByte(InitialColor.LowerBound.Blue, InitialColor.UpperBound.Blue);
				colors[3] = RandomValues.NextByte(InitialColor.LowerBound.Alpha, InitialColor.UpperBound.Alpha);

				lifetimes += 1;
				initialLifetimes += 1;
				age += 1;
				colors += 4;
				scales += 1;
			}
		}

		/// <summary>
		///     Updates the life times and the positions of the particles.
		/// </summary>
		/// <param name="elapsedSeconds">The number of seconds that have elapsed since the last update.</param>
		private unsafe void UpdateParticles(float elapsedSeconds)
		{
			var lifetimes = _particles.Lifetimes;
			var initialLifetime = _particles.InitialLifetimes;
			var age = _particles.Age;
			var positions = _particles.Positions;
			var velocities = _particles.Velocities;
			var count = ParticleCount;

			while (count-- > 0)
			{
				var lifetime = *lifetimes - elapsedSeconds;
				lifetime = lifetime < 0 ? 0 : lifetime;

				*lifetimes = lifetime;
				*age = lifetime / *initialLifetime;

				positions[0] += velocities[0] * elapsedSeconds;
				positions[1] += velocities[1] * elapsedSeconds;
				positions[2] += velocities[2] * elapsedSeconds;

				lifetimes += 1;
				initialLifetime += 1;
				age += 1;
				positions += 3;
				velocities += 3;
			}
		}

		/// <summary>
		///     In debug builds, validates the configuration of the emitter.
		/// </summary>
		[Conditional("DEBUG"), DebuggerHidden]
		private void Validate()
		{
			Assert.InRange(EmissionRate, 1, Int32.MaxValue);
			Assert.InRange(Capacity, 1, Int32.MaxValue);
			Assert.That(InitialLifetime.LowerBound >= 0, "Invalid particle life time.");
			Assert.That(InitialLifetime.UpperBound >= 0, "Invalid particle life time.");
			Assert.That(Duration > 0 || Single.IsPositiveInfinity(Duration), "Invalid duration.");
			Assert.That(InitialSpeed.LowerBound >= 0, "Invalid particle speed.");
			Assert.That(InitialSpeed.UpperBound >= 0, "Invalid particle speed.");
			Assert.NotNull(Renderer);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_particles.SafeDispose();
			Renderer.SafeDispose();
		}

		/// <summary>
		///     Initializes the position and velocity of the given number of newly emitted particles.
		/// </summary>
		/// <param name="positions">The positions of the particles.</param>
		/// <param name="velocities">The velocities of the particles.</param>
		/// <param name="count">The number of particles that should be initialized.</param>
		protected abstract unsafe void InitializeParticles(float* positions, float* velocities, int count);
	}
}