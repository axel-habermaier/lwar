using System;

namespace Lwar.Client.Network.Messages
{
	/// <summary>
	/// Determines the type of an update record.
	/// </summary>
	public enum UpdateRecordType
	{
		Full,
		Position,
		Ray,
		Circle
	}
}