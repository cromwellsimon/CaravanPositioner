using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileGridLibrary.Pathfinding.BreadthFirst;

public class Node(Tile tile, Node? previousNode) : IEnumerable<Node>
{
	public Tile Tile { get; init; } = tile;
	public Node? PreviousNode { get; init; } = previousNode;

	public IEnumerator<Node> GetEnumerator()
	{
		Node? node = PreviousNode;
		while (node != null)
		{
			yield return node;
			node = node.PreviousNode;
		}
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public static class NodeStatics
{
	public static List<Tile> GetPath(this Tile inStartingTile, Tile inEndingTile, Func<Tile, bool>? predicate = null)
	{
		predicate ??= ((_) => true);
		HashSet<Node> searchedTiles = new();
		Queue<Node> unsearchedTiles = new();
		unsearchedTiles.Enqueue(new(inStartingTile, null));

		while (unsearchedTiles.Count > 0)
		{
			Node searchedTile = unsearchedTiles.Dequeue();
			searchedTiles.Add(searchedTile);
			foreach (Tile tile in searchedTile.Tile.Circle())
			{
				if (searchedTiles.Any((node) => node.Tile == tile) == false && unsearchedTiles.Any((node) => node.Tile == tile) == false)
				{
					Node newNode = new(tile, searchedTile);
					if (tile == inEndingTile)
					{
						IEnumerable<Tile> finalPath = [tile];
						finalPath = finalPath.Concat(newNode.Select((node) => node.Tile));
						return finalPath.Reverse().ToList();
					}
					if (predicate.Invoke(tile))
					{
						unsearchedTiles.Enqueue(newNode);
					}
					else
					{
						searchedTiles.Add(newNode);
					}
				}
			}
		}

		return new();
	}
}