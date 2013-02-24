using System;

namespace Pegasus.Framework.Scripting
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class CommandLineArgumentAttribute : Attribute
	{
		public string Argument { get; private set; }
		public string Description { get; private set; }
		public bool Required { get; private set; }
		public object Default { get; private set; }
	}
}