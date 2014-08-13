namespace Lwar.UserInterface.Views
{
	using System;
	using Assets;
	using Pegasus.Framework;
	using Pegasus.Framework.UserInterface.Input;
	using Pegasus.Math;
	using Pegasus.Platform.Graphics;

	partial class GameSessionView
	{
		/// <summary>
		///     Invoked once the UI element and all of its children have been fully loaded.
		/// </summary>
		partial void OnLoaded()
		{
			_scene3D.Cursor = new Cursor(Application.Current.Assets.Load(Textures.Crosshair), new Vector2i(16, 16), new Color(2, 205, 255, 255));
		}
	}
}