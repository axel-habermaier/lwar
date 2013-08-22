using System;

namespace Tests.Parsing
{
	using FluentAssertions;
	using NUnit.Framework;
	using Pegasus.Scripting.Parsing;

	[TestFixture]
	public class InputStreamTests
	{
		private void CheckState(InputStream stream, int position, int line, int lineBegin, int column)
		{
			stream.State.Position.Should().Be(position);
			stream.State.Line.Should().Be(line);
			stream.State.LineBegin.Should().Be(lineBegin);
			stream.State.Column.Should().Be(column);
		}

		[Test]
		public void Constructor()
		{
			var input = "input";
			var name = "name";

			var stream = new InputStream(input, name);

			stream.EndOfInput.Should().BeFalse();
			stream.Name.Should().Be(name);
			CheckState(stream, 0, 1, 0, 1);
		}

		[Test]
		public void EndOfInput_AtEnd()
		{
			var stream = new InputStream("t");
			stream.Skip(1);

			stream.EndOfInput.Should().BeTrue();
		}

		[Test]
		public void EndOfInput_Empty()
		{
			var stream = new InputStream("");
			stream.EndOfInput.Should().BeTrue();
		}

		[Test]
		public void EndOfInput_NotAtEnd()
		{
			var stream = new InputStream("t");
			stream.EndOfInput.Should().BeFalse();
		}

		[Test]
		public void Peek_First()
		{
			var stream = new InputStream("123");
			stream.Peek().Should().Be('1');
		}

		[Test]
		public void Peek_Second()
		{
			var stream = new InputStream("123");
			stream.Skip(1);
			stream.Peek().Should().Be('2');
		}

		[Test]
		public void Peek_Last()
		{
			var stream = new InputStream("123");
			stream.Skip(2);
			stream.Peek().Should().Be('3');
		}

		[Test]
		public void Peek_EndOfInput()
		{
			var stream = new InputStream("");
			stream.Peek().Should().Be(Char.MaxValue);
		}

		[Test]
		public void Substring()
		{
			var stream = new InputStream("Test");
			var substring = stream.Substring(1, 2);
			substring.Should().Be("es");
		}

		[Test]
		public void Skip_One()
		{
			var stream = new InputStream("Test");
			stream.Skip(1);
			CheckState(stream, 1, 1, 0, 2);
		}

		[Test]
		public void Skip_Two()
		{
			var stream = new InputStream("Test");
			stream.Skip(2);
			CheckState(stream, 2, 1, 0, 3);
		}

		[Test]
		public void Skip_Four_OneNewline()
		{
			var stream = new InputStream("Te\nst");
			stream.Skip(4);
			CheckState(stream, 4, 2, 3, 2);
		}

		[Test]
		public void Skip_Four_TwoNewlines()
		{
			var stream = new InputStream("Te\n\nst");
			stream.Skip(4);
			CheckState(stream, 4, 3, 4, 1);
		}

		[Test]
		public void SkipWhiteSpaces_None()
		{
			var stream = new InputStream("abc");
			stream.SkipWhiteSpaces();
			stream.State.Position.Should().Be(0);
		}

		[Test]
		public void SkipWhiteSpaces_OneFollowedByCharacter()
		{
			var stream = new InputStream(" abc");
			stream.SkipWhiteSpaces();
			stream.State.Position.Should().Be(1);
		}

		[Test]
		public void SkipWhiteSpaces_TwoFollowedByCharacter()
		{
			var stream = new InputStream("\n abc");
			stream.SkipWhiteSpaces();
			stream.State.Position.Should().Be(2);
		}

		[Test]
		public void SkipWhiteSpaces_OneFollowedByEndOfInput()
		{
			var stream = new InputStream(" ");
			stream.SkipWhiteSpaces();
			stream.State.Position.Should().Be(1);
		}

		[Test]
		public void SkipWhiteSpaces_TwoFollowedByEndOfInput()
		{
			var stream = new InputStream("\n ");
			stream.SkipWhiteSpaces();
			stream.State.Position.Should().Be(2);
		}
	}
}