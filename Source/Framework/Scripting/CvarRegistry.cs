﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Generated by the Pegasus Asset Compiler.
//     Friday, April 5, 2013, 16:30:18
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;

namespace Pegasus.Framework.Scripting
{
	public class CvarRegistry : Registry<ICvar>
	{
		/// <summary>
		///   Provides access to the actual instances of the cvars or commands managed by the registry.
		/// </summary>
		public new InstanceList Instances { get; private set; }

		/// <summary>
		///   The name of the player that identifies the player in networked games.
		/// </summary>
		public string PlayerName
		{
			get { return Instances.PlayerName.Value; }
			set { Instances.PlayerName.Value = value; }
		}

		/// <summary>
		///   The scaling factor that is applied to all timing values.
		/// </summary>
		public double TimeScale
		{
			get { return Instances.TimeScale.Value; }
			set { Instances.TimeScale.Value = value; }
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
			Register(Instances.TimeScale, "time_scale");
		}

		/// <summary>
		///   Stores the actual instances of the cvars or commands managed by the registry.
		/// </summary>
		public new class InstanceList : Registry<ICvar>.InstanceList
		{
			/// <summary>
			///   Initializes a new instance.
			/// </summary>
			public InstanceList()
			{
				PlayerName = new Cvar<string>("player_name", "UnnamedPlayer", "The name of the player that identifies the player in networked games.", true);
				TimeScale = new Cvar<double>("time_scale", 1.0, "The scaling factor that is applied to all timing values.", false);
			}

			/// <summary>
			///   The name of the player that identifies the player in networked games.
			/// </summary>
			public Cvar<string> PlayerName { get; private set; }

			/// <summary>
			///   The scaling factor that is applied to all timing values.
			/// </summary>
			public Cvar<double> TimeScale { get; private set; }
		}
	}
}

