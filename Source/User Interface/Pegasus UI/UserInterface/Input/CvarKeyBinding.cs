using System;

namespace Pegasus.Framework.UserInterface.Input
{
	using System.Windows.Input;

	public class CvarKeyBinding : InputBinding
	{
		public string Cvar { get; set; }
		public ModifierKeys Modifiers { get; set; }
		public string Method { get; set; }
		public KeyTriggerType Trigger { get; set; }
	}
}