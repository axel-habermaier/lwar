// Copyright (c) 2010-2013 SharpDX - Alexandre Mutel
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
// The following code is a port of DirectXTex http://directxtex.codeplex.com
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
	using System.Runtime.InteropServices;
	using SharpDX.Multimedia;

	/// <summary>
	///   Internal structure used to describe a DDS pixel format.
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct DDSPixelFormat
	{
		/// <summary>
		///   Initializes a new instance of the <see cref="DDSPixelFormat" /> struct.
		/// </summary>
		/// <param name="flags">The flags.</param>
		/// <param name="fourCC">The four CC.</param>
		/// <param name="rgbBitCount">The RGB bit count.</param>
		/// <param name="rBitMask">The r bit mask.</param>
		/// <param name="gBitMask">The g bit mask.</param>
		/// <param name="bBitMask">The b bit mask.</param>
		/// <param name="aBitMask">A bit mask.</param>
		public DDSPixelFormat(PixelFormatFlags flags, int fourCC, int rgbBitCount, uint rBitMask, uint gBitMask, uint bBitMask,
						   uint aBitMask)
		{
			Size = Marshal.SizeOf(typeof(DDSPixelFormat));
			Flags = flags;
			FourCC = fourCC;
			RGBBitCount = rgbBitCount;
			RBitMask = rBitMask;
			GBitMask = gBitMask;
			BBitMask = bBitMask;
			ABitMask = aBitMask;
		}

		public int Size;
		public PixelFormatFlags Flags;
		public int FourCC;
		public int RGBBitCount;
		public uint RBitMask;
		public uint GBitMask;
		public uint BBitMask;
		public uint ABitMask;

		public static readonly DDSPixelFormat DXT1 = new DDSPixelFormat(PixelFormatFlags.FourCC, new FourCC('D', 'X', 'T', '1'), 0, 0, 0,
																  0, 0);

		public static readonly DDSPixelFormat DXT2 = new DDSPixelFormat(PixelFormatFlags.FourCC, new FourCC('D', 'X', 'T', '2'), 0, 0, 0,
																  0, 0);

		public static readonly DDSPixelFormat DXT3 = new DDSPixelFormat(PixelFormatFlags.FourCC, new FourCC('D', 'X', 'T', '3'), 0, 0, 0,
																  0, 0);

		public static readonly DDSPixelFormat DXT4 = new DDSPixelFormat(PixelFormatFlags.FourCC, new FourCC('D', 'X', 'T', '4'), 0, 0, 0,
																  0, 0);

		public static readonly DDSPixelFormat DXT5 = new DDSPixelFormat(PixelFormatFlags.FourCC, new FourCC('D', 'X', 'T', '5'), 0, 0, 0,
																  0, 0);

		public static readonly DDSPixelFormat BC4_UNorm = new DDSPixelFormat(PixelFormatFlags.FourCC, new FourCC('B', 'C', '4', 'U'), 0,
																	   0, 0, 0, 0);

		public static readonly DDSPixelFormat BC4_SNorm = new DDSPixelFormat(PixelFormatFlags.FourCC, new FourCC('B', 'C', '4', 'S'), 0,
																	   0, 0, 0, 0);

		public static readonly DDSPixelFormat BC5_UNorm = new DDSPixelFormat(PixelFormatFlags.FourCC, new FourCC('B', 'C', '5', 'U'), 0,
																	   0, 0, 0, 0);

		public static readonly DDSPixelFormat BC5_SNorm = new DDSPixelFormat(PixelFormatFlags.FourCC, new FourCC('B', 'C', '5', 'S'), 0,
																	   0, 0, 0, 0);

		public static readonly DDSPixelFormat R8G8_B8G8 = new DDSPixelFormat(PixelFormatFlags.FourCC, new FourCC('R', 'G', 'B', 'G'), 0,
																	   0, 0, 0, 0);

		public static readonly DDSPixelFormat G8R8_G8B8 = new DDSPixelFormat(PixelFormatFlags.FourCC, new FourCC('G', 'R', 'G', 'B'), 0,
																	   0, 0, 0, 0);

		public static readonly DDSPixelFormat A8R8G8B8 = new DDSPixelFormat(PixelFormatFlags.Rgba, 0, 32, 0x00ff0000, 0x0000ff00,
																	  0x000000ff, 0xff000000);

		public static readonly DDSPixelFormat X8R8G8B8 = new DDSPixelFormat(PixelFormatFlags.Rgb, 0, 32, 0x00ff0000, 0x0000ff00,
																	  0x000000ff, 0x00000000);

		public static readonly DDSPixelFormat A8B8G8R8 = new DDSPixelFormat(PixelFormatFlags.Rgba, 0, 32, 0x000000ff, 0x0000ff00,
																	  0x00ff0000, 0xff000000);

		public static readonly DDSPixelFormat X8B8G8R8 = new DDSPixelFormat(PixelFormatFlags.Rgb, 0, 32, 0x000000ff, 0x0000ff00,
																	  0x00ff0000, 0x00000000);

		public static readonly DDSPixelFormat G16R16 = new DDSPixelFormat(PixelFormatFlags.Rgb, 0, 32, 0x0000ffff, 0xffff0000, 0x00000000,
																	0x00000000);

		public static readonly DDSPixelFormat R5G6B5 = new DDSPixelFormat(PixelFormatFlags.Rgb, 0, 16, 0x0000f800, 0x000007e0, 0x0000001f,
																	0x00000000);

		public static readonly DDSPixelFormat A1R5G5B5 = new DDSPixelFormat(PixelFormatFlags.Rgba, 0, 16, 0x00007c00, 0x000003e0,
																	  0x0000001f, 0x00008000);

		public static readonly DDSPixelFormat A4R4G4B4 = new DDSPixelFormat(PixelFormatFlags.Rgba, 0, 16, 0x00000f00, 0x000000f0,
																	  0x0000000f, 0x0000f000);

		public static readonly DDSPixelFormat R8G8B8 = new DDSPixelFormat(PixelFormatFlags.Rgb, 0, 24, 0x00ff0000, 0x0000ff00, 0x000000ff,
																	0x00000000);

		public static readonly DDSPixelFormat L8 = new DDSPixelFormat(PixelFormatFlags.Luminance, 0, 8, 0xff, 0x00, 0x00, 0x00);

		public static readonly DDSPixelFormat L16 = new DDSPixelFormat(PixelFormatFlags.Luminance, 0, 16, 0xffff, 0x0000, 0x0000, 0x0000);

		public static readonly DDSPixelFormat A8L8 = new DDSPixelFormat(PixelFormatFlags.LuminanceAlpha, 0, 16, 0x00ff, 0x0000, 0x0000,
																  0xff00);

		public static readonly DDSPixelFormat A8 = new DDSPixelFormat(PixelFormatFlags.Alpha, 0, 8, 0x00, 0x00, 0x00, 0xff);

		public static readonly DDSPixelFormat DX10 = new DDSPixelFormat(PixelFormatFlags.FourCC, new FourCC('D', 'X', '1', '0'), 0, 0, 0,
																  0, 0);
	}
}