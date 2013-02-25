﻿using System;

namespace Pegasus.Framework.Platform
{
	/// <summary>
	///   Represents a fatal error that causes the execution of the application to be aborted.
	/// </summary>
	public class ApplicationAbortedException : Exception
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="message">A message explaining the fatal error.</param>
		public ApplicationAbortedException(string message)
			: base(message)
		{
		}
	}
}