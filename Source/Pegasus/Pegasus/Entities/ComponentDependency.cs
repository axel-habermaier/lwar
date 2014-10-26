namespace Pegasus.Entities
{
	using System;

	/// <summary>
	///     Determines the kind of dependency a behavior has on a component.
	/// </summary>
	public enum ComponentDependency
	{
		/// <summary>
		///     Indicates that the component is required by the behavior. If the component is missing, the entity is
		///     not added to the behavior.
		/// </summary>
		Default = 0,

		/// <summary>
		///     Indicates that the component is not required by the behavior. Missing components are represented using null values.
		/// </summary>
		Optional,

		/// <summary>
		///     Indicates that the component is required by the behavior. If the component is missing, an error occurs.
		/// </summary>
		Required
	}
}