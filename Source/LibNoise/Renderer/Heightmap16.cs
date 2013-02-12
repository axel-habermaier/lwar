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

using System;

namespace LibNoise.Renderer
{
	using Utils;

	/// <summary>
	///   Implements a 16 bits Heightmap, a 2-dimensional array of unsigned short values (0 to 65 535)
	/// </summary>
	public class Heightmap16 : DataMap<ushort>, IMap2D<ushort>
	{
		#region Ctor/Dtor

		/// <summary>
		///   0-args constructor
		/// </summary>
		public Heightmap16()
		{
			BorderValue = ushort.MinValue;
			AllocateBuffer();
		}

		/// <summary>
		///   Create a new Heightmap16 with the given values
		///   The width and height values must be positive.
		/// </summary>
		/// <param name="width">The width of the new noise map.</param>
		/// <param name="height">The height of the new noise map</param>
		public Heightmap16(int width, int height)
		{
			BorderValue = ushort.MinValue;
			AllocateBuffer(width, height);
		}

		/// <summary>
		///   Copy constructor
		/// </summary>
		/// <param name="copy">The heightmap to copy</param>
		public Heightmap16(Heightmap16 copy)
		{
			BorderValue = ushort.MinValue;
			CopyFrom(copy);
		}

		#endregion

		#region Interaction

		/// <summary>
		///   Find the lowest and highest value in the map
		/// </summary>
		/// <param name="min">the lowest value</param>
		/// <param name="max">the highest value</param>
		public void MinMax(out ushort min, out ushort max)
		{
			min = max = 0;
			ushort[] data = Data;

			if (data != null && data.Length > 0)
			{
				// First value, min and max for now
				min = max = data[0];

				for (int i = 0; i < data.Length; i++)
				{
					if (min > data[i])
						min = data[i];
					else if (max < data[i])
						max = data[i];
				}
			}
		}

		#endregion

		#region Internal

		/// <summary>
		///   Return the memory size of a ushort
		/// </summary>
		/// <returns>The memory size of a ushort</returns>
		protected override int SizeofT()
		{
			return 16;
		}

		/// <summary>
		///   Return the maximum value of a ushort type (65535)
		/// </summary>
		/// <returns></returns>
		protected override ushort MaxvalofT()
		{
			return ushort.MaxValue;
		}

		/// <summary>
		///   Return the minimum value of a ushort type (0)
		/// </summary>
		/// <returns></returns>
		protected override ushort MinvalofT()
		{
			return ushort.MinValue;
		}

		#endregion
	}
}