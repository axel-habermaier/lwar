using System;

namespace Lwar.Client.Screens
{
	using System.Linq;
	using Gameplay;
	using Network;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Rendering;
	using Pegasus.Framework.Rendering.UserInterface;

	/// <summary>
	///   Displays the score board.
	/// </summary>
	public class Scoreboard : Screen
	{
		/// <summary>
		///   The width of the scoreboard's background border.
		/// </summary>
		private const int BorderWidth = 5;

		/// <summary>
		///   The game session that is played.
		/// </summary>
		private readonly GameSession _gameSession;

		/// <summary>
		///   The rows that show the stats of all active players.
		/// </summary>
		private readonly Row[] _rows = new Row[Specification.MaxPlayers];

		/// <summary>
		///   The area covered by the scoreboard.
		/// </summary>
		private Rectangle _area;

		/// <summary>
		///   The header row that shows a label for each column.
		/// </summary>
		private Row _header;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="gameSession">The game session that is played.</param>
		public Scoreboard(GameSession gameSession)
		{
			Assert.ArgumentNotNull(gameSession);
			_gameSession = gameSession;
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			// Nothing to do here
		}

		/// <summary>
		///   Initializes the screen.
		/// </summary>
		public override void Initialize()
		{
			var font = Assets.LoadFont("Fonts/Liberation Mono 12");
			_header = Row.CreateHeader(font);

			for (var i = 0; i < _rows.Length; ++i)
				_rows[i] = new Row(font);
		}

		/// <summary>
		///   Updates the screen.
		/// </summary>
		/// <param name="topmost">Indicates whether the app screen is the topmost one.</param>
		public override void Update(bool topmost)
		{
			// Update the visible row contents
			var visibleRows = 0;
			foreach (var player in _gameSession.Players
											   .Where(player => player.Id.Identity != 0)
											   .OrderBy(player => player.Kills)
											   .ThenBy(player => player.Name))
				_rows[visibleRows++].UpdateContents(player);

			// Set all remaining rows to invisible
			for (var i = visibleRows; i < _rows.Length; ++i)
				_rows[i].Visible = false;

			// Compute the new area of the scoreboard
			var width = _header.Width;
			var height = _rows[0].Height + visibleRows * _rows[0].Height;
			var x = (Window.Width - _header.Width) / 2;
			var y = (Window.Height - height) / 2;
			_area = new Rectangle(x, y, width, height);

			// Update the header and the row layouts
			_header.UpdateLayout(new Vector2i(x, y));

			for (var i = 0; i < _rows.Length; ++i)
				_rows[i].UpdateLayout(new Vector2i(x, y + _header.Height + i * _rows[0].Height));
		}

		/// <summary>
		///   Draws the user interface elements of the app screen.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the user interface.</param>
		public override void DrawUserInterface(SpriteBatch spriteBatch)
		{
			// Draw a background
			spriteBatch.Draw(_area.Enlarge(BorderWidth), Texture2D.White, new Color(32, 32, 32, 16));

			// Draw a line that separates the header from the rows
			var line = new RectangleF(_area.Left, _area.Top + _header.Height - Row.RowSpan - 2, _area.Width, 1);
			spriteBatch.Draw(line, Texture2D.White, Color.White);

			// Draw the header and the rows
			_header.Draw(spriteBatch);

			foreach (var row in _rows)
				row.Draw(spriteBatch);
		}

		/// <summary>
		///   Represents a row of the scoreboard.
		/// </summary>
		private struct Row
		{
			/// <summary>
			///   Determines the spacing between two columns.
			/// </summary>
			private const int ColumnSpan = 30;

			/// <summary>
			///   Determines the spacing between two rows.
			/// </summary>
			public const int RowSpan = 5;

			/// <summary>
			///   The label that shows how often the player has died.
			/// </summary>
			private readonly Label _deaths;

			/// <summary>
			///   The label that shows the number of kills the player has scored.
			/// </summary>
			private readonly Label _kills;

			/// <summary>
			///   The label that shows the name of the player.
			/// </summary>
			private readonly Label _name;

			/// <summary>
			///   The label that shows the player's ping.
			/// </summary>
			private readonly Label _ping;

			/// <summary>
			///   Initializes a new instance.
			/// </summary>
			/// <param name="font">The font that should be used to draw the row.</param>
			public Row(Font font)
				: this()
			{
				Assert.ArgumentNotNull(font);

				// Measure the width of player name and the 'death' header
				var name = Enumerable.Repeat("w", Specification.MaximumPlayerNameLength).Aggregate(String.Empty, (acc, v) => acc + v);
				var nameWidth = font.MeasureWidth(name);
				var deathWidth = font.MeasureWidth("Deaths");

				// Initialize the labels
				_name = new Label(font) { Area = new Rectangle(0, 0, nameWidth, 0) };
				_kills = new Label(font) { Area = new Rectangle(0, 0, deathWidth, 0), Alignment = TextAlignment.Right };
				_deaths = new Label(font) { Area = new Rectangle(0, 0, deathWidth, 0), Alignment = TextAlignment.Right };
				_ping = new Label(font) { Area = new Rectangle(0, 0, deathWidth, 0), Alignment = TextAlignment.Right };

				// Compute the width and the height of the row
				Width = nameWidth + 3 * deathWidth + 3 * ColumnSpan;
				Height = font.LineHeight + RowSpan;
			}

			/// <summary>
			///   Gets or sets a value indicating whether the row is visible.
			/// </summary>
			public bool Visible { get; set; }

			/// <summary>
			///   Gets the total width of the row.
			/// </summary>
			public int Width { get; private set; }

			/// <summary>
			///   Gets the total height of the row.
			/// </summary>
			public int Height { get; private set; }

			/// <summary>
			///   Creates the header row.
			/// </summary>
			/// <param name="font">The font that should be used to draw the header.</param>
			public static Row CreateHeader(Font font)
			{
				var row = new Row(font)
				{
					_name = { Text = "Player" },
					_kills = { Text = "Kills" },
					_deaths = { Text = "Deaths" },
					_ping = { Text = "Ping" },
					Visible = true
				};

				row.Height += RowSpan;
				return row;
			}

			/// <summary>
			///   Updates the layout of the row.
			/// </summary>
			/// <param name="offset">The offset that should be applied to the position of the row.</param>
			public void UpdateLayout(Vector2i offset)
			{
				if (!Visible)
					return;

				var offsetX = offset.X;
				ChangeArea(_name, ref offsetX, offset.Y);
				ChangeArea(_kills, ref offsetX, offset.Y);
				ChangeArea(_deaths, ref offsetX, offset.Y);
				ChangeArea(_ping, ref offsetX, offset.Y);
			}

			/// <summary>
			///   Updates the row's contents with the given player's stats.
			/// </summary>
			/// <param name="player">The player whose stats should be shown.</param>
			public void UpdateContents(Player player)
			{
				Assert.ArgumentNotNull(player);

				_name.Text = player.Name;
				_kills.Text = player.Kills.ToString();
				_deaths.Text = player.Deaths.ToString();
				_ping.Text = player.Ping.ToString();
				Visible = true;
			}

			/// <summary>
			///   Draws the row.
			/// </summary>
			/// <param name="spriteBatch">The sprite batch that should be used for drawing.</param>
			public void Draw(SpriteBatch spriteBatch)
			{
				if (!Visible)
					return;

				_name.Draw(spriteBatch);
				_kills.Draw(spriteBatch);
				_deaths.Draw(spriteBatch);
				_ping.Draw(spriteBatch);
			}

			/// <summary>
			///   Changes the area of the given label.
			/// </summary>
			/// <param name="label">The label whose area should be changed.</param>
			/// <param name="offsetX">
			///   The offset in X-direction that should be applied to the label. The offset is increased by the
			///   width of the label and the colum span.
			/// </param>
			/// <param name="offsetY">The offset in Y-direction that should be applied to the label.</param>
			private void ChangeArea(Label label, ref int offsetX, int offsetY)
			{
				label.Area = new Rectangle(offsetX, offsetY, label.Area.Size);
				offsetX += label.Area.Width + ColumnSpan;
			}
		}
	}
}