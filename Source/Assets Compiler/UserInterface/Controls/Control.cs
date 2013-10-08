﻿using System;

namespace Pegasus.AssetsCompiler.UserInterface.Controls
{
	/// <summary>
	///   Provides metadata for the 'Control' UI class.
	/// </summary>
	[RuntimeNamespace("Pegasus.Framework.UserInterface.Controls")]
	internal class Control : UIElement
	{
		public ControlTemplate Template { get; set; }
	}
}