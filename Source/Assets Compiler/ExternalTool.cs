using System;

namespace Pegasus.AssetsCompiler
{
	using System.Diagnostics;
	using Framework;
	using Framework.Platform;
	using Framework.Platform.Graphics;

	public static class ExternalTool
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
		///   Runs an external tool process.
		/// </summary>
		/// <param name="fileName">The file name of the external tool executable.</param>
		/// <param name="commandLine">The command line arguments that should be passed to the tool.</param>
		/// <param name="arguments">The arguments that should be copied into the command line.</param>
		private static void RunProcess(string fileName, string commandLine, params object[] arguments)
		{
			var process = new Process
			{
				EnableRaisingEvents = true,
				StartInfo = new ProcessStartInfo(fileName, String.Format(commandLine, arguments))
				{
					UseShellExecute = false,
					RedirectStandardError = true,
					RedirectStandardInput = true,
					RedirectStandardOutput = true
				}
			};

			process.OutputDataReceived += (o, e) =>
				{
					if (!String.IsNullOrWhiteSpace(e.Data))
						Log.Info(e.Data);
				};
			process.ErrorDataReceived += (o, e) =>
				{
					if (!String.IsNullOrWhiteSpace(e.Data))
						Log.Die(e.Data);
				};

			process.Start();

			process.BeginErrorReadLine();
			process.BeginOutputReadLine();
			process.WaitForExit();
		}

		/// <summary>
		///   Runs the nvcompress tool with the given arguments.
		/// </summary>
		/// <param name="input">The path of the input file that should be processed.</param>
		/// <param name="output">The path of the output file that should be generated.</param>
		/// <param name="format">The format that should be used to compress the texture.</param>
		public static void NvCompress(string input, string output, SurfaceFormat format)
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

			RunProcess(NvCompressPath, @"-dds10 -silent -{0} -premula ""{1}"" ""{2}""", compressionFormat, input, output);
		}

		/// <summary>
		///   Runs the nvassemble tool with the given arguments.
		/// </summary>
		/// <param name="negativeZ">The path of the negative Z input file that should be processed.</param>
		/// <param name="negativeX">The path of the negative X input file that should be processed.</param>
		/// <param name="positiveZ">The path of the positive Z input file that should be processed.</param>
		/// <param name="positiveX">The path of the positive X input file that should be processed.</param>
		/// <param name="negativeY">The path of the negative Y input file that should be processed.</param>
		/// <param name="positiveY">The path of the positive Y input file that should be processed.</param>
		/// <param name="output">The path of the output file that should be generated.</param>
		public static void NvAssemble(string negativeZ, string negativeX, string positiveZ, string positiveX, string negativeY,
									  string positiveY, string output)
		{
			RunProcess(NvAssemblePath, @"-cube ""{0}"" ""{1}"" ""{2}"" ""{3}"" ""{4}"" ""{5}"" -o ""{6}""",
					   negativeZ, negativeX, positiveZ, positiveX, negativeY, positiveY, output);
		}

		/// <summary>
		/// Runs the DirectX offline shader compiler.
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
			RunProcess("fxc", @"{3} /E Main /Ges /T {0} /Fo ""{1}"" ""{2}""", profile, output, input, optimization);
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