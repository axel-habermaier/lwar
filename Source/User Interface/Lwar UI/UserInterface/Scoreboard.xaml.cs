namespace Lwar.UserInterface
{
	using System;
	using System.Collections.Generic;
	using System.Windows.Controls;

	public partial class Scoreboard : UserControl
	{
		public Scoreboard()
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
				new Player { Name = "Player1", Kills = 12, Deaths = 3 },
				new Player { Name = "Player2", Kills = 2, Deaths = 1 },
				new Player { Name = "Player3", Kills = 0, Deaths = 0 },
			};
		}

		public List<Player> Players { get; private set; }

		public class Player
		{
			public string Name { get; set; }
			public int Kills { get; set; }
			public int Deaths { get; set; }
		}
	}
}