namespace Pegasus.Platform.Graphics
{
	using System;

	/// <summary>
	///     Describes the properties of a depth stencil state.
	/// </summary>
	public struct DepthStencilDescription
	{
		/// <summary>
		///     The stencil operation description for back faces.
		/// </summary>
		public StencilOperationDescription BackFace;

		/// <summary>
		///     Indicates whether the depth test is enabled.
		/// </summary>
		public bool DepthEnabled;

		/// <summary>
		///     The depth comparison function.
		/// </summary>
		public Comparison DepthFunction;

		/// <summary>
		///     Indicates whether depth writes are enabled.
		/// </summary>
		public bool DepthWriteEnabled;

		/// <summary>
		///     The stencil operation description for front faces.
		/// </summary>
		public StencilOperationDescription FrontFace;

		/// <summary>
		///     Indicates whether the stencil test is enabled.
		/// </summary>
		public bool StencilEnabled;

		/// <summary>
		///     The stencil read mask.
		/// </summary>
		public byte StencilReadMask;

		/// <summary>
		///     The stencil write mask.
		/// </summary>
		public byte StencilWriteMask;

		/// <summary>
		///     Gets a description initialized to the default values.
		/// </summary>
		public static DepthStencilDescription Default()
		{
			return new DepthStencilDescription
			{
				BackFace = StencilOperationDescription.Default(),
				FrontFace = StencilOperationDescription.Default(),
				DepthFunction = Comparison.Less,
				DepthEnabled = true,
				DepthWriteEnabled = true,
				StencilEnabled = false,
				StencilReadMask = 0xff,
				StencilWriteMask = 0xff
			};
		}
	}
}