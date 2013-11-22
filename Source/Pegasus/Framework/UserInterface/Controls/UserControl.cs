namespace Pegasus.Framework.UserInterface.Controls
{
	using System;

	/// <summary>
	///   Represents a base class for controls typically defined in Xaml.
	/// </summary>
	public class UserControl : ContentControl
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public UserControl()
		{
			Template = DefaultTemplate;
		}
	}
}