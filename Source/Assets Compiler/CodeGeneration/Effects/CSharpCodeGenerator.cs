﻿using System;

namespace Pegasus.AssetsCompiler.CodeGeneration.Effects
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using Assets;
	using Framework;
	using Framework.Platform.Graphics;
	using Framework.Platform.Memory;

	/// <summary>
	///   Generates a C# class for an effect.
	/// </summary>
	internal class CSharpCodeGenerator : DisposableObject
	{
		/// <summary>
		///   The name of the context variable of the Effect base class.
		/// </summary>
		private const string ContextVariableName = "__context";

		/// <summary>
		///   The name of the method that binds the constant buffers and textures.
		/// </summary>
		private readonly string _bindMethodName = String.Format("_{0}Bind", Configuration.ReservedIdentifierPrefix);

		/// <summary>
		///   The writer that is used to write the generated code.
		/// </summary>
		private readonly CodeWriter _writer = new CodeWriter();

		/// <summary>
		///   The effect for which the C# class is generated.
		/// </summary>
		private EffectClass _effect;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public CSharpCodeGenerator()
		{
			_writer.WriterHeader("//");
			_writer.AppendLine("using System;");
			_writer.AppendLine("using System.Diagnostics;");
			_writer.AppendLine("using System.Runtime.InteropServices;");
			_writer.AppendLine("using Pegasus.Framework;");
			_writer.AppendLine("using Pegasus.Framework.Math;");
			_writer.AppendLine("using Pegasus.Framework.Platform;");
			_writer.AppendLine("using Pegasus.Framework.Platform.Graphics;");
			_writer.AppendLine("using Pegasus.Framework.Platform.Memory;");
			_writer.Newline();
		}

		/// <summary>
		///   Gets all non-shared constant buffers declared by the effect.
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
		///   Gets all non-shared constants declared by the effect.
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
		///   Generates the C# effect code.
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

			_writer.Newline();
		}

		/// <summary>
		///   Generates the C# class for the effect.
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
			GenerateOnDisposingMethod();

			GenerateConstantBufferStructs();
		}

		/// <summary>
		///   Generates the dirty fields for all non-shared constant buffers declared by the effect.
		/// </summary>
		private void GenerateDirtyFields()
		{
			foreach (var buffer in ConstantBuffers)
			{
				_writer.AppendLine("/// <summary>");
				_writer.AppendLine("///   Indicates whether the contents of {0} have changed.", buffer.Name);
				_writer.AppendLine("/// </summary>");
				_writer.AppendLine("private bool {0} = true;", GetDirtyFlagName(buffer.Name));
				_writer.Newline();
			}
		}

		/// <summary>
		///   Generates the fields for the non-shared constant buffers declared by the effect.
		/// </summary>
		private void GenerateConstantBufferFields()
		{
			foreach (var buffer in ConstantBuffers)
			{
				_writer.AppendLine("/// <summary>");
				_writer.AppendLine("///   Passes the shader constants in the {0} constant buffer to the GPU.", buffer.Name);
				_writer.AppendLine("/// </summary>");
				_writer.AppendLine("private readonly ConstantBuffer {0};", GetFieldName(buffer.Name));
				_writer.Newline();
			}
		}

		/// <summary>
		///   Generates the fields for all non-shared constants declared by the effect.
		/// </summary>
		private void GenerateConstantsFields()
		{
			foreach (var constant in Constants)
			{
				WriteDocumentation(constant.Documentation);
				_writer.AppendLine("private {0} {1};", ToCSharpType(constant.Type), GetFieldName(constant.Name));
				_writer.Newline();
			}
		}

		/// <summary>
		///   Generates the constructor.
		/// </summary>
		private void GenerateConstructor()
		{
			_writer.AppendLine("/// <summary>");
			_writer.AppendLine("///   Initializes a new instance.");
			_writer.AppendLine("/// </summary>");
			_writer.AppendLine("/// <param name=\"graphicsDevice\">The graphics device this instance belongs to.</param>");
			_writer.AppendLine("/// <param name=\"assets\">The assets manager that should be used to load required assets.</param>");

			_writer.Append("public ");
			if (ConstantBuffers.Any())
				_writer.Append("unsafe ");

			_writer.AppendLine("{0}(GraphicsDevice graphicsDevice, AssetsManager assets)", _effect.Name);
			_writer.AppendLine("\t: base(graphicsDevice, assets)");
			_writer.AppendBlockStatement(() =>
				{
					foreach (var technique in _effect.Techniques)
					{
						var vertexShader = ShaderAsset.GetPath(_effect.FullName, technique.VertexShader.Name, ShaderType.VertexShader);
						var fragmentShader = ShaderAsset.GetPath(_effect.FullName, technique.FragmentShader.Name, ShaderType.FragmentShader);

						_writer.AppendLine("{0} = {1}.CreateTechnique({2},", technique.Name, ContextVariableName, _bindMethodName);
						_writer.AppendLine("\t\"{0}\", ", Path.ChangeExtension(vertexShader, null));
						_writer.AppendLine("\t\"{0}\");", Path.ChangeExtension(fragmentShader, null));
					}

					foreach (var buffer in ConstantBuffers)
					{
						_writer.Newline();
						_writer.AppendLine("{0} = {3}.CreateConstantBuffer({1}, {2});", GetFieldName(buffer.Name), buffer.Size,
										   buffer.Slot, ContextVariableName);
						_writer.AppendLine("{0}.SetName(\"used by {1}\");", GetFieldName(buffer.Name), _effect.FullName);
					}
				});

			_writer.Newline();
		}

		/// <summary>
		///   Generates the properties for all non-shared constants declared by the effect.
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
					_writer.Newline();
				}
			}
		}

		/// <summary>
		///   Generates the properties for the shader texture objects declared by the effect.
		/// </summary>
		private void GenerateTextureProperties()
		{
			foreach (var texture in _effect.Textures)
			{
				WriteDocumentation(texture.Documentation);
				_writer.AppendLine("public {0}View {1} {{ get; set; }}", ToCSharpType(texture.Type), texture.Name);
			}

			if (_effect.Textures.Any())
				_writer.Newline();
		}

		/// <summary>
		///   Generates the properties for all techniques declared by the effect.
		/// </summary>
		private void GenerateTechniqueProperties()
		{
			foreach (var technique in _effect.Techniques)
			{
				WriteDocumentation(technique.Documentation);
				_writer.AppendLine("public EffectTechnique {0} {{ get; private set; }}", technique.Name);
				_writer.Newline();
			}
		}

		/// <summary>
		///   Generates the state binding method.
		/// </summary>
		private void GenerateBindMethod()
		{
			_writer.AppendLine("/// <summary>");
			_writer.AppendLine("///   Binds all textures and non-shared constant buffers required by the effect.");
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

								_writer.Newline();
								_writer.AppendLine("{0} = false;", GetDirtyFlagName(buffer.Name));
								_writer.AppendLine("{2}.Update({0}, &_{1}data);", GetFieldName(buffer.Name),
												   Configuration.ReservedIdentifierPrefix, ContextVariableName);
							});
						_writer.Newline();
					}

					foreach (var texture in _effect.Textures)
						_writer.AppendLine("{2}.Bind({0}, {1});", texture.Name, texture.Slot, ContextVariableName);

					foreach (var buffer in ConstantBuffers)
						_writer.AppendLine("{1}.Bind({0});", GetFieldName(buffer.Name), ContextVariableName);
				});

			if (ConstantBuffers.Any())
				_writer.Newline();
		}

		/// <summary>
		///   Generates the implementation of the OnDisposing() method.
		/// </summary>
		private void GenerateOnDisposingMethod()
		{
			if (!ConstantBuffers.Any())
				return;

			_writer.AppendLine("/// <summary>");
			_writer.AppendLine("///   Disposes the object, releasing all managed and unmanaged resources.");
			_writer.AppendLine("/// </summary>");
			_writer.AppendLine("protected override void __OnDisposing()");
			_writer.AppendBlockStatement(() =>
				{
					if (!ConstantBuffers.Any())
						_writer.AppendLine("// Nothing to do here");

					foreach (var buffer in ConstantBuffers)
						_writer.AppendLine("{0}.SafeDispose();", GetFieldName(buffer.Name));
				});

			if (ConstantBuffers.Any())
				_writer.Newline();
		}

		/// <summary>
		///   Generates the structures for the non-shared constant buffers declared by the effect.
		/// </summary>
		private void GenerateConstantBufferStructs()
		{
			var buffers = ConstantBuffers.ToArray();
			for (var i = 0; i < buffers.Length; ++i)
			{
				_writer.AppendLine("[StructLayout(LayoutKind.Sequential, Size = Size, Pack = 1)]");
				_writer.AppendLine("private struct {0}", GetStructName(buffers[i]));
				_writer.AppendBlockStatement(() =>
					{
						_writer.AppendLine("/// <summary>");
						_writer.AppendLine("///   The size of the struct in bytes.");
						_writer.AppendLine("/// </summary>");
						_writer.AppendLine("public const int Size = {0};", buffers[i].Size);
						_writer.Newline();

						for (var j = 0; j < buffers[i].Constants.Length; ++j)
						{
							WriteDocumentation(buffers[i].Constants[j].Documentation);
							_writer.AppendLine("public {0} {1};", ToCSharpType(buffers[i].Constants[j].Type), buffers[i].Constants[j].Name);

							if (j < buffers[i].Constants.Length - 1)
								_writer.Newline();
						}
					});

				if (i < buffers.Length - 1)
					_writer.Newline();
			}
		}

		/// <summary>
		///   Gets the corresponding C# type.
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
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			File.WriteAllText(Configuration.CSharpEffectFile, _writer.ToString());
		}

		/// <summary>
		///   Gets the name of the corresponding field.
		/// </summary>
		/// <param name="name">The name whose field name should be returned.</param>
		private static string GetFieldName(string name)
		{
			return String.Format("_{0}", name);
		}

		/// <summary>
		///   Gets the name of the constant buffer struct.
		/// </summary>
		/// <param name="buffer">The buffer whose struct name should be returned.</param>
		private static string GetStructName(ConstantBuffer buffer)
		{
			return String.Format("_{1}{0}", buffer.Name, Configuration.ReservedIdentifierPrefix);
		}

		/// <summary>
		///   Gets the name of the corresponding dirty flag.
		/// </summary>
		/// <param name="name">The name whose dirty flag name should be returned.</param>
		private static string GetDirtyFlagName(string name)
		{
			return String.Format("_{1}{0}Dirty", name, Configuration.ReservedIdentifierPrefix);
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
	}
}