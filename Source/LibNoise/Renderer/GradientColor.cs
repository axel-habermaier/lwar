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

namespace LibNoise.Renderer
{
	using System.Collections.Generic;

	/// <summary>
	///   Well-known gradient colors.
	/// </summary>
	public static class GradientColors
	{
		/// <summary>
		///   Create a graysacle GradientColor
		/// </summary>
		public static GradientColor Grayscale
		{
			get
			{
				var gradient = new GradientColor();
				gradient.AddGradientPoint(-1.0f, Colors.Black);
				gradient.AddGradientPoint(1.0f, Colors.White);
				return gradient;
			}
		}

		/// <summary>
		///   Create an empty GradientColor
		/// </summary>
		public static GradientColor Empty
		{
			get
			{
				var gradient = new GradientColor();
				gradient.AddGradientPoint(-1.0f, Colors.Transparent);
				gradient.AddGradientPoint(1.0f, Colors.Transparent);
				return gradient;
			}
		}

		/// <summary>
		///   Create an terrain  GradientColor
		/// </summary>
		public static GradientColor Terrain
		{
			get
			{
				var gradient = new GradientColor();

				gradient.AddGradientPoint(-1.0f, new Color(0, 0, 128, 255)); // deeps
				gradient.AddGradientPoint(-0.25f, new Color(0, 0, 255, 255)); // shallow
				gradient.AddGradientPoint(0.0f, new Color(0, 128, 255, 255)); // shore
				gradient.AddGradientPoint(0.0625f, new Color(240, 240, 64, 255)); // sand
				gradient.AddGradientPoint(0.125f, new Color(32, 160, 0, 255)); // grass
				gradient.AddGradientPoint(0.375f, new Color(224, 224, 0, 255)); // dirt
				gradient.AddGradientPoint(0.75f, new Color(128, 128, 128, 255)); // rock
				gradient.AddGradientPoint(1.0f, new Color(255, 255, 255, 255)); // snow

				/* From noise::utils
				gradient.AddGradientPoint(-1.00f, Color(0, 0, 128, 255));
				gradient.AddGradientPoint(-0.20f, Color(32, 64, 128, 255));
				gradient.AddGradientPoint(-0.04f, Color(64, 96, 192, 255));
				gradient.AddGradientPoint(-0.02f, Color(192, 192, 128, 255));
				gradient.AddGradientPoint(0.00f, Color(0, 192, 0, 255));
				gradient.AddGradientPoint(0.25f, Color(192, 192, 0, 255));
				gradient.AddGradientPoint(0.50f, Color(160, 96, 64, 255));
				gradient.AddGradientPoint(0.75f, Color(128, 255, 255, 255));
				gradient.AddGradientPoint(1.00f, Color(255, 255, 255, 255));
				*/

				return gradient;
			}
		}
	}

	/// <summary>
	///   Defines a color gradient.
	///   A color gradient is a list of gradually-changing colors.  A color
	///   gradient is defined by a list of <i>gradient points</i>.  Each
	///   gradient point has a position and a color.  In a color gradient, the
	///   colors between two adjacent gradient points are linearly interpolated.
	///   To add a gradient point to the color gradient, pass its position and
	///   color to the AddGradientPoint() method.
	///   To retrieve a color from a specific position in the color gradient,
	///   pass that position to the GetColor() method.
	///   This class is a useful tool for coloring height maps based on
	///   elevation.
	///   <b>Gradient example</b>
	///   Suppose a gradient object contains the following gradient points:
	///   - -1.0 maps to black.
	///   - 0.0 maps to white.
	///   - 1.0 maps to red.
	///   If an application passes -0.5 to the GetColor() method, this method
	///   will return a gray color that is halfway between black and white.
	///   If an application passes 0.25 to the GetColor() method, this method
	///   will return a very light pink color that is one quarter of the way
	///   between white and red.
	/// </summary>
	public class GradientColor
	{
		#region Fields

		/// <summary>
		///   The gradient points list
		/// </summary>
		private readonly List<GradientPoint> _gradientPoints = new List<GradientPoint>(10);

		#endregion

		#region Accessors

		#endregion

		#region Ctor/Dtor

		/// <summary>
		///   Create an empty GradientColor
		/// </summary>
		public GradientColor()
		{
		}

		/// <summary>
		///   Create a new GradientColor with one color
		/// </summary>
		/// <param name="color">color at position -1 and 1</param>
		public GradientColor(IColor color)
		{
			AddGradientPoint(-1.0f, color);
			AddGradientPoint(1.0f, color);
		}

		/// <summary>
		///   Create a new GradientColor betwwen start and end
		/// </summary>
		/// <param name="start">The start color at position -1</param>
		/// <param name="end">The end color at position 1</param>
		public GradientColor(IColor start, IColor end)
		{
			AddGradientPoint(-1.0f, start);
			AddGradientPoint(1.0f, end);
		}

		#endregion

		#region Interaction

		/// <summary>
		///   Adds a gradient point to this gradient object.
		///   No two gradient points have the same position.
		///   @throw System.ArgumentException if two control points have the same position.
		///   It does not matter which order these gradient points are added.
		/// </summary>
		/// <param name="position">The position of this gradient point</param>
		/// <param name="color">The color of this gradient point</param>
		public void AddGradientPoint(float position, IColor color)
		{
			AddGradientPoint(new GradientPoint(position, color));
		}

		/// <summary>
		///   Adds a gradient point to this gradient object.
		///   No two gradient points have the same position.
		///   @throw System.ArgumentException if two control points have the same position.
		///   It does not matter which order these gradient points are added.
		/// </summary>
		/// <param name="point">The gradient point to add</param>
		public void AddGradientPoint(GradientPoint point)
		{
			if (_gradientPoints.Contains(point))
			{
				throw new ArgumentException(
					String.Format(
						"Cannont insert GradientPoint({0}, {1}) : Each GradientPoint is required to contain a unique position",
						point.Position, point.Color));
			}
			_gradientPoints.Add(point);

			// Desc order
			_gradientPoints.Sort(delegate(GradientPoint p1, GradientPoint p2)
				{
					if (p1.Position > p2.Position)
						return 1;
					if (p1.Position < p2.Position)
						return -1;
					return 0;
				});
		}

		/// <summary>
		///   Deletes all the gradient points from this gradient object.
		///   All gradient points from this gradient object are deleted.
		/// </summary>
		public void Clear()
		{
			_gradientPoints.Clear();
		}

		/// <summary>
		///   Returns the color at the specified position in the color gradient.
		/// </summary>
		/// <param name="position">The specified position</param>
		/// <returns>The color at that position</returns>
		public IColor GetColor(float position)
		{
			//System.Diagnostics.Debug.Assert(_gradientPoints.Count >= 2, "At least two points must be defined");

			// Find the first element in the gradient point array that has a gradient
			// position larger than the gradient position passed to this method.
			int indexPos;

			for (indexPos = 0; indexPos < _gradientPoints.Count; indexPos++)
			{
				if (position < _gradientPoints[indexPos].Position)
					break;
			}

			// Find the two nearest gradient points so that we can perform linear
			// interpolation on the color.
			int index0 = Libnoise.Clamp(indexPos - 1, 0, _gradientPoints.Count - 1);
			int index1 = Libnoise.Clamp(indexPos, 0, _gradientPoints.Count - 1);

			// If some gradient points are missing (which occurs if the gradient
			// position passed to this method is greater than the largest gradient
			// position or less than the smallest gradient position in the array), get
			// the corresponding gradient color of the nearest gradient point and exit
			// now.
			if (index0 == index1)
				return _gradientPoints[index1].Color;

			// Compute the alpha value used for linear interpolation.
			float input0 = _gradientPoints[index0].Position;
			float input1 = _gradientPoints[index1].Position;
			float alpha = (position - input0) / (input1 - input0);

			// Now perform the linear interpolation given the alpha value.
			return Color.Lerp(_gradientPoints[index0].Color, _gradientPoints[index1].Color, alpha);
		}

		/// <summary>
		///   Return the size of the GradientPoint list
		/// </summary>
		/// <returns>The number of GradientPoint in the list</returns>
		public int CountGradientPoints()
		{
			return _gradientPoints.Count;
		}

		/// <summary>
		///   Returns a read-only <see cref="IList{GradientPoint}" /> wrapper for the current GradientPoint list.
		/// </summary>
		/// <returns>The read only list.</returns>
		public IList<GradientPoint> GetGradientPoints()
		{
			return _gradientPoints.AsReadOnly();
		}

		#endregion
	}
}