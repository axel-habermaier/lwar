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
	using Pegasus.Framework.Math;

	/// <summary>
	///   Builds a noise cube map.
	///   This class builds a noise map by filling it with coherent-noise values
	///   generated from the surface of a sphere.
	/// </summary>
	public class CubeMapBuilder : NoiseMapBuilder
	{
		public override void Build()
		{
			if (PWidth < 0 || PHeight < 0)
				throw new ArgumentException("Dimension must be greater or equal 0");

			if (PWidth != PHeight)
				throw new ArgumentException("Width and height must be the same.");

			if (PSourceModule == null)
				throw new ArgumentException("A source module must be provided");

			if (PNoiseMap == null)
				throw new ArgumentException("A noise map must be provided");

			// Resize the destination noise map so that it can store the new output
			// values from the source model.
			PNoiseMap.SetSize(PWidth * 6, PHeight);

			var module = (IModule3D)PSourceModule;
			var radius = 10;
			var offset = 0;

			// Create a cube and project it into a sphere
			var topLeftFront = new Vector3(-radius, radius, radius);
			var topRightFront = new Vector3(radius, radius, radius);
			var topLeftBack = new Vector3(-radius, radius, -radius);
			var topRightBack = new Vector3(radius, radius, -radius);

			var bottomLeftFront = new Vector3(-radius, -radius, radius);
			var bottomRightFront = new Vector3(radius, -radius, radius);
			var bottomLeftBack = new Vector3(-radius, -radius, -radius);
			var bottomRightBack = new Vector3(radius, -radius, -radius);

			// Generates a face of the cube
			Action<Vector3, Vector3, Vector3> generateFace = (topLeft, topRight, bottomLeft) =>
				{
					var right = topRight - topLeft;
					var down = bottomLeft - topLeft;
					var delta = right.Length / PWidth;

					right = right.Normalize() * delta;
					down = down.Normalize() * delta;

					// Create the vertices
					for (var y = 0; y < PHeight; ++y)
					{
						for (var x = 0; x < PWidth; ++x)
						{
							// Compute the position of the vertex and project it onto the sphere
							var position = topLeft + x * right + y * down;
							position = position.Normalize() * radius;

							float finalValue;
							var level = FilterLevel.Source;

							if (PFilter != null)
								level = PFilter.IsFiltered(x, y);

							if (level == FilterLevel.Constant)
								finalValue = PFilter.ConstantValue;
							else
							{
								finalValue = module.GetValue(position.X, position.Y, position.Z);

								if (level == FilterLevel.Filter)
									finalValue = PFilter.FilterValue(x, y, finalValue);
							}

							PNoiseMap.SetValue(x + offset, y, finalValue);
						}

						if (PCallBack != null)
							PCallBack(y);
					}

					offset += PWidth;
				};

			// Generate the cube faces
			generateFace(topRightBack, topLeftBack, bottomRightBack); // Back
			generateFace(topLeftBack, topLeftFront, bottomLeftBack); // Left
			generateFace(topLeftFront, topRightFront, bottomLeftFront); // Front
			generateFace(topRightFront, topRightBack, bottomRightFront); // Right
			generateFace(topLeftBack, topRightBack, topLeftFront); // Top
			generateFace(bottomLeftFront, bottomRightFront, bottomLeftBack); // Bottom
		}
	}
}