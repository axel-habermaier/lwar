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
	/// <summary>
	///   Renders an image from a noise map.
	///   This class renders an image given the contents of a noise-map object.
	///   An application can configure the output of the image in three ways:
	///   - Specify the color gradient.
	///   - Specify the light source parameters.
	///   - Specify the background image.
	///   <b>Specify the color gradient</b>
	///   This class uses a color gradient to calculate the color for each pixel
	///   in the destination image according to the value from the corresponding
	///   position in the noise map.
	///   A color gradient is a list of gradually-changing colors.  A color
	///   gradient is defined by a list of <i>gradient points</i>.  Each
	///   gradient point has a position and a color.  In a color gradient, the
	///   colors between two adjacent gradient points are linearly interpolated.
	///   For example, suppose this class contains the following color gradient:
	///   - -1.0 maps to dark blue.
	///   - -0.2 maps to light blue.
	///   - -0.1 maps to tan.
	///   - 0.0 maps to green.
	///   - 1.0 maps to white.
	///   The value 0.5 maps to a greenish-white color because 0.5 is halfway
	///   between 0.0 (mapped to green) and 1.0 (mapped to white).
	///   The value -0.6 maps to a medium blue color because -0.6 is halfway
	///   between -1.0 (mapped to dark blue) and -0.2 (mapped to light blue).
	///   The color gradient requires a minimum of two gradient points.
	///   @note The color value ie the gradient color has an alpha
	///   channel.  This alpha channel specifies how a pixel in the background
	///   image (if specified) is blended with the calculated color.  If the
	///   alpha value is high, this class weighs the blend towards the
	///   calculated color, and if the alpha value is low, this class weighs the
	///   blend towards the color from the corresponding pixel in the background
	///   image.
	///   <b>Specify the light source parameters</b>
	///   This class contains a parallel light source that lights the image.  It
	///   interprets the noise map as a bump map.
	///   To enable or disable lighting, pass a Boolean value to the
	///   EnableLight property.
	///   To set the position of the light source in the "sky", use the
	///   LightAzimuth and LightElevation properties.
	///   To set the color of the light source, use the LightColor property.
	///   To set the intensity of the light source, use the LightIntensity property.
	///   A good intensity value is 2.0, although that value tends to
	///   "wash out" very light colors from the image.
	///   To set the contrast amount between areas in light and areas in shadow,
	///   use the LightContrast property.  Determining the correct contrast
	///   amount requires some trial and error, but if your application
	///   interprets the noise map as a height map that has its elevation values
	///   measured in meters and has a horizontal resolution of h meters, a
	///   good contrast amount to use is ( 1.0 / h ).
	///   <b>Specify the background image</b>
	///   To specify a background image, pass an Image object to the
	///   BackgroundImage property.
	///   This class determines the color of a pixel in the destination image by
	///   blending the calculated color with the color of the corresponding
	///   pixel from the background image.
	///   The blend amount is determined by the alpha of the calculated color.
	///   If the alpha value is high, this class weighs the blend towards the
	///   calculated color, and if the alpha value is low, this class weighs the
	///   blend towards the color from the corresponding pixel in the background
	///   image.
	///   <b>Rendering the image</b>
	///   To render the image, perform the following steps:
	///   - Pass a <see cref="GradientColor" /> object to the Gradient property.
	///   - Pass a <see cref="IMap2D{Single}" /> object to the NoiseMap property.
	///   - Pass an <see cref="IMap2D{Color}" /> object to the Image property.
	///   - Pass an <see cref="Image" /> object to the BackgroundImage property (optional)
	///   - Call the Render() method.
	/// </summary>
	public class ImageRenderer : AbstractImageRenderer
	{
		#region Fields

		/// <summary>
		///   The background image
		/// </summary>
		private Image _backgroundImage;

		/// <summary>
		///   The cosine of the azimuth of the light source.
		/// </summary>
		private float _cosAzimuth;

		/// <summary>
		///   The cosine of the elevation of the light source.
		/// </summary>
		private float _cosElevation;

		/// <summary>
		///   The gradient color
		/// </summary>
		private GradientColor _gradient;

		/// <summary>
		///   The azimuth of the light source, in degrees.
		/// </summary>
		private float _lightAzimuth;

		/// <summary>
		///   The brightness of the light source.
		/// </summary>
		private float _lightBrightness;

		/// <summary>
		///   The color of the light source.
		/// </summary>
		private Color _lightColor;

		/// <summary>
		///   The contrast between areas in light and areas in shadow.
		/// </summary>
		private float _lightContrast;

		/// <summary>
		///   The elevation of the light source, in degrees.
		/// </summary>
		private float _lightElevation;

		/// <summary>
		///   A flag specifying whether lighting is enabled.
		/// </summary>
		private bool _lightEnabled;

		/// <summary>
		///   The intensity of the light source.
		/// </summary>
		private float _lightIntensity;

		/// <summary>
		///   Used by the CalcLightIntensity() method to recalculate the light
		///   values only if the light parameters change.
		///   When the light parameters change, this value is set to True.  When
		///   the CalcLightIntensity() method is called, this value is set to
		///   false.
		/// </summary>
		private bool _recalcLightValues;

		/// <summary>
		///   The sine of the azimuth of the light source.
		/// </summary>
		private float _sinAzimuth;

		/// <summary>
		///   The sine of the elevation of the light source.
		/// </summary>
		private float _sinElevation;

		/// <summary>
		///   A flag specifying whether wrapping is enabled.
		/// </summary>
		private bool _wrapEnabled;

		#endregion

		#region Accessors

		/// <summary>
		///   Enables or disables the light source.
		///   If the light source is enabled, this object will interpret the
		///   noise map as a bump map.
		/// </summary>
		public bool LightEnabled
		{
			get { return _lightEnabled; }
			set { _lightEnabled = value; }
		}

		/// <summary>
		///   Enables or disables noise-map wrapping.
		///   This object requires five points (the initial point and its four
		///   neighbors) to calculate light shading.  If wrapping is enabled,
		///   and the initial point is on the edge of the noise map, the
		///   appropriate neighbors that lie outside of the noise map will
		///   "wrap" to the opposite side(s) of the noise map.  Otherwise, the
		///   appropriate neighbors are cropped to the edge of the noise map.
		///   Enabling wrapping is useful when creating spherical renderings and
		///   tileable textures.
		/// </summary>
		public bool WrapEnabled
		{
			get { return _wrapEnabled; }
			set { _wrapEnabled = value; }
		}

		/// <summary>
		///   Gets or sets the destination image
		/// </summary>
		public Image BackgroundImage
		{
			get { return _backgroundImage; }
			set { _backgroundImage = value; }
		}

		/// <summary>
		///   Gets or sets the gradient color
		/// </summary>
		public GradientColor Gradient
		{
			get { return _gradient; }
			set { _gradient = value; }
		}

		/// <summary>
		///   Gets or Sets the azimuth of the light source, in degrees.
		///   The azimuth is the location of the light source around the
		///   horizon:
		///   - 0.0 degrees is east.
		///   - 90.0 degrees is north.
		///   - 180.0 degrees is west.
		///   - 270.0 degrees is south.
		/// </summary>
		public float LightAzimuth
		{
			get { return _lightAzimuth; }
			set
			{
				_lightAzimuth = value;
				_recalcLightValues = true;
			}
		}

		/// <summary>
		///   Gets ors sets the brightness of the light source.
		/// </summary>
		public float LightBrightness
		{
			get { return _lightBrightness; }
			set
			{
				_lightBrightness = value;
				_recalcLightValues = true;
			}
		}

		/// <summary>
		///   Gets or sets the contrast of the light source.
		///   The contrast specifies how sharp the boundary is between the
		///   light-facing areas and the shadowed areas.
		///   The contrast determines the difference between areas in light and
		///   areas in shadow.  Determining the correct contrast amount requires
		///   some trial and error, but if your application interprets the noise
		///   map as a height map that has a spatial resolution of h meters
		///   and an elevation resolution of 1 meter, a good contrast amount to
		///   use is ( 1.0 / h ).
		/// </summary>
		public float LightContrast
		{
			get { return _lightContrast; }
			set
			{
				if (value <= 0.0f)
					throw new ArgumentException("Contrast must be greater than 0");
				else
				{
					_lightContrast = value;
					_recalcLightValues = true;
				}
			}
		}

		/// <summary>
		///   Gets or sets the elevation of the light source, in degrees.
		///   The elevation is the angle above the horizon:
		///   - 0 degrees is on the horizon.
		///   - 90 degrees is straight up.
		/// </summary>
		public float LightElevation
		{
			get { return _lightElevation; }
			set
			{
				_lightElevation = value;
				_recalcLightValues = true;
			}
		}

		/// <summary>
		///   Gets or sets the intensity of the light source.
		/// </summary>
		public float LightIntensity
		{
			get { return _lightIntensity; }
			set
			{
				if (value < 0.0f)
					throw new ArgumentException("Intensity must be greater or equals to 0");
				else
				{
					_lightIntensity = value;
					_recalcLightValues = true;
				}
			}
		}

		/// <summary>
		///   Gets or sets the color of the light source.
		/// </summary>
		public Color LightColor
		{
			get { return _lightColor; }
			set { _lightColor = value; }
		}

		#endregion

		#region Ctor/Dtor

		/// <summary>
		///   Default constructor
		/// </summary>
		public ImageRenderer()
		{
			_lightEnabled = false;
			_wrapEnabled = false;
			_lightAzimuth = 45.0f;
			_lightBrightness = 1.0f;
			_lightContrast = 1.0f;
			_lightElevation = 45.0f;
			_lightIntensity = 1.0f;
			_lightColor = Colors.White;
			_recalcLightValues = true;
		}

		#endregion

		#region Interaction

		/// <summary>
		///   Renders the destination image using the contents of the source
		///   noise map and an optional background image.
		///   @pre NoiseMap has been defined.
		///   @pre Image has been defined.
		///   @pre There are at least two gradient points in the color gradient.
		///   @pre No two gradient points have the same position.
		///   @pre If a background image was specified, it has the exact same
		///   size as the source height map.
		///   @post The original contents of the destination image is destroyed.
		///   @throw ArgumentException See the preconditions.
		///   The background image and the destination image can safely refer to
		///   the same image, although in this case, the destination image is
		///   irretrievably blended into the background image.
		/// </summary>
		public override void Render()
		{
			if (_noiseMap == null)
				throw new ArgumentException("A noise map must be provided");

			if (_image == null)
				throw new ArgumentException("An image map must be provided");

			if (_noiseMap.Width <= 0 || _noiseMap.Height <= 0)
				throw new ArgumentException("Incoherent noise map size (0,0)");

			if (_gradient.CountGradientPoints() < 2)
				throw new ArgumentException("Not enought points in the gradient");

			int width = _noiseMap.Width;
			int height = _noiseMap.Height;
			int rightEdge = width - 1;
			int topEdge = height - 1;
			int leftEdgeOffset = -rightEdge;
			int bottomEdgeOffset = -topEdge;

			// If a background image was provided, make sure it is the same size the
			// source noise map.
			if (_backgroundImage != null)
			{
				if (_backgroundImage.Width != width || _backgroundImage.Height != height)
					throw new ArgumentException("Incoherent background image size");
			}

			// Create the destination image.  It is safe to reuse it if this is also the background image.
			if (!_image.Equals(_backgroundImage))
				_image.SetSize(width, height);

			IColor backgroundColor = Colors.White;
			IColor sourceColor;
			float pSource;

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					pSource = _noiseMap.GetValue(x, y);

					// Get the color based on the value at the current point in the noise
					// map.
					sourceColor = _gradient.GetColor(pSource);

					// If lighting is enabled, calculate the light intensity based on the
					// rate of change at the current point in the noise map.
					float lightIntensity;

					if (_lightEnabled)
					{
						// Calculate the positions of the current point's four-neighbors.
						int xLeftOffset, xRightOffset;
						int yUpOffset, yDownOffset;

						if (_wrapEnabled)
						{
							if (x == 0)
							{
								// left edge
								xLeftOffset = rightEdge; // right edge
								xRightOffset = 1; //next
							}
							else if (x == rightEdge)
							{
// right edge
								xLeftOffset = -1; // previous
								xRightOffset = leftEdgeOffset; // left edge
							}
							else
							{
								// anywhere
								xLeftOffset = -1; // previous
								xRightOffset = 1; // next
							}

							if (y == 0)
							{
								// bottom edge
								yDownOffset = topEdge; // top edge
								yUpOffset = 1; // above
							}
							else if (y == topEdge)
							{
								// top edge
								yDownOffset = -1; // under
								yUpOffset = bottomEdgeOffset; //bottom edge
							}
							else
							{
								// anywhere
								yDownOffset = -1; // under
								yUpOffset = 1; // above
							}
						}
						else
						{
							if (x == 0)
							{
								// left edge
								xLeftOffset = 0; // same
								xRightOffset = 1; // next
							}
							else if (x == rightEdge)
							{
								// right edge
								xLeftOffset = -1; // previous
								xRightOffset = 0; // same
							}
							else
							{
								// anywhere
								xLeftOffset = -1; // previous
								xRightOffset = 1; // next
							}

							if (y == 0)
							{
								// bottom edge
								yDownOffset = 0; // same
								yUpOffset = 1; // above
							}
							else if (y == topEdge)
							{
								// top edge
								yDownOffset = -1; // under
								yUpOffset = 0; // same
							}
							else
							{
								yDownOffset = -1; // under
								yUpOffset = 1; // above
							}
						}

						// Get the noise value of the current point in the source noise map
						// and the noise values of its four-neighbors.
						float nc = _noiseMap.GetValue(x, y);
						float nl = _noiseMap.GetValue(x + xLeftOffset, y);
						float nr = _noiseMap.GetValue(x + xRightOffset, y);
						float nd = _noiseMap.GetValue(x, y + yDownOffset);
						float nu = _noiseMap.GetValue(x, y + yUpOffset);

						// Now we can calculate the lighting intensity.
						lightIntensity = CalcLightIntensity(nc, nl, nr, nd, nu);
						lightIntensity *= _lightBrightness;
					}
					else
					{
						// These values will apply no lighting to the destination image.
						lightIntensity = 1.0f;
					}

					// Get the current background color from the background image.
					if (_backgroundImage != null)
						backgroundColor = _backgroundImage.GetValue(x, y);

					// Blend the source color, background color, and the light
					// intensity together, then update the destination image with that
					// color.
					_image.SetValue(x, y, CalcDestColor(sourceColor, backgroundColor, lightIntensity));
				}

				if (_callBack != null)
					_callBack(y);
			}
		}

		#endregion

		#region Internal

		/// <summary>
		///   Calculates the destination color.
		/// </summary>
		/// <param name="sourceColor">The source color generated from the color gradient</param>
		/// <param name="backgroundColor">The color from the background image at the corresponding position</param>
		/// <param name="lightValue">The intensity of the light at that position</param>
		/// <returns>The destination color</returns>
		private IColor CalcDestColor(IColor sourceColor, IColor backgroundColor, float lightValue)
		{
			float sourceRed = sourceColor.Red / 255.0f;
			float sourceGreen = sourceColor.Green / 255.0f;
			float sourceBlue = sourceColor.Blue / 255.0f;
			float sourceAlpha = sourceColor.Alpha / 255.0f;

			float backgroundRed = backgroundColor.Red / 255.0f;
			float backgroundGreen = backgroundColor.Green / 255.0f;
			float backgroundBlue = backgroundColor.Blue / 255.0f;

			// First, blend the source color to the background color using the alpha
			// of the source color.
			float red = Libnoise.Lerp(backgroundRed, sourceRed, sourceAlpha);
			float green = Libnoise.Lerp(backgroundGreen, sourceGreen, sourceAlpha);
			float blue = Libnoise.Lerp(backgroundBlue, sourceBlue, sourceAlpha);

			if (_lightEnabled)
			{
				// Now calculate the light color.
				float lightRed = lightValue * _lightColor.Red / 255.0f;
				float lightGreen = lightValue * _lightColor.Green / 255.0f;
				float lightBlue = lightValue * _lightColor.Blue / 255.0f;

				// Apply the light color to the new color.
				red *= lightRed;
				green *= lightGreen;
				blue *= lightBlue;
			}

			// Clamp the color channels to the (0..1) range.
			red = Libnoise.Clamp01(red);
			green = Libnoise.Clamp01(green);
			blue = Libnoise.Clamp01(blue);

			// Rescale the color channels to a byte range and return the new color.
			return new Color(
				(byte)((uint)(red * 255.0f) & 0xff),
				(byte)((uint)(green * 255.0f) & 0xff),
				(byte)((uint)(blue * 255.0f) & 0xff),
				Math.Max(sourceColor.Alpha, backgroundColor.Alpha)
				);
		}

		/// <summary>
		///   Calculates the intensity of the light given some elevation values.
		/// </summary>
		/// <param name="center">Elevation of the center point</param>
		/// <param name="left">Elevation of the point directly left of the center point</param>
		/// <param name="right">Elevation of the point directly right of the center point</param>
		/// <param name="down">Elevation of the point directly below the center point</param>
		/// <param name="up">Elevation of the point directly above the center point</param>
		/// <returns>These values come directly from the noise map</returns>
		private float CalcLightIntensity(float center, float left, float right, float down, float up)
		{
			// Recalculate the sine and cosine of the various light values if
			// necessary so it does not have to be calculated each time this method is
			// called.
			if (_recalcLightValues)
			{
				_cosAzimuth = (float)Math.Cos(_lightAzimuth * Libnoise.Deg2Rad);
				_sinAzimuth = (float)Math.Sin(_lightAzimuth * Libnoise.Deg2Rad);
				_cosElevation = (float)Math.Cos(_lightElevation * Libnoise.Deg2Rad);
				_sinElevation = (float)Math.Sin(_lightElevation * Libnoise.Deg2Rad);
				_recalcLightValues = false;
			}

			// Now do the lighting calculations.
			const float iMax = 1.0f;
			float io = iMax * Libnoise.Sqrt2 * _sinElevation / 2.0f;
			float ix = (iMax - io) * _lightContrast * Libnoise.Sqrt2 * _cosElevation * _cosAzimuth;
			float iy = (iMax - io) * _lightContrast * Libnoise.Sqrt2 * _cosElevation * _sinAzimuth;
			float intensity = (ix * (left - right) + iy * (down - up) + io);

			if (intensity < 0.0)
				intensity = 0.0f;

			return intensity;
		}

		#endregion
	}
}