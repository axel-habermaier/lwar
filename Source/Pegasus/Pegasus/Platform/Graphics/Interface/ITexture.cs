namespace Pegasus.Platform.Graphics.Interface
{
	using System;

	/// <summary>
	///     Represents a texture stored in GPU memory.
	/// </summary>
	internal interface ITexture : IDisposable
	{
		/// <summary>
		///     Binds the texture to the given slot.
		/// </summary>
		/// <param name="slot">The slot the texture should be bound to.</param>
		void Bind(int slot);

		/// <summary>
		///     Unbinds the texture from the given slot.
		/// </summary>
		/// <param name="slot">The slot the texture should be unbound from.</param>
		void Unbind(int slot);

		/// <summary>
		///     Generates the mipmaps for this texture.
		/// </summary>
		void GenerateMipmaps();

		/// <summary>
		///     Sets the debug name of the texture.
		/// </summary>
		/// <param name="name">The debug name of the texture.</param>
		void SetName(string name);
	}
}