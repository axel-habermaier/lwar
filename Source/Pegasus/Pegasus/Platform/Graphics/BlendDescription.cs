namespace Pegasus.Platform.Graphics
{
	using System;
	using System.Runtime.InteropServices;

	/// <summary>
	///     Describes the properties of a blend state.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct BlendDescription
	{
		/// <summary>
		///     Indicates whether blend is enabled.
		/// </summary>
		public bool BlendEnabled;

		/// <summary>
		///     The blend operation for the alpha channel.
		/// </summary>
		public BlendOperation BlendOperationAlpha;

		/// <summary>
		///     The blend operation.
		/// </summary>
		public BlendOperation BlendOperation;

		/// <summary>
		///     The blend type for the destination alpha.
		/// </summary>
		public BlendOption DestinationBlendAlpha;

		/// <summary>
		///     The blend type for the destination.
		/// </summary>
		public BlendOption DestinationBlend;

		/// <summary>
		///     The blend type for the source.
		/// </summary>
		public BlendOption SourceBlend;

		/// <summary>
		///     The blend type for the source alpha.
		/// </summary>
		public BlendOption SourceBlendAlpha;

		/// <summary>
		///     Determines which color channel writes are enabled.
		/// </summary>
		public ColorWriteChannels WriteMask;

		/// <summary>
		///     Gets a description initialized to the default values.
		/// </summary>
		public static BlendDescription Default()
		{
			return new BlendDescription
			{
				BlendOperation = BlendOperation.Add,
				BlendOperationAlpha = BlendOperation.Add,
				DestinationBlend = BlendOption.Zero,
				DestinationBlendAlpha = BlendOption.Zero,
				SourceBlend = BlendOption.One,
				SourceBlendAlpha = BlendOption.One,
				WriteMask = ColorWriteChannels.All,
				BlendEnabled = false
			};
		}
	}
}