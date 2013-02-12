// This file is part of libnoise-dotnet.
//
// libnoise-dotnet is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// libnoise-dotnet is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with libnoise-dotnet.  If not, see <http://www.gnu.org/licenses/>.
// 
// From the original Jason Bevins's Libnoise (http://libnoise.sourceforge.net)

using System;

namespace LibNoise
{
	/// <summary>
	///   Base class for all noise primitive.
	/// </summary>
	public abstract class PrimitiveModule : IModule
	{
		#region Constants

		/// <summary>
		///   Default noise seed for the noise module.
		/// </summary>
		public const int DefaultSeed = 0;

		/// <summary>
		///   Default noise quality for the noise module.
		/// </summary>
		public const NoiseQuality DefaultQuality = NoiseQuality.Standard;

		#endregion

		#region Fields

		/// <summary>
		///   The quality of the Perlin noise.
		/// </summary>
		private NoiseQuality _quality = DefaultQuality;

		/// <summary>
		///   The seed value used by the Perlin-noise function.
		/// </summary>
		private int _seed = DefaultSeed;

		#endregion

		#region Accessors

		/// <summary>
		///   Gets or sets the seed of the perlin noise.
		/// </summary>
		public virtual int Seed
		{
			get { return _seed; }
			set { _seed = value; }
		}

		/// <summary>
		///   Gets or sets the quality
		/// </summary>
		public virtual NoiseQuality Quality
		{
			get { return _quality; }
			set { _quality = value; }
		}

		#endregion

		#region Ctor/Dtor

		/// <summary>
		///   A 0-args constructor
		/// </summary>
		protected PrimitiveModule()
			: this(DefaultSeed, DefaultQuality)
		{
		}

		/// <summary>
		///   A basic connstrucutor
		/// </summary>
		/// <param name="seed"></param>
		protected PrimitiveModule(int seed)
			: this(seed, DefaultQuality)
		{
		}

		/// <summary>
		///   A basic connstrucutor
		/// </summary>
		/// <param name="seed"></param>
		/// <param name="quality"></param>
		protected PrimitiveModule(int seed, NoiseQuality quality)
		{
			_seed = seed;
			_quality = quality;
		}

		#endregion
	}
}