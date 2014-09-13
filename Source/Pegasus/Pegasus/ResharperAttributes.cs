/*
 * Copyright 2007-2012 JetBrains s.r.o.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace Pegasus
{
	using System;

	// =================================================================================================================
	// Attributes provided by Jetbrain's Resharper for static code analysis                                          
	// =================================================================================================================

	/// <summary>
	///     Specifies how the symbol is used implicitly when marked with <see cref="MeansImplicitUseAttribute" /> or
	///     <see cref="UsedImplicitlyAttribute" />.
	/// </summary>
	[Flags]
	public enum ImplicitUseKindFlags
	{
		/// <summary>
		///     Indicates that the marked symbol is accessed, assigned, and instantiated implicitly.
		/// </summary>
		Default = Access | Assign | InstantiatedWithFixedConstructorSignature,

		/// <summary>
		///     Indicates that the marked symbol is accessed.
		/// </summary>
		Access = 1,

		/// <summary>
		///     Indicates that the marked symbol is assigned.
		/// </summary>
		Assign = 2,

		/// <summary>
		///     Indicates implicit instantiation of a type with a fixed constructor signature.
		/// </summary>
		InstantiatedWithFixedConstructorSignature = 4,

		/// <summary>
		///     Indicates implicit instantiation of a type without a fixed constructor signature.
		/// </summary>
		InstantiatedNoFixedConstructorSignature = 8,
	}

	/// <summary>
	///     Specifies whether only the symbol or all of its members are considered used implicitly when marked with
	///     <see cref="MeansImplicitUseAttribute" /> or <see cref="UsedImplicitlyAttribute" />.
	/// </summary>
	[Flags]
	public enum ImplicitUseTargetFlags
	{
		/// <summary>
		///     The marked symbol itself is considered used.
		/// </summary>
		Default = Itself,

		/// <summary>
		///     The marked symbol itself is considered used.
		/// </summary>
		Itself = 1,

		/// <summary>
		///     All members of the marked symbol are considered used.
		/// </summary>
		Members = 2,

		/// <summary>
		///     The marked symbol itself and all of its members are considered used.
		/// </summary>
		WithMembers = Itself | Members
	}

	/// <summary>
	///     Indicates that the marked attribute causes the symbol it is applied to to be considered used implicitly, for instance
	///     when the symbol is used via reflection only. Prevents code analysis tools like Resharper to incorrectly mark the symbol
	///     as unused.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class MeansImplicitUseAttribute : Attribute
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="useKindFlags">Specifies how the symbol is used implicitly.</param>
		/// <param name="targetFlags">Specifies whether only the marked symbol or all of its members are considered used.</param>
		[UsedImplicitly]
		public MeansImplicitUseAttribute(ImplicitUseKindFlags useKindFlags = ImplicitUseKindFlags.Default,
										 ImplicitUseTargetFlags targetFlags = ImplicitUseTargetFlags.Default)
		{
			UseKindFlags = useKindFlags;
			TargetFlags = targetFlags;
		}

		/// <summary>
		///     Gets a value indicating how the marked symbol is used implicitly.
		/// </summary>
		[UsedImplicitly]
		public ImplicitUseKindFlags UseKindFlags { get; private set; }

		/// <summary>
		///     Gets a value indicating whether only the marked symbol or all of its members are considered used.
		/// </summary>
		[UsedImplicitly]
		public ImplicitUseTargetFlags TargetFlags { get; private set; }
	}

	/// <summary>
	///     Indicates that the marked symbol is used implicitly, for instance when the symbol is used via reflection only.
	///     Prevents code analysis tools like Resharper to incorrectly mark the symbol as unused.
	/// </summary>
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	public sealed class UsedImplicitlyAttribute : Attribute
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="targetFlags">Specifies whether only the marked symbol or all of its members are considered used.</param>
		[UsedImplicitly]
		public UsedImplicitlyAttribute(ImplicitUseTargetFlags targetFlags)
			: this(ImplicitUseKindFlags.Default, targetFlags)
		{
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="useKindFlags">Specifies how the symbol is used implicitly.</param>
		[UsedImplicitly]
		public UsedImplicitlyAttribute(ImplicitUseKindFlags useKindFlags)
			: this(useKindFlags, ImplicitUseTargetFlags.Default)
		{
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="useKindFlags">Specifies how the symbol is used implicitly.</param>
		/// <param name="targetFlags">Specifies whether only the marked symbol or all of its members are considered used.</param>
		[UsedImplicitly]
		public UsedImplicitlyAttribute(ImplicitUseKindFlags useKindFlags = ImplicitUseKindFlags.Default,
									   ImplicitUseTargetFlags targetFlags = ImplicitUseTargetFlags.Default)
		{
			UseKindFlags = useKindFlags;
			TargetFlags = targetFlags;
		}

		/// <summary>
		///     Gets a value indicating how the marked symbol is used implicitly.
		/// </summary>
		[UsedImplicitly]
		public ImplicitUseKindFlags UseKindFlags { get; private set; }

		/// <summary>
		///     Gets a value indicating whether only the marked symbol or all of its members are considered used.
		/// </summary>
		[UsedImplicitly]
		public ImplicitUseTargetFlags TargetFlags { get; private set; }
	}

	/// <summary>
	///     Indicates that the marked method builds strings by format pattern and (optional) arguments. Applying this attribute to
	///     the method with the name of the string format parameter passed to the attribute's constructor allows tools like
	///     Resharper to highlight format parameters and warn about format string and parameter mismatches.
	/// </summary>
	/// <example>
	///     <code>
	/// 		[StringFormatMethod("message")]
	/// 		public void ShowError(string message, params object[] args)
	/// 		{
	/// 		  // ...
	/// 		}
	/// 		public void Foo()
	/// 		{
	/// 		  ShowError("Failed: {0}"); // Warning: Non-existing argument in format string
	/// 		}
	/// 	</code>
	/// </example>
	[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public sealed class StringFormatMethodAttribute : Attribute
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="formatParameterName">The name of the format string parameter.</param>
		public StringFormatMethodAttribute(string formatParameterName)
		{
			FormatParameterName = formatParameterName;
		}

		/// <summary>
		///     Gets the name of the format string parameter.
		/// </summary>
		[UsedImplicitly]
		public string FormatParameterName { get; private set; }
	}

	/// <summary>
	///     Indicates that a method does not make any observable state changes.
	/// </summary>
	/// <example>
	///     <code>
	/// 		[Pure]
	/// 		private int Multiply(int x, int y)
	/// 		{
	/// 		  return x*y;
	/// 		}
	/// 		
	/// 		public void Foo()
	/// 		{
	/// 		  const int a=2, b=2;
	/// 		  Multiply(a, b); // Waring: Return value of pure method is not used
	/// 		}
	///   </code>
	/// </example>
	[AttributeUsage(AttributeTargets.Method, Inherited = true)]
	public sealed class PureAttribute : Attribute
	{
	}

	/// <summary>
	///     Describes the contract of a method.
	/// </summary>
	/// <remarks>
	///     Syntax:
	///     FDT      ::= FDTRow [;FDTRow]*
	///     FDTRow   ::= Input =&gt; Output | Output &lt;= Input
	///     Input    ::= ParameterName: Value [, Input]*
	///     Output   ::= [ParameterName: Value]* {halt|stop|void|nothing|Value}
	///     Value    ::= true | false | null | notnull | canbenull
	///     If a method has single input parameter, it's name can be omitted. Using <c>halt</c> (or <c>void</c>/<c>nothing</c>,
	///     which are the same) for method output means that the methods doesn't return normally. The <c>canbenull</c> annotation
	///     is only applicable for output parameters. You can use multiple attributes for each FDT row, or
	///     use single attribute with rows separated by semicolon.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
	public sealed class ContractAnnotationAttribute : Attribute
	{
		public ContractAnnotationAttribute(string contract, bool forceFullStates = false)
		{
			Contract = contract;
			ForceFullStates = forceFullStates;
		}

		public string Contract { get; private set; }
		public bool ForceFullStates { get; private set; }
	}
}