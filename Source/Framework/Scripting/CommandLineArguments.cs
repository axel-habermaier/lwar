using System;

namespace Pegasus.Framework.Scripting
{
	public class CommandLineArguments
	{

		public string AssetsSourceDirectory { get; set; }
		public string AssetsTargetDirectory { get; set; }
		public string AssetsTempDirectory { get; set; }
		public string AssetsBinary { get; set; }

		public bool Help { get; set; }
		public bool Clean { get; set; }
		public bool Compile { get; set; }
	}
}