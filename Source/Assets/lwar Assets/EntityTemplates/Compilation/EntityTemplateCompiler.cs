using Pegasus.AssetsCompiler.Assets.Attributes;

[assembly: Ignore("EntityTemplates/Compilation/EntityTemplateCompiler.cs")]

namespace Lwar.Assets.EntityTemplates.Compilation
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.CSharp.Resolver;
	using ICSharpCode.NRefactory.TypeSystem;
	using Pegasus.AssetsCompiler;
	using Pegasus.AssetsCompiler.Assets;
	using Pegasus.AssetsCompiler.Compilers;
	using Pegasus.AssetsCompiler.CSharp;
	using Pegasus.Platform.Logging;

	/// <summary>
	///     Compiles entity template declarations.
	/// </summary>
	internal class EntityTemplateCompiler : AssetCompiler<EntityTemplateAsset>
	{
		/// <summary>
		///     The path of the client file containing the template declarations.
		/// </summary>
		private const string ClientTemplates = "../../Source/lwar/Gameplay/Entities/EntityTemplates.cs";

		/// <summary>
		///     The path of the client file containing the entity type enumeration.
		/// </summary>
		private const string ClientTypeEnumeration = "../../Source/lwar/Gameplay/Entities/EntityType.cs";

		/// <summary>
		///     The path of the server file containing the template declarations.
		/// </summary>
		private const string ServerTemplates = "../../Source/Other/Server/templates.c";

		/// <summary>
		///     The path of the server header file.
		/// </summary>
		private const string ServerHeader = "../../Source/Other/Server/entity.h";

		/// <summary>
		///     Compiles all assets of the compiler's asset source type.
		/// </summary>
		/// <param name="assets">The assets that should be compiled.</param>
		public override bool Compile(IEnumerable<Asset> assets)
		{
			if (DetermineAction(assets.OfType<EntityTemplateAsset>()) == CompilationAction.Skip)
				Log.Info("Skipping entity templates compilation (no changes detected).");
			else
			{
				Log.Info("Compiling entity templates...");

				foreach (var asset in assets)
					Hash.Compute(asset.SourcePath).WriteTo(asset.HashPath);

				var templates = GetTemplates(GetClassNames(assets.OfType<EntityTemplateAsset>()))
					.OrderBy(template => template.Name).ToArray();

				GenerateServerTemplates(templates);
				GenerateServerHeader(templates);

				GenerateClientTemplates(templates);
				GenerateClientTypeEnumeration(templates);
			}

			return true;
		}

		/// <summary>
		///     Checks whether any of the templates have changed.
		/// </summary>
		/// <param name="assets">The assets that should be checked to determine the compilation action.</param>
		private static CompilationAction DetermineAction(IEnumerable<Asset> assets)
		{
			foreach (var asset in assets)
			{
				if (!File.Exists(asset.HashPath))
					return CompilationAction.Process;

				var oldHash = Hash.FromFile(asset.HashPath);
				var newHash = Hash.Compute(asset.SourcePath);

				if (oldHash != newHash)
					return CompilationAction.Process;
			}

			return CompilationAction.Skip;
		}

		/// <summary>
		///     Writes the contents of the given code writer to the file at the given path.
		/// </summary>
		/// <param name="path">The path of the file that should be written.</param>
		/// <param name="writer">The contents of the file that should be written.</param>
		private static void WriteToFile(string path, CodeWriter writer)
		{
			Log.Info("Writing '{0}'...", path);
			File.WriteAllText(path, writer.ToString());
		}

		/// <summary>
		///     Generates the server templates file.
		/// </summary>
		/// <param name="templates">The declared templates.</param>
		private static void GenerateServerTemplates(EntityTemplate[] templates)
		{
			var writer = new CodeWriter();
			writer.WriterHeader();

			// Write required includes
			writer.AppendLine("#include <math.h>");
			writer.AppendLine("#include <stdint.h>");
			writer.AppendLine("#include <stddef.h>");
			writer.AppendLine("#include <stdlib.h>");
			writer.NewLine();
			writer.AppendLine("#include \"server.h\"");
			writer.AppendLine("#include \"rules.h\"");
			writer.AppendLine("#include \"vector.h\"");
			writer.NewLine();

			// Write the callback prototypes
			foreach (var act in templates.Select(t => t.Act).Where(a => a != null).Distinct())
				writer.AppendLine("void {0}(Entity *self);", act);

			writer.NewLine();

			foreach (var collide in templates.Select(t => t.Collide).Where(c => c != null).Distinct())
				writer.AppendLine("void {0}(Entity *self, Entity *other, Real impact);", collide);

			writer.NewLine();

			// Write the template definitions
			foreach (var template in templates)
			{
				writer.AppendLine("EntityType type_{0} =", template.Name.ToLower());
				writer.AppendBlockStatement(() =>
				{
					writer.AppendLine("ENTITY_TYPE_{0}, // entity type", template.Name.ToUpper());
					writer.AppendLine("{0}, // action", template.Act ?? "NULL");
					writer.AppendLine("{0}, // collide", template.Collide ?? "NULL");
					writer.AppendLine("{0}, // interval", template.Interval);
					writer.AppendLine("{0:0.0######}f, // energy", template.Energy);
					writer.AppendLine("{0:0.0######}f, // health", template.Health);
					writer.AppendLine("{0:0.0######}f, // shield", template.Shield);
					writer.AppendLine("{0:0.0######}f, // length", template.Length);
					writer.AppendLine("{0:0.0######}f, // mass", template.Mass);
					writer.AppendLine("{0:0.0######}f, // radius", template.Radius);
					writer.AppendLine("{0}, {1}, // acceleration", template.Acceleration.X, template.Acceleration.Y);
					writer.AppendLine("{0}, {1}, // deceleration", template.Decelaration.X, template.Decelaration.Y);
					writer.AppendLine("{0:0.0######}f, // rotation", template.Rotation);
				}, true);

				writer.NewLine();
			}

			writer.NewLine();
			writer.AppendLine("void templates_register()");
			writer.AppendBlockStatement(() =>
			{
				foreach (var template in templates)
					writer.AppendLine("entity_type_register(\"{0}\", &type_{0}, {1});", template.Name.ToLower(),
									  template.Format == null ? "0" : ("&" + template.Format));
			});

			WriteToFile(ServerTemplates, writer);
		}

		/// <summary>
		///     Generates the server header file.
		/// </summary>
		/// <param name="templates">The declared templates.</param>
		private static void GenerateServerHeader(IEnumerable<EntityTemplate> templates)
		{
			var writer = new CodeWriter();
			writer.WriterHeader();

			writer.AppendLine("enum");
			writer.AppendBlockStatement(() =>
			{
				var i = 0;
				foreach (var template in templates)
					writer.AppendLine("ENTITY_TYPE_{0} = {1},", template.Name.ToUpper(), ++i);
			}, true);

			writer.NewLine();
			foreach (var template in templates)
				writer.AppendLine("extern EntityType type_{0};", template.Name.ToLower());

			WriteToFile(ServerHeader, writer);
		}

		/// <summary>
		///     Generates the server templates file.
		/// </summary>
		/// <param name="templates">The declared templates.</param>
		private static void GenerateClientTemplates(EntityTemplate[] templates)
		{
			var writer = new CodeWriter();
			writer.WriterHeader();

			writer.AppendLine("namespace Lwar.Gameplay.Entities");
			writer.AppendBlockStatement(() =>
			{
				writer.AppendLine("using Assets;");
				writer.AppendLine("using Pegasus;");
				writer.AppendLine("using Pegasus.Platform;");
				writer.AppendLine("using Pegasus.Platform.Assets;");
				writer.AppendLine("using Pegasus.Platform.Graphics;");
				writer.AppendLine("using Pegasus.Platform.Memory;");
				writer.AppendLine("using Pegasus.Rendering;");
				writer.NewLine();

				writer.AppendLine("public static class EntityTemplates");
				writer.AppendBlockStatement(() =>
				{
					foreach (var template in templates)
						writer.AppendLine("public static EntityTemplate {0} {{ get; private set; }}", template.Name);

					writer.NewLine();

					writer.AppendLine("public static void Initialize(GraphicsDevice graphicsDevice, AssetsManager assets)");
					writer.AppendBlockStatement(() =>
					{
						writer.AppendLine("Assert.ArgumentNotNull(graphicsDevice);");
						writer.AppendLine("Assert.ArgumentNotNull(assets);");
						writer.NewLine();

						for (var i = 0; i < templates.Length; ++i)
						{
							var template = templates[i];

							writer.AppendLine("{0} = new EntityTemplate", template.Name);
							writer.AppendLine("(");
							writer.IncreaseIndent();
							writer.AppendLine("maxEnergy: {0:0.0######}f,", template.Energy);
							writer.AppendLine("maxHealth: {0:0.0######}f,", template.Health);
							writer.AppendLine("radius: {0:0.0######}f,", template.Radius);
							if (template.Texture != null)
								writer.AppendLine("texture: assets.Load({0}),", template.Texture);
							else
								writer.AppendLine("texture: null,");

							if (template.CubeMap != null)
								writer.AppendLine("cubeMap: assets.Load({0}),", template.CubeMap);
							else
								writer.AppendLine("cubeMap: null,");

							var model = template.Model(template);
							if (model != null)
								writer.AppendLine("model: {0}", model);
							else
								writer.AppendLine("model: null");

							writer.DecreaseIndent();
							writer.AppendLine(");");

							if (i + 1 < templates.Length)
								writer.NewLine();
						}
					});

					writer.NewLine();
					writer.AppendLine("public static void Dispose()");
					writer.AppendBlockStatement(() =>
					{
						foreach (var template in templates)
							writer.AppendLine("{0}.SafeDispose();", template.Name);
					});
				});
			});

			WriteToFile(ClientTemplates, writer);
		}

		/// <summary>
		///     Generates the server entity type enumeration file.
		/// </summary>
		/// <param name="templates">The declared templates.</param>
		private static void GenerateClientTypeEnumeration(IEnumerable<EntityTemplate> templates)
		{
			var writer = new CodeWriter();
			writer.WriterHeader();

			writer.AppendLine("namespace Lwar.Gameplay.Entities");
			writer.AppendBlockStatement(() =>
			{
				writer.AppendLine("public enum EntityType");
				writer.AppendBlockStatement(() =>
				{
					var i = 0;
					foreach (var template in templates)
						writer.AppendLine("{0} = {1}, ", template.Name, ++i);
				});
			});

			WriteToFile(ClientTypeEnumeration, writer);
		}

		/// <summary>
		///     Gets the declared templates.
		/// </summary>
		/// <param name="templates">The names of the classes that declare the templates.</param>
		private static IEnumerable<EntityTemplate> GetTemplates(IEnumerable<string> templates)
		{
			foreach (var templateClass in templates)
			{
				var type = Type.GetType(templateClass);
				foreach (var templateField in type.GetFields(BindingFlags.Static | BindingFlags.Public)
												  .Where(field => field.FieldType == typeof(EntityTemplate)))
				{
					var template = (EntityTemplate)templateField.GetValue(null);
					template.Name = templateField.Name;

					yield return template;
				}
			}
		}

		/// <summary>
		///     Gets the names of the classes declaring entity templates.
		/// </summary>
		/// <param name="templates">The template assets that declare the entity templates.</param>
		private static IEnumerable<string> GetClassNames(IEnumerable<EntityTemplateAsset> templates)
		{
			var parser = new CSharpParser();

			foreach (var template in templates)
			{
				var syntaxTree = parser.Parse(File.ReadAllText(template.SourcePath), template.SourcePath);
				var unresolvedFile = syntaxTree.ToTypeSystem();

				IProjectContent project = new CSharpProjectContent();
				project = project.AddOrUpdateFiles(unresolvedFile);

				var compilation = project.CreateCompilation();
				var resolver = new CSharpAstResolver(compilation, syntaxTree, unresolvedFile);

				foreach (var type in syntaxTree.DescendantsAndSelf.OfType<TypeDeclaration>())
				{
					if (type.ClassType == ClassType.Class)
						yield return resolver.Resolve(type).Type.FullName;
					else
					{
						var file = template.SourcePath.ToLocationString(type.NameToken.StartLocation, type.NameToken.EndLocation);
						Log.Warn("{0}: warning: Unexpected type declaration '{1}'.", file, type.Name);
					}
				}
			}
		}

		/// <summary>
		///     Removes the compiled assets and all temporary files written by the compiler.
		/// </summary>
		/// <param name="assets">The assets that should be cleaned.</param>
		public override void Clean(IEnumerable<Asset> assets)
		{
			foreach (var asset in assets.OfType<EntityTemplateAsset>())
			{
				File.Delete(asset.TempPath);
				File.Delete(asset.HashPath);
			}
		}
	}
}