using System;

namespace Pegasus.AssetsCompiler.UserInterface
{
	using CodeGeneration;
	using Markup;

	/// <summary>
	///   Generates the code for a Xaml object.
	/// </summary>
	internal interface ICodeGenerator
	{
		/// <summary>
		///   Generates the code for the given Xaml object.
		/// </summary>
		/// <param name="xamlObject">The Xaml object the code is generated for.</param>
		/// <param name="writer">The code writer that should be used to write the generated code.</param>
		/// <param name="assignmentFormat">The target the generated object should be assigned to.</param>
		void GenerateCode(XamlObject xamlObject, CodeWriter writer, string assignmentFormat);
	}
}