namespace Tests.UserInterface
{
	using System;
	using Pegasus.Framework.UserInterface;

	/// <summary>
	///     The control that is used for UI-related unit tests. The view model is recursive.
	/// </summary>
	public class TestViewModel : ViewModel
	{
		private int _integer;
		private TestViewModel _model;
		private Thickness _thickness;
		private double _width;

		public Thickness Thickness
		{
			get { return _thickness; }
			set { ChangePropertyValue(ref _thickness, value); }
		}

		public double Width
		{
			get { return _width; }
			set { ChangePropertyValue(ref _width, value); }
		}

		public TestViewModel Model
		{
			get { return _model; }
			set { ChangePropertyValue(ref _model, value); }
		}

		public int Integer
		{
			get { return _integer; }
			set { ChangePropertyValue(ref _integer, value); }
		}

		/// <summary>
		///     Initializes the recursive Model property for the given number of levels.
		/// </summary>
		/// <param name="levels">
		///     The number of levels that should be initialized. This is the number of times the Model property
		///     can be called, starting with the current instance.
		/// </param>
		public void InitializeRecursively(int levels)
		{
			if (levels <= 0)
				return;

			Model = new TestViewModel();
			Model.InitializeRecursively(levels - 1);
		}
	}
}