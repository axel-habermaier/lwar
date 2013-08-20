//using System;

//namespace Lwar.Screens
//{
//	using System.Linq;
//	using Assets;
//	using Gameplay;
//	using Network;
//	using Pegasus;
//	using Pegasus.Math;
//	using Pegasus.Platform.Assets;
//	using Pegasus.Platform.Graphics;
//	using Pegasus.Platform.Input;
//	using Pegasus.Platform.Memory;
//	using Pegasus.Rendering;
//	using Pegasus.Rendering.UserInterface;
//	using Scripting;

//	/// <summary>
//	///   Displays the score board.
//	/// </summary>
//	public class Scoreboard : DisposableObject
//	{
//		/// <summary>
//		///   The frame around the scoreboard.
//		/// </summary>
//		private readonly Frame _frame = new Frame();

//		/// <summary>
//		///   The game session that is played.
//		/// </summary>
//		private readonly GameSession _gameSession;

//		/// <summary>
//		///   The input device that is used to check for user input.
//		/// </summary>
//		private readonly LogicalInputDevice _inputDevice;

//		/// <summary>
//		///   The rows that show the stats of all active players.
//		/// </summary>
//		private readonly Row[] _rows = new Row[Specification.MaxPlayers];

//		/// <summary>
//		///   The input that causes the scoreboard to be shown.
//		/// </summary>
//		private readonly LogicalInput _showScoreboard = new LogicalInput(Cvars.InputShowScoreboardCvar, InputLayers.Game);

//		/// <summary>
//		///   The header row that shows a label for each column.
//		/// </summary>
//		private Row _header;

//		/// <summary>
//		///   Initializes a new instance.
//		/// </summary>
//		/// <param name="inputDevice">The input device that should be used to check for user input.</param>
//		/// <param name="assets">The assets manager that should be used to load required assets.</param>
//		/// <param name="gameSession">The game session that is played.</param>
//		public Scoreboard(LogicalInputDevice inputDevice, AssetsManager assets, GameSession gameSession)
//		{
//			Assert.ArgumentNotNull(inputDevice);
//			Assert.ArgumentNotNull(assets);
//			Assert.ArgumentNotNull(gameSession);

//			_inputDevice = inputDevice;
//			_gameSession = gameSession;

//			var font = assets.LoadFont(Fonts.LiberationMono11);
//			_header = Row.CreateHeader(font);

//			for (var i = 0; i < _rows.Length; ++i)
//				_rows[i] = new Row(font);

//			_inputDevice.Add(_showScoreboard);
//		}

//		/// <summary>
//		///   Updates the scoreboard.
//		/// </summary>
//		/// <param name="size">The size of the window.</param>
//		public void Update(Size size)
//		{
//			if (!_showScoreboard.IsTriggered)
//				return;

//			// Update the visible row contents
//			var visibleRows = 0;
//			foreach (var player in _gameSession.Players
//											   .Where(player => player.Identifier != Specification.ServerPlayerIdentifier)
//											   .OrderByDescending(player => player.Kills)
//											   .ThenBy(player => player.Deaths)
//											   .ThenBy(player => player.Name))
//				_rows[visibleRows++].UpdateContents(player);

//			// Set all remaining rows to invisible
//			for (var i = visibleRows; i < _rows.Length; ++i)
//				_rows[i].Visible = false;

//			// Compute the new area of the scoreboard
//			var width = _header.Width;
//			var height = _rows[0].Height + visibleRows * _rows[0].Height;
//			var x = (size.Width - _header.Width) / 2;
//			var y = (size.Height - height) / 2;
//			_frame.ContentArea = new Rectangle(x, y, width, height);

//			// Update the header and the row layouts
//			_header.UpdateLayout(new Vector2i(x, y));

//			for (var i = 0; i < _rows.Length; ++i)
//				_rows[i].UpdateLayout(new Vector2i(x, y + _header.Height + i * _rows[0].Height));
//		}

//		/// <summary>
//		///   Draws the scoreboard.
//		/// </summary>
//		/// <param name="spriteBatch">The sprite batch that should be used for drawing.</param>
//		public void Draw(SpriteBatch spriteBatch)
//		{
//			if (!_showScoreboard.IsTriggered)
//				return;

//			// Draw the frame
//			_frame.Draw(spriteBatch);

//			// Draw a line that separates the header from the rows
//			var area = _frame.ContentArea;
//			var line = new RectangleF(area.Left, area.Top + _header.Height - Row.RowSpan - 2, area.Width, 1);
//			spriteBatch.Draw(line, Texture2D.White, Color.White);

//			// Draw the header and the rows
//			_header.Draw(spriteBatch);

//			foreach (var row in _rows)
//				row.Draw(spriteBatch);
//		}

//		/// <summary>
//		///   Disposes the object, releasing all managed and unmanaged resources.
//		/// </summary>
//		protected override void OnDisposing()
//		{
//			_inputDevice.Remove(_showScoreboard);

//			_header.Dispose();
//			foreach (var row in _rows)
//				row.Dispose();
//		}

//		/// <summary>
//		///   Represents a row of the scoreboard.
//		/// </summary>
//		private struct Row
//		{
//			/// <summary>
//			///   Determines the spacing between two columns.
//			/// </summary>
//			private const int ColumnSpan = 30;

//			/// <summary>
//			///   Determines the spacing between two rows.
//			/// </summary>
//			public const int RowSpan = 5;

//			/// <summary>
//			///   The label that shows how often the player has died.
//			/// </summary>
//			private readonly Label _deaths;

//			/// <summary>
//			///   The label that shows the number of kills the player has scored.
//			/// </summary>
//			private readonly Label _kills;

//			/// <summary>
//			///   The label that shows the name of the player.
//			/// </summary>
//			private readonly Label _name;

//			/// <summary>
//			///   The label that shows the player's ping.
//			/// </summary>
//			private readonly Label _ping;

//			/// <summary>
//			///   Initializes a new instance.
//			/// </summary>
//			/// <param name="font">The font that should be used to draw the row.</param>
//			public Row(Font font)
//				: this()
//			{
//				Assert.ArgumentNotNull(font);

//				// Measure the width of player name and the 'death' header
//				var name = Enumerable.Repeat("w", Specification.PlayerNameLength).Aggregate(String.Empty, (acc, v) => acc + v);
//				var nameWidth = font.MeasureWidth(name);
//				var deathWidth = font.MeasureWidth("Deaths");

//				// Initialize the labels
//				_name = new Label(font) { Area = new Rectangle(0, 0, nameWidth, 0) };
//				_kills = new Label(font) { Area = new Rectangle(0, 0, deathWidth, 0), Alignment = TextAlignment.Right };
//				_deaths = new Label(font) { Area = new Rectangle(0, 0, deathWidth, 0), Alignment = TextAlignment.Right };
//				_ping = new Label(font) { Area = new Rectangle(0, 0, deathWidth, 0), Alignment = TextAlignment.Right };

//				// Compute the width and the height of the row
//				Width = nameWidth + 3 * deathWidth + 3 * ColumnSpan;
//				Height = font.LineHeight + RowSpan;
//			}

//			/// <summary>
//			///   Gets or sets a value indicating whether the row is visible.
//			/// </summary>
//			public bool Visible { get; set; }

//			/// <summary>
//			///   Gets the total width of the row.
//			/// </summary>
//			public int Width { get; private set; }

//			/// <summary>
//			///   Gets the total height of the row.
//			/// </summary>
//			public int Height { get; private set; }

//			/// <summary>
//			///   Disposes the row, releasing all labels.
//			/// </summary>
//			public void Dispose()
//			{
//				_name.SafeDispose();
//				_kills.SafeDispose();
//				_deaths.SafeDispose();
//				_ping.SafeDispose();
//			}

//			/// <summary>
//			///   Creates the header row.
//			/// </summary>
//			/// <param name="font">The font that should be used to draw the header.</param>
//			public static Row CreateHeader(Font font)
//			{
//				var row = new Row(font)
//				{
//					_name = { Text = "Player" },
//					_kills = { Text = "Kills" },
//					_deaths = { Text = "Deaths" },
//					_ping = { Text = "Ping" },
//					Visible = true
//				};

//				row.Height += RowSpan;
//				return row;
//			}

//			/// <summary>
//			///   Updates the layout of the row.
//			/// </summary>
//			/// <param name="offset">The offset that should be applied to the position of the row.</param>
//			public void UpdateLayout(Vector2i offset)
//			{
//				if (!Visible)
//					return;

//				var offsetX = offset.X;
//				ChangeArea(_name, ref offsetX, offset.Y);
//				ChangeArea(_kills, ref offsetX, offset.Y);
//				ChangeArea(_deaths, ref offsetX, offset.Y);
//				ChangeArea(_ping, ref offsetX, offset.Y);
//			}

//			/// <summary>
//			///   Updates the row's contents with the given player's stats.
//			/// </summary>
//			/// <param name="player">The player whose stats should be shown.</param>
//			public void UpdateContents(Player player)
//			{
//				Assert.ArgumentNotNull(player);

//				_name.Text = player.Name;
//				_kills.Text = player.Kills.ToString();
//				_deaths.Text = player.Deaths.ToString();
//				_ping.Text = player.Ping.ToString();
//				Visible = true;
//			}

//			/// <summary>
//			///   Draws the row.
//			/// </summary>
//			/// <param name="spriteBatch">The sprite batch that should be used for drawing.</param>
//			public void Draw(SpriteBatch spriteBatch)
//			{
//				if (!Visible)
//					return;

//				_name.Draw(spriteBatch);
//				_kills.Draw(spriteBatch);
//				_deaths.Draw(spriteBatch);
//				_ping.Draw(spriteBatch);
//			}

//			/// <summary>
//			///   Changes the area of the given label.
//			/// </summary>
//			/// <param name="label">The label whose area should be changed.</param>
//			/// <param name="offsetX">
//			///   The offset in X-direction that should be applied to the label. The offset is increased by the
//			///   width of the label and the colum span.
//			/// </param>
//			/// <param name="offsetY">The offset in Y-direction that should be applied to the label.</param>
//			private static void ChangeArea(Label label, ref int offsetX, int offsetY)
//			{
//				label.Area = new Rectangle(offsetX, offsetY, label.Area.Size);
//				offsetX += label.Area.Width + ColumnSpan;
//			}
//		}
//	}
//}