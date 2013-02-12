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
	/// <summary>
	///   Abstract base class for an heightmap renderer
	/// </summary>
	public abstract class AbstractHeightmapRenderer : AbstractRenderer
	{
		#region Fields

		/// <summary>
		///   If wrapping is/ enabled, and the initial point is on the edge of
		///   the noise map, the appropriate neighbors that lie outside of the
		///   noise map will "wrap" to the opposite side(s) of the noise map.
		///   Enabling wrapping is useful when creating tileable heightmap
		/// </summary>
		protected bool _WrapEnabled = false;

		/// <summary>
		///   Lower height boundary of the heightmap
		/// </summary>
		protected float _lowerHeightBound = 0f;

		/// <summary>
		///   Upper height boundary of the heightmap
		/// </summary>
		protected float _upperHeightBound = 0f;

		#endregion

		#region Accessors

		/// <summary>
		///   Gets or sets the lower height boundary of the heightmap
		/// </summary>
		public float LowerHeightBound
		{
			get { return _lowerHeightBound; }
		}

		/// <summary>
		///   Gets or sets the upper height boundary of the heightmap
		/// </summary>
		public float UpperHeightBound
		{
			get { return _upperHeightBound; }
		}

		/// <summary>
		///   Enables or disables heightmap wrapping.
		/// </summary>
		public bool WrapEnabled
		{
			get { return _WrapEnabled; }
			set { _WrapEnabled = value; }
		}

		#endregion

		#region Ctor/Dtor

		/// <summary>
		///   template constructor
		/// </summary>
		public AbstractHeightmapRenderer()
		{
			_WrapEnabled = false;
		}

		#endregion

		#region Interaction

		/// <summary>
		///   Sets the boundaries of the heightmap.
		///   @throw ArgumentException if the lower boundary equals the upper boundary
		///   or if the lower boundary is greater than upper boundary
		/// </summary>
		/// <param name="lowerBound">The lower boundary of the heightmap</param>
		/// <param name="upperBound">The upper boundary of the heightmap</param>
		public void SetBounds(float lowerBound, float upperBound)
		{
			if (lowerBound == upperBound || lowerBound > upperBound)
				throw new ArgumentException("Incoherent bounds : lowerBound == upperBound or lowerBound > upperBound");

			_lowerHeightBound = lowerBound;
			_upperHeightBound = upperBound;
		}

		/// <summary>
		///   Find in the noise map the lowest and highest value to define
		///   the LowerHeightBound and UpperHeightBound
		/// </summary>
		public void ExactFit()
		{
			_noiseMap.MinMax(out _lowerHeightBound, out _upperHeightBound);
		}

		/// <summary>
		///   Renders the destination heightmap using the contents of the source
		///   noise map
		/// </summary>
		/// This class defines the main algorithm, children must implement
		/// RenderHeight() method to render a value for the target heightmap
		public override void Render()
		{
			if (_noiseMap == null)
				throw new ArgumentException("A noise map must be provided");

			if (CheckHeightmap() == false)
				throw new ArgumentException("An heightmap must be provided");

			if (_noiseMap.Width <= 0 || _noiseMap.Height <= 0)
				throw new ArgumentException("Incoherent noise map size (0,0)");

			if (_lowerHeightBound == _upperHeightBound || _lowerHeightBound > _upperHeightBound)
				throw new ArgumentException("Incoherent bounds : lowerBound == upperBound or lowerBound > upperBound");

			int width = _noiseMap.Width;
			int height = _noiseMap.Height;

			int rightEdge = width - 1;
			int topEdge = height - 1;

			int leftEdge = 0;
			int bottomEdge = 0;

			SetHeightmapSize(width, height);

			float pSource, pSourceOffset;

			int yOffset, xOffset;

			float boundDiff = _upperHeightBound - _lowerHeightBound;

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					pSource = _noiseMap.GetValue(x, y);

					if (_WrapEnabled)
					{
						if (x == rightEdge)
						{
// right edge
							xOffset = leftEdge; // left edge
						}
						else if (x == leftEdge)
						{
// left edge
							xOffset = rightEdge; // right edge
						}
						else
						{
							// anywhere
							xOffset = x; // same
						}

						if (y == topEdge)
						{
							// top edge
							yOffset = bottomEdge; //bottom edge
						}
						else if (y == bottomEdge)
						{
							// bottom edge
							yOffset = topEdge; //top edge
						}
						else
						{
							// anywhere
							yOffset = y; // same
						}

						// Lerp between edge values
						if (xOffset != x || yOffset != y)
						{
							pSourceOffset = _noiseMap.GetValue(xOffset, yOffset);
							pSource = Libnoise.Lerp(pSource, pSourceOffset, 0.5f);
						}
					}

					// Implemented by children
					RenderHeight(x, y, pSource, boundDiff);
				}

				if (_callBack != null)
					_callBack(y);
			}
		}

		#endregion

		#region internal

		/// <summary>
		/// </summary>
		/// <returns></returns>
		protected abstract bool CheckHeightmap();

		/// <summary>
		///   Sets the new size for the target heightmap.
		/// </summary>
		/// <param name="width">width The new width for the heightmap</param>
		/// <param name="height">height The new height for the heightmap</param>
		protected abstract void SetHeightmapSize(int width, int height);

		/// <summary>
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="source"></param>
		/// <param name="boundDiff"></param>
		protected abstract void RenderHeight(int x, int y, float source, float boundDiff);

		#endregion
	}
}