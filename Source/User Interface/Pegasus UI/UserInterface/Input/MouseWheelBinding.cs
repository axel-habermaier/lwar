namespace Pegasus.Framework.UserInterface.Input
{
	using System;
	using System.Windows.Input;

	public class MouseWheelBinding : InputBinding
	{
		public ModifierKeys Modifiers { get; set; }
		public string Method { get; set; }
		public MouseWheelDirection Direction { get; set; }
		public bool Preview { get; set; }
	}
}