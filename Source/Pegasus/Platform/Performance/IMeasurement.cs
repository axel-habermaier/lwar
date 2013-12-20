namespace Pegasus.Platform.Performance
{
	using System;
	using System.Text;

	/// <summary>
	///   Represents a performance measurement.
	/// </summary>
	public interface IMeasurement
	{
		/// <summary>
		///   Invoked when the measurement should begin.
		/// </summary>
		void Begin();

		/// <summary>
		///   Invoked when the measurement should end.
		/// </summary>
		void End();

		/// <summary>
		///   Writes the results of the measurement into the given string builder.
		/// </summary>
		/// <param name="builder">The string builder the results should be written to.</param>
		void WriteResults(StringBuilder builder);
	}
}