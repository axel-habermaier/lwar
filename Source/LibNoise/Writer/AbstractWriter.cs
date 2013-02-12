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

namespace LibNoise.Writer
{
	using System.IO;

	/// <summary>
	///   Abstract base class for all writer classes
	/// </summary>
	public abstract class AbstractWriter
	{
		#region Fields

		/// <summary>
		///   the name of the file to write.
		/// </summary>
		protected string _filename;

		/// <summary>
		///   A binary writer
		/// </summary>
		protected BinaryWriter _writer;

		#endregion

		#region Accessors

		/// <summary>
		///   Gets or sets the name of the file to write.
		/// </summary>
		public string Filename
		{
			get { return _filename; }
			set { _filename = value; }
		}

		#endregion

		#region Interaction

		/// <summary>
		///   Writes the destination content
		/// </summary>
		public abstract void WriteFile();

		#endregion

		#region internal

		/// <summary>
		///   Create a new BinaryWriter
		/// </summary>
		protected void OpenFile()
		{
			if (_writer != null)
				return; // Should throw exception ?

			if (File.Exists(_filename))
			{
				try
				{
					File.Delete(_filename);
				}
				catch (Exception e)
				{
					throw new IOException("Unable to delete destination file", e);
				}
			}

			BufferedStream stream;

			try
			{
				stream = new BufferedStream(new FileStream(_filename, FileMode.Create));
			}
			catch (Exception e)
			{
				throw new IOException("Unable to create destination file", e);
			}

			_writer = new BinaryWriter(stream);
		}

		/// <summary>
		///   Release a BinaryWriter previously opened
		/// </summary>
		protected void CloseFile()
		{
			try
			{
				_writer.Flush();
				_writer.Close();
				_writer = null;
			}
			catch (Exception e)
			{
				throw new IOException("Unable to release stream", e);
			}
		}

		#endregion
	}
}