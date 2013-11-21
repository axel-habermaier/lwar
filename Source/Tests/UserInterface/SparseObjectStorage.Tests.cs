namespace Tests.UserInterface
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using FluentAssertions;
	using NUnit.Framework;
	using Pegasus.Framework;

	[TestFixture]
	public class SparseObjectStorageTests
	{
		private class Test : SparseObjectStorage<Test>.IStorageLocation
		{
			public Test(int location)
			{
				Location = location;
			}

			public int Location { get; private set; }
		}

		private static void TestStorage(int count)
		{
			var storage = new SparseObjectStorage<Test>();
			var random = new Random();
			var objects = new Test[count];

			var addedLocations = new HashSet<int>();
			var unknownLocations = new HashSet<int>();

			do
			{
				addedLocations.Add(random.Next());
			} while (addedLocations.Count < count);

			do
			{
				var location = random.Next();
				if (!addedLocations.Contains(location))
					unknownLocations.Add(location);
			} while (unknownLocations.Count < count);

			for (var i = 0; i < count; ++i)
			{
				var location = addedLocations.First();
				addedLocations.Remove(location);

				objects[i] = new Test(location);
				storage.Add(objects[i]);
			}

			foreach (var obj in objects)
				storage.Get(obj.Location).Should().Be(obj);

			foreach (var location in unknownLocations)
				storage.Get(location).Should().BeNull();
		}

		[Test]
		public void TestStorage()
		{
			for (var i = 0; i < 100; ++i)
				TestStorage(i);
		}
	}
}