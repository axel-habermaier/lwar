﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Generated by the Pegasus Asset Compiler.
//     Thursday, April 4, 2013, 17:43:55
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;

namespace Lwar.Client.Scripting
{
	using Pegasus.Framework.Scripting;

	public class CvarRegistry : Pegasus.Framework.Scripting.CvarRegistry
	{
		/// <summary>
		///   If true, all 3D geometry is drawn in wireframe mode.
		/// </summary>
		private readonly Cvar<bool> _drawWireframe = new Cvar<bool>("draw_wireframe", false, "If true, all 3D geometry is drawn in wireframe mode.");

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public CvarRegistry()
		{
			Register(_drawWireframe, "draw_wireframe");
		}

		/// <summary>
		///   If true, all 3D geometry is drawn in wireframe mode.
		/// </summary>
		public bool DrawWireframe
		{
			get { return _drawWireframe.Value; }
			set { _drawWireframe.Value = value; }
		}
	}
}

