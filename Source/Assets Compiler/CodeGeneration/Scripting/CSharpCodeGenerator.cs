using System;

namespace Pegasus.AssetsCompiler.CodeGeneration.Scripting
{
	using Framework;

	/// <summary>
	///   Generates a C# registry class.
	/// </summary>
	internal class CSharpCodeGenerator
	{
		/// <summary>
		///   The registry the code is generated for.
		/// </summary>
		private readonly Registry _registry;

		/// <summary>
		///   The writer that is used to write the generated code.
		/// </summary>
		private readonly CodeWriter _writer;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="writer">The writer that should be used to write the generated code.</param>
		/// <param name="registry">The registry the code should be generated for.</param>
		public CSharpCodeGenerator(CodeWriter writer, Registry registry)
		{
			Assert.ArgumentNotNull(writer, () => writer);
			Assert.ArgumentNotNull(registry, () => registry);

			_writer = writer;
			_registry = registry;
		}

		/// <summary>
		///   Generates the registry class.
		/// </summary>
		public void GenerateCode()
		{
			_writer.WriterHeader("//");
			_writer.AppendLine("using System;");
			_writer.Newline();

			_writer.AppendLine("namespace {0}", _registry.Namespace);
			_writer.AppendBlockStatement(()=>{});
		}
	}
}