namespace Pegasus.UserInterface
{
	using System;
	using Controls;
	using Utilities;

	/// <summary>
	///     Represents the native device interface.
	/// </summary>
	[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
	internal unsafe interface IWindowInterface
	{
		void Open(string title, int x, int y, int width, int height, WindowMode mode);
		void GetSize(int* width, int* height);
		void GetPosition(int* x, int* y);
		WindowMode GetMode();
		void ChangeToFullscreenMode();
		void ChangeToWindowedMode();
		bool HasFocus();
		bool IsClosing();
	}
}