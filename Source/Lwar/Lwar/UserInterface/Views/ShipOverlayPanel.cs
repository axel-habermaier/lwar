namespace Lwar.UserInterface.Views
{
	using System;
	using Gameplay;
	using Gameplay.Entities;
	using Network;
	using Pegasus;
	using Pegasus.Framework.UserInterface;
	using Pegasus.Framework.UserInterface.Controls;
	using Pegasus.Math;
	using Pegasus.Platform.Graphics;
	using Rendering;

	internal class ShipOverlayPanel : Canvas
	{
		/// <summary>
		///     The list of players taking part in the game session.
		/// </summary>
		public static readonly DependencyProperty<PlayerList> PlayersProperty = new DependencyProperty<PlayerList>();

		/// <summary>
		///     The game camera that is used to draw the game session.
		/// </summary>
		public static readonly DependencyProperty<GameCamera> GameCameraProperty = new DependencyProperty<GameCamera>();

		/// <summary>
		///     The player infos storing the state of the individual ship overlays.
		/// </summary>
		private readonly PlayerInfo[] _playerInfos = new PlayerInfo[Specification.MaxPlayers];

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public ShipOverlayPanel()
		{
			CreateDataBinding(PlayersProperty, BindingMode.OneWay, "Players");
			CreateDataBinding(GameCameraProperty, BindingMode.OneWay, "CameraManager", "GameCamera");

			AddChangedHandler(PlayersProperty, OnPlayersChanged);

			for (var i = 0; i < _playerInfos.Length; ++i)
			{
				var overlay = new ShipOverlayView { Visibility = Visibility.Collapsed };

				_playerInfos[i].Overlay = overlay;
				Add(overlay);
			}
		}

		/// <summary>
		///     Gets the game camera that is used to draw the game session.
		/// </summary>
		private GameCamera Camera
		{
			get { return GetValue(GameCameraProperty); }
		}

		/// <summary>
		///     Ensures that the overlay is updated when players join or leave the game session.
		/// </summary>
		private void OnPlayersChanged(DependencyObject obj, DependencyPropertyChangedEventArgs<PlayerList> args)
		{
			Assert.IsNull(args.OldValue, "Cannot change player list.");
			Assert.NotNull(args.NewValue, "Player list required.");

			args.NewValue.PlayerAdded += AddPlayer;
			args.NewValue.PlayerRemoved += RemovePlayer;

			foreach (var player in args.NewValue)
				AddPlayer(player);
		}

		/// <summary>
		///     Adds the player to the overlay panel.
		/// </summary>
		/// <param name="player">The player that should be added.</param>
		private void AddPlayer(Player player)
		{
			for (var i = 0; i < _playerInfos.Length; ++i)
			{
				if (_playerInfos[i].Player != null)
					continue;

				_playerInfos[i].Player = player;
				_playerInfos[i].Overlay.Visibility = Visibility.Visible;

				// Ensure that the overlay is not visible before it is fully up-to-date
				SetLeft(_playerInfos[i].Overlay, -10000);
				SetTop(_playerInfos[i].Overlay, -10000);

				return;
			}

			Assert.NotReached("Out of player info slots.");
		}

		/// <summary>
		///     Removes the player from the overlay panel.
		/// </summary>
		/// <param name="player">The player that should be removed.</param>
		private void RemovePlayer(Player player)
		{
			for (var i = 0; i < _playerInfos.Length; ++i)
			{
				if (_playerInfos[i].Player != player)
					continue;

				_playerInfos[i].Player = null;
				_playerInfos[i].Overlay.Visibility = Visibility.Collapsed;
				return;
			}

			Assert.NotReached("Did not find player info slot.");
		}

		/// <summary>
		///     Draws the child UI elements of the current UI element using the given sprite batch.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the UI element's children.</param>
		protected override void DrawChildren(SpriteBatch spriteBatch)
		{
			if (!Camera.IsActive)
				return;

			// First, draw the children, then update the children's state. This delays state updates by one frame,
			// but avoids issues with incorrect layouting.
			base.DrawChildren(spriteBatch);

			for (var i = 0; i < _playerInfos.Length; ++i)
			{
				if (_playerInfos[i].Player == null)
					continue;

				if (_playerInfos[i].Player.Ship == null)
					_playerInfos[i].Overlay.Visibility = Visibility.Collapsed;
				else
				{
					_playerInfos[i].Overlay.Visibility = Visibility.Visible;
					Update(i);
				}
			}
		}

		/// <summary>
		///     Updates the player at the given index.
		/// </summary>
		/// <param name="index">The index of the player that should be updated.</param>
		private void Update(int index)
		{
			_playerInfos[index].Overlay.PlayerName = _playerInfos[index].Player.Name;
			_playerInfos[index].Overlay.Health = _playerInfos[index].Player.Ship.Health;

			var camera = Camera;
			var viewport = camera.Viewport;
			var ship = _playerInfos[index].Player.Ship;
			var radius = EntityTemplates.Ship.Radius;
			var overlayWidth = (float)_playerInfos[index].Overlay.ActualWidth;
			var overlayHeight = (float)_playerInfos[index].Overlay.ActualHeight;
			var visualArea = VisualArea;

			// Determine the screen-space size of the ship
			var topLeft = camera.ToScreenCoodinates(new Vector2(ship.Position.X + radius, ship.Position.Y - radius));
			var bottomRight = camera.ToScreenCoodinates(new Vector2(ship.Position.X - radius, ship.Position.Y + radius));

			// Map the positions to the actual screen positions, as the viewport size and the actual size of the render area
			// can be different
			topLeft = new Vector2(MathUtils.Scale(topLeft.X, viewport.Width, (float)visualArea.Width),
				MathUtils.Scale(topLeft.Y, viewport.Height, (float)visualArea.Height));
			bottomRight = new Vector2(MathUtils.Scale(bottomRight.X, viewport.Width, (float)visualArea.Width),
				MathUtils.Scale(bottomRight.Y, viewport.Height, (float)visualArea.Height));

			var shipWidth = bottomRight.X - topLeft.X;
			var shipHeight = bottomRight.Y - topLeft.Y;

			// Position the overlay centered below the ship
			var position = new Vector2d(topLeft.X + shipWidth / 2.0f - overlayWidth / 2.0f, visualArea.Height - bottomRight.Y + shipHeight);
			var area = new RectangleD(position.X, position.Y, overlayWidth, overlayHeight);

			// If the ship is outside the viewport, calculate the intersection of the line connecting the player to the current player
			// and the viewport and show the name there
			if (area.Left < visualArea.Left || area.Top < visualArea.Top ||
				area.Right > visualArea.Right || area.Bottom > visualArea.Bottom)
			{
				// The line segments defining the viewport; make the line segments slightly larger to account for rounding errors
				var left = new LineSegment(new Vector2d(0, -1), new Vector2d(0, visualArea.Height + 1));
				var top = new LineSegment(new Vector2d(-1, 0), new Vector2d(visualArea.Width + 1, 0));
				var right = new LineSegment(new Vector2d(visualArea.Width, -1), new Vector2d(visualArea.Width, visualArea.Height + 1));
				var bottom = new LineSegment(new Vector2d(-1, visualArea.Height), new Vector2d(visualArea.Width + 1, visualArea.Height));

				// The line segment from the center of the viewport to the ship's screen space position to 
				var line = new LineSegment(new Vector2d(visualArea.Width / 2.0f, visualArea.Height / 2.0f),
					new Vector2d(area.Left, area.Top) + new Vector2d(area.Width / 2.0f, area.Height / 2.0f));

				Vector2d intersection;
				if (line.Intersects(left, out intersection) || line.Intersects(top, out intersection) ||
					line.Intersects(right, out intersection) || line.Intersects(bottom, out intersection))
				{
					position = intersection;
				}

				// Clamp to the viewport
				position.X = MathUtils.Clamp(position.X, 0, visualArea.Width - overlayWidth);
				position.Y = MathUtils.Clamp(position.Y, 0, visualArea.Height - overlayHeight);
			}

			SetLeft(_playerInfos[index].Overlay, Math.Round(position.X));
			SetTop(_playerInfos[index].Overlay, Math.Round(position.Y));
		}

		/// <summary>
		///     Associates a player with its ship overlay.
		/// </summary>
		private struct PlayerInfo
		{
			/// <summary>
			///     The ship overlay of the player.
			/// </summary>
			public ShipOverlayView Overlay;

			/// <summary>
			///     The player the ship overlay is shown for.
			/// </summary>
			public Player Player;
		}
	}
}