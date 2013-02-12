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

namespace LibNoise.Builder
{
	/// A delegate to a callback function used by the NoiseMapBuilder class.
	/// 
	/// The NoiseMapBuilder::Build() method calls this callback function each
	/// time it fills a row of the noise map with coherent-noise values.
	/// 
	/// This callback function has a single integer parameter that contains
	/// a count of the rows that have been completed.  It returns void.  Pass
	/// a function with this signature to the NoiseMapBuilder::SetCallback()
	/// method.
	public delegate void NoiseMapBuilderCallback(int row);

	/// <summary>
	///   Abstract base class for a noise-map builder
	///   A builder class builds a noise map by filling it with coherent-noise
	///   values generated from the surface of a three-dimensional mathematical
	///   object.  Each builder class defines a specific three-dimensional
	///   surface, such as a cylinder, sphere, or plane.
	///   A builder class describes these input values using a coordinate system
	///   applicable for the mathematical object (e.g., a latitude/longitude
	///   coordinate system for the spherical noise-map builder.)  It then
	///   "flattens" these coordinates onto a plane so that it can write the
	///   coherent-noise values into a two-dimensional noise map.
	///   <b>Building the Noise Map</b>
	///   To build the noise map, perform the following steps:
	///   - Pass the bounding coordinates to the SetBounds() method.
	///   - Pass the noise map size, in points, to the SetDestSize() method.
	///   - Pass a NoiseMap object to the SetDestNoiseMap() method.
	///   - Pass a noise module (derived from Noise.Module) to the
	///   SetSourceModule() method.
	///   - Call the Build() method.
	///   You may also pass a callback function to the SetCallback() method.
	///   The Build() method calls this callback function each time it fills a
	///   row of the noise map with coherent-noise values.  This callback
	///   function has a single integer parameter that contains a count of the
	///   rows that have been completed.  It returns void.
	///   Note that SetBounds() is not defined in the abstract base class; it is
	///   only defined in the derived classes.  This is because each model uses
	///   a different coordinate system.
	/// </summary>
	public abstract class NoiseMapBuilder
	{
		#region Fields

		/// <summary>
		///   The callback function that Build() calls each time it fills a
		///   row of the noise map with coherent-noise values.
		/// </summary>
		protected NoiseMapBuilderCallback PCallBack;

		/// <summary>
		///   Filter.
		/// </summary>
		protected IBuilderFilter PFilter;

		/// <summary>
		///   The height of the map.
		/// </summary>
		protected int PHeight = 0;

		/// <summary>
		///   The destination noise map that will contain the coherent-noise values.
		/// </summary>
		protected IMap2D<float> PNoiseMap;

		/// <summary>
		///   The source input module.
		/// </summary>
		protected IModule PSourceModule;

		/// <summary>
		///   The width of the map.
		/// </summary>
		protected int PWidth = 0;

		#endregion

		#region Accessors

		/// <summary>
		///   Gets or sets the source module.
		/// </summary>
		public IModule SourceModule
		{
			get { return PSourceModule; }
			set { PSourceModule = value; }
		}

		/// <summary>
		///   Gets or sets the noise map.
		/// </summary>
		public IMap2D<float> NoiseMap
		{
			get { return PNoiseMap; }
			set { PNoiseMap = value; }
		}

		/// <summary>
		///   Gets or sets the callback function.
		/// </summary>
		public NoiseMapBuilderCallback CallBack
		{
			get { return PCallBack; }
			set { PCallBack = value; }
		}

		/// <summary>
		///   Gets the width of the NoiseMap.
		/// </summary>
		public int Width
		{
			get { return PWidth; }
		}

		/// <summary>
		///   Gets the height of the NoiseMap.
		/// </summary>
		public int Height
		{
			get { return PHeight; }
		}

		/// <summary>
		///   Filter.
		/// </summary>
		public IBuilderFilter Filter
		{
			get { return PFilter; }
			set { PFilter = value; }
		}

		#endregion

		#region Ctor/Dtor

		#endregion

		#region Interaction

		/// <summary>
		///   Builds the noise map.
		///   @pre SetBounds() was previously called.
		///   @pre NoiseMap was previously defined.
		///   @pre a SourceModule was previously defined.
		///   @pre The width and height values specified by SetSize() are
		///   positive.
		///   @pre The width and height values specified by SetSize() do not
		///   exceed the maximum possible width and height for the noise map.
		///   @post The original contents of the destination noise map is
		///   destroyed.
		///   @throw noise::ArgumentException See the preconditions.
		///   If this method is successful, the destination noise map contains
		///   the coherent-noise values from the noise module specified by
		///   the SourceModule.
		/// </summary>
		public abstract void Build();

		/// <summary>
		///   Sets the new size for the destination noise map.
		///   This method does not change the size of the destination noise map
		///   until the Build() method is called.
		///   @pre The width and height values are positive.
		///   @throw ArgumentException See the preconditions.
		/// </summary>
		/// <param name="width">width The new width for the destination noise map.</param>
		/// <param name="height">height The new height for the destination noise map.</param>
		public void SetSize(int width, int height)
		{
			if (width < 0 || height < 0)
				throw new ArgumentException("Dimension must be greater or equal 0");

			PHeight = height;
			PWidth = width;
		}

		#endregion
	}
}