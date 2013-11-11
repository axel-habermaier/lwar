namespace Pegasus.AssetsCompiler.Assets
{
	using System;
	using System.Linq;
	using Platform.Graphics;

	/// <summary>
	///   Represents a shader that requires compilation.
	/// </summary>
	public class ShaderAsset : Asset
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="effect">The name of the effect the shader belongs to.</param>
		/// <param name="name">The name of the shader method.</param>
		/// <param name="type">The type of the shader.</param>
		public ShaderAsset(string effect, string name, ShaderType type)
			: base(GetPath(effect, name, type), Configuration.TempDirectory)
		{
			Type = type;
		}

		/// <summary>
		///   Gets the type of the shader.
		/// </summary>
		public ShaderType Type { get; private set; }

		/// <summary>
		///   Gets the shader asset path for the given effect name, shader method name, and shader type.
		/// </summary>
		/// <param name="effect">The name of the effect the shader belongs to.</param>
		/// <param name="name">The name of the shader method.</param>
		/// <param name="type">The type of the shader.</param>
		public static string GetPath(string effect, string name, ShaderType type)
		{
			Assert.ArgumentNotNullOrWhitespace(effect);
			Assert.ArgumentNotNullOrWhitespace(name);
			Assert.ArgumentInRange(type);

			var lastNamespaceEnd = effect.IndexOf('.', Configuration.AssetsProject.RootNamespace.Length + 1);
			effect = effect.Substring(lastNamespaceEnd + 1);

			if (effect.StartsWith("Internal."))
				effect = effect.Substring("Internal.".Length);
			else
				effect = effect.Replace(".Internal.", "");

			var extension = type.ToString().Where(Char.IsUpper).Aggregate(String.Empty, (s, c) => s + c).ToLower();
			return String.Format("Effects/{0}.{1}.{2}", effect, name, extension);
		}
	}
}