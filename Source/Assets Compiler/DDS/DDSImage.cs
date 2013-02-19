﻿// Copyright (c) 2010-2013 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// -----------------------------------------------------------------------------
// Part of te following code is a port of http://directxtex.codeplex.com
// -----------------------------------------------------------------------------
// Microsoft Public License (Ms-PL)
//
// This license governs use of the accompanying software. If you use the 
// software, you accept this license. If you do not accept the license, do not
// use the software.
//
// 1. Definitions
// The terms "reproduce," "reproduction," "derivative works," and 
// "distribution" have the same meaning here as under U.S. copyright law.
// A "contribution" is the original software, or any additions or changes to 
// the software.
// A "contributor" is any person that distributes its contribution under this 
// license.
// "Licensed patents" are a contributor's patent claims that read directly on 
// its contribution.
//
// 2. Grant of Rights
// (A) Copyright Grant- Subject to the terms of this license, including the 
// license conditions and limitations in section 3, each contributor grants 
// you a non-exclusive, worldwide, royalty-free copyright license to reproduce
// its contribution, prepare derivative works of its contribution, and 
// distribute its contribution or any derivative works that you create.
// (B) Patent Grant- Subject to the terms of this license, including the license
// conditions and limitations in section 3, each contributor grants you a 
// non-exclusive, worldwide, royalty-free license under its licensed patents to
// make, have made, use, sell, offer for sale, import, and/or otherwise dispose
// of its contribution in the software or derivative works of the contribution 
// in the software.
//
// 3. Conditions and Limitations
// (A) No Trademark License- This license does not grant you rights to use any 
// contributors' name, logo, or trademarks.
// (B) If you bring a patent claim against any contributor over patents that 
// you claim are infringed by the software, your patent license from such 
// contributor to the software ends automatically.
// (C) If you distribute any portion of the software, you must retain all 
// copyright, patent, trademark, and attribution notices that are present in the
// software.
// (D) If you distribute any portion of the software in source code form, you 
// may do so only under this license by including a complete copy of this 
// license with your distribution. If you distribute any portion of the software
// in compiled or object code form, you may only do so under a license that 
// complies with this license.
// (E) The software is licensed "as-is." You bear the risk of using it. The
// contributors give no express warranties, guarantees or conditions. You may
// have additional consumer rights under your local laws which this license 
// cannot change. To the extent permitted under your local laws, the 
// contributors exclude the implied warranties of merchantability, fitness for a
// particular purpose and non-infringement.

using System;

namespace Pegasus.AssetsCompiler.DDS
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Runtime.InteropServices;
	using Framework;
	using Framework.Platform.Graphics;
	using SharpDX;
	using SharpDX.DXGI;

	/// <summary>
	/// Provides method to instantiate an image 1D/2D/3D supporting TextureArray and mipmaps on the CPU or to load/save an image from the disk.
	/// </summary>
	public sealed class DDSImage : DisposableObject
	{
		public delegate DDSImage ImageLoadDelegate(IntPtr dataPointer, int dataSize, bool makeACopy, GCHandle? handle);
		public delegate void ImageSaveDelegate(PixelBuffer[] pixelBuffers, int count, ImageDescription description, Stream imageStream);

		/// <summary>
		/// Pixel buffers.
		/// </summary>
		internal PixelBuffer[] pixelBuffers;
		private DataBox[] dataBoxArray;
		private List<int> mipMapToZIndex;
		private int zBufferCountPerArraySlice;
		private MipMapDescription[] mipmapDescriptions;
		private static List<LoadSaveDelegate> loadSaveDelegates = new List<LoadSaveDelegate>();

		/// <summary>
		/// Provides access to all pixel buffers.
		/// </summary>
		/// <remarks>
		/// For Texture3D, each z slice of the Texture3D has a pixelBufferArray * by the number of mipmaps.
		/// For other textures, there is Description.MipLevels * Description.ArraySize pixel buffers.
		/// </remarks>
		private PixelBufferArray pixelBufferArray;

		/// <summary>
		/// Gets the total number of bytes occupied by this image in memory.
		/// </summary>
		private int totalSizeInBytes;

		/// <summary>
		/// Pointer to the buffer.
		/// </summary>
		private IntPtr buffer;

		/// <summary>
		/// True if the buffer must be disposed.
		/// </summary>
		private bool bufferIsDisposable;

		/// <summary>
		/// Handke != null if the buffer is a pinned managed object on the LOH (Large Object Heap).
		/// </summary>
		private GCHandle? handle;

		/// <summary>
		/// Description of this image.
		/// </summary>
		public ImageDescription Description;

		private DDSImage()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DDSImage" /> class.
		/// </summary>
		/// <param name="description">The image description.</param>
		/// <param name="dataPointer">The pointer to the data buffer.</param>
		/// <param name="offset">The offset from the beginning of the data buffer.</param>
		/// <param name="handle">The handle (optionnal).</param>
		/// <param name="bufferIsDisposable">if set to <c>true</c> [buffer is disposable].</param>
		/// <exception cref="System.InvalidOperationException">If the format is invalid, or width/height/depth/arraysize is invalid with respect to the dimension.</exception>
		internal unsafe DDSImage(ImageDescription description, IntPtr dataPointer, int offset, GCHandle? handle, bool bufferIsDisposable, PitchFlags pitchFlags = PitchFlags.None)
		{
			Initialize(description, dataPointer, offset, handle, bufferIsDisposable, pitchFlags);
		}

		protected override void OnDisposing()
		{
			if (handle.HasValue)
			{
				handle.Value.Free();
			}

			Assert.That(!bufferIsDisposable, "Should free memory.");
			if (bufferIsDisposable)
			{
				//Utilities.FreeMemory(buffer);
			}
		}

		/// <summary>
		/// Gets the mipmap description of this instance for the specified mipmap level.
		/// </summary>
		/// <param name="mipmap">The mipmap.</param>
		/// <returns>A description of a particular mipmap for this texture.</returns>
		public MipMapDescription GetMipMapDescription(int mipmap)
		{
			return mipmapDescriptions[mipmap];
		}

		/// <summary>
		/// Gets the pixel buffer for the specified array/z slice and mipmap level.
		/// </summary>
		/// <param name="arrayOrZSliceIndex">For 3D image, the parameter is the Z slice, otherwise it is an index into the texture array.</param>
		/// <param name="mipmap">The mipmap.</param>
		/// <returns>A <see cref="pixelBufferArray"/>.</returns>
		/// <exception cref="System.ArgumentException">If arrayOrZSliceIndex or mipmap are out of range.</exception>
		public PixelBuffer GetPixelBuffer(int arrayOrZSliceIndex, int mipmap)
		{
			// Check for parameters, as it is easy to mess up things...
			if (mipmap > Description.MipLevels)
				throw new ArgumentException("Invalid mipmap level", "mipmap");

			if (Description.Dimension == TextureType.Texture3D)
			{
				if (arrayOrZSliceIndex > Description.Depth)
					throw new ArgumentException("Invalid z slice index", "arrayOrZSliceIndex");

				// For 3D textures
				return GetPixelBufferUnsafe(0, arrayOrZSliceIndex, mipmap);
			}

			if (arrayOrZSliceIndex > Description.ArraySize)
			{
				throw new ArgumentException("Invalid array slice index", "arrayOrZSliceIndex");
			}

			// For 1D, 2D textures
			return GetPixelBufferUnsafe(arrayOrZSliceIndex, 0, mipmap);
		}

		/// <summary>
		/// Gets the pixel buffer for the specified array/z slice and mipmap level.
		/// </summary>
		/// <param name="arrayIndex">Index into the texture array. Must be set to 0 for 3D images.</param>
		/// <param name="zIndex">Z index for 3D image. Must be set to 0 for all 1D/2D images.</param>
		/// <param name="mipmap">The mipmap.</param>
		/// <returns>A <see cref="pixelBufferArray"/>.</returns>
		/// <exception cref="System.ArgumentException">If arrayIndex, zIndex or mipmap are out of range.</exception>
		public PixelBuffer GetPixelBuffer(int arrayIndex, int zIndex, int mipmap)
		{
			// Check for parameters, as it is easy to mess up things...
			if (mipmap > Description.MipLevels)
				throw new ArgumentException("Invalid mipmap level", "mipmap");

			if (arrayIndex > Description.ArraySize)
				throw new ArgumentException("Invalid array slice index", "arrayIndex");

			if (zIndex > Description.Depth)
				throw new ArgumentException("Invalid z slice index", "zIndex");

			return this.GetPixelBufferUnsafe(arrayIndex, zIndex, mipmap);
		}


		/// <summary>
		/// Registers a loader/saver for a specified image file type.
		/// </summary>
		/// <param name="type">The file type (use integer and explicit casting to <see cref="ImageFileType"/> to register other fileformat.</param>
		/// <param name="loader">The loader delegate (can be null).</param>
		/// <param name="saver">The saver delegate (can be null).</param>
		/// <exception cref="System.ArgumentException"></exception>
		public static void Register(ImageLoadDelegate loader, ImageSaveDelegate saver)
		{
			// If reference equals, then it is null
			if (ReferenceEquals(loader, saver))
				throw new ArgumentNullException("Can set both loader and saver to null", "loader/saver");

			var newDelegate = new LoadSaveDelegate(loader, saver);
			for (int i = 0; i < loadSaveDelegates.Count; i++)
			{
				var loadSaveDelegate = loadSaveDelegates[i];
					loadSaveDelegates[i] = newDelegate;
			}
			loadSaveDelegates.Add(newDelegate);
		}


		/// <summary>
		/// Gets a pointer to the image buffer in memory.
		/// </summary>
		/// <value>A pointer to the image buffer in memory.</value>
		public IntPtr DataPointer
		{
			get { return this.buffer; }
		}

		/// <summary>
		/// Provides access to all pixel buffers.
		/// </summary>
		/// <remarks>
		/// For Texture3D, each z slice of the Texture3D has a pixelBufferArray * by the number of mipmaps.
		/// For other textures, there is Description.MipLevels * Description.ArraySize pixel buffers.
		/// </remarks>
		public PixelBufferArray PixelBuffer
		{
			get { return pixelBufferArray; }
		}

		/// <summary>
		/// Gets the total number of bytes occupied by this image in memory.
		/// </summary>
		public int TotalSizeInBytes
		{
			get { return totalSizeInBytes; }
		}

		/// <summary>
		/// Gets the databox from this image.
		/// </summary>
		/// <returns>The databox of this image.</returns>
		public DataBox[] ToDataBox()
		{
			return (DataBox[])dataBoxArray.Clone();
		}

		/// <summary>
		/// Gets the databox from this image.
		/// </summary>
		/// <returns>The databox of this image.</returns>
		private DataBox[] ComputeDataBox()
		{
			dataBoxArray = new DataBox[Description.ArraySize * Description.MipLevels];
			int i = 0;
			for (int arrayIndex = 0; arrayIndex < Description.ArraySize; arrayIndex++)
			{
				for (int mipIndex = 0; mipIndex < Description.MipLevels; mipIndex++)
				{
					// Get the first z-slize (A DataBox for a Texture3D is pointing to the whole texture).
					var pixelBuffer = this.GetPixelBufferUnsafe(arrayIndex, 0, mipIndex);

					dataBoxArray[i].DataPointer = pixelBuffer.DataPointer;
					dataBoxArray[i].RowPitch = pixelBuffer.RowStride;
					dataBoxArray[i].SlicePitch = pixelBuffer.BufferStride;
					i++;
				}
			}
			return dataBoxArray;
		}

		/// <summary>
		/// Loads an image from an unmanaged memory pointer.
		/// </summary>
		/// <param name="dataPointer">Pointer to an unmanaged memory. If <see cref="makeACopy"/> is false, this buffer must be allocated with <see cref="Utilities.AllocateMemory"/>.</param>
		/// <param name="dataSize">Size of the unmanaged buffer.</param>
		/// <param name="makeACopy">True to copy the content of the buffer to a new allocated buffer, false otherwise.</param>
		/// <returns>An new image.</returns>
		/// <remarks>If <see cref="makeACopy"/> is set to false, the returned image is now the holder of the unmanaged pointer and will release it on Dispose. </remarks>
		public static DDSImage Load(IntPtr dataPointer, int dataSize, bool makeACopy = false)
		{
			return Load(dataPointer, dataSize, makeACopy, null);
		}

		/// <summary>
		/// Loads an image from a managed buffer.
		/// </summary>
		/// <param name="buffer">Reference to a managed buffer.</param>
		/// <returns>An new image.</returns>
		/// <remarks>This method support the following format: <c>dds, bmp, jpg, png, gif, tiff, wmp, tga</c>.</remarks>
		public unsafe static DDSImage Load(byte[] buffer)
		{
			if (buffer == null)
				throw new ArgumentNullException("buffer");

			int size = buffer.Length;

			// If buffer is allocated on Larget Object Heap, then we are going to pin it instead of making a copy.
			if (size > (85 * 1024))
			{
				var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
				return Load(handle.AddrOfPinnedObject(), size, false, handle);
			}

			fixed (void* pbuffer = buffer)
			{
				return Load((IntPtr)pbuffer, size, true);
			}
		}

		/// <summary>
		/// Loads an image from the specified pointer.
		/// </summary>
		/// <param name="dataPointer">The data pointer.</param>
		/// <param name="dataSize">Size of the data.</param>
		/// <param name="makeACopy">if set to <c>true</c> [make A copy].</param>
		/// <param name="handle">The handle.</param>
		/// <returns></returns>
		/// <exception cref="System.NotSupportedException"></exception>
		private static DDSImage Load(IntPtr dataPointer, int dataSize, bool makeACopy, GCHandle? handle)
		{
			foreach (var loadSaveDelegate in loadSaveDelegates)
			{
				if (loadSaveDelegate.Load != null)
				{
					var image = loadSaveDelegate.Load(dataPointer, dataSize, makeACopy, handle);
					if (image != null)
					{
						return image;
					}
				}
			}
			return null;
		}


		static DDSImage()
		{
			Register(DDSReader.LoadFromDDSMemory, null);
		}


		internal unsafe void Initialize(ImageDescription description, IntPtr dataPointer, int offset, GCHandle? handle, bool bufferIsDisposable, PitchFlags pitchFlags = PitchFlags.None)
		{
			if (!FormatHelper.IsValid(description.Format) || FormatHelper.IsVideo(description.Format))
				throw new InvalidOperationException("Unsupported DXGI Format");

			this.handle = handle;

			switch (description.Dimension)
			{
				case TextureType.Texture1D:
					if (description.Width <= 0 || description.Height != 1 || description.Depth != 1 || description.ArraySize == 0)
						throw new InvalidOperationException("Invalid Width/Height/Depth/ArraySize for Image 1D");

					break;

				case TextureType.Texture2D:
				case TextureType.CubeMap:
					if (description.Width <= 0 || description.Height <= 0 || description.Depth != 1 || description.ArraySize == 0)
						throw new InvalidOperationException("Invalid Width/Height/Depth/ArraySize for Image 2D");

					if (description.Dimension == TextureType.CubeMap)
					{
						if ((description.ArraySize % 6) != 0)
							throw new InvalidOperationException("TextureCube must have an arraysize = 6");
					}

					break;

				case TextureType.Texture3D:
					if (description.Width <= 0 || description.Height <= 0 || description.Depth <= 0 || description.ArraySize != 1)
						throw new InvalidOperationException("Invalid Width/Height/Depth/ArraySize for Image 3D");

					break;
			}

			// Calculate mipmaps
			int pixelBufferCount;
			this.mipMapToZIndex = CalculateImageArray(description, pitchFlags, out pixelBufferCount, out totalSizeInBytes);
			this.mipmapDescriptions = CalculateMipMapDescription(description, pitchFlags);
			zBufferCountPerArraySlice = this.mipMapToZIndex[this.mipMapToZIndex.Count - 1];

			// Allocate all pixel buffers
			pixelBuffers = new PixelBuffer[pixelBufferCount];
			pixelBufferArray = new PixelBufferArray(this);

			// Setup all pointers
			// only release buffer that is not pinned and is asked to be disposed.
			this.bufferIsDisposable = !handle.HasValue && bufferIsDisposable;
			this.buffer = dataPointer;

			if (dataPointer == IntPtr.Zero)
			{
				buffer = Utilities.AllocateMemory(totalSizeInBytes);
				offset = 0;
				this.bufferIsDisposable = true;
			}

			SetupImageArray((IntPtr)((byte*)buffer + offset), totalSizeInBytes, description, pitchFlags, pixelBuffers);

			Description = description;

			// PreCompute databoxes
			dataBoxArray = ComputeDataBox();
		}

		private PixelBuffer GetPixelBufferUnsafe(int arrayIndex, int zIndex, int mipmap)
		{
			var depthIndex = this.mipMapToZIndex[mipmap];
			var pixelBufferIndex = arrayIndex * this.zBufferCountPerArraySlice + depthIndex + zIndex;
			return pixelBuffers[pixelBufferIndex];
		}

		private static ImageDescription CreateDescription(TextureType dimension, int width, int height, int depth, MipMapCount mipMapCount, PixelFormat format, int arraySize)
		{
			return new ImageDescription()
			{
				Width = width,
				Height = height,
				Depth = depth,
				ArraySize = arraySize,
				Dimension = dimension,
				Format = format,
				MipLevels = mipMapCount,
			};
		}

		/// <summary>
		/// Offset from the beginning of the buffer where pixel buffers are stored.
		/// This offset is used to keep data aligned on 16 bytes (if the original buffer is aligned on 16 bytes as well).
		/// </summary>
		private const int OffsetBufferTKTX = 48;


		[Flags]
		internal enum PitchFlags
		{
			None = 0x0,      // Normal operation
			LegacyDword = 0x1,      // Assume pitch is DWORD aligned instead of BYTE aligned
			Bpp24 = 0x10000,  // Override with a legacy 24 bits-per-pixel format size
			Bpp16 = 0x20000,  // Override with a legacy 16 bits-per-pixel format size
			Bpp8 = 0x40000,  // Override with a legacy 8 bits-per-pixel format size
		};

		internal static void ComputePitch(Format fmt, int width, int height, out int rowPitch, out int slicePitch, out int widthCount, out int heightCount, PitchFlags flags = PitchFlags.None)
		{
			widthCount = width;
			heightCount = height;

			if (FormatHelper.IsCompressed(fmt))
			{
				int bpb = (fmt == Format.BC1_Typeless
							 || fmt == Format.BC1_UNorm
							 || fmt == Format.BC1_UNorm_SRgb
							 || fmt == Format.BC4_Typeless
							 || fmt == Format.BC4_UNorm
							 || fmt == Format.BC4_SNorm) ? 8 : 16;
				widthCount = Math.Max(1, (width + 3) / 4);
				heightCount = Math.Max(1, (height + 3) / 4);
				rowPitch = widthCount * bpb;

				slicePitch = rowPitch * heightCount;
			}
			else if (FormatHelper.IsPacked(fmt))
			{
				rowPitch = ((width + 1) >> 1) * 4;

				slicePitch = rowPitch * height;
			}
			else
			{
				int bpp;

				if ((flags & PitchFlags.Bpp24) != 0)
					bpp = 24;
				else if ((flags & PitchFlags.Bpp16) != 0)
					bpp = 16;
				else if ((flags & PitchFlags.Bpp8) != 0)
					bpp = 8;
				else
					bpp = FormatHelper.SizeOfInBits(fmt);

				if ((flags & PitchFlags.LegacyDword) != 0)
				{
					// Special computation for some incorrectly created DDS files based on
					// legacy DirectDraw assumptions about pitch alignment
					rowPitch = ((width * bpp + 31) / 32) * sizeof(int);
					slicePitch = rowPitch * height;
				}
				else
				{
					rowPitch = (width * bpp + 7) / 8;
					slicePitch = rowPitch * height;
				}
			}
		}

		internal static MipMapDescription[] CalculateMipMapDescription(ImageDescription metadata, PitchFlags cpFlags = PitchFlags.None)
		{
			int nImages;
			int pixelSize;
			return CalculateMipMapDescription(metadata, cpFlags, out nImages, out pixelSize);
		}

		internal static MipMapDescription[] CalculateMipMapDescription(ImageDescription metadata, PitchFlags cpFlags, out int nImages, out int pixelSize)
		{
			pixelSize = 0;
			nImages = 0;

			int w = metadata.Width;
			int h = metadata.Height;
			int d = metadata.Depth;

			var mipmaps = new MipMapDescription[metadata.MipLevels];

			for (int level = 0; level < metadata.MipLevels; ++level)
			{
				int rowPitch, slicePitch;
				int widthPacked;
				int heightPacked;
				ComputePitch(metadata.Format, w, h, out rowPitch, out slicePitch, out widthPacked, out heightPacked, PitchFlags.None);

				mipmaps[level] = new MipMapDescription(
					w,
					h,
					d,
					rowPitch,
					slicePitch,
					widthPacked,
					heightPacked
					);

				pixelSize += d * slicePitch;
				nImages += d;

				if (h > 1)
					h >>= 1;

				if (w > 1)
					w >>= 1;

				if (d > 1)
					d >>= 1;
			}
			return mipmaps;
		}

		/// <summary>
		/// Determines number of image array entries and pixel size.
		/// </summary>
		/// <param name="imageDesc">Description of the image to create.</param>
		/// <param name="pitchFlags">Pitch flags.</param>
		/// <param name="bufferCount">Output number of mipmap.</param>
		/// <param name="pixelSizeInBytes">Output total size to allocate pixel buffers for all images.</param>
		private static List<int> CalculateImageArray(ImageDescription imageDesc, PitchFlags pitchFlags, out int bufferCount, out int pixelSizeInBytes)
		{
			pixelSizeInBytes = 0;
			bufferCount = 0;

			var mipmapToZIndex = new List<int>();

			for (int j = 0; j < imageDesc.ArraySize; j++)
			{
				int w = imageDesc.Width;
				int h = imageDesc.Height;
				int d = imageDesc.Depth;

				for (int i = 0; i < imageDesc.MipLevels; i++)
				{
					int rowPitch, slicePitch;
					int widthPacked;
					int heightPacked;
					ComputePitch(imageDesc.Format, w, h, out rowPitch, out slicePitch, out widthPacked, out heightPacked, pitchFlags);

					// Store the number of z-slicec per miplevels
					if (j == 0)
						mipmapToZIndex.Add(bufferCount);

					// Keep a trace of indices for the 1st array size, for each mip levels
					pixelSizeInBytes += d * slicePitch;
					bufferCount += d;

					if (h > 1)
						h >>= 1;

					if (w > 1)
						w >>= 1;

					if (d > 1)
						d >>= 1;
				}

				// For the last mipmaps, store just the number of zbuffers in total
				if (j == 0)
					mipmapToZIndex.Add(bufferCount);
			}
			return mipmapToZIndex;
		}

		/// <summary>
		/// Allocates PixelBuffers 
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="pixelSize"></param>
		/// <param name="imageDesc"></param>
		/// <param name="pitchFlags"></param>
		/// <param name="output"></param>
		private static unsafe void SetupImageArray(IntPtr buffer, int pixelSize, ImageDescription imageDesc, PitchFlags pitchFlags, PixelBuffer[] output)
		{
			int index = 0;
			var pixels = (byte*)buffer;
			for (uint item = 0; item < imageDesc.ArraySize; ++item)
			{
				int w = imageDesc.Width;
				int h = imageDesc.Height;
				int d = imageDesc.Depth;

				for (uint level = 0; level < imageDesc.MipLevels; ++level)
				{
					int rowPitch, slicePitch;
					int widthPacked;
					int heightPacked;
					ComputePitch(imageDesc.Format, w, h, out rowPitch, out slicePitch, out widthPacked, out heightPacked, pitchFlags);

					for (uint zSlice = 0; zSlice < d; ++zSlice)
					{
						// We use the same memory organization that Direct3D 11 needs for D3D11_SUBRESOURCE_DATA
						// with all slices of a given miplevel being continuous in memory
						output[index] = new PixelBuffer(w, h, imageDesc.Format, rowPitch, slicePitch, (IntPtr)pixels);
						++index;

						pixels += slicePitch;
					}

					if (h > 1)
						h >>= 1;

					if (w > 1)
						w >>= 1;

					if (d > 1)
						d >>= 1;
				}
			}
		}

		private class LoadSaveDelegate
		{
			public LoadSaveDelegate(ImageLoadDelegate load, ImageSaveDelegate save)
			{
				Load = load;
				Save = save;
			}

			public ImageLoadDelegate Load;

			public ImageSaveDelegate Save;
		}
	}
}