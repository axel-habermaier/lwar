using System;

namespace Pegasus.AssetsCompiler.UserInterface.Controls
{
	using System.Linq;
	using CodeGeneration;
	using Markup;

	/// <summary>
	///   Provides metadata for the 'ControlTemplate' UI delegate.
	/// </summary>
	[RuntimeNamespace("Pegasus.Framework.UserInterface.Controls")]
	[ContentProperty("Content")]
	internal class ControlTemplate : ICodeGenerator
	{
		[IgnoreAtRuntime]
		public Type TargetType { get; set; }

		public XamlObject Content { get; set; }

		/// <summary>
		///   Generates the code for the given Xaml object.
		/// </summary>
		/// <param name="xamlObject">The Xaml object the code is generated for.</param>
		/// <param name="writer">The code writer that should be used to write the generated code.</param>
		/// <param name="assignmentFormat">The target the generated object should be assigned to.</param>
		public void GenerateCode(XamlObject xamlObject, CodeWriter writer, string assignmentFormat)
		{
			Assert.ArgumentSatisfies(xamlObject.Type == typeof(ControlTemplate), "Unexpected Xaml object.");

			var content = xamlObject.Properties.Single(p => p.Name == "Content");

			writer.Append("t =>", xamlObject.Name);
			writer.AppendBlockStatement(() => content.Value.GenerateCode(writer, "return {0};"));
		}
	}
}