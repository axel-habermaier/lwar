namespace Pegasus.AssetsCompiler.Assets
{
	using System;
	using System.IO;
	using System.Linq;
	using Platform.Graphics;

	/// <summary>
	///     Represents a shader that requires compilation.
	/// </summary>
	public abstract class ShaderAsset : Asset
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="effect">The name of the effect the shader belongs to.</param>
		/// <param name="name">The name of the shader method.</param>
		/// <param name="type">The type of the shader.</param>
		protected ShaderAsset(string effect, string name, ShaderType type)
			: base(GetPath(effect, name, type), Configuration.TempDirectory)
		{
			Type = type;
		}

		/// <summary>
		///     Gets the type of the shader.
		/// </summary>
		public ShaderType Type { get; private set; }

		/// <summary>
		///     Gets the name that should be used for the asset identifier. If null is returned, no asset identifier is generated
		///     for this asset instance.
		/// </summary>
		public override string IdentifierName
		{
			get { return Path.GetFileNameWithoutExtension(FileNameWithoutExtension.Replace(".", "/")); }
		}

		/// <summary>
		///     Gets the name of the asset identifier.
		/// </summary>
		public static string GetAssetIdentifier(string relativeDirectory, string fileName)
		{
			return Path.Combine(relativeDirectory.Replace("Effects", ""), "Shaders", fileName.Replace(".", "/")).Replace("\\", "/");
		}

		/// <summary>
		///     Gets the shader asset path for the given effect name, shader method name, and shader type.
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