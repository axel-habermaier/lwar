namespace Pegasus.UserInterface.Controls
{
	using System;

	/// <summary>
	///     Represents a selectable item in a list box.
	/// </summary>
	public class ListBoxItem : ContentControl
	{
		/// <summary>
		///     Indicates whether the item is the selected item of the list box.
		/// </summary>
		public static readonly DependencyProperty<bool> IsSelectedProperty =
			new DependencyProperty<bool>(prohibitsAnimations: true, isReadOnly: true);

		/// <summary>
		///     Gets a value indicating whether the item is the selected item of the list box.
		/// </summary>
		public bool IsSelected
		{
			get { return GetValue(IsSelectedProperty); }
		}
	}
}