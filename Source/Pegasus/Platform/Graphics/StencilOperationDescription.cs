namespace Pegasus.Platform.Graphics
{
	using System;
	using System.Runtime.InteropServices;

	/// <summary>
	///     Describes the properties of a stencil operation.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct StencilOperationDescription
	{
		/// <summary>
		///     Gets or sets the fail operation.
		/// </summary>
		public StencilOperation FailOperation { get; set; }

		/// <summary>
		///     Gets or sets the depth fail operation.
		/// </summary>
		public StencilOperation DepthFailOperation { get; set; }

		/// <summary>
		///     Gets or sets the pass operation.
		/// </summary>
		public StencilOperation PassOperation { get; set; }

		/// <summary>
		///     Gets or sets the stencil function.
		/// </summary>
		public Comparison StencilFunction { get; set; }
	}
}