namespace Pegasus.AssetsCompiler.Commands
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using Assets;
	using CommandLine;
	using CSharp;
	using ICSharpCode.NRefactory.CSharp;
	using Utilities;

	/// <summary>
	///     Generates C++ and IL native interop code.
	/// </summary>
	[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
	public class InteropCommand : ICommand
	{
		/// <summary>
		///     Gets the names of all interop delegates.
		/// </summary>
		private string[] _interopDelegates;

		/// <summary>
		///     Gets the names of all interop enums.
		/// </summary>
		private string[] _interopEnums;

		/// <summary>
		///     Gets the names of all interop structs.
		/// </summary>
		private string[] _interopStructs;

		/// <summary>
		///     Gets the path of the generated file containing enumerations.
		/// </summary>
		private string GeneratedEnumsFile
		{
			get { return Path.Combine(Output, "Enums.hpp"); }
		}

		/// <summary>
		///     Gets the path of the generated file containing enumerations.
		/// </summary>
		private string GeneratedCallbacksFile
		{
			get { return Path.Combine(Output, "Callbacks.hpp"); }
		}

		/// <summary>
		///     Gets the path of the generated file containing structs.
		/// </summary>
		private string GeneratedStructsFile
		{
			get { return Path.Combine(Output, "Structs.hpp"); }
		}

		/// <summary>
		///     Gets the path of the generated file containing interfaces.
		/// </summary>
		private string GeneratedInterfacesFile
		{
			get { return Path.Combine(Output, "Interfaces.hpp"); }
		}

		/// <summary>
		///     Gets the path of the generated file containing interop functions.
		/// </summary>
		private string GeneratedFunctionsFile
		{
			get { return Path.Combine(Output, "Funcs.hpp"); }
		}

		/// <summary>
		///     Gets or sets the action that should be performed.
		/// </summary>
		public CompilationActions Actions { get; set; }

		/// <summary>
		///     Gets the files interop code should be generated for.
		/// </summary>
		[Option("files", Required = true, HelpText = "The files interop code should be generated for.")]
		public string Files { get; set; }

		/// <summary>
		///     Gets the path to the directory where the generated C++ code should be written to.
		/// </summary>
		[Option("output", Required = true, HelpText = "The path to the directory where the generated C++ code should be written to.")]
		public string Output { get; set; }

		/// <summary>
		///     Executes the command.
		/// </summary>
		public void Execute()
		{
			Configuration.BasePath = Environment.CurrentDirectory;
			Configuration.TempDirectory = Path.Combine(Configuration.BasePath, "obj", "interop");

			Directory.CreateDirectory(Output);

			var files = Files.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(file => new InteropAsset(file)).ToArray();

			if (Actions.HasFlag(CompilationActions.Compile))
				GenerateCode(files);

			if (Actions.HasFlag(CompilationActions.Clean))
				CleanCode(files);
		}

		/// <summary>
		///     Generates the code for the given registry.
		/// </summary>
		/// <param name="assets">The assets the code should be generated for.</param>
		private void GenerateCode(InteropAsset[] assets)
		{
			if (assets.All(asset => !asset.RequiresCompilation))
				return;

			var watch = new Stopwatch();
			watch.Start();

			var parser = new CSharpParser();
			var syntaxTrees = assets.Select(asset => parser.Parse(File.ReadAllText(asset.SourcePath), asset.SourcePath)).ToArray();

			_interopEnums = syntaxTrees
				.SelectMany(syntaxTree => syntaxTree.Descendants.OfType<TypeDeclaration>().Where(type => type.ClassType == ClassType.Enum))
				.Select(e => e.Name)
				.ToArray();

			_interopStructs = syntaxTrees
				.SelectMany(syntaxTree => syntaxTree.Descendants.OfType<TypeDeclaration>().Where(type => type.ClassType == ClassType.Struct))
				.Select(e => e.Name)
				.ToArray();

			_interopDelegates = syntaxTrees
				.SelectMany(syntaxTree => syntaxTree.Descendants.OfType<DelegateDeclaration>())
				.Select(e => e.Name)
				.ToArray();

			GenerateEnums(syntaxTrees);
			GenerateStructs(syntaxTrees);
			GenerateInterfacesCpp(syntaxTrees);
			GenerateInterfacesIL(syntaxTrees);
			GenerateFunctions(syntaxTrees);
			GenerateCallbacks(syntaxTrees);

			foreach (var asset in assets)
				asset.WriteMetadata();

			Log.Info("Generated native interop code ({0:F2}s).", watch.Elapsed.TotalSeconds);
		}

		/// <summary>
		///     Generates the enumerations.
		/// </summary>
		/// <param name="syntaxTrees">The syntax trees the interop code should be generated for.</param>
		private void GenerateEnums(SyntaxTree[] syntaxTrees)
		{
			var writer = new CodeWriter();
			writer.WriterHeader();
			writer.AppendLine("#pragma once");

			var enums = syntaxTrees.SelectMany(syntaxTree =>
				syntaxTree.Descendants.OfType<TypeDeclaration>().Where(type => type.ClassType == ClassType.Enum));

			foreach (var enumDeclaration in enums)
			{
				writer.NewLine();
				writer.AppendLine("enum class {0}", enumDeclaration.Name);
				writer.AppendBlockStatement(() =>
				{
					foreach (var member in enumDeclaration.Members.OfType<EnumMemberDeclaration>())
					{
						if (member.Initializer.IsNull)
							writer.AppendLine("{0},", member.Name);
						else
							writer.AppendLine("{0} = {1},", member.Name, member.Initializer);
					}
				}, terminateWithSemicolon: true);
			}

			File.WriteAllText(GeneratedEnumsFile, writer.ToString());
		}

		/// <summary>
		///     Generates the structs.
		/// </summary>
		/// <param name="syntaxTrees">The syntax trees the interop code should be generated for.</param>
		private void GenerateStructs(SyntaxTree[] syntaxTrees)
		{
			var writer = new CodeWriter();
			writer.WriterHeader();

			writer.AppendLine("#pragma once");

			var structs = syntaxTrees.SelectMany(syntaxTree =>
				syntaxTree.Descendants.OfType<TypeDeclaration>().Where(type => type.ClassType == ClassType.Struct)).ToArray();

			foreach (var structDeclaration in structs)
			{
				if (!structDeclaration.Attributes.Any(a => a.ToString().Contains("StructLayout") && a.ToString().Contains("LayoutKind.Sequential")))
					Log.Die("'LayoutKind.Sequential' is required for interop type '{0}'.", structDeclaration.Name);

				writer.NewLine();
				writer.AppendLine("struct {0}", structDeclaration.Name);
				writer.AppendBlockStatement(() =>
				{
					foreach (var field in structDeclaration.Members.OfType<FieldDeclaration>().Where(field => !field.HasModifier(Modifiers.Static)))
						writer.AppendLine("{0} {1};", MapTypeToCpp(field.ReturnType.ToString()), field.Variables.Single().Name);
				}, terminateWithSemicolon: true);
			}

			writer.NewLine();
			writer.AppendLine("#define PG_DECLARE_STRUCTCHECK_FUNC \\");
			writer.IncreaseIndent();

			var orderedStructs = structs.OrderBy(s => s.Name).ToArray();
			var parameters = String.Join(", ",
				orderedStructs.Select(s => String.Format("int32 {0}{1}", Char.ToLower(s.Name[0]), s.Name.Substring(1))));
			writer.AppendLine("PG_API_EXPORT void ValidateStructSizes({0}) \\", parameters);
			writer.AppendLine("{{ \\");
			writer.IncreaseIndent();

			foreach (var structDeclaration in orderedStructs)
			{
				writer.AppendLine(
					"if (sizeof({0}) != static_cast<size_t>({1}{2})) PG_DIE(\"sizeof({0}): Discrepancy between native and managed code.\"); \\",
					structDeclaration.Name, Char.ToLower(structDeclaration.Name[0]), structDeclaration.Name.Substring(1));
			}

			writer.DecreaseIndent();
			writer.AppendLine("}}");
			writer.DecreaseIndent();

			File.WriteAllText(GeneratedStructsFile, writer.ToString());
		}

		/// <summary>
		///     Generates the C++ classes for the native interface.
		/// </summary>
		/// <param name="syntaxTrees">The syntax trees the interop code should be generated for.</param>
		private void GenerateInterfacesCpp(SyntaxTree[] syntaxTrees)
		{
			var writer = new CodeWriter();
			writer.WriterHeader();

			writer.AppendLine("#pragma once");

			var interfaces = syntaxTrees.SelectMany(syntaxTree =>
				syntaxTree.Descendants.OfType<TypeDeclaration>().Where(type => type.ClassType == ClassType.Interface)).ToArray();

			foreach (var interfaceDeclaration in interfaces)
			{
				var name = interfaceDeclaration.Name.StartsWith("I") ? interfaceDeclaration.Name.Substring(1) : interfaceDeclaration.Name;

				writer.NewLine();
				writer.AppendLine("struct {0}", name);
				writer.AppendBlockStatement(() =>
				{
					writer.AppendLine("void* _this;");
					writer.NewLine();

					var properties = interfaceDeclaration.Members.OfType<PropertyDeclaration>().OrderBy(m => m.Name).ToArray();
					foreach (var property in properties)
						writer.AppendLine("{0} {1};", MapTypeToCpp(property.ReturnType.ToString()), property.Name);

					if (properties.Length > 0)
						writer.NewLine();

					foreach (var method in interfaceDeclaration.Members.OfType<MethodDeclaration>().OrderBy(m => m.Name))
						writer.AppendLine("void* {0};", method.Name);
				}, terminateWithSemicolon: true);

				writer.NewLine();
				writer.AppendLine("#define PG_DECLARE_{0}_METHODS \\", name.ToUpper());
				var methods = from method in interfaceDeclaration.Members.OfType<MethodDeclaration>().OrderBy(m => m.Name)
							  let returnType = MapTypeToCpp(method.ReturnType.ToString())
							  let parameters =
								  String.Join(", ", method.Parameters.Select(p => String.Format("{0} {1}", MapTypeToCpp(p), p.Name)))
							  select String.Format("\t{0} {1}({2});", returnType, method.Name, parameters);
				writer.AppendLine("{0}", String.Join(" \\" + Environment.NewLine, methods));

				writer.NewLine();
				writer.AppendLine("#define PG_INITIALIZE_{0}(implType, suffix, interfaceObj) \\", name.ToUpper());
				methods = from method in interfaceDeclaration.Members.OfType<MethodDeclaration>().OrderBy(m => m.Name)
						  select String.Format("\tinterfaceObj->{0} = reinterpret_cast<void*>(implType##suffix##_{0});", method.Name);
				writer.AppendLine("{0}", String.Join(" \\" + Environment.NewLine, methods));

				writer.NewLine();
				writer.AppendLine("#define PG_DECLARE_{0}_API(type, suffix) \\", name.ToUpper());
				methods = from method in interfaceDeclaration.Members.OfType<MethodDeclaration>().OrderBy(m => m.Name)
						  let returnType = MapTypeToCpp(method.ReturnType.ToString())
						  let returnKeyword = returnType == "void" ? String.Empty : "return "
						  let parameters =
							  String.Join(", ", method.Parameters.Select(p => String.Format("{0} {1}", MapTypeToCpp(p), p.Name)))
						  let arguments =
							  String.Join(", ", method.Parameters.Select(p => p.Name))
						  let firstComma = method.Parameters.Count > 0 ? ", " : String.Empty
						  select String.Format("\tPG_API_EXPORT {0} type##suffix##_{1}(type* _this{2}{3}) {{ {4}_this->{1}({5}); }}",
							  returnType, method.Name, firstComma, parameters, returnKeyword, arguments);
				writer.AppendLine("{0}", String.Join(" \\" + Environment.NewLine, methods));
			}

			writer.NewLine();
			writer.AppendLine("#define PG_DECLARE_INTERFACECHECK_FUNC \\");
			writer.IncreaseIndent();

			var orderedInterfaces = interfaces.OrderBy(i => i.Name).ToArray();
			var sizeParameters = String.Join(", ",
				orderedInterfaces.Select(s => String.Format("int32 {0}{1}", Char.ToLower(s.Name[1]), s.Name.Substring(2))));
			writer.AppendLine("PG_API_EXPORT void ValidateInterfaceSizes({0}) \\", sizeParameters);
			writer.AppendLine("{{ \\");
			writer.IncreaseIndent();

			foreach (var interfaceDeclaration in orderedInterfaces)
			{
				writer.AppendLine(
					"if (sizeof({0}) != static_cast<size_t>({1}{2})) PG_DIE(\"sizeof({0}): Discrepancy between native and managed code.\"); \\",
					interfaceDeclaration.Name.Substring(1), Char.ToLower(interfaceDeclaration.Name[1]), interfaceDeclaration.Name.Substring(2));
			}

			writer.DecreaseIndent();
			writer.AppendLine("}}");
			writer.DecreaseIndent();

			File.WriteAllText(GeneratedInterfacesFile, writer.ToString());
		}

		/// <summary>
		///     Generates the IL classes for the native interface.
		/// </summary>
		/// <param name="syntaxTrees">The syntax trees the interop code should be generated for.</param>
		private void GenerateInterfacesIL(SyntaxTree[] syntaxTrees)
		{
			foreach (var syntaxTree in syntaxTrees)
			{
				var interfaces = syntaxTree.Descendants.OfType<TypeDeclaration>().Where(type => type.ClassType == ClassType.Interface);
				foreach (var interfaceDeclaration in interfaces)
				{
					var writer = new CodeWriter();
					writer.WriterHeader();

					var namespaceName = interfaceDeclaration.GetParent<NamespaceDeclaration>().NamespaceName;
					var interfaceName = interfaceDeclaration.Name.StartsWith("I") ? interfaceDeclaration.Name.Substring(1) : interfaceDeclaration.Name;
					var name = String.Format("{0}.{1}", namespaceName, interfaceName);

					writer.AppendLine(".class public sequential ansi sealed beforefieldinit {0}", name);
					writer.AppendLine("		extends [System.Runtime]System.ValueType");
					writer.AppendBlockStatement(() =>
					{
						writer.AppendLine(".pack 0");
						writer.AppendLine(".size 0");
						writer.NewLine();

						writer.AppendLine(".field public void* _this");
						writer.NewLine();

						var properties = interfaceDeclaration.Members.OfType<PropertyDeclaration>().OrderBy(m => m.Name).ToArray();
						foreach (var property in properties)
							writer.AppendLine(".field private {0} {1}", MapTypeToIL(property.ReturnType.ToString()), property.Name);

						if (properties.Length > 0)
							writer.NewLine();

						foreach (var method in interfaceDeclaration.Members.OfType<MethodDeclaration>().OrderBy(m => m.Name))
							writer.AppendLine(".field private void* _{0}", method.Name);

						foreach (var method in interfaceDeclaration.Members.OfType<MethodDeclaration>())
						{
							// Mono's ilasm doesn't support the aggressiveinlining flag...
							var inlining = Environment.OSVersion.Platform == PlatformID.Win32NT ? "aggressiveinlining" : "";
							var parameters = method.Parameters.Select(p => String.Format("{0} {1}", MapTypeToIL(p.Type.ToString()), p.Name));
							var parameterTypes = method.Parameters.Select(p => p.Type.ToString() == "bool" ? "int8" : MapTypeToIL(p.Type.ToString()));

							writer.NewLine();
							writer.AppendLine(".method public hidebysig {0} {1}({2}) cil managed {3}",
								MapTypeToIL(method.ReturnType.ToString()), method.Name, String.Join(", ", parameters), inlining);
							writer.AppendBlockStatement(() =>
							{
								writer.AppendLine(".maxstack {0}", method.Parameters.Count + 2);
								writer.NewLine();

								writer.AppendLine("ldarg.0");
								writer.AppendLine("ldfld void* {0}::_this", name);

								var i = 0;
								foreach (var parameter in method.Parameters)
								{
									writer.AppendLine("ldarg.s {0}", ++i);
									if (parameter.Type.ToString() == "bool")
										writer.AppendLine("conv.i1");
								}

								writer.AppendLine("ldarg.0");
								writer.AppendLine("ldfld void* {0}::_{1}", name, method.Name);

								var returnType = MapTypeToIL(method.ReturnType.ToString());
								writer.AppendLine("calli unmanaged stdcall {0}(void*{1})", returnType == "bool" ? "int8" : returnType,
									!parameterTypes.Any() ? String.Empty : (", " + String.Join(", ", parameterTypes)));

								if (returnType == "bool")
									writer.AppendLine("conv.i4");

								writer.AppendLine("ret");
							});
						}
					});

					File.WriteAllText(Path.ChangeExtension(syntaxTree.FileName, "asm"), writer.ToString());
				}
			}
		}

		/// <summary>
		///     Generates the interop functions.
		/// </summary>
		/// <param name="syntaxTrees">The syntax trees the interop code should be generated for.</param>
		private void GenerateFunctions(SyntaxTree[] syntaxTrees)
		{
			var writer = new CodeWriter();
			writer.WriterHeader();

			writer.AppendLine("#pragma once");

			var classes = syntaxTrees.SelectMany(syntaxTree =>
				syntaxTree.Descendants.OfType<TypeDeclaration>().Where(type => type.ClassType == ClassType.Class));

			foreach (var classDeclaration in classes)
			{
				writer.NewLine();

				foreach (var method in classDeclaration.Members.OfType<MethodDeclaration>())
				{
					var parameters = String.Join(", ",
						method.Parameters.Select(p => String.Format("{0} {1}", MapTypeToCpp(p), p.Name)));
					writer.AppendLine("PG_API_EXPORT {0} {1}({2});", MapTypeToCpp(method.ReturnType.ToString()), method.Name, parameters);
				}
			}

			File.WriteAllText(GeneratedFunctionsFile, writer.ToString());
		}

		/// <summary>
		///     Generates the interop callbacks.
		/// </summary>
		/// <param name="syntaxTrees">The syntax trees the interop code should be generated for.</param>
		private void GenerateCallbacks(SyntaxTree[] syntaxTrees)
		{
			var writer = new CodeWriter();
			writer.WriterHeader();

			writer.AppendLine("#pragma once");
			writer.NewLine();

			var callbacks = syntaxTrees.SelectMany(syntaxTree => syntaxTree.Descendants.OfType<DelegateDeclaration>());

			foreach (var callback in callbacks)
			{
				var parameters = String.Join(", ",
					callback.Parameters.Select(p => String.Format("{0} {1}", MapTypeToCpp(p), p.Name)));
				writer.AppendLine("using {1} = {0} (*)({2});", MapTypeToCpp(callback.ReturnType.ToString()), callback.Name, parameters);
			}

			File.WriteAllText(GeneratedCallbacksFile, writer.ToString());
		}

		/// <summary>
		///     Cleans the code for the given registry.
		/// </summary>
		/// <param name="assets">The assets that should be cleaned.</param>
		private void CleanCode(InteropAsset[] assets)
		{
			foreach (var asset in assets)
				asset.DeleteMetadata();

			File.Delete(GeneratedEnumsFile);
			File.Delete(GeneratedStructsFile);

			Log.Info("Cleaned native interop code.");
		}

		/// <summary>
		///     Maps the given type to its C++ equivalent.
		/// </summary>
		/// <param name="parameter">The parameter whose type should be mapped.</param>
		private string MapTypeToCpp(ParameterDeclaration parameter)
		{
			switch (parameter.ParameterModifier)
			{
				case ParameterModifier.Ref:
				case ParameterModifier.Out:
					return MapTypeToCpp(parameter.Type + "*");
				default:
					return MapTypeToCpp(parameter.Type.ToString());
			}
		}

		/// <summary>
		///     Maps the given type to its C++ equivalent.
		/// </summary>
		/// <param name="type">The type that should be mapped.</param>
		private string MapTypeToCpp(string type)
		{
			if (type.StartsWith("ref") || type.StartsWith("out"))
				throw new Exception("ga");

			if (type.EndsWith("*") || _interopEnums.Contains(type) || _interopDelegates.Contains(type) || _interopStructs.Contains(type))
				return type;

			if (type.EndsWith("[]"))
				return MapTypeToCpp(type.Substring(0, type.Length - 2)) + "*";

			switch (type)
			{
				case "IntPtr":
					return "void*";
				case "void":
					return "void";
				case "bool":
					return "bool";
				case "byte":
					return "byte";
				case "sbyte":
					return "sbyte";
				case "ushort":
					return "uint16";
				case "short":
					return "int16";
				case "int":
					return "int32";
				case "uint":
					return "uint32";
				case "long":
					return "int64";
				case "ulong":
					return "uint64";
				case "float":
					return "float32";
				case "double":
					return "float64";
				case "string":
					return "const char*";
				default:
					return String.Format("{0}*", type);
			}
		}

		/// <summary>
		///     Maps the given type to its IL equivalent.
		/// </summary>
		/// <param name="type">The type that should be mapped.</param>
		private string MapTypeToIL(string type)
		{
			if (type.EndsWith("*"))
				return "void*";

			switch (type)
			{
				case "void":
					return "void";
				case "bool":
					return "bool";
				case "byte":
					return "unsigned int8";
				case "sbyte":
					return "int8";
				case "ushort":
					return "unsigned int16";
				case "short":
					return "int16";
				case "int":
					return "int32";
				case "uint":
					return "unsigned int32";
				case "long":
					return "int64";
				case "ulong":
					return "unsigned int64";
				case "float":
					return "float32";
				case "double":
					return "float64";
				default:
					if (_interopEnums.Contains(type))
						return "int32";
					return "void*";
			}
		}
	}
}