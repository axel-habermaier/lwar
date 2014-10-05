namespace Lwar.UserInterface.Views
{
	using System;
	using Pegasus.Math;

	partial class ShipOverlayView
	{
		public float Health
		{
			get { return _health.Width; }
			set
			{
				var health = MathUtils.Clamp(value, 0, 100);
				_health.Width = health;
				_damage.Width = 100 - health;
			}
		}

		public string PlayerName
		{
			get { return _playerName.Text; }
			set { _playerName.Text = value; }
		}
	}
}