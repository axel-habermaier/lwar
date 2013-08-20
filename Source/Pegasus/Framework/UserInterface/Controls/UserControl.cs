using System;

namespace Pegasus.Framework.UserInterface.Controls
{
	using Platform.Assets;

	/// <summary>
	///   Represents a base class for control typically defined in XAML.
	/// </summary>
	public class UserControl : ContentControl
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="assets">The assets manager that should be used to load the assets required by the control.</param>
		/// <param name="viewModel">The view model that should be bound to the control.</param>
		public UserControl(AssetsManager assets, ViewModel viewModel)
		{
			Assert.ArgumentNotNull(assets);
			Assert.ArgumentNotNull(viewModel);

			ViewModel = viewModel;
		}
	}
}