namespace Pegasus.AssetsCompiler.UserInterface
{
	using System;
	using System.Linq;
	using CodeGeneration;
	using Markup;

	/// <summary>
	///   Provides metadata for the 'Setter' UI class.
	/// </summary>
	[RuntimeNamespace("Pegasus.Framework.UserInterface")]
	internal class Setter : ICodeGenerator
	{
		public DependencyPropertyReference Property { get; set; }

		public object Value { get; set; }

		/// <summary>
		///   Generates the code for the given Xaml object.
		/// </summary>
		/// <param name="xamlObject">The Xaml object the code is generated for.</param>
		/// <param name="writer">The code writer that should be used to write the generated code.</param>
		/// <param name="assignmentFormat">The target the generated object should be assigned to.</param>
		public void GenerateCode(XamlObject xamlObject, CodeWriter writer, string assignmentFormat)
		{
			Assert.ArgumentSatisfies(xamlObject.Type == typeof(Setter), "Unexpected Xaml object.");

			var property = (XamlValue)xamlObject.Properties.Single(p => p.Name == "Property").Value;
			var dependencyProperty = (DependencyPropertyReference)property.Value;
			var value = xamlObject.Properties.Single(p => p.Name == "Value").Value;

			writer.Append("var {0} = new Pegasus.Framework.UserInterface.Setter<{1}>(", xamlObject.Name, dependencyProperty.RuntimePropertyType);

			property.GenerateCode(writer, "{0}Property");
			writer.Append(", ");
			value.GenerateCode(writer, "{0}");

			writer.AppendLine(");");
			writer.AppendLine(assignmentFormat, xamlObject.Name);
		}
	}
}