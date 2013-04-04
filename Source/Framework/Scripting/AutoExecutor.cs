using System;

namespace Pegasus.Framework.Platform
{
	using System.IO;
	using System.Linq;
	using System.Text;
	using Scripting;

	/// <summary>
	///   Automatically executes the contents of a file on startup and persists changes to persistent cvars in that file, so
	///   that the values of those cvars are restored the next time the application is started.
	/// </summary>
	internal class AutoExecutor : DisposableObject
	{
		/// <summary>
		///   The cvar registry that is used to lookup all cvars.
		/// </summary>
		private readonly CvarRegistry _cvars;

		/// <summary>
		///   The path of the auto exec file.
		/// </summary>
		private readonly string _filePath;

		/// <summary>
		///   The command registry that is used to handle all commands.
		/// </summary>
		private CommandRegistry _commands;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="commands">The command registry that should be used to handle all commands.</param>
		/// <param name="cvars">The cvar registry that should be used to lookup all cvars.</param>
		/// <param name="appName">The name of the application.</param>
		public AutoExecutor(CommandRegistry commands, CvarRegistry cvars, string appName)
		{
			Assert.ArgumentNotNull(commands, () => commands);
			Assert.ArgumentNotNull(cvars, () => cvars);
			Assert.ArgumentNotNullOrWhitespace(appName, () => appName);

			_filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), appName, "autoexec.cfg");
			_commands = commands;
			_cvars = cvars;
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			var builder = new StringBuilder();
			foreach (var cvar in _cvars.Instances.Where(cvar => cvar.Persistent))
				builder.AppendFormat("{0} {1}", cvar.Name, TypeRepresentation.ToString(cvar.Value)).AppendLine();

			File.WriteAllText(_filePath, builder.ToString());
		}
	}
}