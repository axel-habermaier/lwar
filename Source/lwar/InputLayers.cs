using System;

namespace Lwar
{
	using Pegasus.Platform.Input;

	/// <summary>
	///   Provides access to the input layers used by the application.
	/// </summary>
	public static class InputLayers
	{
		/// <summary>
		///   The layer used by all input to the game.
		/// </summary>
		public static readonly InputLayer Game = new InputLayer(1);

		/// <summary>
		///   The layer used by all input to the debug camera.
		/// </summary>
		public static readonly InputLayer Debug = new InputLayer(2);

		/// <summary>
		///   The layer used by the chat input.
		/// </summary>
		public static readonly InputLayer Chat = new InputLayer(3);
	}
}