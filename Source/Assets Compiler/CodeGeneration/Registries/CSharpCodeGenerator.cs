using System;

namespace Pegasus.AssetsCompiler.CodeGeneration.Registries
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Text.RegularExpressions;

	/// <summary>
	///   Generates a C# registry class.
	/// </summary>
	internal class CSharpCodeGenerator
	{
		/// <summary>
		///   The imported registry.
		/// </summary>
		private readonly Registry _importedRegistry;

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
		/// <param name="importedRegistry">The imported registry.</param>
		public CSharpCodeGenerator(CodeWriter writer, Registry registry, Registry importedRegistry)
		{
			Assert.ArgumentNotNull(writer);
			Assert.ArgumentNotNull(registry);
			Assert.ArgumentNotNull(importedRegistry);

			_writer = writer;
			_registry = registry;
			_importedRegistry = importedRegistry;
		}

		/// <summary>
		///   Generates the registry class.
		/// </summary>
		/// <param name="namespaceName">The namespace in which the generated class should live.</param>
		public void GenerateCode(string namespaceName)
		{
			_writer.WriterHeader("//");
			_writer.AppendLine("using System;");
			_writer.AppendLine("using System.Diagnostics;");
			_writer.Newline();

			_writer.AppendLine("namespace {0}", namespaceName);
			_writer.AppendBlockStatement(() =>
			{
				foreach (var import in _registry.ImportedNamespaces
												.Union(_importedRegistry.ImportedNamespaces)
												.OrderBy(import => import))
					_writer.AppendLine("using {0};", import);

				_writer.Newline();

				_writer.AppendLine("internal static class {0}", _registry.Name);
				_writer.AppendBlockStatement(() =>
				{
					GenerateProperties();

					GenerateCvarProperties();
					GenerateCvarEvents();

					GenerateCommandMethods();
					GenerateCommandEvents();

					GenerateInitializeMethod();
					GenerateResolveMethod();
				});
			});
		}

		/// <summary>
		///   Generates the cvar and command properties.
		/// </summary>
		private void GenerateProperties()
		{
			foreach (var cvar in _registry.Cvars.Concat(_importedRegistry.Cvars))
			{
				WriteDocumentation(cvar.Documentation);
				_writer.AppendLine("public static Cvar<{0}> {1} {{ get; private set; }}", cvar.Type, GetPropertyName(cvar));
				_writer.Newline();
			}

			foreach (var command in _registry.Commands.Concat(_importedRegistry.Commands))
			{
				WriteDocumentation(GetSummary(command.Documentation));
				_writer.AppendLine("public static Command{0} {1} {{ get; private set; }}", GetTypeArguments(command), GetPropertyName(command));
				_writer.Newline();
			}
		}

		/// <summary>
		///   Generates the cvar properties.
		/// </summary>
		private void GenerateCvarProperties()
		{
			foreach (var cvar in _registry.Cvars.Concat(_importedRegistry.Cvars))
			{
				WriteDocumentation(cvar.Documentation);
				_writer.AppendLine("public static {0} {1}", cvar.Type, cvar.Name);
				_writer.AppendBlockStatement(() =>
				{
					_writer.AppendLine("get {{ return {0}.Value; }}", GetPropertyName(cvar));
					_writer.AppendLine("[DebuggerHidden]");
					_writer.AppendLine("set");
					_writer.AppendBlockStatement(() =>
					{
						_writer.AppendLine("Assert.ArgumentNotNull((object)value);");
						_writer.AppendLine("{0}.Value = value;", GetPropertyName(cvar));
					});
				});

				_writer.Newline();
			}
		}

		/// <summary>
		///   Generates the cvar events.
		/// </summary>
		private void GenerateCvarEvents()
		{
			foreach (var cvar in _registry.Cvars.Concat(_importedRegistry.Cvars))
			{
				_writer.AppendLine("/// <summary>");
				_writer.AppendLine("///   Raised when the '{0}' cvar is changing. The new value is passed to the event handler.", cvar.Name);
				_writer.AppendLine("/// </summary>");
				_writer.AppendLine("public static event Action<{0}> {1}Changing", cvar.Type, cvar.Name);
				_writer.AppendBlockStatement(() =>
				{
					_writer.AppendLine("add {{ {0}.Changing += value; }}", GetPropertyName(cvar));
					_writer.AppendLine("remove {{ {0}.Changing -= value; }}", GetPropertyName(cvar));
				});

				_writer.Newline();

				_writer.AppendLine("/// <summary>");
				_writer.AppendLine("///   Raised when the '{0}' cvar is changed. The previous value is passed to the event handler.", cvar.Name);
				_writer.AppendLine("/// </summary>");
				_writer.AppendLine("public static event Action<{0}> {1}Changed", cvar.Type, cvar.Name);
				_writer.AppendBlockStatement(() =>
				{
					_writer.AppendLine("add {{ {0}.Changed += value; }}", GetPropertyName(cvar));
					_writer.AppendLine("remove {{ {0}.Changed -= value; }}", GetPropertyName(cvar));
				});

				_writer.Newline();
			}
		}

		/// <summary>
		///   Generates the command methods.
		/// </summary>
		private void GenerateCommandMethods()
		{
			foreach (var command in _registry.Commands.Concat(_importedRegistry.Commands))
			{
				WriteDocumentation(command.Documentation);
				_writer.AppendLine("[DebuggerHidden]");
				_writer.AppendLine("public static void {0}({1})", command.Name, GetArgumentDeclarations(command));
				_writer.AppendBlockStatement(() =>
				{
					foreach (var parameter in command.Parameters)
						_writer.AppendLine("Assert.ArgumentNotNull((object){0});", parameter.Name);

					_writer.AppendLine("{0}.Invoke({1});", GetPropertyName(command), GetInvocationArguments(command));
				});

				_writer.Newline();
			}
		}

		/// <summary>
		///   Generates the command events.
		/// </summary>
		private void GenerateCommandEvents()
		{
			foreach (var command in _registry.Commands.Concat(_importedRegistry.Commands))
			{
				_writer.AppendLine("/// <summary>");
				_writer.AppendLine("///   Raised when the '{0}' command is invoked.", command.Name);
				_writer.AppendLine("/// </summary>");
				_writer.AppendLine("public static event Action{0} On{1}", GetTypeArguments(command), command.Name);
				_writer.AppendBlockStatement(() =>
				{
					_writer.AppendLine("add {{ {0}.Invoked += value; }}", GetPropertyName(command));
					_writer.AppendLine("remove {{ {0}.Invoked -= value; }}", GetPropertyName(command));
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
			_writer.AppendLine("///   Initializes the instances declared by the registry.");
			_writer.AppendLine("/// </summary>");
			_writer.AppendLine("public static void Initialize()");
			_writer.AppendBlockStatement(() =>
			{
				GenerateInstanceInitialization();
				_writer.Newline();

				GenerateInstanceRegistration();
			});
			_writer.Newline();
		}

		/// <summary>
		///   Generates the code for the initialization of the cvar and command instances.
		/// </summary>
		private void GenerateInstanceInitialization()
		{
			foreach (var cvar in _registry.Cvars)
			{
				_writer.Append("{1} = new Cvar<{0}>(\"{2}\", {3}, \"{4}\", {5}, {6}, {7}",
							   cvar.Type, GetPropertyName(cvar), GetRuntimeName(cvar.Name), cvar.DefaultValue,
							   GetSummaryText(cvar.Documentation), cvar.UpdateMode, cvar.Persistent.ToString().ToLower(),
							   cvar.SystemOnly.ToString().ToLower());

				AppendValidators(cvar.Validators);
				_writer.AppendLine(");");
			}

			if (_registry.Cvars.Any() && _registry.Commands.Any())
				_writer.Newline();

			foreach (var command in _registry.Commands)
			{
				_writer.Append("{1} = new Command{0}(\"{2}\", \"{3}\", {4}",
							   GetTypeArguments(command), GetPropertyName(command),
							   GetRuntimeName(command.Name), GetSummaryText(command.Documentation),
							   command.SystemOnly.ToString().ToLower());

				GenerateCommandParameters(command);
				_writer.AppendLine(");");
			}
		}

		/// <summary>
		///   Generates the code for the registration of the cvar and command instances.
		/// </summary>
		private void GenerateInstanceRegistration()
		{
			foreach (var cvar in _registry.Cvars)
				_writer.AppendLine("CvarRegistry.Register({0});", GetPropertyName(cvar));

			if (_registry.Cvars.Any() && _registry.Commands.Any())
				_writer.Newline();

			foreach (var command in _registry.Commands)
				_writer.AppendLine("CommandRegistry.Register({0});", GetPropertyName(command));
		}

		/// <summary>
		///   Generates the code for the lookup of imported cvar and command instances.
		/// </summary>
		private void GenerateResolveMethod()
		{
			_writer.AppendLine("/// <summary>");
			_writer.AppendLine("///   Initializes the instances imported by the registry.");
			_writer.AppendLine("/// </summary>");
			_writer.AppendLine("public static void Resolve()");
			_writer.AppendBlockStatement(() =>
			{
				foreach (var cvar in _importedRegistry.Cvars)
					_writer.AppendLine("{2} = CvarRegistry.Resolve<{1}>(\"{0}\");", GetRuntimeName(cvar.Name), cvar.Type, GetPropertyName(cvar));

				if (_importedRegistry.Cvars.Any() && _importedRegistry.Commands.Any())
					_writer.Newline();

				foreach (var command in _importedRegistry.Commands)
				{
					_writer.AppendLine("{2} = CommandRegistry.Resolve{1}(\"{0}\");", GetRuntimeName(command.Name), GetTypeArguments(command),
									   GetPropertyName(command));
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
		///   Gets the field name of the given cvar.
		/// </summary>
		/// <param name="cvar">The cvar field name should be returned.</param>
		private static string GetPropertyName(Cvar cvar)
		{
			return String.Format("{0}Cvar", cvar.Name);
		}

		/// <summary>
		///   Gets the field name of the given command.
		/// </summary>
		/// <param name="command">The command field name should be returned.</param>
		private static string GetPropertyName(Command command)
		{
			return String.Format("{0}Command", command.Name);
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