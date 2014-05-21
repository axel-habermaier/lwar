namespace Lwar.UserInterface
{
	using System;
	using Pegasus.Platform.Network;

	/// <summary>
	///     Displays a game session.
	/// </summary>
	public class InGameViewModel : LwarViewModel<InGameView>
	{
		public InGameViewModel(IPEndPoint serverEndPoint)
		{
		}
	}
}