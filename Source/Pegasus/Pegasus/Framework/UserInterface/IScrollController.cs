namespace Pegasus.Framework.UserInterface
{
	using System;

	/// <summary>
	///     Provides methods that can be used to control a scroll handler.
	/// </summary>
	public interface IScrollController
	{
		/// <summary>
		///     Scrolls up by a step.
		/// </summary>
		void ScrollUp();

		/// <summary>
		///     Scrolls down by a step.
		/// </summary>
		void ScrollDown();

		/// <summary>
		///     Scrolls left by a step.
		/// </summary>
		void ScrollLeft();

		/// <summary>
		///     Scrolls right by a step.
		/// </summary>
		void ScrollRight();

		/// <summary>
		///     Scrolls to the top of the content area.
		/// </summary>
		void ScrollToTop();

		/// <summary>
		///     Scrolls to the bottom of the content area.
		/// </summary>
		void ScrollToBottom();

		/// <summary>
		///     Scrolls to the left of the content area.
		/// </summary>
		void ScrollToLeft();

		/// <summary>
		///     Scrolls to the right of the content area.
		/// </summary>
		void ScrollToRight();
	}
}