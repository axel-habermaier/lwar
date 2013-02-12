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
	///   Implements a 8 bits Heightmap, a 2-dimensional array of unsigned byte values (0 to 255).
	/// </summary>
	public class Heightmap8 : DataMap<byte>, IMap2D<byte>
	{
		#region Ctor/Dtor

		/// <summary>
		///   0-args constructor.
		/// </summary>
		public Heightmap8()
		{
			BorderValue = byte.MinValue;
			AllocateBuffer();
		}

		/// <summary>
		///   Create a new Heightmap8 with the given values
		///   The width and height values must be positive.
		/// </summary>
		/// <param name="width">The width of the new noise map.</param>
		/// <param name="height">The height of the new noise map</param>
		public Heightmap8(int width, int height)
		{
			BorderValue = byte.MinValue;
			AllocateBuffer(width, height);
		}

		/// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="copy">The heightmap to copy.</param>
		public Heightmap8(Heightmap8 copy)
		{
			BorderValue = byte.MinValue;
			CopyFrom(copy);
		}

		#endregion

		#region Interaction

		/// <summary>
		///   Find the lowest and highest value in the map.
		/// </summary>
		/// <param name="min">The lowest value.</param>
		/// <param name="max">The highest value.</param>
		public void MinMax(out byte min, out byte max)
		{
			min = max = 0;
			byte[] data = Data;

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
		///   Return the memory size of a unsigned byte.
		/// </summary>
		/// <returns>The memory size of a unsigned byte.</returns>
		protected override int SizeofT()
		{
			return 8;
		}

		/// <summary>
		///   Return the maximum value of a unsigned byte type (255).
		/// </summary>
		/// <returns>Maximum value.</returns>
		protected override byte MaxvalofT()
		{
			return byte.MaxValue;
		}

		/// <summary>
		///   Return the minimum value of a unsigned byte type (0).
		/// </summary>
		/// <returns>Minimum value.</returns>
		protected override byte MinvalofT()
		{
			return byte.MinValue;
		}

		#endregion
	}
}