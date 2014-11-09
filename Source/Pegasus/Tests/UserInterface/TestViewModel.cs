namespace Tests.UserInterface
{
	using System;
	using Pegasus.UserInterface;

	/// <summary>
	///     The control that is used for UI-related unit tests. The view model is recursive.
	/// </summary>
	public class TestViewModel : NotifyPropertyChanged
	{
		private bool _bool;
		private double _double;
		private int _integer;
		private TestViewModel _model;
		private object _object;
		private string _string;
		private Thickness _thickness;
		private float _width;

		public Thickness Thickness
		{
			get { return _thickness; }
			set { ChangePropertyValue(ref _thickness, value); }
		}

		public float Width
		{
			get { return _width; }
			set { ChangePropertyValue(ref _width, value); }
		}

		public TestViewModel Model
		{
			get { return _model; }
			set { ChangePropertyValue(ref _model, value); }
		}

		public object Object
		{
			get { return _object; }
			set { ChangePropertyValue(ref _object, value); }
		}

		public double Double
		{
			get { return _double; }
			set { ChangePropertyValue(ref _double, value); }
		}

		public int Integer
		{
			get { return _integer; }
			set { ChangePropertyValue(ref _integer, value); }
		}

		public string String
		{
			get { return _string; }
			set { ChangePropertyValue(ref _string, value); }
		}

		public bool Bool
		{
			get { return _bool; }
			set { ChangePropertyValue(ref _bool, value); }
		}

		public object ObjectNoSetter
		{
			get { return _object; }
		}

		public object ObjectNoGetter
		{
			set { ChangePropertyValue(ref _object, value); }
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