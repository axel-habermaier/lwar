namespace Pegasus.UserInterface
{
	using System;
	using Utilities;

	/// <summary>
	///     Represents a list of objects that can no longer be changed once it has been sealed.
	/// </summary>
	/// <typeparam name="T">The type of the objects contained in the collection.</typeparam>
	public sealed class SealableCollection<T> : CustomCollection<T>, ISealable
		where T : ISealable
	{
		/// <summary>
		///     Represents an empty sealable collection that cannot be modified.
		/// </summary>
		public static readonly SealableCollection<T> Empty = new SealableCollection<T> { IsSealed = true };

		/// <summary>
		///     Gets a value indicating whether the collection is sealed and can no longer be modified.
		/// </summary>
		public bool IsSealed { get; private set; }

		/// <summary>
		///     Seals the collection such that it can no longer be modified.
		/// </summary>
		public void Seal()
		{
			IsSealed = true;

			foreach (var item in this)
				item.Seal();
		}

		/// <summary>
		///     Removes all elements from the collection.
		/// </summary>
		protected override void ClearItems()
		{
			Assert.NotSealed(this);
			base.ClearItems();
		}

		/// <summary>
		///     Inserts an element into the collection at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which the item that should be inserted.</param>
		/// <param name="item">The item that should be inserted.</param>
		protected override void InsertItem(int index, T item)
		{
			Assert.NotSealed(this);
			base.InsertItem(index, item);
		}

		/// <summary>
		///     Removes the element at the specified index of the collection.
		/// </summary>
		/// <param name="index">The zero-based index of the element that should be removed.</param>
		protected override void RemoveItem(int index)
		{
			Assert.NotSealed(this);
			base.RemoveItem(index);
		}

		/// <summary>
		///     Replaces the element at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the element that should be replaced.</param>
		/// <param name="item">The new value for the element at the specified index.</param>
		protected override void SetItem(int index, T item)
		{
			Assert.NotSealed(this);
			base.SetItem(index, item);
		}
	}
}