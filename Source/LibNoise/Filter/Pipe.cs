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

using System;

namespace LibNoise.Filter
{
	/// <summary>
	///   Noise module that outputs the input source value without modification.
	///   Just a convenient class for any purpose.
	/// </summary>
	public class Pipe : FilterModule, IModule4D, IModule3D, IModule2D, IModule1D
	{
		#region Ctor/Dtor

		#endregion

		#region IModule1D Members

		/// <summary>
		///   Generates an output value given the coordinates of the specified input value.
		/// </summary>
		/// <param name="x">The input coordinate on the x-axis.</param>
		/// <returns>The resulting output value.</returns>
		public float GetValue(float x)
		{
			x *= _frequency;

			return _source1D.GetValue(x);
		}

		#endregion

		#region IModule2D Members

		/// <summary>
		///   Generates an output value given the coordinates of the specified input value.
		/// </summary>
		/// <param name="x">The input coordinate on the x-axis.</param>
		/// <param name="y">The input coordinate on the y-axis.</param>
		/// <returns>The resulting output value.</returns>
		public float GetValue(float x, float y)
		{
			x *= _frequency;
			y *= _frequency;

			return _source2D.GetValue(x, y);
		}

		#endregion

		#region IModule3D Members

		/// <summary>
		///   Generates an output value given the coordinates of the specified input value.
		/// </summary>
		/// <param name="x">The input coordinate on the x-axis.</param>
		/// <param name="y">The input coordinate on the y-axis.</param>
		/// <param name="z">The input coordinate on the z-axis.</param>
		/// <returns>The resulting output value.</returns>
		public float GetValue(float x, float y, float z)
		{
			x *= _frequency;
			y *= _frequency;
			z *= _frequency;

			return _source3D.GetValue(x, y, z);
		}

		#endregion

		#region IModule4D Members

		/// <summary>
		///   Generates an output value given the coordinates of the specified input value.
		/// </summary>
		/// <param name="x">The input coordinate on the x-axis.</param>
		/// <param name="y">The input coordinate on the y-axis.</param>
		/// <param name="z">The input coordinate on the z-axis.</param>
		/// <param name="t">The input coordinate on the t-axis.</param>
		/// <returns>The resulting output value.</returns>
		public float GetValue(float x, float y, float z, float t)
		{
			x *= _frequency;
			y *= _frequency;
			z *= _frequency;
			t *= _frequency;

			return _source4D.GetValue(x, y, z, t);
		}

		#endregion
	}
}