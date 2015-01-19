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
		///     The fail operation.
		/// </summary>
		public StencilOperation FailOperation;

		/// <summary>
		///     The depth fail operation.
		/// </summary>
		public StencilOperation DepthFailOperation;

		/// <summary>
		///     The pass operation.
		/// </summary>
		public StencilOperation PassOperation;

		/// <summary>
		///     The stencil function.
		/// </summary>
		public Comparison StencilFunction;

		/// <summary>
		///     Gets a description initialized to the default values.
		/// </summary>
		public static StencilOperationDescription Default()
		{
			return new StencilOperationDescription
			{
				FailOperation = StencilOperation.Keep,
				DepthFailOperation = StencilOperation.Keep,
				PassOperation = StencilOperation.Keep,
				StencilFunction = Comparison.Always
			};
		}
	}
}