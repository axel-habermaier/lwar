namespace Pegasus.Platform.Graphics
{
	using System;
	using System.Runtime.InteropServices;
	using Rendering;

	/// <summary>
	///     Describes the properties of a sampler state.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct SamplerDescription
	{
		/// <summary>
		///     The address mode along the U axis.
		/// </summary>
		public TextureAddressMode AddressU;

		/// <summary>
		///     The address mode along the V axis.
		/// </summary>
		public TextureAddressMode AddressV;

		/// <summary>
		///     The address mode along the W axis.
		/// </summary>
		public TextureAddressMode AddressW;

		/// <summary>
		///     The border color.
		/// </summary>
		public Color BorderColor;

		/// <summary>
		///     The comparison function.
		/// </summary>
		public Comparison Comparison;

		/// <summary>
		///     The texture filter.
		/// </summary>
		public TextureFilter Filter;

		/// <summary>
		///     The maximum anisotropy value.
		/// </summary>
		public int MaximumAnisotropy;

		/// <summary>
		///     The maximum LOD.
		/// </summary>
		public float MaximumLod;

		/// <summary>
		///     The minimum LOD.
		/// </summary>
		public float MinimumLod;

		/// <summary>
		///     The offset from the calculated mipmap level.
		/// </summary>
		public float MipLodBias;

		/// <summary>
		///     Gets a description initialized to the default values.
		/// </summary>
		public static SamplerDescription Default()
		{
			return new SamplerDescription
			{
				AddressU = TextureAddressMode.Clamp,
				AddressV = TextureAddressMode.Clamp,
				AddressW = TextureAddressMode.Clamp,
				BorderColor = new Color(),
				Comparison = Comparison.Never,
				Filter = TextureFilter.Bilinear,
				MaximumAnisotropy = 16,
				MaximumLod = Single.MaxValue,
				MinimumLod = Single.MinValue,
				MipLodBias = 0
			};
		}
	}
}