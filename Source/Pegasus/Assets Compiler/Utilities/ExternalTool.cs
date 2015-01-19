namespace Pegasus.AssetsCompiler.Utilities
{
	using System;
	using System.IO;
	using System.Linq;
	using Commands;
	using Textures;

	/// <summary>
	///     Runs an external asset compilation tool.
	/// </summary>
	internal static class ExternalTool
	{
		/// <summary>
		///     Runs the nvcompress tool with the given arguments.
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

			using (var nvcompress = new ExternalProcess(GetExecutable("nvcompress"),
				@"-dds10 -silent {3} -{0} ""{1}"" ""{2}""",
				compressionFormat, input, output, mipmaps ? String.Empty : "-nomips"))
			{
				var logEntries = nvcompress.Run();

				foreach (var log in logEntries)
					log.RaiseLogEvent();
			}
		}

		/// <summary>
		///     Runs the nvassemble tool with the given arguments.
		/// </summary>
		/// <param name="paths">The paths of the cube map faces.</param>
		/// <param name="output">The path of the output file that should be generated.</param>
		public static void NvAssemble(string[] paths, string output)
		{
			using (var nvassemble = new ExternalProcess(GetExecutable("nvassemble"),
				@"-cube ""{0}"" ""{1}"" ""{2}"" ""{3}"" ""{4}"" ""{5}"" -o ""{6}""",
				paths[0], paths[1], paths[2], paths[3], paths[4], paths[5], output))
			{
				var logEntries = nvassemble.Run();

				foreach (var log in logEntries)
					log.RaiseLogEvent();
			}
		}

		/// <summary>
		///     Runs the DirectX offline shader compiler.
		/// </summary>
		/// <param name="input">The shader file that should be compiled.</param>
		/// <param name="output">The output file that should store the compiled shader.</param>
		/// <param name="profile">The profile that should be used to compile the shader.</param>
		public static void Fxc(string input, string output, string profile)
		{
			Fxc(input, output, profile, Configuration.Debug);
		}

		/// <summary>
		///     Runs the DirectX offline shader compiler.
		/// </summary>
		/// <param name="input">The shader file that should be compiled.</param>
		/// <param name="output">The output file that should store the compiled shader.</param>
		/// <param name="profile">The profile that should be used to compile the shader.</param>
		/// <param name="generateDebugInfo">Indicates whether debugging information should be generated for the shader.</param>
		public static void Fxc(string input, string output, string profile, bool generateDebugInfo)
		{
			var optimization = "/O3";
			if (generateDebugInfo)
				optimization = "/Od /Zi";

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
		///     Constructs the platform-specific executable path to a tool.
		/// </summary>
		/// <param name="path">The platform-independent path to the tool.</param>
		private static string GetExecutable(string path)
		{
			var basePath = Path.GetDirectoryName(typeof(ExternalTool).Assembly.Location);
			return Path.Combine(basePath, path + ((Environment.OSVersion.Platform == PlatformID.Unix) ? String.Empty : ".exe"));
		}
	}
}