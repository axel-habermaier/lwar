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

namespace Pegasus.Platform
{
	using System;

	/// <summary>
	///     Indicates that the marked method builds string by format pattern and (optional) arguments.
	///     Parameter, which contains format string, should be given in constructor.
	///     The format string should be in <see cref="string.Format(IFormatProvider,string,object[])" /> -like form
	/// </summary>
	/// <example>
	///     <code>
	/// [StringFormatMethod("message")]
	/// public void ShowError(string message, params object[] args)
	/// {
	///   //Do something
	/// }
	/// public void Foo()
	/// {
	///   ShowError("Failed: {0}"); // Warning: Non-existing argument in format string
	/// }
	/// </code>
	/// </example>
	[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public sealed class StringFormatMethodAttribute : Attribute
	{
		/// <summary>
		///     Initializes new instance of StringFormatMethodAttribute
		/// </summary>
		/// <param name="formatParameterName">Specifies which parameter of an annotated method should be treated as format-string</param>
		public StringFormatMethodAttribute(string formatParameterName)
		{
			FormatParameterName = formatParameterName;
		}

		/// <summary>
		///     Gets format parameter name
		/// </summary>
		[UsedImplicitly]
		public string FormatParameterName { get; private set; }
	}

	/// <summary>
	///     Indicates that the value of the marked element could be <c>null</c> sometimes,
	///     so the check for <c>null</c> is necessary before its usage.
	/// </summary>
	/// <example>
	///     <code>
	/// [CanBeNull]
	/// public object Test()
	/// {
	///   return null;
	/// }
	/// 
	/// public void UseTest()
	/// {
	///   var p = Test(); 
	///   var s = p.ToString(); // Warning: Possible 'System.NullReferenceException' 
	/// }
	/// </code>
	/// </example>
	[AttributeUsage(
		AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Delegate |
		AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public sealed class CanBeNullAttribute : Attribute
	{
	}

	/// <summary>
	///     Indicates that the value of the marked element could never be <c>null</c>
	/// </summary>
	/// <example>
	///     <code>
	/// [NotNull]
	/// public object Foo()
	/// {
	///   return null; // Warning: Possible 'null' assignment
	/// } 
	/// </code>
	/// </example>
	[AttributeUsage(
		AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Delegate |
		AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public sealed class NotNullAttribute : Attribute
	{
	}

	/// <summary>
	///     When applied to a target attribute, specifies a requirement for any type marked with
	///     the target attribute to implement or inherit specific type or types.
	/// </summary>
	/// <example>
	///     <code>
	/// [BaseTypeRequired(typeof(IComponent)] // Specify requirement
	/// public class ComponentAttribute : Attribute 
	/// {}
	/// 
	/// [Component] // ComponentAttribute requires implementing IComponent interface
	/// public class MyComponent : IComponent
	/// {}
	/// </code>
	/// </example>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	[BaseTypeRequired(typeof(Attribute))]
	public sealed class BaseTypeRequiredAttribute : Attribute
	{
		/// <summary>
		///     Initializes new instance of BaseTypeRequiredAttribute
		/// </summary>
		/// <param name="baseType">Specifies which types are required</param>
		public BaseTypeRequiredAttribute(Type baseType)
		{
			BaseTypes = new[] { baseType };
		}

		/// <summary>
		///     Gets enumerations of specified base types
		/// </summary>
		public Type[] BaseTypes { get; private set; }
	}

	/// <summary>
	///     Indicates that the marked symbol is used implicitly (e.g. via reflection, in external library),
	///     so this symbol will not be marked as unused (as well as by other usage inspections)
	/// </summary>
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	public sealed class UsedImplicitlyAttribute : Attribute
	{
		[UsedImplicitly]
		public UsedImplicitlyAttribute()
			: this(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.Default)
		{
		}

		[UsedImplicitly]
		public UsedImplicitlyAttribute(ImplicitUseKindFlags useKindFlags, ImplicitUseTargetFlags targetFlags)
		{
			UseKindFlags = useKindFlags;
			TargetFlags = targetFlags;
		}

		[UsedImplicitly]
		public UsedImplicitlyAttribute(ImplicitUseKindFlags useKindFlags)
			: this(useKindFlags, ImplicitUseTargetFlags.Default)
		{
		}

		[UsedImplicitly]
		public UsedImplicitlyAttribute(ImplicitUseTargetFlags targetFlags)
			: this(ImplicitUseKindFlags.Default, targetFlags)
		{
		}

		[UsedImplicitly]
		public ImplicitUseKindFlags UseKindFlags { get; private set; }

		/// <summary>
		///     Gets value indicating what is meant to be used
		/// </summary>
		[UsedImplicitly]
		public ImplicitUseTargetFlags TargetFlags { get; private set; }
	}

	/// <summary>
	///     Should be used on attributes and causes ReSharper
	///     to not mark symbols marked with such attributes as unused (as well as by other usage inspections)
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class MeansImplicitUseAttribute : Attribute
	{
		[UsedImplicitly]
		public MeansImplicitUseAttribute()
			: this(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.Default)
		{
		}

		[UsedImplicitly]
		public MeansImplicitUseAttribute(ImplicitUseKindFlags useKindFlags, ImplicitUseTargetFlags targetFlags)
		{
			UseKindFlags = useKindFlags;
			TargetFlags = targetFlags;
		}

		[UsedImplicitly]
		public MeansImplicitUseAttribute(ImplicitUseKindFlags useKindFlags)
			: this(useKindFlags, ImplicitUseTargetFlags.Default)
		{
		}

		[UsedImplicitly]
		public MeansImplicitUseAttribute(ImplicitUseTargetFlags targetFlags)
			: this(ImplicitUseKindFlags.Default, targetFlags)
		{
		}

		[UsedImplicitly]
		public ImplicitUseKindFlags UseKindFlags { get; private set; }

		/// <summary>
		///     Gets value indicating what is meant to be used
		/// </summary>
		[UsedImplicitly]
		public ImplicitUseTargetFlags TargetFlags { get; private set; }
	}

	[Flags]
	public enum ImplicitUseKindFlags
	{
		Default = Access | Assign | InstantiatedWithFixedConstructorSignature,

		/// <summary>
		///     Only entity marked with attribute considered used
		/// </summary>
		Access = 1,

		/// <summary>
		///     Indicates implicit assignment to a member
		/// </summary>
		Assign = 2,

		/// <summary>
		///     Indicates implicit instantiation of a type with fixed constructor signature.
		///     That means any unused constructor parameters won't be reported as such.
		/// </summary>
		InstantiatedWithFixedConstructorSignature = 4,

		/// <summary>
		///     Indicates implicit instantiation of a type
		/// </summary>
		InstantiatedNoFixedConstructorSignature = 8,
	}

	/// <summary>
	///     Specify what is considered used implicitly when marked with <see cref="MeansImplicitUseAttribute" /> or
	///     <see cref="UsedImplicitlyAttribute" />
	/// </summary>
	[Flags]
	public enum ImplicitUseTargetFlags
	{
		Default = Itself,

		Itself = 1,

		/// <summary>
		///     Members of entity marked with attribute are considered used
		/// </summary>
		Members = 2,

		/// <summary>
		///     Entity marked with attribute and all its members considered used
		/// </summary>
		WithMembers = Itself | Members
	}

	/// <summary>
	///     Indicates that a method does not make any observable state changes.
	///     The same as <see cref="System.Diagnostics.Contracts.PureAttribute" />
	/// </summary>
	/// <example>
	///     <code>
	///  [Pure]
	///  private int Multiply(int x, int y)
	///  {
	///    return x*y;
	///  }
	/// 
	///  public void Foo()
	///  {
	///    const int a=2, b=2;
	///    Multiply(a, b); // Waring: Return value of pure method is not used
	///  }
	///  </code>
	/// </example>
	[AttributeUsage(AttributeTargets.Method, Inherited = true)]
	public sealed class PureAttribute : Attribute
	{
	}
}