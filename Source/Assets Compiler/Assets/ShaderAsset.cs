using System;

namespace Pegasus.AssetsCompiler.Assets
{
	using System.Linq;
	using Framework;
	using Framework.Platform.Graphics;

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
			Effect = effect;
			Name = name;
			Type = type;
		}

		/// <summary>
		///   Gets the type of the shader.
		/// </summary>
		public ShaderType Type { get; private set; }

		/// <summary>
		///   Gets the name of the effect the shader belongs to.
		/// </summary>
		public string Effect { get; private set; }

		/// <summary>
		///   Gets the name of the shader method.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		///   Gets the shader asset path for the given effect name, shader method name, and shader type.
		/// </summary>
		/// <param name="effect">The name of the effect the shader belongs to.</param>
		/// <param name="name">The name of the shader method.</param>
		/// <param name="type">The type of the shader.</param>
		public static string GetPath(string effect, string name, ShaderType type)
		{
			Assert.ArgumentNotNullOrWhitespace(effect, () => effect);
			Assert.ArgumentNotNullOrWhitespace(name, () => name);
			Assert.ArgumentInRange(type, () => type);

			var extension = type.ToString().Where(Char.IsUpper).Aggregate(String.Empty, (s, c) => s + c).ToLower();
			return String.Format("Effects/{0}.{1}.{2}", effect, name, extension);
		}
	}
}