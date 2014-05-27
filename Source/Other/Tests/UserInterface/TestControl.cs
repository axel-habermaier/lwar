namespace Tests.UserInterface
{
	using System;
	using Pegasus.Framework.UserInterface;
	using Pegasus.Framework.UserInterface.Controls;

	/// <summary>
	///     The control that is used for UI-related unit tests.
	/// </summary>
	public class TestControl : UserControl
	{
		public static readonly DependencyProperty<bool> BooleanTestProperty1 =
			new DependencyProperty<bool>(defaultBindingMode: BindingMode.OneWayToSource);

		public static readonly DependencyProperty<object> ObjectTestProperty = new DependencyProperty<object>();
		public static readonly DependencyProperty<bool> BooleanTestProperty2 = new DependencyProperty<bool>();
		public static readonly DependencyProperty<int> IntegerTestProperty1 = new DependencyProperty<int>();
		public static readonly DependencyProperty<int> IntegerTestProperty2 = new DependencyProperty<int>();
		public static readonly DependencyProperty<string> StringTestProperty = new DependencyProperty<string>();
		public static readonly DependencyProperty<string> DefaultStringTestProperty = new DependencyProperty<string>("ABCD");

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public TestControl()
		{
			IsAttachedToRoot = true;

			Button1 = new Button();
			Button2 = new Button();
			Button3 = new Button();

			Canvas1 = new Canvas();
			Canvas2 = new Canvas();

			Canvas1.Children.Add(Button1);
			Canvas1.Children.Add(Button2);
			Canvas1.Children.Add(Canvas2);

			Canvas2.Children.Add(Button3);

			Content = Canvas1;
		}

		public object ObjectTest
		{
			get { return GetValue(ObjectTestProperty); }
			set { SetValue(ObjectTestProperty, value); }
		}

		public bool BooleanTest1
		{
			get { return GetValue(BooleanTestProperty1); }
			set { SetValue(BooleanTestProperty1, value); }
		}

		public bool BooleanTest2
		{
			get { return GetValue(BooleanTestProperty2); }
			set { SetValue(BooleanTestProperty2, value); }
		}

		public int IntegerTest1
		{
			get { return GetValue(IntegerTestProperty1); }
			set { SetValue(IntegerTestProperty1, value); }
		}

		public int IntegerTest2
		{
			get { return GetValue(IntegerTestProperty2); }
			set { SetValue(IntegerTestProperty2, value); }
		}

		public string StringTest
		{
			get { return GetValue(StringTestProperty); }
			set { SetValue(StringTestProperty, value); }
		}

		public string DefaultStringTest
		{
			get { return GetValue(DefaultStringTestProperty); }
			set { SetValue(DefaultStringTestProperty, value); }
		}

		/// <summary>
		///     Gets the canvas that is the direct child of the control.
		/// </summary>
		public Canvas Canvas1 { get; private set; }

		/// <summary>
		///     Gets the canvas that is the third child of Canvas1.
		/// </summary>
		public Canvas Canvas2 { get; private set; }

		/// <summary>
		///     Gets the first button of the control that is the first child of Canvas1.
		/// </summary>
		public Button Button1 { get; private set; }

		/// <summary>
		///     Gets the second button of the control that is the second child of Canvas1.
		/// </summary>
		public Button Button2 { get; private set; }

		/// <summary>
		///     Gets the third button of the control that is the first child of Canvas2.
		/// </summary>
		public Button Button3 { get; private set; }
	}
}