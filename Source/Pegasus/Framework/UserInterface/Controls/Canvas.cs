using System;

namespace Pegasus.Framework.UserInterface.Controls
{
	using Math;

	/// <summary>
	///   Represents a layout where all child elements can position themselves with coordinates relative to the canvas.
	/// </summary>
	public class Canvas : Panel
	{
		/// <summary>
		///   The distance between the left side of an UI element and and the left side of its parent Canvas.
		/// </summary>
		public static readonly DependencyProperty<double> LeftProperty =
			new DependencyProperty<double>(defaultValue: Double.NaN, affectsArrange: true, validationCallback: ValidatePosition);

		/// <summary>
		///   The distance between the right side of an UI element and the right side of its parent Canvas.
		///   If the Left property is also set, the Right value is ignored.
		/// </summary>
		public static readonly DependencyProperty<double> RightProperty =
			new DependencyProperty<double>(defaultValue: Double.NaN, affectsArrange: true, validationCallback: ValidatePosition);

		/// <summary>
		///   The distance between the top of an UI element and the top of its parent Canvas.
		/// </summary>
		public static readonly DependencyProperty<double> TopProperty =
			new DependencyProperty<double>(defaultValue: Double.NaN, affectsArrange: true, validationCallback: ValidatePosition);

		/// <summary>
		///   The distance between the bottom of an UI element and the bottom of its parent Canvas.
		///   If the Top property is also set, the Bottom value is ignored.
		/// </summary>
		public static readonly DependencyProperty<double> BottomProperty =
			new DependencyProperty<double>(defaultValue: Double.NaN, affectsArrange: true, validationCallback: ValidatePosition);

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

		/// <summary>
		///   Checks whether the given value is a valid canvas position.
		/// </summary>
		/// <param name="value">The value that should be validated.</param>
		private static bool ValidatePosition(double value)
		{
			// NaN as well as all positive/negative values are supported except for infinity
			return !Double.IsInfinity(value);
		}

		/// <summary>
		///   Computes and returns the desired size of the element given the available space allocated by the parent UI element.
		/// </summary>
		/// <param name="constraint">
		///   The available space that the parent UI element can allocate to this UI element. Can be infinity if the parent wants
		///   to size itself to its contents. The computed desired size is allowed to exceed the available space; the parent UI
		///   element might be able to use scrolling in this case.
		/// </param>
		protected override SizeD MeasureCore(SizeD constraint)
		{
			// The canvas does not restrict the size its children
			var availableSize = new SizeD(Double.PositiveInfinity, Double.PositiveInfinity);

			foreach (var child in Children)
				child.Measure(availableSize);

			// The canvas itself consumes no space
			return new SizeD();
		}

		/// <summary>
		///   Determines the size of the UI element and positions all of its children. Returns the actual size used by the UI
		///   element. If this value is smaller than the given size, the UI element's alignment properties position it
		///   appropriately.
		/// </summary>
		/// <param name="finalSize">
		///   The final area allocated by the UI element's parent that the UI element should use to arrange
		///   itself and its children.
		/// </param>
		protected override SizeD ArrangeCore(SizeD finalSize)
		{
			// Arrange all children by checking the values of their Left, Right, Top, and Bottom properties
			foreach (var child in Children)
			{
				var x = 0.0;
				var y = 0.0;

				// Check if Left is set to a valid value (and if so, ignore Right)
				var left = child.GetValue(LeftProperty);
				if (!Double.IsNaN(left))
					x = left;
				else
				{
					// Else, check Right; if Right is also invalid, just position the element to the left of the canvas
					var right = child.GetValue(RightProperty);
					if (!Double.IsNaN(right))
						x = finalSize.Width - child.DesiredSize.Width - right;
				}

				// Check if Top is set to a valid value (and if so, ignore Bottom)
				var top = child.GetValue(TopProperty);
				if (!Double.IsNaN(top))
					y = top;
				else
				{
					// Else, check Bottom; if Bottom is also invalid, just position the element to the top of the canvas
					var bottom = child.GetValue(BottomProperty);
					if (!Double.IsNaN(bottom))
						y = finalSize.Height - child.DesiredSize.Height - bottom;
				}

				child.Arrange(new RectangleD(x, y, finalSize));
			}

			return finalSize;
		}
	}
}