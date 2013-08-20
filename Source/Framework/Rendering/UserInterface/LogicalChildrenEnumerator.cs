using System;

namespace Pegasus.Framework.Rendering.UserInterface
{
	/// <summary>
	///   Enumerates the logical children of an UI element.
	/// </summary>
	public struct LogicalChildrenEnumerator
	{
		/// <summary>
		///   Creates an enumerator that does not enumerate any logical children.
		/// </summary>
		public static readonly LogicalChildrenEnumerator Empty = new LogicalChildrenEnumerator();

		/// <summary>
		///   The single logical child that is enumerated.
		/// </summary>
		private UIElement _singleChild;

		/// <summary>
		///   Gets the logical child of the UI element at the current position of the enumerator.
		/// </summary>
		public UIElement Current { get; private set; }

		/// <summary>
		///   Creates an enumerator for a single logical child.
		/// </summary>
		/// <param name="child">The logical child of the UI element that should be enumerated.</param>
		public static LogicalChildrenEnumerator SingleChild(UIElement child)
		{
			Assert.ArgumentNotNull(child);
			return new LogicalChildrenEnumerator { _singleChild = child };
		}

		/// <summary>
		///   Advances the enumerator to the next logical child of the UI element.
		/// </summary>
		public bool MoveNext()
		{
			if (_singleChild != null)
			{
				Current = _singleChild;
				_singleChild = null;

				return true;
			}

			return false;
		}

		/// <summary>
		///   Gets the enumerator that can be used with C#'s foreach loops, for instance.
		/// </summary>
		/// <remarks>
		///   This method just returns the enumerator object. It is only required to enable foreach support.
		/// </remarks>
		public LogicalChildrenEnumerator GetEnumerator()
		{
			return this;
		}
	}
}