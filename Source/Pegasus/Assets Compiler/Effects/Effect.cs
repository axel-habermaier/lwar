namespace Pegasus.AssetsCompiler.Effects
{
	using System;
	using Utilities;

	/// <summary>
	///     Represents an effect that provides shaders for drawing operations.
	/// </summary>
	[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
	public abstract class Effect
	{
		/// <summary>
		///     The position of the camera in world space.
		/// </summary>
		protected readonly Vector3 CameraPosition;

		/// <summary>
		///     The projection matrix that should be used to project the geometry onto the screen.
		/// </summary>
		protected readonly Matrix Projection;

		/// <summary>
		///     The view matrix of the camera that should be used to render the geometry.
		/// </summary>
		protected readonly Matrix View;

		/// <summary>
		///     The view matrix multiplied with the projection matrix.
		/// </summary>
		protected readonly Matrix ViewProjection;

		/// <summary>
		///     The size of the viewport.
		/// </summary>
		protected readonly Vector2 ViewportSize;

		/// <summary>
		///     Computes the sine of the given value.
		/// </summary>
		/// <param name="value">The value (in radians) whose sine should be computed.</param>
		[MapsTo(Intrinsic.Sine)]
		protected float Sin(float value)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///     Computes the cosine of the given value.
		/// </summary>
		/// <param name="value">The value (in radians) whose cosine should be computed.</param>
		[MapsTo(Intrinsic.Cosine)]
		protected float Cos(float value)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///     Computes the tangent of the given value.
		/// </summary>
		/// <param name="value">The value (in radians) whose tangent should be computed.</param>
		[MapsTo(Intrinsic.Tangent)]
		protected float Tan(float value)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///     Computes the arc sine of the given value.
		/// </summary>
		/// <param name="value">The value (in range [-1, 1]) whose arc sine should be computed.</param>
		[MapsTo(Intrinsic.ArcSine)]
		protected float Asin(float value)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///     Computes the arc cosine of the given value.
		/// </summary>
		/// <param name="value">The value (in range [-1, 1]) whose arc cosine should be computed.</param>
		[MapsTo(Intrinsic.ArcCosine)]
		protected float Acos(float value)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///     Computes the arc tangent of the given value.
		/// </summary>
		/// <param name="value">The value (in range [-1, 1]) whose arc tangent should be computed.</param>
		[MapsTo(Intrinsic.ArcTangent)]
		protected float Atan(float value)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///     Returns the smallest integer value that is greater than or equal to the specified value.
		/// </summary>
		/// <param name="value">The value that should be rounded up.</param>
		[MapsTo(Intrinsic.Ceil)]
		protected float Ceil(float value)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///     Returns the largest integer that is less than or equal to the specified value.
		/// </summary>
		/// <param name="value">The value that should be rounded down.</param>
		[MapsTo(Intrinsic.Floor)]
		protected float Floor(float value)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///     Clamps the specified value to the specified minimum and maximum range.
		/// </summary>
		/// <param name="value">The value that should be clamped.</param>
		/// <param name="min">The minimum value that the given value should be clamped to.</param>
		/// <param name="max">The maximum value that the given value should be clamped to.</param>
		[MapsTo(Intrinsic.Clamp)]
		protected float Clamp(float value, float min, float max)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///     Clamps the specified value to the specified minimum and maximum range.
		/// </summary>
		/// <param name="value">The value that should be clamped.</param>
		/// <param name="min">The minimum value that the given value should be clamped to.</param>
		/// <param name="max">The maximum value that the given value should be clamped to.</param>
		[MapsTo(Intrinsic.Clamp)]
		protected float Clamp(Vector2 value, Vector2 min, Vector2 max)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///     Clamps the specified value to the specified minimum and maximum range.
		/// </summary>
		/// <param name="value">The value that should be clamped.</param>
		/// <param name="min">The minimum value that the given value should be clamped to.</param>
		/// <param name="max">The maximum value that the given value should be clamped to.</param>
		[MapsTo(Intrinsic.Clamp)]
		protected float Clamp(Vector3 value, Vector3 min, Vector3 max)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///     Clamps the specified value to the specified minimum and maximum range.
		/// </summary>
		/// <param name="value">The value that should be clamped.</param>
		/// <param name="min">The minimum value that the given value should be clamped to.</param>
		/// <param name="max">The maximum value that the given value should be clamped to.</param>
		[MapsTo(Intrinsic.Clamp)]
		protected float Clamp(Vector4 value, Vector4 min, Vector4 max)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///     Returns the square root of the given value.
		/// </summary>
		/// <param name="value">The value for which the square root should be returned.</param>
		[MapsTo(Intrinsic.SquareRoot)]
		protected float Sqrt(float value)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///     Returns the inverse square root of the given value.
		/// </summary>
		/// <param name="value">The value for which the inverse square root should be returned.</param>
		[MapsTo(Intrinsic.InverseSquareRoot)]
		protected float InverseSqrt(float value)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///     Returns the base-e exponential of the specified value.
		/// </summary>
		/// <param name="value">The value for which the base-e exponential should be returned.</param>
		[MapsTo(Intrinsic.Exponential)]
		protected float Exp(float value)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///     Returns the specified value raised to the specified power.
		/// </summary>
		/// <param name="value">The value that should be raised to the specified power.</param>
		/// <param name="power">The power that the specifiec value should be raised to.</param>
		[MapsTo(Intrinsic.Power)]
		protected float Pow(float value, float power)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///     Returns the absolute value of the specified value.
		/// </summary>
		/// <param name="value">The value for which the absolute value should be returned.</param>
		[MapsTo(Intrinsic.Absolute)]
		protected float Abs(float value)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///     Rounds the specified value to the nearest integer.
		/// </summary>
		/// <param name="value">The value that should be rounded to the nearest integer.</param>
		[MapsTo(Intrinsic.Round)]
		protected float Round(float value)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///     Selects the greater of x and y.
		/// </summary>
		/// <param name="value1">The first value that should be selected if it is greater than the second value.</param>
		/// <param name="value2">The second value that should be selected if it is greater than the first value.</param>
		[MapsTo(Intrinsic.Max)]
		protected float Max(float value1, float value2)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///     Selects the lesser of x and y.
		/// </summary>
		/// <param name="value1">The first value that should be selected if it is less than the second value.</param>
		/// <param name="value2">The second value that should be selected if it is less than the first value.</param>
		[MapsTo(Intrinsic.Min)]
		protected float Min(float value1, float value2)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///     Returns a distance scalar between two vectors.
		/// </summary>
		/// <param name="origin">The origin of the distance calculation.</param>
		/// <param name="target">The target that determines the distance.</param>
		[MapsTo(Intrinsic.Distance)]
		protected float Distance(Vector2 origin, Vector2 target)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///     Returns a distance scalar between two vectors.
		/// </summary>
		/// <param name="origin">The origin of the distance calculation.</param>
		/// <param name="target">The target that determines the distance.</param>
		[MapsTo(Intrinsic.Distance)]
		protected float Distance(Vector3 origin, Vector3 target)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///     Returns a distance scalar between two vectors.
		/// </summary>
		/// <param name="origin">The origin of the distance calculation.</param>
		/// <param name="target">The target that determines the distance.</param>
		[MapsTo(Intrinsic.Distance)]
		protected float Distance(Vector4 origin, Vector4 target)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///     Returns the dot product of the two vectors.
		/// </summary>
		/// <param name="vector1">The first vector of the dot product.</param>
		/// <param name="vector2">The second vector of the dot product.</param>
		[MapsTo(Intrinsic.Dot)]
		protected float Dot(Vector2 vector1, Vector2 vector2)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///     Returns the dot product of the two vectors.
		/// </summary>
		/// <param name="vector1">The first vector of the dot product.</param>
		/// <param name="vector2">The second vector of the dot product.</param>
		[MapsTo(Intrinsic.Dot)]
		protected float Dot(Vector3 vector1, Vector3 vector2)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///     Returns the dot product of the two vectors.
		/// </summary>
		/// <param name="vector1">The first vector of the dot product.</param>
		/// <param name="vector2">The second vector of the dot product.</param>
		[MapsTo(Intrinsic.Dot)]
		protected float Dot(Vector4 vector1, Vector4 vector2)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///     Returns the cross product of the two vectors.
		/// </summary>
		/// <param name="vector1">The first vector of the cross product.</param>
		/// <param name="vector2">The second vector of the cross product.</param>
		[MapsTo(Intrinsic.Cross)]
		protected float Cross(Vector2 vector1, Vector2 vector2)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///     Returns the cross product of the two vectors.
		/// </summary>
		/// <param name="vector1">The first vector of the cross product.</param>
		/// <param name="vector2">The second vector of the cross product.</param>
		[MapsTo(Intrinsic.Cross)]
		protected float Cross(Vector3 vector1, Vector3 vector2)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///     Returns the cross product of the two vectors.
		/// </summary>
		/// <param name="vector1">The first vector of the cross product.</param>
		/// <param name="vector2">The second vector of the cross product.</param>
		[MapsTo(Intrinsic.Cross)]
		protected float Cross(Vector4 vector1, Vector4 vector2)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///     Normalizes the given vector to unit length and returns the result.
		/// </summary>
		/// <param name="vector">The vector that should be normalized.</param>
		[MapsTo(Intrinsic.Normalize)]
		protected Vector2 Normalize(Vector2 vector)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///     Normalizes the given vector to unit length and returns the result.
		/// </summary>
		/// <param name="vector">The vector that should be normalized.</param>
		[MapsTo(Intrinsic.Normalize)]
		protected Vector3 Normalize(Vector3 vector)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///     Normalizes the given vector to unit length and returns the result.
		/// </summary>
		/// <param name="vector">The vector that should be normalized.</param>
		[MapsTo(Intrinsic.Normalize)]
		protected Vector4 Normalize(Vector4 vector)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///     Clamps the given value within the range 0 and 1.
		/// </summary>
		/// <param name="value">The value that should be saturated.</param>
		[MapsTo(Intrinsic.Saturate)]
		protected Vector4 Saturate(float value)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///     Clamps the given value within the range 0 and 1.
		/// </summary>
		/// <param name="value">The value that should be saturated.</param>
		[MapsTo(Intrinsic.Saturate)]
		protected Vector4 Saturate(Vector2 value)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///     Clamps the given value within the range 0 and 1.
		/// </summary>
		/// <param name="value">The value that should be saturated.</param>
		[MapsTo(Intrinsic.Saturate)]
		protected Vector4 Saturate(Vector3 value)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///     Clamps the given value within the range 0 and 1.
		/// </summary>
		/// <param name="value">The value that should be saturated.</param>
		[MapsTo(Intrinsic.Saturate)]
		protected Vector4 Saturate(Vector4 value)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///     Performs a linear interpolation.
		/// </summary>
		/// <param name="source">The source value.</param>
		/// <param name="target">The target value.</param>
		/// <param name="amount">A value between 0 and 1 indicating the weight of the target value.</param>
		[MapsTo(Intrinsic.Lerp)]
		protected Vector4 Lerp(float source, float target, float amount)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///     Performs a linear interpolation.
		/// </summary>
		/// <param name="source">The source value.</param>
		/// <param name="target">The target value.</param>
		/// <param name="amount">A value between 0 and 1 for each component indicating the weight of the target value.</param>
		[MapsTo(Intrinsic.Lerp)]
		protected Vector4 Lerp(Vector2 source, Vector2 target, Vector2 amount)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///     Performs a linear interpolation.
		/// </summary>
		/// <param name="source">The source value.</param>
		/// <param name="target">The target value.</param>
		/// <param name="amount">A value between 0 and 1 for each component indicating the weight of the target value.</param>
		[MapsTo(Intrinsic.Lerp)]
		protected Vector4 Lerp(Vector3 source, Vector3 target, Vector3 amount)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///     Performs a linear interpolation.
		/// </summary>
		/// <param name="source">The source value.</param>
		/// <param name="target">The target value.</param>
		/// <param name="amount">A value between 0 and 1 for each component indicating the weight of the target value.</param>
		[MapsTo(Intrinsic.Lerp)]
		protected Vector4 Lerp(Vector4 source, Vector4 target, Vector4 amount)
		{
			throw new NotImplementedException();
		}
	}
}