using System;

namespace Pegasus.AssetsCompiler.CodeGeneration.Registries
{
	using System.Collections.Generic;
	using System.Linq;
	using Framework;
	using ICSharpCode.NRefactory.CSharp;

	/// <summary>
	///   Represents a validator attribute on a cvar or command parameter.
	/// </summary>
	internal class Validator : RegistryElement
	{
		/// <summary>
		///   The declaration of the attribute that represents the validator.
		/// </summary>
		private readonly Attribute _attribute;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="attribute">The declaration of the attribute that represents the validator.</param>
		public Validator(Attribute attribute)
		{
			Assert.ArgumentNotNull(attribute);
			_attribute = attribute;
		}

		/// <summary>
		///   Gets the name of the validator.
		/// </summary>
		public string Name
		{
			get
			{
				var name = _attribute.Type.ToString();
				if (name.EndsWith("Attribute"))
					return name;

				return name + "Attribute";
			}
		}

		/// <summary>
		///   Gets the arguments of the validator.
		/// </summary>
		public IEnumerable<string> Arguments
		{
			get { return _attribute.Arguments.Select(argument =>
				{
					if (Name == "RangeAttribute" && argument.GetConstantValue(Resolver) is string)
						return argument.GetConstantValue(Resolver).ToString();
					return argument.ToString();
				}); }
		}
	}
}