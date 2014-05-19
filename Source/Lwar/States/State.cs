namespace Lwar.States
{
	using System;

	/// <summary>
	///     Represents a state of an application or another state. States can be arbitrarily nested.
	/// </summary>
	public abstract class State
	{
		/// <summary>
		///     Invoked when the state has been entered.
		/// </summary>
		public virtual void OnEntered()
		{
		}

		/// <summary>
		///     Invoked when the state has been left.
		/// </summary>
		public virtual void OnLeft()
		{
		}

		/// <summary>
		///     Invoked when the state should update itself.
		/// </summary>
		public virtual void Update()
		{
		}
	}
}