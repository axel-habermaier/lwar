namespace Pegasus.AssetsCompiler.Xaml
{
	using System;

	/// <summary>
	///     Stores the parsed information of a Xaml data binding.
	/// </summary>
	internal struct XamlDataBinding
	{
		/// <summary>
		///     Gets or sets the property path of the data binding.
		/// </summary>
		public string Path { get; set; }

		/// <summary>
		///     Gets or sets the name of the value converter that is used by the binding.
		/// </summary>
		public string Converter { get; set; }

		/// <summary>
		///     Gets or sets the binding mode of the data binding.
		/// </summary>
		public string BindingMode { get; set; }
	}
}