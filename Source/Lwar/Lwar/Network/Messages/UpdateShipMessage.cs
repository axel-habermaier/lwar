namespace Lwar.Network.Messages
{
	using System;
	using System.Runtime.InteropServices;
	using Gameplay;

	/// <summary>
	///     Holds the payload of an UpdateShip message.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct UpdateShipMessage
	{
		/// <summary>
		///     The ship that is updated.
		/// </summary>
		public Identifier Ship;

		/// <summary>
		///     The new hull integrity of the ship in the range [0,100].
		/// </summary>
		public int HullIntegrity;

		/// <summary>
		///     The new shield energy level of the ship in the range [0,100].
		/// </summary>
		public int Shields;

		/// <summary>
		///     The new energy level of the ship's first weapon slot in the range [0,100].
		/// </summary>
		public int Energy1;

		/// <summary>
		///     The new energy level of the ship's second weapon slot in the range [0,100].
		/// </summary>
		public int Energy2;

		/// <summary>
		///     The new energy level of the ship's third weapon slot in the range [0,100].
		/// </summary>
		public int Energy3;

		/// <summary>
		///     The new energy level of the ship's fourth weapon slot in the range [0,100].
		/// </summary>
		public int Energy4;
	}
}