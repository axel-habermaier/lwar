﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Generated by the Pegasus Asset Compiler.
//     Sunday, April 7, 2013, 11:34:06
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;

namespace Pegasus.Framework.Scripting
{
	using Pegasus.Framework;

	public class CvarRegistry : Registry<ICvar>
	{
		/// <summary>
		///   Provides access to the actual instances of the cvars or commands managed by the registry.
		/// </summary>
		public new InstanceList Instances { get; private set; }

		/// <summary>
		///   The scaling factor that is applied to all timing values.
		/// </summary>
		public double TimeScale
		{
			get { return Instances.TimeScale.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value, () => value);
				Instances.TimeScale.Value = value;
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
				TimeScale = new Cvar<double>("time_scale", 1.0, "The scaling factor that is applied to all timing values.", false, new RangeAttribute(0.1, 10.0));
			}

			/// <summary>
			///   The scaling factor that is applied to all timing values.
			/// </summary>
			public Cvar<double> TimeScale { get; private set; }
		}
	}
}

