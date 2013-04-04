using System;

namespace Pegasus.Framework.Rendering
{
	using System.Runtime.InteropServices;
	using Math;
	using Platform.Graphics;

	/// <summary>
	///   Represents a rectangle with possibly non-axis aligned edges.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct Quad
	{
		/// <summary>
		///   The vertex that conceptually represents the bottom left corner of the quad.
		/// </summary>
		private VertexPositionColorTexture _bottomLeft;

		/// <summary>
		///   The vertex that conceptually represents the bottom right corner of the quad.
		/// </summary>
		private VertexPositionColorTexture _bottomRight;

		/// <summary>
		///   The vertex that conceptually represents the top left corner of the quad.
		/// </summary>
		private VertexPositionColorTexture _topLeft;

		/// <summary>
		///   The vertex that conceptually represents the top right corner of the quad.
		/// </summary>
		private VertexPositionColorTexture _topRight;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="rectangle">The position and size of the rectangular quad.</param>
		/// <param name="color">The color of the quad.</param>
		/// <param name="textureArea">
		///   The area of the texture that contains the quad's image data. If not given, the whole texture
		///   is placed onto the rectangle.
		/// </param>
		public Quad(RectangleF rectangle, Color color, RectangleF? textureArea = null)
			: this()
		{
			_bottomLeft.Position = new Vector4(rectangle.Left, rectangle.Bottom, 0, 1);
			_bottomRight.Position = new Vector4(rectangle.Right, rectangle.Bottom, 0, 1);
			_topLeft.Position = new Vector4(rectangle.Left, rectangle.Top, 0, 1);
			_topRight.Position = new Vector4(rectangle.Right, rectangle.Top, 0, 1);

			_bottomLeft.Color = color;
			_bottomRight.Color = color;
			_topLeft.Color = color;
			_topRight.Color = color;

			var texture = textureArea ?? new RectangleF(0, 0, 1, 1);
			_bottomLeft.TextureCoordinates = new Vector2(texture.Left, texture.Top);
			_bottomRight.TextureCoordinates = new Vector2(texture.Right, texture.Top);
			_topLeft.TextureCoordinates = new Vector2(texture.Left, texture.Bottom);
			_topRight.TextureCoordinates = new Vector2(texture.Right, texture.Bottom);
		}

		/// <summary>
		///   Changes the color of the quad.
		/// </summary>
		/// <param name="color">The new color of the quad.</param>
		public void SetColor(Color color)
		{
			_bottomLeft.Color = color;
			_bottomRight.Color = color;
			_topLeft.Color = color;
			_topRight.Color = color;
		}

		/// <summary>
		///   Applies the given transformation matrix to the quad's vertices.
		/// </summary>
		/// <param name="quad">The quad that should be transformed.</param>
		/// <param name="matrix">The transformation matrix that should be applied.</param>
		public static void Transform(ref Quad quad, ref Matrix matrix)
		{
			quad._bottomLeft.Position = Vector4.Transform(ref quad._bottomLeft.Position, ref matrix);
			quad._bottomRight.Position = Vector4.Transform(ref quad._bottomRight.Position, ref matrix);
			quad._topLeft.Position = Vector4.Transform(ref quad._topLeft.Position, ref matrix);
			quad._topRight.Position = Vector4.Transform(ref quad._topRight.Position, ref matrix);
		}
	}
}