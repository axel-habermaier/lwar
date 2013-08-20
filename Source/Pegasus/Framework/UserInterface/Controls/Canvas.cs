using System;

namespace Pegasus.Framework.UserInterface.Controls
{
	/// <summary>
	///   Represents a layout where all child elements can position themselves with coordinates relative to the canvas.
	/// </summary>
	public class Canvas : Layout
	{
		/// <summary>
		///   The distance between the left side of an UI element and its parent Canvas.
		/// </summary>
		public static readonly DependencyProperty<double> LeftProperty = new DependencyProperty<double>(affectsArrange: true);

		/// <summary>
		///   The distance between the right side of an UI element and its parent Canvas. If the Left property is also set, the
		///   Right value is ignored.
		/// </summary>
		public static readonly DependencyProperty<double> RightProperty = new DependencyProperty<double>(affectsArrange: true);

		/// <summary>
		///   The distance between the top of an UI element and its parent Canvas.
		/// </summary>
		public static readonly DependencyProperty<double> TopProperty = new DependencyProperty<double>(affectsArrange: true);

		/// <summary>
		///   The distance between the bottom of an UI element and its parent Canvas. If the Top property is also set, the Bottom
		///   value is ignored.
		/// </summary>
		public static readonly DependencyProperty<double> BottomProperty = new DependencyProperty<double>(affectsArrange: true);

		/// <summary>
		///   Gets or sets the distance between the left side of an UI element and its parent Canvas.
		/// </summary>
		public double Left
		{
			get { return GetValue(LeftProperty); }
			set { SetValue(LeftProperty, value); }
		}

		/// <summary>
		///   Gets or sets the distance between the right side of an UI element and its parent Canvas. If the Left property is also
		///   set, the Right value is ignored.
		/// </summary>
		public double Right
		{
			get { return GetValue(RightProperty); }
			set { SetValue(RightProperty, value); }
		}

		/// <summary>
		///   Gets or sets the distance between the top of an UI element and its parent Canvas.
		/// </summary>
		public double Top
		{
			get { return GetValue(TopProperty); }
			set { SetValue(TopProperty, value); }
		}

		/// <summary>
		///   Gets or sets the distance between the bottom of an UI element and its parent Canvas. If the Top property is also set,
		///   the Bottom value is ignored.
		/// </summary>
		public double Bottom
		{
			get { return GetValue(BottomProperty); }
			set { SetValue(BottomProperty, value); }
		}
	}
}