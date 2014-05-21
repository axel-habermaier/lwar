namespace Lwar.UserInterface
{
	using System;
	using Pegasus.Framework.UserInterface.Controls;

	/// <summary>
	///     A base class for Lwar specific view models that provides strongly-typed access to the view model's view.
	/// </summary>
	/// <typeparam name="TView">The type of the view associated with the view model.</typeparam>
	public abstract class LwarViewModel<TView> : LwarViewModel
		where TView : UserControl
	{
		/// <summary>
		///     Gets or sets the view associated with the view model.
		/// </summary>
		protected new TView View
		{
			get { return (TView)base.View; }
			set { base.View = value; }
		}
	}
}