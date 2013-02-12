// This file is part of Libnoise-dotnet.
//
// Libnoise-dotnet is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Libnoise-dotnet is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with Libnoise-dotnet.  If not, see <http://www.gnu.org/licenses/>.

using System;

namespace LibNoise.Builder
{
	using Renderer;

	/// <summary>
	///   Shape filter.
	/// </summary>
	public class ShapeFilter : IBuilderFilter
	{
		#region Nested type: LevelCache

		/// <summary>
		///   A simple 2d-coordinates struct used as a cached value
		/// </summary>
		protected struct LevelCache
		{
			/// <summary>
			///   Level.
			/// </summary>
			public byte Level;

			private int _x;

			private int _y;

			/// <summary>
			///   Default constructor.
			/// </summary>
			/// <param name="x">X.</param>
			/// <param name="y">Y.</param>
			/// <param name="level">Level.</param>
			public LevelCache(int x, int y, byte level)
			{
				_x = x;
				_y = y;
				Level = level;
			}

			/// <summary>
			///   IsCached.
			/// </summary>
			/// <param name="x">X.</param>
			/// <param name="y">Y.</param>
			public bool IsCached(int x, int y)
			{
				return _x == x && _y == y;
			}

			/// <summary>
			///   Update.
			/// </summary>
			/// <param name="x">X.</param>
			/// <param name="y">Y.</param>
			/// <param name="level">Level.</param>
			public void Update(int x, int y, byte level)
			{
				_x = x;
				_y = y;
				Level = level;
			}
		}

		#endregion

		#region Constants

		/// <summary>
		///   Default value.
		/// </summary>
		public const float DefaultValue = -0.5f;

		#endregion

		#region Fields

		/// <summary>
		/// </summary>
		protected LevelCache Cache = new LevelCache(-1, -1, 0);

		/// <summary>
		/// </summary>
		protected float Constant = DefaultValue;

		/// <summary>
		///   The shape image
		/// </summary>
		protected IMap2D<IColor> PShape;

		#endregion

		#region Accessors

		/// <summary>
		///   Gets or sets the shape image
		/// </summary>
		public IMap2D<IColor> Shape
		{
			get { return PShape; }
			set { PShape = value; }
		}

		/// <summary>
		///   the constant output value.
		/// </summary>
		public float ConstantValue
		{
			get { return Constant; }
			set { Constant = value; }
		}

		#endregion

		#region Ctor/Dtor

		#endregion

		#region Interaction

		/// <summary>
		///   Return the filter level at this position
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public FilterLevel IsFiltered(int x, int y)
		{
			byte level = GetGreyscaleLevel(x, y);

			if (level == byte.MinValue)
				return FilterLevel.Constant;

			return level == byte.MaxValue ? FilterLevel.Source : FilterLevel.Filter;
		}

		/// <summary>
		///   Filter value.
		/// </summary>
		/// <param name="x">X.</param>
		/// <param name="y">Y.</param>
		/// <param name="source">Source.</param>
		/// <returns>Filtered value.</returns>
		public float FilterValue(int x, int y, float source)
		{
			byte level = GetGreyscaleLevel(x, y);

			if (level == byte.MaxValue)
			{
				//|| source > _constant
				return source;
			}
			if (level == byte.MinValue)
				return Constant;

			return Libnoise.Lerp(
				Constant,
				source,
				level / 255.0f
				);
		}

		#endregion

		#region Internal

		/// <summary>
		///   Get greyscale level.
		/// </summary>
		/// <param name="x">X.</param>
		/// <param name="y">Y.</param>
		/// <returns>Value.</returns>
		protected byte GetGreyscaleLevel(int x, int y)
		{
			// Is this position is stored in cache ?
			if (!Cache.IsCached(x, y))
			{
				// Assuming controlColor is a greyscale value
				// just test the red channel
				Cache.Update(x, y, PShape.GetValue(x, y).Red);
			}

			return Cache.Level;
		}

		#endregion
	}
}