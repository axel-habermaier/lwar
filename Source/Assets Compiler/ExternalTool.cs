using System;

namespace Pegasus.AssetsCompiler
{
	using System.Linq;
	using Framework.Platform;
	using Framework.Platform.Graphics;
	using Framework.Platform.Logging;

	/// <summary>
	///   Runs an external asset compilation tool.
	/// </summary>
	internal static class ExternalTool
	{
		/// <summary>
		///   The path to the nvcompress executable.
		/// </summary>
		private static readonly string NvCompressPath = GetExecutable(PlatformInfo.Platform, "../../Tools/nvcompress");

		/// <summary>
		///   The path to the nvassemble executable.
		/// </summary>
		private static readonly string NvAssemblePath = GetExecutable(PlatformInfo.Platform, "../../Tools/nvassemble");

		/// <summary>
		///   Runs the nvcompress tool with the given arguments.
		/// </summary>
		/// <param name="input">The path of the input file that should be processed.</param>
		/// <param name="output">The path of the output file that should be generated.</param>
		/// <param name="format">The format that should be used to compress the texture.</param>
		/// <param name="mipmaps">Indicates whether mipmaps should be generated.</param>
		public static void NvCompress(string input, string output, SurfaceFormat format, bool mipmaps)
		{
			string compressionFormat;
			switch (format)
			{
				case SurfaceFormat.R8:
				case SurfaceFormat.Rgba8:
					compressionFormat = "rgb";
					break;
				case SurfaceFormat.Bc1:
					compressionFormat = "bc1";
					break;
				case SurfaceFormat.Bc2:
					compressionFormat = "bc2";
					break;
				case SurfaceFormat.Bc3:
					compressionFormat = "bc3";
					break;
				case SurfaceFormat.Bc4:
					compressionFormat = "bc4";
					break;
				case SurfaceFormat.Bc5:
					compressionFormat = "bc5";
					break;
				default:
					throw new InvalidOperationException("Unsupported format.");
			}

			using (var nvcompress = new ExternalProcess(NvCompressPath,
														@"-dds10 -silent {3} -{0} -premula ""{1}"" ""{2}""",
														compressionFormat, input, output, mipmaps ? String.Empty : "-nomips"))
			{
				var logEntries = nvcompress.Run();

				foreach (var log in logEntries)
					log.RaiseLogEvent();
			}
		}

		/// <summary>
		///   Runs the nvassemble tool with the given arguments.
		/// </summary>
		/// <param name="paths">The paths of the cube map faces.</param>
		/// <param name="output">The path of the output file that should be generated.</param>
		public static void NvAssemble(string[] paths, string output)
		{
			using (var nvassemble = new ExternalProcess(NvAssemblePath,
														@"-cube ""{0}"" ""{1}"" ""{2}"" ""{3}"" ""{4}"" ""{5}"" -o ""{6}""",
														paths[0], paths[1], paths[2], paths[3], paths[4], paths[5], output))
			{
				var logEntries = nvassemble.Run();

				foreach (var log in logEntries)
					log.RaiseLogEvent();
			}
		}

		/// <summary>
		///   Runs the DirectX offline shader compiler. Returns false to indicate that compiler errors have occurred.
		/// </summary>
		/// <param name="input">The shader file that should be compiled.</param>
		/// <param name="output">The output file that should store the compiled shader.</param>
		/// <param name="profile">The profile that should be used to compile the shader.</param>
		public static void Fxc(string input, string output, string profile)
		{
			string optimization;
#if DEBUG
			optimization = "/Od /Zi";
#else
			optimization = "/O3";
#endif

			using (var fxc = new ExternalProcess("fxc",
												 @"/nologo {3} /E Main /Ges /T {0} /Fo ""{1}"" ""{2}""",
												 profile, output, input, optimization))
			{
				var logEntries = fxc.Run();

				foreach (var log in logEntries.Where(l => l.LogType != LogType.Info))
				{
					if (log.Message.Contains(": warning X"))
						Log.Warn("{0}", log.Message);
					else
						Log.Error("{0}", log.Message);
				}

				if (logEntries.Any(l => l.Message.Contains("error")))
					throw new InvalidOperationException("HLSL shader code contains errors. No shader file has been generated.");
			}
		}

		/// <summary>
		///   Constructs the platform-specific executable path to a tool.
		/// </summary>
		/// <param name="platform">The platform for which the tool should be executed.</param>
		/// <param name="path">The platform-independent path to the tool.</param>
		private static string GetExecutable(PlatformType platform, string path)
		{
			return path + (platform == PlatformType.Windows ? ".exe" : String.Empty);
		}
	}
}