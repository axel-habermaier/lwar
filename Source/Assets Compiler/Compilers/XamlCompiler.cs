using System;

namespace Pegasus.AssetsCompiler.Compilers
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Xaml;
	using Assets;
	using CodeGeneration;
	using Platform.Memory;
	using UserInterface.Markup;

	/// <summary>
	///   Compiles Xaml assets into C# code targeting the Pegasus UI library.
	/// </summary>
	internal class XamlCompiler : AssetCompiler<XamlAsset>
	{
		// TODO: REMOVE
		public override bool Compile(IEnumerable<Asset> assets)
		{
			foreach (var xaml in assets.OfType<XamlAsset>())
				File.Delete(xaml.HashPath);
			return base.Compile(assets);
		}

		/// <summary>
		///   Compiles the asset.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		/// <param name="buffer">The buffer the compilation output should be appended to.</param>
		protected override void Compile(XamlAsset asset, BufferWriter buffer)
		{
			return;
			XamlServices.Transform(new XamlXmlReader(asset.SourcePath), new MyXamlWriter());
			

			var className = Path.GetFileNameWithoutExtension(asset.RelativePath);
			var namespaceName = asset.RelativePath.Substring(0, asset.RelativePath.Length - asset.FileName.Length - 1);

			var writer = new CodeWriter();
			writer.WriterHeader("//");

			var xamlFile = new XamlFile(asset.SourcePath);
			xamlFile.GenerateCode(writer, namespaceName.Replace("/", "."), className);

			buffer.Copy(Encoding.UTF8.GetBytes(writer.ToString()));
		}
	}

	internal class MyXamlWriter : XamlWriter
	{
		/// <summary>
		///   When implemented in a derived class, gets the active XAML schema context.
		/// </summary>
		/// <returns>
		///   The active XAML schema context.
		/// </returns>
		public override XamlSchemaContext SchemaContext
		{
			get { return new XamlSchemaContext(); }
		}

		/// <summary>
		///   When implemented in a derived class, produces an object for cases where the object is a default or implicit value of
		///   the property being set, instead of being specified as a discrete object value in the input XAML node set.
		/// </summary>
		public override void WriteGetObject()
		{
			return;
		}

		/// <summary>
		///   When implemented in a derived class, writes the representation of a start object node.
		/// </summary>
		/// <param name="type">The XAML type of the object to write.</param>
		public override void WriteStartObject(XamlType type)
		{
			return;
		}

		/// <summary>
		///   When implemented in a derived class, produces the representation of an end object node.
		/// </summary>
		public override void WriteEndObject()
		{
			return;
		}

		/// <summary>
		///   When implemented in a derived class, writes the representation of a start member node.
		/// </summary>
		/// <param name="xamlMember">The member node to write.</param>
		public override void WriteStartMember(XamlMember xamlMember)
		{
			//xamlMember.TypeConverter.ConverterInstance.ConvertFrom()
			return;
		}

		/// <summary>
		///   When implemented in a derived class, produces the representation of an end member node.
		/// </summary>
		public override void WriteEndMember()
		{
			return;
		}

		/// <summary>
		///   When implemented in a derived class, writes a value node.
		/// </summary>
		/// <param name="value">The value to write.</param>
		public override void WriteValue(object value)
		{
			return;
		}

		/// <summary>
		///   When implemented in a derived class, writes a XAML namespace declaration node.
		/// </summary>
		/// <param name="namespaceDeclaration">The namespace declaration to write.</param>
		public override void WriteNamespace(NamespaceDeclaration namespaceDeclaration)
		{
			return;
		}
	}
}