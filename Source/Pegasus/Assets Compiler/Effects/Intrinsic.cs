namespace Pegasus.AssetsCompiler.Effects
{
	using System;

	/// <summary>
	///     Describes an intrinsic shader function.
	/// </summary>
	internal enum Intrinsic
	{
		Unknown,
		Sample,
		SampleLevel,
		Sine,
		Cosine,
		Tangent,
		ArcSine,
		ArcCosine,
		ArcTangent,
		Ceil,
		Floor,
		Clamp,
		SquareRoot,
		InverseSquareRoot,
		Exponential,
		Power,
		Absolute,
		Round,
		Max,
		Min,
		Distance,
		Dot,
		Cross,
		Normalize,
		Saturate,
		Lerp
	}
}