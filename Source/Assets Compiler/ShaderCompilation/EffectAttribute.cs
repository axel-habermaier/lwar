using System;

namespace Pegasus.AssetsCompiler.ShaderCompilation
{
	using Framework;

	/// <summary>
	///   Must be applied to classes that contain methods that should be cross-compiled to GPU shader code.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	[MeansImplicitUse(ImplicitUseKindFlags.Assign | ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.WithMembers)]
	[BaseTypeRequired(typeof(Effect))]
	public class EffectAttribute : Attribute
	{
	}
}