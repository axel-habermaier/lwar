﻿using System;

namespace Pegasus.AssetsCompiler.CodeGeneration.Scripting
{
	using System.Collections.Generic;
	using Framework;
	using ICSharpCode.NRefactory.CSharp;

	/// <summary>
	///   Represents a cvar property of the registry interface.
	/// </summary>
	internal class Cvar : RegistryElement
	{
		/// <summary>
		///   The declaration of the property that represents the cvar.
		/// </summary>
		private readonly PropertyDeclaration _property;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="property">The declaration of the property that represents the cvar.</param>
		public Cvar(PropertyDeclaration property)
		{
			Assert.ArgumentNotNull(property, () => property);
			_property = property;
		}

		/// <summary>
		///   Gets the name of the cvar.
		/// </summary>
		public string Name
		{
			get { return _property.Name; }
		}

		/// <summary>
		///   Gets the type of the cvar.
		/// </summary>
		public string Type
		{
			get { return _property.ReturnType.ToString(); }
		}

		/// <summary>
		///   Gets the documentation of the cvar.
		/// </summary>
		public IEnumerable<string> Documentation
		{
			get { return _property.GetDocumentation(); }
		}

		/// <summary>
		///   Invoked when the element should initialize itself.
		/// </summary>
		protected override void Initialize()
		{
			base.Initialize();
		}

		/// <summary>
		///   Invoked when the element should validate itself. This method is invoked only if no errors occurred during
		///   initialization.
		/// </summary>
		protected override void Validate()
		{
			base.Validate();
		}
	}
}