namespace Pegasus.Framework.UserInterface.Views
{
	using System;
	using System.Windows.Controls;

	public partial class MessageBoxView : UserControl
	{
		public MessageBoxView()
		{
			InitializeComponent();
		}
	}

	internal class MessageBoxViewModel
	{
		public string Header
		{
			get { return "Header"; }
		}

		public string Message
		{
			get { return "Something has happened and the message boxs informs the user about it."; }
		}
	}
}