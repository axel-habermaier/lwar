using System;
#if Linux && x64
// FT_Long is 64 bits wide on x64 Linux
using Long = System.Int64;
#else
// FT_Long is 32 bits wide on x64 Windows and x86 Windows and Linux
using Long = System.Int32;

#endif

namespace Pegasus.AssetsCompiler.Fonts
{
	using System.Runtime.InteropServices;
	using Platform.Logging;

	/// <summary>
	///   Provides access to native freetype functions.
	/// </summary>
#if !DEBUG
	[SuppressUnmanagedCodeSecurity]
#endif
	internal static class FreeType
	{
		/// <summary>
		///   The name of the freetype dynamic link library.
		/// </summary>
		private const string LibraryName =
#if Linux
			"libfreetype.so.6";
#elif !Linux && x86
			"../../Dependencies/freetype250.x86.dll";
#elif !Linux && x64
			"../../Dependencies/freetype250.x64.dll";
#endif

		/// <summary>
		///   Invokes the given function, which should return a freetype error result. Throws an exception if an error occurred.
		/// </summary>
		/// <param name="function">The function that should be invoked.</param>
		public static void Invoke(Func<Error> function)
		{
			Assert.ArgumentNotNull(function);

			var result = function();
			if (result != 0)
				Log.Die("Freetype error: {0}.", result);
		}

		public const int LoadTargetMono = ((int)RenderMode.Aliased & 15) << 16;
		public const int LoadTargetNormal = ((int)RenderMode.Antialiased & 15) << 16;
		public const int Kerning = 0x0040;

		[DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Init_FreeType")]
		public static extern Error Initialize(out IntPtr library);

		[DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Done_Library")]
		public static extern Error DisposeLibrary(IntPtr library);

		[DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_New_Face")]
		public static extern Error NewFace(IntPtr library, string path, Long index, out IntPtr face);

		[DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Done_Face")]
		public static extern Error DisposeFace(IntPtr face);

		[DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Set_Pixel_Sizes")]
		public static extern Error SetPixelSize(IntPtr face, uint width, uint height);

		[DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_Char_Index")]
		public static extern uint GetGlyphIndex(IntPtr face, Long characterCode);

		[DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Load_Glyph")]
		public static extern Error LoadGlyph(IntPtr face, uint glyphIndex, int loadFlags);

		[DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Render_Glyph")]
		public static extern Error RenderGlyph(IntPtr glyphSlot, RenderMode renderMode);

		[DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "FT_Get_Kerning")]
		public static extern Error GetKerning(IntPtr face, uint leftGlyph, uint rightGlyph, uint kernMode, out Vector kerning);

		// Disable annoying "private field is never used" warnings on Mono
#pragma warning disable 0169

		[StructLayout(LayoutKind.Sequential)]
		public class Face
		{
			internal Long num_faces;
			internal Long face_index;

			internal Long face_flags;
			internal Long style_flags;

			internal Long num_glyphs;

			[MarshalAs(UnmanagedType.LPStr)]
			internal string family_name;

			[MarshalAs(UnmanagedType.LPStr)]
			internal string style_name;

			internal int num_fixed_sizes;
			internal IntPtr available_sizes;

			internal int num_charmaps;
			internal IntPtr charmaps;

			internal IntPtr generic_data;
			internal IntPtr generic_finalizer;

			internal Long bbox_xmin, bbox_ymin, bbox_xmax, bbox_ymax;

			internal ushort units_per_EM;
			internal short ascender;
			internal short descender;
			internal short height;

			internal short max_advance_width;
			internal short max_advance_height;

			internal short underline_position;
			internal short underline_thickness;

			internal IntPtr glyph;
			internal IntPtr size;
			internal IntPtr charmap;

			private IntPtr driver;
			private IntPtr memory;
			private IntPtr stream;

			private IntPtr sizes_list_head;
			private IntPtr sizes_list_tail;
			private IntPtr autohint_data;
			private IntPtr autohint_finalizer;
			private IntPtr extensions;

			private IntPtr @internal;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal class GlyphSlot
		{
			internal IntPtr library;
			internal IntPtr face;
			internal IntPtr next;
			internal uint reserved;
			internal IntPtr generic_data;
			internal IntPtr generic_finalizer;

			internal GlyphMetrics metrics;
			internal Long linearHoriAdvance;
			internal Long linearVertAdvance;
			internal Long advance_x;
			internal Long advance_y;

			internal int format;

			internal Bitmap bitmap;
			internal int bitmap_left;
			internal int bitmap_top;

			internal Outline outline;

			internal uint num_subglyphs;
			internal IntPtr subglyphs;

			internal IntPtr control_data;
			internal Long control_len;

			internal Long lsb_delta;
			internal Long rsb_delta;

			internal IntPtr other;

			private IntPtr @internal;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct GlyphMetrics
		{
			internal Long width;
			internal Long height;

			internal Long horiBearingX;
			internal Long horiBearingY;
			internal Long horiAdvance;

			internal Long vertBearingX;
			internal Long vertBearingY;
			internal Long vertAdvance;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct Outline
		{
			internal short n_contours;
			internal short n_points;

			internal IntPtr points;
			internal IntPtr tags;
			internal IntPtr contours;

			internal int flags;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct Bitmap
		{
			internal int rows;
			internal int width;
			internal int pitch;
			internal IntPtr buffer;
			internal short num_grays;
			internal byte pixel_mode;
			internal byte palette_mode;
			internal IntPtr palette;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal class Size
		{
			internal IntPtr face;
			internal IntPtr generic_data;
			internal IntPtr generic_finalizer;
			internal ushort x_ppem;
			internal ushort y_ppem;

			internal Long x_scale;
			internal Long y_scale;
			internal Long ascender;
			internal Long descender;
			internal Long height;
			internal Long max_advance;
			private IntPtr @internal;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct Vector
		{
			internal Long x;
			internal Long y;
		}

#pragma warning restore 0169
	}
}