using System;

namespace Pegasus.Framework.UserInterface.Input
{
	using System.Windows.Input;

	public class ConfigurableBinding : InputBinding
	{
		public string Cvar { get; set; }
		public string Method { get; set; }
		public TriggerMode TriggerMode { get; set; }
	}
}