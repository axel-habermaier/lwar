using System;

namespace Pegasus.AssetsCompiler.CodeGeneration.Registries
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
			Assert.ArgumentNotNull(writer);
			Assert.ArgumentNotNull(registry);

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
			_writer.AppendLine("using System.Diagnostics;");
			_writer.Newline();

			_writer.AppendLine("namespace {0}", _registry.Namespace);
			_writer.AppendBlockStatement(() =>
				{
					foreach (var import in _registry.ImportedNamespaces.OrderBy(import => import))
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
						_writer.AppendLine("[DebuggerHidden]");
						_writer.AppendLine("set");
						_writer.AppendBlockStatement(() =>
							{
								_writer.AppendLine("Assert.ArgumentNotNull((object)value);");
								_writer.AppendLine("Instances.{0}.Value = value;", cvar.Name);
							});
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
				_writer.AppendLine("[DebuggerHidden]");
				_writer.AppendLine("public void {0}({1})", command.Name, GetArgumentDeclarations(command));
				_writer.AppendBlockStatement(() =>
					{
						foreach (var parameter in command.Parameters)
							_writer.AppendLine("Assert.ArgumentNotNull((object){0});", parameter.Name);

						_writer.AppendLine("Instances.{0}.Invoke({1});", command.Name, GetInvocationArguments(command));
					});

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
						_writer.Append("{1} = new Cvar<{0}>(\"{2}\", {3}, \"{4}\", {5}",
									   cvar.Type, cvar.Name, GetRuntimeName(cvar.Name), cvar.DefaultValue,
									   GetSummaryText(cvar.Documentation), cvar.Persistent.ToString().ToLower());

						AppendValidators(cvar.Validators);
						_writer.AppendLine(");");
					}

					if (_registry.Cvars.Any() && _registry.Commands.Any())
						_writer.Newline();

					foreach (var command in _registry.Commands)
					{
						_writer.Append("{1} = new Command{0}(\"{2}\", \"{3}\"",
									   GetTypeArguments(command), command.Name,
									   GetRuntimeName(command.Name), GetSummaryText(command.Documentation));

						GenerateCommandParameters(command);
						_writer.AppendLine(");");
					}
				});
		}

		/// <summary>
		///   Appends the validator creation expressions to the current line.
		/// </summary>
		/// <param name="validators">The validators that should be added.</param>
		private void AppendValidators(IEnumerable<Validator> validators)
		{
			if (!validators.Any())
				return;

			_writer.Append(", ");
			_writer.AppendSeparated(validators, ", ", validator =>
				{
					_writer.Append("new {0}(", validator.Name);
					_writer.AppendSeparated(validator.Arguments, ", ", argument => _writer.Append(argument));
					_writer.Append(")");
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
		///   Generates the command parameter argument list for the command.
		/// </summary>
		/// <param name="command">The command for which the parameter should be returned.</param>
		private void GenerateCommandParameters(Command command)
		{
			if (!command.Parameters.Any())
				return;

			_writer.IncreaseIndent();
			_writer.AppendLine(", ");
			_writer.AppendSeparated(command.Parameters, () => _writer.AppendLine(","), parameter =>
				{
					_writer.Append("new CommandParameter(\"{0}\", typeof({1}), {2}, {3}, \"{4}\"",
								   parameter.Name,
								   parameter.Type,
								   parameter.HasDefaultValue.ToString().ToLower(),
								   parameter.HasDefaultValue ? parameter.DefaultValue : String.Format("default({0})", parameter.Type),
								   GetParameterTag(command.Documentation, parameter.Name));

					AppendValidators(parameter.Validators);
					_writer.Append(")");
				});

			_writer.DecreaseIndent();
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
		private static string GetSummaryText(IEnumerable<string> documentation)
		{
			var comment = String.Join(" ", documentation.Select(c => c.Trim()));
			var match = Regex.Match(comment, "<summary>(.*)</summary>");

			return Escape(match.Groups[1].Value.Trim());
		}

		/// <summary>
		///   Gets the text from parameter tag for the given parameter from the documentation.
		/// </summary>
		/// <param name="documentation">The documentation the parameter tag should be extracted from.</param>
		/// <param name="parameter">The name of the parameter for which the parameter tag should be returned.</param>
		private static string GetParameterTag(IEnumerable<string> documentation, string parameter)
		{
			var comment = String.Join(" ", documentation.Select(c => c.Trim()));
			var match = Regex.Match(comment, String.Format("<param name=\"{0}\">(.*?)</param>", parameter));

			return Escape(match.Groups[1].Value.Trim());
		}

		/// <summary>
		///   Gets the summary tag from the documentation.
		/// </summary>
		/// <param name="documentation">The documentation the summary tag should be extracted from.</param>
		private static IEnumerable<string> GetSummary(IEnumerable<string> documentation)
		{
			yield return " <summary>";
			yield return "   " + GetSummaryText(documentation);
			yield return " </summary>";
		}

		/// <summary>
		///   Gets the type arguments for the given command.
		/// </summary>
		/// <param name="command">The command for which the type arguments should be returned.</param>
		private static string GetTypeArguments(Command command)
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
		private static string GetArgumentDeclarations(Command command)
		{
			return String.Join(", ", command.Parameters.Select(parameter =>
				{
					if (parameter.HasDefaultValue)
						return String.Format("{0} {1} = {2}", parameter.Type, parameter.Name, parameter.DefaultValue);
						
					return String.Format("{0} {1}", parameter.Type, parameter.Name);
				}));
		}

		/// <summary>
		///   Gets the arguments required to invoke the command.
		/// </summary>
		/// <param name="command">The command for which the arguments should be returned.</param>
		private static string GetInvocationArguments(Command command)
		{
			return String.Join(", ", command.Parameters.Select(parameter => parameter.Name));
		}

		/// <summary>
		///   Escapes quotes and backslashes.
		/// </summary>
		/// <param name="input">The input that should be escaped.</param>
		private static string Escape(string input)
		{
			return input.Replace("\\", "\\\\").Replace("\"", "\\\"");
		}
	}
}