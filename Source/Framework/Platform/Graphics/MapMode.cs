﻿using System;

namespace Pegasus.Framework.Platform.Graphics
{
	/// <summary>
	///   Indicates how a resource is accessed by the CPU.
	/// </summary>
	public enum MapMode
	{
		/// <summary>
		///   Resource is mapped for reading. The resource must have been created with read access.
		/// </summary>
		Read = 1701,

		/// <summary>
		///   Resource is mapped for writing. The resource must have been created with write access.
		/// </summary>
		Write = 1702,

		/// <summary>
		///   Resource is mapped for reading and writing. The resource must have been created with read and write access.
		/// </summary>
		ReadWrite = 1703,

		/// <summary>
		///   Resource is mapped for writing; the previous contents of the resource will be undefined. The resource must
		///   have been created with write access and dynamic usage.
		/// </summary>
		WriteDiscard = 1704,

		/// <summary>
		///   Resource is mapped for writing; the existing contents of the resource cannot be overwritten. This flag is
		///   only valid on vertex and index buffers. The resource must have been created with write access.
		/// </summary>
		WriteNoOverwrite = 1705,
	}
}