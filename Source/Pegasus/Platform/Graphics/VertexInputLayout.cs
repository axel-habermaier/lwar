using System;

namespace Pegasus.Framework.Platform.Graphics
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Runtime.InteropServices;
	using System.Security;

	/// <summary>
	///   An input-layout holds a definition of how to feed vertex data that
	///   is laid out in memory into the input-assembler stage of the graphics pipeline.
	/// </summary>
	public sealed class VertexInputLayout : GraphicsObject
	{
		/// <summary>
		///   The index buffer used by the vertex layout or null if the layout does not use indices.
		/// </summary>
		private readonly IndexBuffer _indexBuffer;

		/// <summary>
		///   The input bindings used by the vertex layout.
		/// </summary>
		private readonly VertexInputBinding[] _vertexInputBindings;

		/// <summary>
		///   The native vertex input layout instance.
		/// </summary>
		private readonly IntPtr _vertexInputLayout;

		/// <summary>
		///   Initializes a new instance without an index buffer binding.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="vertexInputBindings">The bindings for the vertex inputs that should belong to the input layout.</param>
		public VertexInputLayout(GraphicsDevice graphicsDevice, params VertexInputBinding[] vertexInputBindings)
			: this(graphicsDevice, null, 0, vertexInputBindings)
		{
		}

		/// <summary>
		///   Initializes a new instance with an index buffer binding.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="indexBuffer">The input index buffer that should be used by the input layout.</param>
		/// <param name="vertexInputBindings">The bindings for the vertex inputs that should belong to the input layout.</param>
		public VertexInputLayout(GraphicsDevice graphicsDevice, IndexBuffer indexBuffer,
								 params VertexInputBinding[] vertexInputBindings)
			: this(graphicsDevice, indexBuffer, 0, vertexInputBindings)
		{
		}

		/// <summary>
		///   Initializes a new instance with an index buffer binding.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="indexBuffer">The input index buffer that should be used by the input layout.</param>
		/// <param name="indexOffset">The offset into the index buffer.</param>
		/// <param name="vertexInputBindings">The bindings for the vertex inputs that should belong to the input layout.</param>
		public VertexInputLayout(GraphicsDevice graphicsDevice, IndexBuffer indexBuffer, int indexOffset,
								 params VertexInputBinding[] vertexInputBindings)
			: base(graphicsDevice)
		{
			Assert.ArgumentNotNull(vertexInputBindings);
			Assert.ArgumentInRange(indexOffset, 0, Int32.MaxValue);
			Assert.That(!vertexInputBindings.GroupBy(e => e.Semantics).Select((s, _) => s.Count()).Any(c => c > 1),
						"The list of vertex input elements contains at least two elements with the same value set for the Semantics property.");

			_vertexInputBindings = vertexInputBindings;
			_indexBuffer = indexBuffer;

			if (indexBuffer == null)
				_vertexInputLayout = NativeMethods.CreateInputLayout(graphicsDevice.NativePtr, IntPtr.Zero, 0, IndexSize.SixteenBits,
																	 _vertexInputBindings, _vertexInputBindings.Length);
			else
				_vertexInputLayout = NativeMethods.CreateInputLayout(graphicsDevice.NativePtr, indexBuffer.NativePtr, indexOffset,
																	 indexBuffer.IndexSize, _vertexInputBindings, _vertexInputBindings.Length);
		}

		/// <summary>
		///   Gets the list of vertex input bindings for this input layout.
		/// </summary>
		public IEnumerable<VertexInputBinding> VertexInputBindings
		{
			get { return _vertexInputBindings; }
		}

		/// <summary>
		///   Gets the index buffer of this input layout.
		/// </summary>
		public IndexBuffer IndexBuffer
		{
			get { return _indexBuffer; }
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			NativeMethods.DestroyInputLayout(_vertexInputLayout);
		}

		/// <summary>
		///   Binds the input layout to the input-assembler stage of the pipeline.
		/// </summary>
		public void Bind()
		{
			Assert.NotDisposed(this);
			NativeMethods.BindInputLayout(_vertexInputLayout);
		}

		/// <summary>
		///   Provides access to the native vertex input layout functions.
		/// </summary>
#if !DEBUG
		[SuppressUnmanagedCodeSecurity]
#endif
		private static class NativeMethods
		{
			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgCreateInputLayout")]
			public static extern IntPtr CreateInputLayout(IntPtr device, IntPtr indexBuffer, int indexOffset, IndexSize indexSize,
														  VertexInputBinding[] vertexInputBindings, int bindingsCount);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgDestroyInputLayout")]
			public static extern void DestroyInputLayout(IntPtr vertexInputLayout);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgBindInputLayout")]
			public static extern void BindInputLayout(IntPtr vertexInputLayout);
		}
	}
}