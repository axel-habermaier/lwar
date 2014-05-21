namespace Pegasus.Framework.UserInterface.Controls
{
	using System;
	using System.Windows.Controls;
	using System.Windows.Media;

	public abstract class RenderOutputPanel : Border
	{
		protected RenderOutputPanel()
		{
			Background = new SolidColorBrush(Colors.Black);
		}
	}

	// TODO: REMOVE
	public class ConsoleOutputPanel : RenderOutputPanel
	{
	}
}