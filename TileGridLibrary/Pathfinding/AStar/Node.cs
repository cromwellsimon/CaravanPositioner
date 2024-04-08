using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileGridLibrary.Pathfinding.AStar;

/// <remarks>
/// Functionally, this works identically to <see cref="BreadthFirst.Node"/> except there is weighting applied to the search
/// </remarks>
public class Node : IEnumerable<Node>
{
    public Node? PreviousNode { get; set; }
    public Tile Tile { get; init; } = default!;

	public int DistanceFromStart { get; private set; }
	public int DistanceFromEnd { get; private set; }
	public int SumOfDistance => DistanceFromStart + DistanceFromEnd;

    public Node(Tile inStartTile, Tile inTile, Tile inEndTile, Node? inPreviousNode)
    {
        Tile = inTile;
		DistanceFromStart = Tile.DistanceTo(inStartTile);
		DistanceFromEnd = Tile.DistanceTo(inEndTile);
		PreviousNode = inPreviousNode;
    }

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

public static class AStarStatics
{
	public static List<Tile>? GetPath(this Tile inStartingTile, Tile inEndingTile, Func<Tile, bool>? predicate = null)
	{
		predicate ??= ((_) => true);
		HashSet<Node> searchedTiles = new();
		List<Node> unsearchedTiles = [new(inStartingTile, inStartingTile, inEndingTile, null)];

		while (unsearchedTiles.Count > 0)
		{
			Node searchedTile = unsearchedTiles.MinBy((node) => node.SumOfDistance)!;
			unsearchedTiles.Remove(searchedTile);
			searchedTiles.Add(searchedTile);
			foreach (Tile tile in searchedTile.Tile.Circle())
			{
				if (searchedTiles.Any((node) => node.Tile == tile) == false && unsearchedTiles.Any((node) => node.Tile == tile) == false)
				{
					Node newNode = new(inStartingTile, tile, inEndingTile, searchedTile);
					if (tile == inEndingTile)
					{
						IEnumerable<Tile> finalPath = [tile];
						finalPath = finalPath.Concat(newNode.Select((node) => node.Tile));
						return finalPath.Reverse().ToList();
					}
					if (predicate.Invoke(tile))
					{
						unsearchedTiles.Add(newNode);
					}
					else
					{
						searchedTiles.Add(newNode);
					}
				}
			}
		}

		return null;
	}
}