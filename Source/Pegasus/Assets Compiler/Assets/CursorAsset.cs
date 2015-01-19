namespace Pegasus.AssetsCompiler.Assets
{
	using System;
	using System.Xml.Linq;
	using Utilities;

	/// <summary>
	///     Represents a cursor.
	/// </summary>
	internal class CursorAsset : Asset
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="metadata">The metadata of the asset.</param>
		public CursorAsset(XElement metadata)
			: base(metadata, "File")
		{
			var hotSpot = GetStringMetadata("HotSpot");
			var parts = hotSpot.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

			int x, y;
			if (parts.Length != 2 || !Int32.TryParse(parts[0].Trim(), out x) || !Int32.TryParse(parts[1].Trim(), out y))
				Log.Die("Invalid value for attribute 'HotSpot': '{0}'.", hotSpot);
			else
			{
				HotSpotX = x;
				HotSpotY = y;
			}
		}

		/// <summary>
		///     Gets the hot spot of the cursor in X direction.
		/// </summary>
		public int HotSpotX { get; private set; }

		/// <summary>
		///     Gets the hot spot of the cursor in Y direction.
		/// </summary>
		public int HotSpotY { get; private set; }

		/// <summary>
		///     Gets the type of the asset.
		/// </summary>
		public override byte AssetType
		{
			get { return 7; }
		}

		/// <summary>
		///     Gets the runtime type of the asset.
		/// </summary>
		public override string RuntimeType
		{
			get { return "Pegasus.UserInterface.Input.Cursor"; }
		}
	}
}