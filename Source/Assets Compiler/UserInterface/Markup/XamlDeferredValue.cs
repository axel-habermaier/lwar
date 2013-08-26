using System;

namespace Pegasus.AssetsCompiler.UserInterface.Markup
{
	using CodeGeneration;
	using TypeConverters;

	/// <summary>
	///   Represents a Xaml value that is not evaluated immediately.
	/// </summary>
	internal class XamlDeferredValue : XamlElement
	{
		/// <summary>
		///   The un-evaluated raw value.
		/// </summary>
		private readonly string _rawValue;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="xamlFile">The Xaml file that defines the Xaml value.</param>
		/// <param name="valueType">The type of the value.</param>
		/// <param name="value">The string representation of the value.</param>
		public XamlDeferredValue(XamlFile xamlFile, Type valueType, string value)
			: base(false)
		{
			Assert.ArgumentNotNull(xamlFile);
			Assert.ArgumentNotNull(valueType);
			Assert.ArgumentNotNull(value);

			Type = valueType;
			Name = xamlFile.GenerateUniqueName(Type);
			_rawValue = value;
		}

		/// <summary>
		///   Gets the value that was specified in Xaml.
		/// </summary>
		public object Value { get; private set; }

		/// <summary>
		///   Generates the code for the Xaml object.
		/// </summary>
		/// <param name="writer">The code writer that should be used to write the generated code.</param>
		/// <param name="assignmentFormat">The target the generated object should be assigned to.</param>
		public override void GenerateCode(CodeWriter writer, string assignmentFormat)
		{
			Assert.ArgumentNotNull(writer);
			writer.AppendLine(assignmentFormat, TypeConverter.GenerateCode(Value));
		}
	}
}