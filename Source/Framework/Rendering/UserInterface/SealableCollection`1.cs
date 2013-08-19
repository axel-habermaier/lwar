using System;

namespace Pegasus.Framework.Rendering.UserInterface
{
	using System.Collections.ObjectModel;
	using System.Diagnostics;

	/// <summary>
	///   Represents a list of objects that can no longer be changed once it has been sealed.
	/// </summary>
	/// <typeparam name="T">The type of the objects contained in the collection.</typeparam>
	public class SealableCollection<T> : Collection<T>
	{
		/// <summary>
		///   Gets a value indicating whether the collection is sealed and can no longer be modified.
		/// </summary>
		public bool IsSealed { get; private set; }

		/// <summary>
		///   Seals the collection such that it can no longer be modified.
		/// </summary>
		internal void Seal()
		{
			IsSealed = true;
		}

		/// <summary>
		///   In debug builds, checks whether the collection has already been sealed.
		/// </summary>
		[Conditional("DEBUG")]
		private void CheckSealed()
		{
			Assert.That(!IsSealed, "The collection has already been sealed and can no longer be modified.");
		}

		/// <summary>
		///   Removes all elements from the collection.
		/// </summary>
		protected override void ClearItems()
		{
			CheckSealed();
			base.ClearItems();
		}

		/// <summary>
		///   Inserts an element into the collection at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which the item that should be inserted.</param>
		/// <param name="item">The item that should be inserted.</param>
		protected override void InsertItem(int index, T item)
		{
			CheckSealed();
			base.InsertItem(index, item);
		}

		/// <summary>
		///   Removes the element at the specified index of the collection.
		/// </summary>
		/// <param name="index">The zero-based index of the element that should be removed.</param>
		protected override void RemoveItem(int index)
		{
			CheckSealed();
			base.RemoveItem(index);
		}

		/// <summary>
		///   Replaces the element at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the element that should be replaced.</param>
		/// <param name="item">The new value for the element at the specified index.</param>
		protected override void SetItem(int index, T item)
		{
			CheckSealed();
			base.SetItem(index, item);
		}
	}
}