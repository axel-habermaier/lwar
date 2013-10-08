namespace Pegasus.AssetsCompiler.UserInterface
{
	using System;

	/// <summary>
	///   Provides metadata for the 'Thickness' UI struct.
	/// </summary>
	[RuntimeNamespace("Pegasus.Framework.UserInterface")]
	internal struct Thickness
	{
		public double Bottom;
		public double Left;
		public double Right;
		public double Top;

		public Thickness(double width)
		{
			Left = width;
			Right = width;
			Top = width;
			Bottom = width;
		}

		public Thickness(double left, double right, double top, double bottom)
		{
			Left = left;
			Right = right;
			Top = top;
			Bottom = bottom;
		}
	}
}