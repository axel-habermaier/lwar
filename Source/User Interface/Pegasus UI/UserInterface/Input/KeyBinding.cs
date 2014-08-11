namespace Pegasus.Framework.UserInterface.Input
{
	using System;
	using System.Windows.Input;

	public class KeyBinding : InputBinding
	{
		public string Key { get; set; }
		public ModifierKeys Modifiers { get; set; }
		public string Method { get; set; }
		public KeyTriggerType Trigger { get; set; }
	}
}