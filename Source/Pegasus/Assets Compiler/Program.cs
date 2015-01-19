namespace Pegasus.AssetsCompiler
{
	using System;
	using System.Globalization;
	using CommandLine;
	using CommandLine.Text;
	using Commands;
	using Utilities;

	internal static class Program
	{
		/// <summary>
		///     Runs the application.
		/// </summary>
		/// <param name="args">The arguments the asset compiler has been invoked with.</param>
		private static int Main(string[] args)
		{
			CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
			CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

			//var index = 0;
			//foreach (var arg in args)
			//	Log.Info("{0}: {1}", index++, arg);

			ICommand command = null;
			var options = new Options();

			if (!Parser.Default.ParseArguments(args, options, (verb, parsedCommand) => command = (ICommand)parsedCommand))
				return -1;

			try
			{
				command.Execute();
				return 0;
			}
			catch (AggregateException e)
			{
				LogAggregateException(e);
				return -1;
			}
			catch (PegasusException)
			{
				Log.Error("Aborted after fatal compilation error.");
				return -1;
			}
			catch (Exception e)
			{
				Log.Error("A fatal compilation error occurred: {0}", e.Message);
				Log.Error("{0}", e.Message);
				Log.Error("{0}", e.StackTrace);

				if (e.InnerException != null)
				{
					Log.Error("Inner exception:");
					Log.Error("{0}", e.InnerException.Message);
					Log.Error("{0}", e.InnerException.StackTrace);
				}

				return -1;
			}
		}

		/// <summary>
		///     Logs the given aggregate exception.
		/// </summary>
		/// <param name="exception">The exception that should be logged.</param>
		private static void LogAggregateException(AggregateException exception)
		{
			foreach (var innerException in exception.InnerExceptions)
			{
				var innerAggregateException = innerException as AggregateException;
				if (innerAggregateException != null)
					LogAggregateException(innerAggregateException);
				else
				{
					Log.Error("{0}", String.IsNullOrWhiteSpace(innerException.Message) ? "Unknown error" : innerException.Message);
					Log.Error("{0}", innerException.StackTrace);
				}

				Log.Error("----");
			}

			Log.Error("{0}", exception.StackTrace);
		}

		/// <summary>
		///     Provides access to the command line arguments.
		/// </summary>
		[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
		private class Options
		{
			/// <summary>
			///     Initializes a new instance.
			/// </summary>
			public Options()
			{
				CompileBundle = new AssetBundleCommand { Actions = CompilationActions.Compile };
				RecompileBundle = new AssetBundleCommand { Actions = CompilationActions.Compile | CompilationActions.Clean };
				CleanBundle = new AssetBundleCommand { Actions = CompilationActions.Clean };
				CompileXaml = new XamlCommand { Actions = CompilationActions.Compile };
				RecompileXaml = new XamlCommand { Actions = CompilationActions.Compile | CompilationActions.Clean };
				CleanXaml = new XamlCommand { Actions = CompilationActions.Clean };
			}

			/// <summary>
			///     Gets the asset bundle compilation command.
			/// </summary>
			[VerbOption("compile-assets", HelpText = "Compiles an asset bundle.")]
			public AssetBundleCommand CompileBundle { get; set; }

			/// <summary>
			///     Gets the asset bundle re-compilation command.
			/// </summary>
			[VerbOption("recompile-assets", HelpText = "Cleans and compiles an asset bundle.")]
			public AssetBundleCommand RecompileBundle { get; set; }

			/// <summary>
			///     Gets the asset bundle clean command.
			/// </summary>
			[VerbOption("clean-assets", HelpText = "Cleans an asset bundle.")]
			public AssetBundleCommand CleanBundle { get; set; }

			/// <summary>
			///     Gets the asset bundle compilation command.
			/// </summary>
			[VerbOption("compile-xaml", HelpText = "Compiles a Xaml bundle.")]
			public XamlCommand CompileXaml { get; set; }

			/// <summary>
			///     Gets the asset bundle re-compilation command.
			/// </summary>
			[VerbOption("recompile-xaml", HelpText = "Cleans and compiles a Xaml bundle.")]
			public XamlCommand RecompileXaml { get; set; }

			/// <summary>
			///     Gets the asset bundle clean command.
			/// </summary>
			[VerbOption("clean-xaml", HelpText = "Cleans a Xaml bundle.")]
			public XamlCommand CleanXaml { get; set; }

			/// <summary>
			///     Creates a help message about the usage of the assets compiler.
			/// </summary>
			[HelpVerbOption]
			public string GetUsage(string verb)
			{
				return HelpText.AutoBuild(this, verb);
			}
		}
	}
}