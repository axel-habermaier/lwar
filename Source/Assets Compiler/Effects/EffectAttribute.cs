using System;

namespace Pegasus.AssetsCompiler.Effects
{
	using Framework;

	/// <summary>
	///   Must be applied to classes that contain methods that should be cross-compiled to GPU shader code.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	[MeansImplicitUse(ImplicitUseKindFlags.Assign | ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.WithMembers)]
	[BaseTypeRequired(typeof(Effect))]
	public class EffectAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	[MeansImplicitUse(ImplicitUseKindFlags.Assign | ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.WithMembers)]
	[BaseTypeRequired(typeof(Effect))]
	public class Expose : Attribute
	{
		public Expose(string name)
		{
			
		}

		public string VertexShader { get; set; }
		public string FragmentShader { get; set; }
	}
}