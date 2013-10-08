﻿namespace Pegasus.AssetsCompiler.UserInterface
{
	using System;

	/// <summary>
	///   When applied to a UIElement-derived class, denotes the namespace in which the runtime-version of the UI element is
	///   defined.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Struct, AllowMultiple = false)]
	internal class RuntimeNamespaceAttribute : Attribute
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="name">The namespace at runtime.</param>
		public RuntimeNamespaceAttribute(string name)
		{
			Assert.ArgumentNotNullOrWhitespace(name);
			Name = name;
		}

		/// <summary>
		///   Gets the namespace at runtime.
		/// </summary>
		public string Name { get; private set; }
	}
}