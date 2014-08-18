namespace Pegasus.Framework.UserInterface
{
	using System;

	/// <summary>
	///     Handles a change notification for a resource dictionary.
	/// </summary>
	/// <param name="resourceDictionary">The resource dictionary that has been changed.</param>
	/// <param name="key">The key of the resource that has been changed.</param>
	internal delegate void ResourceKeyChangedHandler(ResourceDictionary resourceDictionary, object key);
}