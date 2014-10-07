namespace Lwar.UserInterface.Views
{
	using System;
	using System.Collections.Generic;
	using System.Windows.Controls;

	public partial class ScoreboardView : UserControl
	{
		public ScoreboardView()
		{
			InitializeComponent();
		}
	}

	internal class ScoreboardViewModel
	{
		public ScoreboardViewModel()
		{
			Players = new List<Player>
			{
				new Player { DisplayName = "Player1", Kills = 12, Deaths = 3 },
				new Player { DisplayName = "Player2", Kills = 2, Deaths = 1 },
				new Player { DisplayName = "Player3", Kills = 0, Deaths = 0 },
			};
		}

		public List<Player> Players { get; private set; }

		public class Player
		{
			public string DisplayName { get; set; }
			public int Kills { get; set; }
			public int Deaths { get; set; }
		}
	}
}