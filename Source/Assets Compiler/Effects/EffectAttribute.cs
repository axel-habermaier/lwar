using System;

namespace Pegasus.AssetsCompiler.Effects
{
	using Framework;
	using Framework.Platform;

	/// <summary>
	///   Must be applied to classes that contain methods that should be cross-compiled to GPU shader code.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	[MeansImplicitUse(ImplicitUseKindFlags.Assign | ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.WithMembers)]
	[BaseTypeRequired(typeof(Effect))]
	public class EffectAttribute : Attribute
	{
	}
}