﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Generated by the Pegasus Asset Compiler.
//     Tuesday, April 9, 2013, 15:43:09
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;

namespace Lwar.Client.Scripting
{
	using Network;
	using Pegasus.Framework;
	using Pegasus.Framework.Scripting;

	public class CvarRegistry : Pegasus.Framework.Scripting.CvarRegistry
	{
		/// <summary>
		///   Provides access to the actual instances of the cvars or commands managed by the registry.
		/// </summary>
		public new InstanceList Instances { get; private set; }

		/// <summary>
		///   The name of the player.
		/// </summary>
		public string PlayerName
		{
			get { return Instances.PlayerName.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value, () => value);
				Instances.PlayerName.Value = value;
			}
		}

		/// <summary>
		///   If true, all 3D geometry is drawn in wireframe mode.
		/// </summary>
		public bool DrawWireframe
		{
			get { return Instances.DrawWireframe.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value, () => value);
				Instances.DrawWireframe.Value = value;
			}
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

			Register(Instances.PlayerName, "player_name");
			Register(Instances.DrawWireframe, "draw_wireframe");
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
				PlayerName = new Cvar<string>("player_name", "UnnamedPlayer", "The name of the player.", true, new NotEmptyAttribute(), new MaximumLengthAttribute(Specification.MaximumPlayerNameLength, true));
				DrawWireframe = new Cvar<bool>("draw_wireframe", false, "If true, all 3D geometry is drawn in wireframe mode.", false);
			}

			/// <summary>
			///   The name of the player.
			/// </summary>
			public Cvar<string> PlayerName { get; private set; }

			/// <summary>
			///   If true, all 3D geometry is drawn in wireframe mode.
			/// </summary>
			public Cvar<bool> DrawWireframe { get; private set; }
		}
	}
}

