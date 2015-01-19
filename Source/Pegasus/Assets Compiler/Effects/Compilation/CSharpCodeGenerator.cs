namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using Commands;
	using CSharp;
	using Utilities;

	/// <summary>
	///     Generates a C# class for an effect.
	/// </summary>
	internal class CSharpCodeGenerator : IDisposable
	{
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
			_writer.AppendLine("using Pegasus.Utilities;");
			_writer.AppendLine("using Pegasus.Math;");
			_writer.AppendLine("using Pegasus.Platform;");
			_writer.AppendLine("using Pegasus.Assets;");
			_writer.AppendLine("using Pegasus.Platform.Graphics;");
			_writer.AppendLine("using Pegasus.Platform.Memory;");
			_writer.AppendLine("using Pegasus.Rendering;");
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
			var path = Path.Combine(Configuration.EffectCodePath, _effect.Name + ".cs");

			Directory.CreateDirectory(Path.GetDirectoryName(path));
			File.WriteAllText(path, _writer.ToString());
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
				_writer.AppendLine("{1} sealed class {0} : GraphicsObject", _effect.Name, Configuration.Visibility);
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
			GenerateShaderFields();

			GenerateConstructor();
			GenerateLoadMethods();

			GenerateConstantsProperties();
			GenerateTextureProperties();
			GenerateTechniqueProperties();

			GenerateOnDisposingMethod();

			GenerateConstantBufferStructs();
			GenerateConstantArrayClasses();

			GenerateTechniqueClasses();
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
			foreach (var constant in Constants.Where(c => !c.IsArray))
			{
				WriteDocumentation(constant.Documentation);
				_writer.AppendLine("private {0} {1};", ToCSharpType(constant.Type), GetFieldName(constant.Name));
				_writer.NewLine();
			}
		}

		/// <summary>
		///     Generates the fields for all shaders declared by the effect.
		/// </summary>
		private void GenerateShaderFields()
		{
			foreach (var shader in _effect.Shaders)
			{
				_writer.AppendLine("private {0} {1};", ToCSharpType(shader), GetFieldName(shader.Name));
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
			_writer.AppendLine("private {0}(GraphicsDevice graphicsDevice)", _effect.Name);
			_writer.IncreaseIndent();
			_writer.AppendLine(": base(graphicsDevice)");
			_writer.DecreaseIndent();
			_writer.AppendBlockStatement(() =>
			{
				var first = true;
				foreach (var buffer in ConstantBuffers)
				{
					if (first)
						first = false;
					else
						_writer.NewLine();

					_writer.AppendLine("{0} = new ConstantBuffer(GraphicsDevice, {1}.Size, {1}.Slot);", GetFieldName(buffer.Name),
						GetStructName(buffer));
					_writer.AppendLine("{0}.SetName(\"{1}.{2}\");", GetFieldName(buffer.Name), _effect.FullName, buffer.Name);
				}

				if (Constants.Any(c => c.IsArray))
					_writer.NewLine();

				foreach (var constant in Constants.Where(c => c.IsArray))
					_writer.AppendLine("{0} = new {1}();", constant.Name, GetClassName(constant));
			});

			_writer.NewLine();
		}

		/// <summary>
		///     Generates the Load methods.
		/// </summary>
		private void GenerateLoadMethods()
		{
			_writer.AppendLine("/// <summary>");
			_writer.AppendLine("///     Loads an effect from the given buffer.");
			_writer.AppendLine("/// </summary>");
			_writer.AppendLine("/// <param name=\"graphicsDevice\">The graphics device the effect should be created for.</param>");
			_writer.AppendLine("/// <param name=\"buffer\">The buffer the effect should be read from.</param>");
			_writer.AppendLine("public static {0} Create(GraphicsDevice graphicsDevice, ref BufferReader buffer)", _effect.Name);
			_writer.AppendBlockStatement(() =>
			{
				_writer.AppendLine("Assert.ArgumentNotNull(graphicsDevice);");

				_writer.NewLine();
				_writer.AppendLine("var effect = new {0}(graphicsDevice);", _effect.Name);
				_writer.AppendLine("effect.Load(ref buffer);");
				_writer.AppendLine("return effect;");
			});

			_writer.NewLine();
			_writer.AppendLine("/// <summary>");
			_writer.AppendLine("///     Loads the effect from the given buffer.");
			_writer.AppendLine("/// </summary>");
			_writer.AppendLine("/// <param name=\"buffer\">The buffer the effect should be initialized from.</param>");
			_writer.AppendLine("internal void Load(ref BufferReader buffer)");
			_writer.AppendBlockStatement(() =>
			{
				foreach (var shader in _effect.Shaders)
				{
					_writer.AppendLine("{0}.SafeDispose();", GetFieldName(shader.Name));
					_writer.AppendLine("{0} = {1}.Create(GraphicsDevice, ref buffer);", GetFieldName(shader.Name), ToCSharpType(shader));
					_writer.AppendLine("{0}.SetName(\"{1}.{2}\");", GetFieldName(shader.Name), _effect.FullName, shader.Name);
					_writer.NewLine();
				}

				foreach (var technique in _effect.Techniques)
				{
					_writer.AppendLine("{0}.SafeDispose();", technique.Name);
					_writer.AppendLine("{0} = new {1}(GraphicsDevice, {2}, {3}, this);", technique.Name, GetClassName(technique),
						GetFieldName(technique.VertexShader.Name), GetFieldName(technique.FragmentShader.Name));
					_writer.NewLine();
				}

				_writer.AppendLine("ShaderSignature.LoadSignatures(ref buffer);");
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

					if (constant.IsArray)
						_writer.AppendLine("public {0} {1} {{ get; private set; }}", GetClassName(constant), constant.Name);
					else
					{
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
					}
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
		///     Generates the implementation of the OnDisposing() method.
		/// </summary>
		private void GenerateOnDisposingMethod()
		{
			_writer.AppendLine("/// <summary>");
			_writer.AppendLine("///     Disposes the object, releasing all managed and unmanaged resources.");
			_writer.AppendLine("/// </summary>");
			_writer.AppendLine("protected override void OnDisposing()");
			_writer.AppendBlockStatement(() =>
			{
				foreach (var buffer in ConstantBuffers)
					_writer.AppendLine("{0}.SafeDispose();", GetFieldName(buffer.Name));

				_writer.NewLine();
				foreach (var shader in _effect.Shaders)
					_writer.AppendLine("{0}.SafeDispose();", GetFieldName(shader.Name));

				_writer.NewLine();
				foreach (var technique in _effect.Techniques)
					_writer.AppendLine("{0}.SafeDispose();", technique.Name);
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
				_writer.Append("private ");

				if (buffers[i].Constants.Any(c => c.IsArray))
					_writer.Append("unsafe ");

				_writer.AppendLine("struct {0}", GetStructName(buffers[i]));
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

						if (constants[j].Constant.IsArray)
							_writer.AppendLine("public fixed byte {0}[{1}.ElementCount * ({1}.ElementSize + {1}.Padding)];",
								constants[j].Constant.Name, GetClassName(constants[j].Constant));
						else
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
		///     Generates the classes for shader constant arrays.
		/// </summary>
		private void GenerateConstantArrayClasses()
		{
			var constants = ConstantBuffers.SelectMany(b => b.GetLayoutedConstants()).Where(c => c.Constant.IsArray);
			foreach (var constant in constants)
			{
				_writer.NewLine();
				_writer.AppendLine("/// <summary>");
				_writer.AppendLine("///     Represents a fixed-length array of shader constants.");
				_writer.AppendLine("/// </summary>");
				_writer.AppendLine("public class {0}", GetClassName(constant.Constant));
				_writer.AppendBlockStatement(() =>
				{
					_writer.AppendLine("/// <summary>");
					_writer.AppendLine("///     The data stored in the shader constant array.");
					_writer.AppendLine("/// </summary>");
					_writer.AppendLine("private readonly {0}[] _data = new {0}[ElementCount];", ToCSharpType(constant.Constant.Type));
					_writer.NewLine();

					_writer.AppendLine("/// <summary>");
					_writer.AppendLine("///     The number of elements stored within the array.");
					_writer.AppendLine("/// </summary>");
					_writer.AppendLine("public const int ElementCount = {0};", constant.ElementCount);
					_writer.NewLine();

					_writer.AppendLine("/// <summary>");
					_writer.AppendLine("///     The size in bytes of a single element.");
					_writer.AppendLine("/// </summary>");
					_writer.AppendLine("public const int ElementSize = {0};", constant.ElementSize);
					_writer.NewLine();

					_writer.AppendLine("/// <summary>");
					_writer.AppendLine("///     The padding in bytes after each element.");
					_writer.AppendLine("/// </summary>");
					_writer.AppendLine("public const int Padding = {0};", constant.Padding);
					_writer.NewLine();

					_writer.AppendLine("/// <summary>");
					_writer.AppendLine("///     Gets a value indicating whether the data contained in the array is dirty.");
					_writer.AppendLine("/// </summary>");
					_writer.AppendLine("internal bool IsDirty {{ get; private set; }}");
					_writer.NewLine();

					_writer.AppendLine("/// <summary>");
					_writer.AppendLine("///     The number of elements stored within the array.");
					_writer.AppendLine("/// </summary>");
					_writer.AppendLine("public int Length {{ get {{ return ElementCount; }} }}");
					_writer.NewLine();

					_writer.AppendLine("/// <summary>");
					_writer.AppendLine("///     Gets or sets an element of the constant array.");
					_writer.AppendLine("/// </summary>");
					_writer.AppendLine("/// <param name=\"index\">The index of the element that should be set or retrieved.</param>");
					_writer.AppendLine("public {0} this[int index]", ToCSharpType(constant.Constant.Type));
					_writer.AppendBlockStatement(() =>
					{
						_writer.AppendLine("get");
						_writer.AppendBlockStatement(() =>
						{
							_writer.AppendLine("Assert.InRange(index, _data);");
							_writer.AppendLine("return _data[index];");
						});
						_writer.AppendLine("set");
						_writer.AppendBlockStatement(() =>
						{
							_writer.AppendLine("Assert.InRange(index, _data);");
							_writer.NewLine();

							_writer.AppendLine("_data[index] = value;");
							_writer.AppendLine("IsDirty = true;");
						});
					});
					_writer.NewLine();

					_writer.AppendLine("/// <summary>");
					_writer.AppendLine("///     Writes the data to the given constant buffer.");
					_writer.AppendLine("/// </summary>");
					_writer.AppendLine(
						"/// <param name=\"buffer\">A pointer to a location within a constant buffer where the data should be written to.</param>");
					_writer.AppendLine("internal unsafe void WriteToConstantBuffer(byte* buffer)");
					_writer.AppendBlockStatement(() =>
					{
						_writer.AppendLine("Assert.ArgumentNotNull(new IntPtr(buffer));");
						_writer.NewLine();

						_writer.AppendLine("foreach (var data in _data)");
						_writer.AppendBlockStatement(() =>
						{
							_writer.AppendLine("var typedBuffer = ({0}*)buffer;", ToCSharpType(constant.Constant.Type));
							_writer.AppendLine("*typedBuffer = data;");
							_writer.AppendLine("buffer += ElementSize + Padding;");
						});
					});
				});
			}
		}

		/// <summary>
		///     Generates a class for each technique.
		/// </summary>
		private void GenerateTechniqueClasses()
		{
			foreach (var technique in _effect.Techniques)
			{
				_writer.NewLine();
				WriteDocumentation(technique.Documentation);
				_writer.AppendLine("private class {0} : EffectTechnique", GetClassName(technique));
				_writer.AppendBlockStatement(() =>
				{
					_writer.AppendLine("/// <summary>");
					_writer.AppendLine("///     The effect the technique belongs to.");
					_writer.AppendLine("/// </summary>");
					_writer.AppendLine("private {0} _effect;", _effect.Name);
					_writer.NewLine();

					_writer.AppendLine("/// <summary>");
					_writer.AppendLine("///     Initializes a new instance.");
					_writer.AppendLine("/// </summary>");
					_writer.AppendLine("/// <param name=\"graphicsDevice\">The graphics device that should be used by the technique.</param>");
					_writer.AppendLine("/// <param name=\"vertexShader\">The vertex shader that should be used by the technique.</param>");
					_writer.AppendLine("/// <param name=\"fragmentShader\">The fragment shader that should be used by the technique.</param>");
					_writer.AppendLine("/// <param name=\"effect\">The effect the technique belongs to.</param>");
					_writer.AppendLine("public {0}(GraphicsDevice graphicsDevice, VertexShader vertexShader, FragmentShader fragmentShader, {1} effect)",
						GetClassName(technique), _effect.Name);
					_writer.IncreaseIndent();
					_writer.AppendLine(": base(graphicsDevice, vertexShader, fragmentShader)");
					_writer.DecreaseIndent();
					_writer.AppendBlockStatement(() => _writer.AppendLine("_effect = effect;"));

					_writer.NewLine();
					_writer.AppendLine("/// <summary>");
					_writer.AppendLine("///     Binds all textures and non-shared constant buffers required by the technique.");
					_writer.AppendLine("/// </summary>");
					_writer.Append("public override ");
					if (Constants.Any())
						_writer.Append("unsafe ");

					_writer.AppendLine("void Bind()");
					_writer.AppendBlockStatement(() =>
					{
						var constantBuffers = ConstantBuffers.Where(constantBuffer => technique.Uses(constantBuffer)).ToArray();
						var textures = _effect.Textures.Where(texture => technique.Uses(texture)).ToArray();

						_writer.AppendLine("ShaderProgram.Bind();");

						if (constantBuffers.Length == 0 && textures.Length == 0)
							_writer.NewLine();

						foreach (var buffer in constantBuffers)
						{
							var arrayConstants = buffer.Constants.Where(c => c.IsArray).ToArray();
							_writer.Append("if (_effect.{0}", GetDirtyFlagName(buffer.Name));
							if (arrayConstants.Length > 0)
							{
								_writer.Append(" || ");
								_writer.AppendSeparated(arrayConstants, " || ", c => _writer.Append("_effect.{0}.IsDirty", c.Name));
							}
							_writer.AppendLine(")");

							_writer.AppendBlockStatement(() =>
							{
								_writer.AppendLine("var _{1}data = new {0}();", GetStructName(buffer), CompilationContext.ReservedIdentifierPrefix);
								foreach (var constant in buffer.Constants)
								{
									if (constant.IsArray)
										_writer.AppendLine("_effect.{0}.WriteToConstantBuffer(_{1}data.{0});", constant.Name, CompilationContext.ReservedIdentifierPrefix);
									else
										_writer.AppendLine("_{1}data.{0} = _effect.{0};", constant.Name, CompilationContext.ReservedIdentifierPrefix);
								}

								_writer.NewLine();
								_writer.AppendLine("_effect.{0} = false;", GetDirtyFlagName(buffer.Name));
								_writer.AppendLine("_effect.{0}.CopyData(&_{1}data);", GetFieldName(buffer.Name),
									CompilationContext.ReservedIdentifierPrefix);
							});
							_writer.NewLine();
						}

						foreach (var texture in textures)
							_writer.AppendLine("_effect.{0}.Bind({1});", texture.Name, texture.Slot);

						foreach (var buffer in constantBuffers)
							_writer.AppendLine("_effect.{0}.Bind();", GetFieldName(buffer.Name));
					});

					_writer.NewLine();
					_writer.AppendLine("/// <summary>");
					_writer.AppendLine("///     Unbinds all textures required by the '{0}' technique.", technique.Name);
					_writer.AppendLine("/// </summary>");
					_writer.Append("public override void Unbind()");
					_writer.AppendBlockStatement(() =>
					{
						var textures = _effect.Textures.Where(texture => technique.Uses(texture)).ToArray();
						if (textures.Length == 0)
							_writer.AppendLine("// Nothing to do here");

						foreach (var texture in textures)
							_writer.AppendLine("_effect.{0}.Unbind({1});", texture.Name, texture.Slot);
					});
				});
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
			return String.Format("_{1}{0}", buffer.Name, CompilationContext.ReservedIdentifierPrefix);
		}

		/// <summary>
		///     Gets the name of the array constant class.
		/// </summary>
		/// <param name="constant">The constant whose class name should be returned.</param>
		private static string GetClassName(ShaderConstant constant)
		{
			return String.Format("{0}Array", constant.Name);
		}

		/// <summary>
		///     Gets the name of the technique class.
		/// </summary>
		/// <param name="technique">The technique whose class name should be returned.</param>
		private static string GetClassName(EffectTechnique technique)
		{
			return String.Format("_{1}{0}Technique", technique.Name, CompilationContext.ReservedIdentifierPrefix);
		}

		/// <summary>
		///     Gets the name of the corresponding dirty flag.
		/// </summary>
		/// <param name="name">The name whose dirty flag name should be returned.</param>
		private static string GetDirtyFlagName(string name)
		{
			return String.Format("_{1}{0}Dirty", name, CompilationContext.ReservedIdentifierPrefix);
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

		/// <summary>
		///     Gets the C# type corresponding to the method's shader type.
		/// </summary>
		/// <param name="method">The shader method the C# type should be returned for.</param>
		protected string ToCSharpType(ShaderMethod method)
		{
			switch (method.Type)
			{
				case ShaderType.VertexShader:
					return "VertexShader";
				case ShaderType.FragmentShader:
					return "FragmentShader";
				default:
					throw new InvalidOperationException("Unknown shader type.");
			}
		}
	}
}