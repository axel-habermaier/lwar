namespace Tests.Scene
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using FluentAssertions;
	using NUnit.Framework;
	using Pegasus.Platform.Memory;
	using Pegasus.Scene;

	[TestFixture]
	public class SceneGraphTests
	{
		private class Node : IEnumerable<string>
		{
			private readonly List<Node> _children = new List<Node>();
			private readonly string _name;

			public Node(string name)
			{
				_name = name;
			}

			public IEnumerator<string> GetEnumerator()
			{
				throw new NotImplementedException();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			public void Add(Node node)
			{
				_children.Add(node);
			}

			public void ToSceneGraph(PoolAllocator allocator, SceneNode parent)
			{
				var children = _children.ToArray().Reverse();
				foreach (var child in children)
				{
					var node = TestNode.Create(allocator, child._name);
					node.AttachTo(parent);

					child.ToSceneGraph(allocator, node);
				}
			}
		}

		private class TestNode : SceneNode
		{
			private string _description;

			static TestNode()
			{
				ConstructorCache.Register(() => new TestNode());
			}

			public static TestNode Create(PoolAllocator allocator, string description)
			{
				var node = allocator.Allocate<TestNode>();
				node._description = description;
				return node;
			}

			public override string ToString()
			{
				return _description;
			}
		}

		private void CheckOrder(Node tree, string startNodeName, string[] nodes, Func<SceneGraph, SceneNode, List<SceneNode>> getList)
		{
			using (var allocator = new PoolAllocator())
			using (var sceneGraph = new SceneGraph(allocator))
			{
				tree.ToSceneGraph(allocator, sceneGraph.Root);

				var startNode = FindNode(sceneGraph.Root, startNodeName);
				var list = getList(sceneGraph, startNode);

				list.Should().HaveCount(nodes.Length);
				list.Select(n => n.ToString()).Should().ContainInOrder(nodes);
			}
		}

		private void CheckPreOrder(Node tree, string startNodeName, params string[] nodes)
		{
			CheckOrder(tree, startNodeName, nodes, (sceneGraph, startNode) =>
			{
				var list = new List<SceneNode>();
				foreach (var node in sceneGraph.EnumeratePreOrder(startNode))
					list.Add(node);
				return list;
			});
		}

		private void CheckPostOrder(Node tree, string startNodeName, params string[] nodes)
		{
			CheckOrder(tree, startNodeName, nodes, (sceneGraph, startNode) =>
			{
				var list = new List<SceneNode>();
				foreach (var node in sceneGraph.EnumeratePostOrder(startNode))
					list.Add(node);
				return list;
			});
		}

		private SceneNode FindNode(SceneNode node, string name)
		{
			if (node.ToString() == name)
				return node;

			foreach (var child in node.Children)
			{
				var result = FindNode(child, name);
				if (result != null)
					return result;
			}

			return null;
		}

		[Test]
		public void EnumerateEmptySceneGraph()
		{
			var tree = new Node("Root");
			CheckPreOrder(tree, "Root", "Root");
			CheckPostOrder(tree, "Root", "Root");
		}

		[Test]
		public void EnumerateSceneGraphWithMultipleSubTrees()
		{
			var tree = new Node("Root")
			{
				new Node("A")
				{
					new Node("B"),
					new Node("C")
					{
						new Node("D")
						{
							new Node("E")
						}
					}
				},
				new Node("F")
				{
					new Node("G")
					{
						new Node("H")
						{
							new Node("I"),
							new Node("J")
						}
					},
					new Node("K"),
					new Node("L")
				},
				new Node("M"),
				new Node("N"),
				new Node("O")
				{
					new Node("P")
					{
						new Node("Q"),
						new Node("R")
					}
				}
			};

			CheckPreOrder(tree, "Root", "Root", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R");
			CheckPreOrder(tree, "A", "A", "B", "C", "D", "E");
			CheckPreOrder(tree, "F", "F", "G", "H", "I", "J", "K", "L");
			CheckPreOrder(tree, "H", "H", "I", "J");
			CheckPreOrder(tree, "O", "O", "P", "Q", "R");
			CheckPreOrder(tree, "P", "P", "Q", "R");

			CheckPostOrder(tree, "Root", "B", "E", "D", "C", "A", "I", "J", "H", "G", "K", "L", "F", "M", "N", "Q", "R", "P", "O", "Root");
			CheckPostOrder(tree, "A", "B", "E", "D", "C", "A");
			CheckPostOrder(tree, "F", "I", "J", "H", "G", "K", "L", "F");
			CheckPostOrder(tree, "H", "I", "J", "H");
			CheckPostOrder(tree, "O", "Q", "R", "P", "O");
			CheckPostOrder(tree, "P", "Q", "R", "P");
		}

		[Test]
		public void EnumerateSceneGraphWithSingleSubNode()
		{
			var tree = new Node("Root")
			{
				new Node("A")
			};

			CheckPreOrder(tree, "Root", "Root", "A");
			CheckPreOrder(tree, "A", "A");

			CheckPostOrder(tree, "Root", "A", "Root");
			CheckPostOrder(tree, "A", "A");
		}

		[Test]
		public void EnumerateSceneGraphWithSingleSubTrees()
		{
			var tree = new Node("Root")
			{
				new Node("A")
				{
					new Node("B"),
					new Node("C")
				}
			};

			CheckPreOrder(tree, "Root", "Root", "A", "B", "C");
			CheckPreOrder(tree, "A", "A", "B", "C");
			CheckPreOrder(tree, "B", "B");
			CheckPreOrder(tree, "C", "C");

			CheckPostOrder(tree, "Root", "B", "C", "A", "Root");
			CheckPostOrder(tree, "A", "B", "C", "A");
			CheckPostOrder(tree, "B", "B");
			CheckPostOrder(tree, "C", "C");
		}

		[Test]
		public void EnumerateSceneGraphWithTwoSubNodes()
		{
			var tree = new Node("Root")
			{
				new Node("A"),
				new Node("B")
			};

			CheckPreOrder(tree, "Root", "Root", "A", "B");
			CheckPreOrder(tree, "A", "A");
			CheckPreOrder(tree, "B", "B");

			CheckPostOrder(tree, "Root", "A", "B", "Root");
			CheckPostOrder(tree, "A", "A");
			CheckPostOrder(tree, "B", "B");
		}

		[Test]
		public void EnumerateSceneGraphWithTwoSubTrees()
		{
			var tree = new Node("Root")
			{
				new Node("A")
				{
					new Node("B"),
					new Node("C")
				},
				new Node("D")
				{
					new Node("E")
				}
			};

			CheckPreOrder(tree, "Root", "Root", "A", "B", "C", "D", "E");
			CheckPreOrder(tree, "A", "A", "B", "C");
			CheckPreOrder(tree, "B", "B");
			CheckPreOrder(tree, "C", "C");
			CheckPreOrder(tree, "D", "D", "E");
			CheckPreOrder(tree, "E", "E");

			CheckPostOrder(tree, "Root", "B", "C", "A", "E", "D", "Root");
			CheckPostOrder(tree, "A", "B", "C", "A");
			CheckPostOrder(tree, "B", "B");
			CheckPostOrder(tree, "C", "C");
			CheckPostOrder(tree, "D", "E", "D");
			CheckPostOrder(tree, "E", "E");
		}
	}
}