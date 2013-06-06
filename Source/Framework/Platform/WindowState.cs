﻿using System;

namespace Pegasus.Framework.Platform
{
	/// <summary>
	///   Indicates the whether the window is minimized, maximized, or neither minimized nor maximized.
	/// </summary>
	public enum WindowState
	{
		/// <summary>
		///   Indicates that the window is neither minimized nor maximized.
		/// </summary>
		Normal = 1,

		/// <summary>
		///   Indicates that the window is maximized, filling the entire screen.
		/// </summary>
		Maximized = 2,

		/// <summary>
		///   Indicates that the window is minimized and invisible.
		/// </summary>
		Minimized = 3
	}
}