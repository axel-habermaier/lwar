namespace Pegasus.Platform.Graphics
{
	using System;
	using System.Runtime.InteropServices;

	/// <summary>
	///     Describes the properties of a rasterizer state.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct RasterizerDescription
	{
		/// <summary>
		///     Indicates whether lines are antialiased.
		/// </summary>
		public bool AntialiasedLineEnabled;

		/// <summary>
		///     Indicates whether depth clip is enabled.
		/// </summary>
		public bool DepthClipEnabled;

		/// <summary>
		///     Indicates whether counter-clockwise faces are front-facing.
		/// </summary>
		public bool FrontIsCounterClockwise;

		/// <summary>
		///     Indicates whether multisampling is enabled.
		/// </summary>
		public bool MultisampleEnabled;

		/// <summary>
		///     Indicates whether the scissor test is enabled.
		/// </summary>
		public bool ScissorEnabled;

		/// <summary>
		///     The cull mode.
		/// </summary>
		public CullMode CullMode;

		/// <summary>
		///     The fill mode.
		/// </summary>
		public FillMode FillMode;

		/// <summary>
		///     The depth bias.
		/// </summary>
		public int DepthBias;

		/// <summary>
		///     The depth bias clamp.
		/// </summary>
		public float DepthBiasClamp;

		/// <summary>
		///     The slope scaled depth bias.
		/// </summary>
		public float SlopeScaledDepthBias;

		/// <summary>
		///     Gets a description initialized to the default values.
		/// </summary>
		public static RasterizerDescription Default()
		{
			return new RasterizerDescription
			{
				FillMode = FillMode.Solid,
				CullMode = CullMode.Back,
				DepthBias = 0,
				DepthBiasClamp = 0,
				SlopeScaledDepthBias = 0,
				AntialiasedLineEnabled = false,
				DepthClipEnabled = true,
				FrontIsCounterClockwise = false,
				MultisampleEnabled = false,
				ScissorEnabled = false
			};
		}
	}
}