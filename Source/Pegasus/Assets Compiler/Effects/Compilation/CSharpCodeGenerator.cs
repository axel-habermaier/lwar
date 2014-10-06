namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using Assets;
	using CSharp;

	/// <summary>
	///     Generates a C# class for an effect.
	/// </summary>
	internal class CSharpCodeGenerator : IDisposable
	{
		/// <summary>
		///     The name of the context variable of the Effect base class.
		/// </summary>
		private const string ContextVariableName = "__context";

		/// <summary>
		///     The name of the method that binds the constant buffers and textures.
		/// </summary>
		private readonly string _bindMethodName = String.Format("_{0}Bind", Configuration.ReservedIdentifierPrefix);

		/// <summary>
		///     The name of the method that unbinds the textures.
		/// </summary>
		private readonly string _unbindMethodName = String.Format("_{0}Unbind", Configuration.ReservedIdentifierPrefix);

		/// <summary>
		///     The writer that is used to write the generated code.
		/// </summary>
		private readonly CodeWriter _writer = new CodeWriter();

		/// <summary>
		///     The effect for which the C# class is generated.
		/// </summary>
		private EffectClass _effect;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public CSharpCodeGenerator()
		{
			_writer.WriterHeader();
			_writer.AppendLine("using System;");
			_writer.AppendLine("using System.Diagnostics;");
			_writer.AppendLine("using System.Runtime.InteropServices;");
			_writer.AppendLine("using Pegasus;");
			_writer.AppendLine("using Pegasus.Math;");
			_writer.AppendLine("using Pegasus.Platform;");
			_writer.AppendLine("using Pegasus.Assets;");
			_writer.AppendLine("using Pegasus.Platform.Graphics;");
			_writer.AppendLine("using Pegasus.Platform.Memory;");
			_writer.NewLine();
		}

		/// <summary>
		///     Gets all non-shared constant buffers declared by the effect.
		/// </summary>
		private IEnumerable<ConstantBuffer> ConstantBuffers
		{
			get
			{
				return from buffer in _effect.ConstantBuffers
					   where !buffer.Shared
					   select buffer;
			}
		}

		/// <summary>
		///     Gets all non-shared constants declared by the effect.
		/// </summary>
		private IEnumerable<ShaderConstant> Constants
		{
			get
			{
				return from buffer in ConstantBuffers
					   from constant in buffer.Constants
					   select constant;
			}
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			File.WriteAllText(Configuration.CSharpEffectFile, _writer.ToString());
		}

		/// <summary>
		///     Generates the C# effect code.
		/// </summary>
		/// <param name="effect">The effect for which the C# code should be generated.</param>
		public void GenerateCode(EffectClass effect)
		{
			Assert.ArgumentNotNull(effect);

			_effect = effect;

			_writer.AppendLine("namespace {0}", _effect.Namespace);
			_writer.AppendBlockStatement(() =>
			{
				WriteDocumentation(_effect.Documentation);
				_writer.AppendLine("public sealed class {0} : Effect", _effect.Name);
				_writer.AppendBlockStatement(GenerateClass);
			});

			_writer.NewLine();
		}

		/// <summary>
		///     Generates the C# class for the effect.
		/// </summary>
		private void GenerateClass()
		{
			GenerateDirtyFields();
			GenerateConstantBufferFields();
			GenerateConstantsFields();

			GenerateConstructor();

			GenerateConstantsProperties();
			GenerateTextureProperties();
			GenerateTechniqueProperties();

			GenerateBindMethod();
			GenerateUnbindMethod();
			GenerateOnDisposingMethod();

			GenerateConstantBufferStructs();
		}

		/// <summary>
		///     Generates the dirty fields for all non-shared constant buffers declared by the effect.
		/// </summary>
		private void GenerateDirtyFields()
		{
			foreach (var buffer in ConstantBuffers)
			{
				_writer.AppendLine("/// <summary>");
				_writer.AppendLine("///     Indicates whether the contents of {0} have changed.", buffer.Name);
				_writer.AppendLine("/// </summary>");
				_writer.AppendLine("private bool {0} = true;", GetDirtyFlagName(buffer.Name));
				_writer.NewLine();
			}
		}

		/// <summary>
		///     Generates the fields for the non-shared constant buffers declared by the effect.
		/// </summary>
		private void GenerateConstantBufferFields()
		{
			foreach (var buffer in ConstantBuffers)
			{
				_writer.AppendLine("/// <summary>");
				_writer.AppendLine("///     Passes the shader constants in the {0} constant buffer to the GPU.", buffer.Name);
				_writer.AppendLine("/// </summary>");
				_writer.AppendLine("private readonly ConstantBuffer {0};", GetFieldName(buffer.Name));
				_writer.NewLine();
			}
		}

		/// <summary>
		///     Generates the fields for all non-shared constants declared by the effect.
		/// </summary>
		private void GenerateConstantsFields()
		{
			foreach (var constant in Constants)
			{
				WriteDocumentation(constant.Documentation);
				_writer.AppendLine("private {0} {1};", ToCSharpType(constant.Type), GetFieldName(constant.Name));
				_writer.NewLine();
			}
		}

		/// <summary>
		///     Generates the constructor.
		/// </summary>
		private void GenerateConstructor()
		{
			_writer.AppendLine("/// <summary>");
			_writer.AppendLine("///     Initializes a new instance.");
			_writer.AppendLine("/// </summary>");
			_writer.AppendLine("/// <param name=\"graphicsDevice\">The graphics device this instance belongs to.</param>");
			_writer.AppendLine("/// <param name=\"assets\">The assets manager that should be used to load required assets.</param>");

			_writer.AppendLine("public {0}(GraphicsDevice graphicsDevice, AssetsManager assets)", _effect.Name);
			_writer.AppendLine("\t: base(graphicsDevice, assets)");
			_writer.AppendBlockStatement(() =>
			{
				foreach (var technique in _effect.Techniques)
				{
					_writer.AppendLine("{0} = {1}.CreateTechnique({2}, {3}, {4}, {5});", technique.Name, ContextVariableName,
						_bindMethodName, _unbindMethodName,
						ShaderAsset.GetAssetIdentifier(_effect.Namespace.Replace(".", "/"), _effect.Name + "/" + technique.VertexShader.Name)
								   .Replace("/", "."),
						ShaderAsset.GetAssetIdentifier(_effect.Namespace.Replace(".", "/"), _effect.Name + "/" + technique.FragmentShader.Name)
								   .Replace("/", "."));
				}

				foreach (var buffer in ConstantBuffers)
				{
					_writer.NewLine();
					_writer.AppendLine("{0} = {2}.CreateConstantBuffer({1}.Size, {1}.Slot);", GetFieldName(buffer.Name),
						GetStructName(buffer), ContextVariableName);
					_writer.AppendLine("{0}.SetName(\"used by {1}\");", GetFieldName(buffer.Name), _effect.FullName);
				}
			});

			_writer.NewLine();
		}

		/// <summary>
		///     Generates the properties for all non-shared constants declared by the effect.
		/// </summary>
		private void GenerateConstantsProperties()
		{
			foreach (var buffer in ConstantBuffers)
			{
				foreach (var constant in buffer.Constants)
				{
					WriteDocumentation(constant.Documentation);
					_writer.AppendLine("public {0} {1}", ToCSharpType(constant.Type), constant.Name);
					_writer.AppendBlockStatement(() =>
					{
						_writer.AppendLine("get {{ return {0}; }}", GetFieldName(constant.Name));
						_writer.AppendLine("set");
						_writer.AppendBlockStatement(() =>
						{
							_writer.AppendLine("{0} = value;", GetFieldName(constant.Name));
							_writer.AppendLine("{0} = true;", GetDirtyFlagName(buffer.Name));
						});
					});
					_writer.NewLine();
				}
			}
		}

		/// <summary>
		///     Generates the properties for the shader texture objects declared by the effect.
		/// </summary>
		private void GenerateTextureProperties()
		{
			foreach (var texture in _effect.Textures)
			{
				WriteDocumentation(texture.Documentation);
				_writer.AppendLine("public {0}View {1} {{ get; set; }}", ToCSharpType(texture.Type), texture.Name);
			}

			if (_effect.Textures.Any())
				_writer.NewLine();
		}

		/// <summary>
		///     Generates the properties for all techniques declared by the effect.
		/// </summary>
		private void GenerateTechniqueProperties()
		{
			foreach (var technique in _effect.Techniques)
			{
				WriteDocumentation(technique.Documentation);
				_writer.AppendLine("public EffectTechnique {0} {{ get; private set; }}", technique.Name);
				_writer.NewLine();
			}
		}

		/// <summary>
		///     Generates the state binding method.
		/// </summary>
		private void GenerateBindMethod()
		{
			_writer.AppendLine("/// <summary>");
			_writer.AppendLine("///     Binds all textures and non-shared constant buffers required by the effect.");
			_writer.AppendLine("/// </summary>");
			_writer.Append("private ");
			if (Constants.Any())
				_writer.Append("unsafe ");

			_writer.AppendLine("void {0}()", _bindMethodName);
			_writer.AppendBlockStatement(() =>
			{
				if (!ConstantBuffers.Any() && !_effect.Textures.Any())
					_writer.AppendLine("// Nothing to do here");

				foreach (var buffer in ConstantBuffers)
				{
					_writer.AppendLine("if ({0})", GetDirtyFlagName(buffer.Name));
					_writer.AppendBlockStatement(() =>
					{
						_writer.AppendLine("var _{1}data = new {0}();", GetStructName(buffer), Configuration.ReservedIdentifierPrefix);
						foreach (var constant in buffer.Constants)
							_writer.AppendLine("_{1}data.{0} = {0};", constant.Name, Configuration.ReservedIdentifierPrefix);

						_writer.NewLine();
						_writer.AppendLine("{0} = false;", GetDirtyFlagName(buffer.Name));
						_writer.AppendLine("{2}.Update({0}, &_{1}data);", GetFieldName(buffer.Name),
							Configuration.ReservedIdentifierPrefix, ContextVariableName);
					});
					_writer.NewLine();
				}

				foreach (var texture in _effect.Textures)
					_writer.AppendLine("{2}.Bind({0}, {1});", texture.Name, texture.Slot, ContextVariableName);

				foreach (var buffer in ConstantBuffers)
					_writer.AppendLine("{1}.Bind({0});", GetFieldName(buffer.Name), ContextVariableName);
			});

			_writer.NewLine();
		}

		/// <summary>
		///     Generates the texture unbinding method.
		/// </summary>
		private void GenerateUnbindMethod()
		{
			_writer.AppendLine("/// <summary>");
			_writer.AppendLine("///     Unbinds all textures required by the effect.");
			_writer.AppendLine("/// </summary>");
			_writer.Append("private void {0}()", _unbindMethodName);
			_writer.AppendBlockStatement(() =>
			{
				if (!_effect.Textures.Any())
					_writer.AppendLine("// Nothing to do here");

				foreach (var texture in _effect.Textures)
					_writer.AppendLine("{2}.Unbind({0}, {1});", texture.Name, texture.Slot, ContextVariableName);
			});

			if (ConstantBuffers.Any())
				_writer.NewLine();
		}

		/// <summary>
		///     Generates the implementation of the OnDisposing() method.
		/// </summary>
		private void GenerateOnDisposingMethod()
		{
			_writer.AppendLine("/// <summary>");
			_writer.AppendLine("///     Disposes the object, releasing all managed and unmanaged resources.");
			_writer.AppendLine("/// </summary>");
			_writer.AppendLine("protected override void __OnDisposing()");
			_writer.AppendBlockStatement(() =>
			{
				foreach (var buffer in ConstantBuffers)
					_writer.AppendLine("{0}.SafeDispose();", GetFieldName(buffer.Name));
			});

			if (ConstantBuffers.Any())
				_writer.NewLine();
		}

		/// <summary>
		///     Generates the structures for the non-shared constant buffers declared by the effect.
		/// </summary>
		private void GenerateConstantBufferStructs()
		{
			var buffers = ConstantBuffers.ToArray();
			for (var i = 0; i < buffers.Length; ++i)
			{
				_writer.AppendLine("[StructLayout(LayoutKind.Explicit, Size = Size)]");
				_writer.AppendLine("private struct {0}", GetStructName(buffers[i]));
				_writer.AppendBlockStatement(() =>
				{
					_writer.AppendLine("/// <summary>");
					_writer.AppendLine("///     The size of the struct in bytes.");
					_writer.AppendLine("/// </summary>");
					_writer.AppendLine("public const int Size = {0};", buffers[i].Size);
					_writer.NewLine();

					_writer.AppendLine("/// <summary>");
					_writer.AppendLine("///     The slot the constant buffer is assigned to.");
					_writer.AppendLine("/// </summary>");
					_writer.AppendLine("public const int Slot = {0};", buffers[i].Slot);
					_writer.NewLine();

					var constants = buffers[i].GetLayoutedConstants().ToArray();
					for (var j = 0; j < constants.Length; ++j)
					{
						WriteDocumentation(constants[j].Constant.Documentation);
						_writer.AppendLine("[FieldOffset({0})]", constants[j].Offset);
						_writer.AppendLine("public {0} {1};", ToCSharpType(constants[j].Constant.Type), constants[j].Constant.Name);

						if (j < buffers[i].Constants.Length - 1)
							_writer.NewLine();
					}
				});

				if (i < buffers.Length - 1)
					_writer.NewLine();
			}
		}

		/// <summary>
		///     Gets the corresponding C# type.
		/// </summary>
		/// <param name="dataType">The data type that should be converted.</param>
		private static string ToCSharpType(DataType dataType)
		{
			switch (dataType)
			{
				case DataType.Boolean:
					return "bool";
				case DataType.Integer:
					return "int";
				case DataType.Float:
					return "float";
				case DataType.Vector2:
					return "Vector2";
				case DataType.Vector3:
					return "Vector3";
				case DataType.Vector4:
					return "Vector4";
				case DataType.Matrix:
					return "Matrix";
				case DataType.Texture2D:
					return "Texture2D";
				case DataType.CubeMap:
					return "CubeMap";
				default:
					throw new NotSupportedException("Unsupported data type.");
			}
		}

		/// <summary>
		///     Gets the name of the corresponding field.
		/// </summary>
		/// <param name="name">The name whose field name should be returned.</param>
		private static string GetFieldName(string name)
		{
			return String.Format("_{0}", name);
		}

		/// <summary>
		///     Gets the name of the constant buffer struct.
		/// </summary>
		/// <param name="buffer">The buffer whose struct name should be returned.</param>
		private static string GetStructName(ConstantBuffer buffer)
		{
			return String.Format("_{1}{0}", buffer.Name, Configuration.ReservedIdentifierPrefix);
		}

		/// <summary>
		///     Gets the name of the corresponding dirty flag.
		/// </summary>
		/// <param name="name">The name whose dirty flag name should be returned.</param>
		private static string GetDirtyFlagName(string name)
		{
			return String.Format("_{1}{0}Dirty", name, Configuration.ReservedIdentifierPrefix);
		}

		/// <summary>
		///     Writes the given documentation to the output.
		/// </summary>
		/// <param name="documentation">The documentation that should be written.</param>
		private void WriteDocumentation(IEnumerable<string> documentation)
		{
			foreach (var line in documentation)
				_writer.AppendLine("///{0}", line);
		}
	}
}