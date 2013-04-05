﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Generated by the Pegasus Asset Compiler.
//     Friday, April 5, 2013, 17:11:59
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
		///   Provides access to the actual instances of the cvars or commands managed by the registry.
		/// </summary>
		public new InstanceList Instances { get; private set; }

		/// <summary>
		///   If true, all 3D geometry is drawn in wireframe mode.
		/// </summary>
		public bool DrawWireframe
		{
			get { return Instances.DrawWireframe.Value; }
			set { Instances.DrawWireframe.Value = value; }
		}

		/// <summary>
		///   If true, all 3D geometry is drawn in wireframe mode.
		/// </summary>
		public bool DrawWireframe2
		{
			get { return Instances.DrawWireframe2.Value; }
			set { Instances.DrawWireframe2.Value = value; }
		}

		/// <summary>
		///   If true, all 3D geometry is drawn in wireframe mode.
		/// </summary>
		public float DrawWireframe3
		{
			get { return Instances.DrawWireframe3.Value; }
			set { Instances.DrawWireframe3.Value = value; }
		}

		/// <summary>
		///   Initializes the registry.
		/// </summary>
		protected override void Initialize(object instances)
		{
			if (instances == null)
				instances = new InstanceList();

			Instances = (InstanceList)instances;
			base.Initialize(instances);

			Register(Instances.DrawWireframe, "draw_wireframe");
			Register(Instances.DrawWireframe2, "draw_wireframe2");
			Register(Instances.DrawWireframe3, "draw_wireframe3");
		}

		/// <summary>
		///   Stores the actual instances of the cvars or commands managed by the registry.
		/// </summary>
		public new class InstanceList : Pegasus.Framework.Scripting.CvarRegistry.InstanceList
		{
			/// <summary>
			///   Initializes a new instance.
			/// </summary>
			public InstanceList()
			{
				DrawWireframe = new Cvar<bool>("draw_wireframe", false, "If true, all 3D geometry is drawn in wireframe mode.", false);
				DrawWireframe2 = new Cvar<bool>("draw_wireframe2", false, "If true, all 3D geometry is drawn in wireframe mode.", true);
				DrawWireframe3 = new Cvar<float>("draw_wireframe3", 3.14f, "If true, all 3D geometry is drawn in wireframe mode.", true);
			}

			/// <summary>
			///   If true, all 3D geometry is drawn in wireframe mode.
			/// </summary>
			public Cvar<bool> DrawWireframe { get; private set; }

			/// <summary>
			///   If true, all 3D geometry is drawn in wireframe mode.
			/// </summary>
			public Cvar<bool> DrawWireframe2 { get; private set; }

			/// <summary>
			///   If true, all 3D geometry is drawn in wireframe mode.
			/// </summary>
			public Cvar<float> DrawWireframe3 { get; private set; }
		}
	}
}

