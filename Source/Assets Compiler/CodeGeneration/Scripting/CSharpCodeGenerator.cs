using System;

namespace Pegasus.AssetsCompiler.CodeGeneration.Scripting
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Text.RegularExpressions;
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
		/// <param name="baseClass">The class that the generated registry class should be derived from.</param>
		public void GenerateCode(string baseClass)
		{
			_writer.WriterHeader("//");
			_writer.AppendLine("using System;");
			_writer.Newline();

			_writer.AppendLine("namespace {0}", _registry.Namespace);
			_writer.AppendBlockStatement(() =>
				{
					foreach (var import in _registry.ImportedNamespaces)
						_writer.AppendLine("using {0};", import);

					if (_registry.ImportedNamespaces.Any())
						_writer.Newline();

					_writer.AppendLine("public class {0} : {1}", _registry.Name, baseClass);
					_writer.AppendBlockStatement(() =>
						{
							_writer.AppendLine("/// <summary>");
							_writer.AppendLine("///   Provides access to the actual instances of the cvars or commands managed by the registry.");
							_writer.AppendLine("/// </summary>");
							_writer.AppendLine("public new InstanceList Instances {{ get; private set; }}");
							_writer.Newline();

							GenerateCvarProperties();
							GenerateCommandMethods();
							GenerateCommandEvents();

							GenerateInitializeMethod();

							GenerateNestedClass(baseClass);
						});
				});
		}

		/// <summary>
		///   Generates the cvar properties
		/// </summary>
		private void GenerateCvarProperties()
		{
			foreach (var cvar in _registry.Cvars)
			{
				WriteDocumentation(cvar.Documentation);
				_writer.AppendLine("public {0} {1}", cvar.Type, cvar.Name);
				_writer.AppendBlockStatement(() =>
					{
						_writer.AppendLine("get {{ return Instances.{0}.Value; }}", cvar.Name);
						_writer.AppendLine("set {{ Instances.{0}.Value = value; }}", cvar.Name);
					});

				_writer.Newline();
			}
		}

		/// <summary>
		///   Generates the command methods.
		/// </summary>
		private void GenerateCommandMethods()
		{
			foreach (var command in _registry.Commands)
			{
				WriteDocumentation(command.Documentation);
				_writer.AppendLine("public void {0}({1})", command.Name, GetArgumentDeclarations(command));
				_writer.AppendBlockStatement(
					() => _writer.AppendLine("Instances.{0}.Invoke({1});", command.Name, GetInvocationArguments(command)));

				_writer.Newline();
			}
		}

		/// <summary>
		///   Generates the command events.
		/// </summary>
		private void GenerateCommandEvents()
		{
			foreach (var command in _registry.Commands)
			{
				_writer.AppendLine("/// <summary>");
				_writer.AppendLine("///   Raised when the {0} command is invoked.", command.Name);
				_writer.AppendLine("/// </summary>");
				_writer.AppendLine("public event Action{0} On{1}", GetTypeArguments(command), command.Name);
				_writer.AppendBlockStatement(() =>
					{
						_writer.AppendLine("add {{ Instances.{0}.Invoked += value; }}", command.Name);
						_writer.AppendLine("remove {{ Instances.{0}.Invoked -= value; }}", command.Name);
					});

				_writer.Newline();
			}
		}

		/// <summary>
		///   Generates the initialization method.
		/// </summary>
		private void GenerateInitializeMethod()
		{
			_writer.AppendLine("/// <summary>");
			_writer.AppendLine("///   Initializes the registry.");
			_writer.AppendLine("/// </summary>");
			_writer.AppendLine("protected override void Initialize(object instances)");
			_writer.AppendBlockStatement(() =>
				{
					_writer.AppendLine("if (instances == null)");
					_writer.AppendLine("	instances = new InstanceList();");
					_writer.Newline();

					_writer.AppendLine("Instances = (InstanceList)instances;");
					_writer.AppendLine("base.Initialize(instances);");
					_writer.Newline();

					foreach (var cvar in _registry.Cvars)
						_writer.AppendLine("Register(Instances.{0}, \"{1}\");", cvar.Name, GetRuntimeName(cvar.Name));

					if (_registry.Cvars.Any() && _registry.Commands.Any())
						_writer.Newline();

					foreach (var command in _registry.Commands)
						_writer.AppendLine("Register(Instances.{0}, \"{1}\");", command.Name, GetRuntimeName(command.Name));
				});

			_writer.Newline();
		}

		/// <summary>
		///   Generates the nested class.
		/// </summary>
		/// <param name="baseClass">The class that the generated registry class should be derived from.</param>
		private void GenerateNestedClass(string baseClass)
		{
			_writer.AppendLine("/// <summary>");
			_writer.AppendLine("///   Stores the actual instances of the cvars or commands managed by the registry.");
			_writer.AppendLine("/// </summary>");
			_writer.AppendLine("public new class InstanceList : {0}.InstanceList", baseClass);
			_writer.AppendBlockStatement(() =>
				{
					GenerateConstructor();
					GenerateCvarInstanceProperties();
					GenerateCommandInstanceProperties();
				});
		}

		/// <summary>
		///   Generates the constructor.
		/// </summary>
		private void GenerateConstructor()
		{
			_writer.AppendLine("/// <summary>");
			_writer.AppendLine("///   Initializes a new instance.");
			_writer.AppendLine("/// </summary>");
			_writer.AppendLine("public InstanceList()");
			_writer.AppendBlockStatement(() =>
				{
					foreach (var cvar in _registry.Cvars)
					{
						_writer.AppendLine("{1} = new Cvar<{0}>(\"{2}\", {3}, \"{4}\", {5});",
										   cvar.Type, cvar.Name, GetRuntimeName(cvar.Name), cvar.DefaultValue,
										   GetSummaryText(cvar.Documentation), cvar.Persistent.ToString().ToLower());
					}

					if (_registry.Cvars.Any() && _registry.Commands.Any())
						_writer.Newline();

					foreach (var command in _registry.Commands)
					{
						_writer.AppendLine("{1} = new Command{0}(\"{2}\", \"{3}\");",
										   GetTypeArguments(command), command.Name,
										   GetRuntimeName(command.Name), GetSummaryText(command.Documentation));
					}
				});
		}

		/// <summary>
		///   Generates the cvar instance properties
		/// </summary>
		private void GenerateCvarInstanceProperties()
		{
			foreach (var cvar in _registry.Cvars)
			{
				_writer.Newline();

				WriteDocumentation(cvar.Documentation);
				_writer.AppendLine("public Cvar<{0}> {1} {{ get; private set; }}", cvar.Type, cvar.Name);
			}
		}

		/// <summary>
		///   Generates the command instance properties
		/// </summary>
		private void GenerateCommandInstanceProperties()
		{
			foreach (var command in _registry.Commands)
			{
				_writer.Newline();

				WriteDocumentation(GetSummary(command.Documentation));
				_writer.AppendLine("public Command{0} {1} {{ get; private set; }}", GetTypeArguments(command), command.Name);
			}
		}

		/// <summary>
		///   Writes the given documentation to the output.
		/// </summary>
		/// <param name="documentation">The documentation that should be written.</param>
		private void WriteDocumentation(IEnumerable<string> documentation)
		{
			foreach (var line in documentation)
				_writer.AppendLine("///{0}", line);
		}

		/// <summary>
		///   Converts the name to its corresponding run-time name.
		/// </summary>
		/// <param name="name">The name whose run-time name should be returned.</param>
		private static IEnumerable<char> MakeUnderscoreName(string name)
		{
			yield return Char.ToLower(name[0]);

			foreach (var c in name.Skip(1))
			{
				if (Char.IsUpper(c))
				{
					yield return '_';
					yield return Char.ToLower(c);
				}
				else
					yield return c;
			}
		}

		/// <summary>
		///   Converts the name to its corresponding run-time name.
		/// </summary>
		/// <param name="name">The name whose run-time name should be returned.</param>
		private static IEnumerable<char> GetRuntimeName(string name)
		{
			return MakeUnderscoreName(name).Aggregate(String.Empty, (s, c) => s + c);
		}

		/// <summary>
		///   Gets the text from the summary tag in the documentation.
		/// </summary>
		/// <param name="documentation">The documentation the summary should be extracted from.</param>
		private string GetSummaryText(IEnumerable<string> documentation)
		{
			var comment = String.Join(" ", documentation.Select(c => c.Trim()));
			var match = Regex.Match(comment, "<summary>(.*)</summary>");

			return match.Groups[1].Value.Trim();
		}

		/// <summary>
		///   Gets the summary tag from the documentation.
		/// </summary>
		/// <param name="documentation">The documentation the summary tag should be extracted from.</param>
		private IEnumerable<string> GetSummary(IEnumerable<string> documentation)
		{
			yield return " <summary>";
			yield return "   " + GetSummaryText(documentation);
			yield return " </summary>";
		}

		/// <summary>
		///   Gets the type arguments for the given command.
		/// </summary>
		/// <param name="command">The command for which the type arguments should be returned.</param>
		private string GetTypeArguments(Command command)
		{
			var types = String.Join(", ", command.Parameters.Select(parameter => parameter.Type));
			if (String.IsNullOrWhiteSpace(types))
				return String.Empty;

			return String.Format("<{0}>", types);
		}

		/// <summary>
		///   Gets the argument declarations for the given command.
		/// </summary>
		/// <param name="command">The command for which the argument declarations should be returned.</param>
		private string GetArgumentDeclarations(Command command)
		{
			return String.Join(", ", command.Parameters.Select(parameter => String.Format("{0} {1}", parameter.Type, parameter.Name)));
		}

		/// <summary>
		///   Gets the arguments required to invoke the command.
		/// </summary>
		/// <param name="command">The command for which the arguments should be returned.</param>
		private string GetInvocationArguments(Command command)
		{
			return String.Join(", ", command.Parameters.Select(parameter => parameter.Name));
		}
	}
}