// This file is part of Libnoise-dotnet.
// Libnoise-dotnet is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// Libnoise-dotnet is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// You should have received a copy of the GNU Lesser General Public License
// along with Libnoise-dotnet.  If not, see <http://www.gnu.org/licenses/>.

using System;

namespace LibNoise.Builder
{
	/// <summary>
	///   Filter level.
	/// </summary>
	public enum FilterLevel
	{
		/// <summary>
		///   Caller should use Constant property.
		/// </summary>
		Constant,

		/// <summary>
		///   Caller should use source module value.
		/// </summary>
		Source,

		/// <summary>
		///   Caller should use FilterValue method.
		/// </summary>
		Filter
	}

	/// <summary>
	///   Interface for builder filter.
	/// </summary>
	public interface IBuilderFilter
	{
		#region Public Properties

		/// <summary>
		///   Gets or sets constant value.
		/// </summary>
		float ConstantValue { get; set; }

		#endregion

		#region Public Methods and Operators

		/// <summary>Filter value.</summary>
		/// <param name="x">The x.</param>
		/// <param name="y">The y.</param>
		/// <param name="source">The source.</param>
		/// <returns>Filtered value.</returns>
		float FilterValue(int x, int y, float source);

		/// <summary>Is filtered.</summary>
		/// <param name="x">The X.</param>
		/// <param name="y">The Y.</param>
		/// <returns>Filter level.</returns>
		FilterLevel IsFiltered(int x, int y);

		#endregion
	}
}