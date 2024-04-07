using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileGridLibrary.Pathfinding.AStar;

public class Node
{
    public Node? PreviousNode { get; set; }
    public Tile StartTile { get; init; } = default!;
    public Tile Tile { get; init; } = default!;
    public Tile EndTile { get; init; } = default!;
    public Node? NextNode { get; set; }

    public int PathLength
    {
        get
        {
            int distance = 0;
            Node? previousNode = PreviousNode;
            while (previousNode != null)
            {
                distance++;
                previousNode = previousNode.PreviousNode;
            }
            return distance;
        }
    }

    public Node(Tile inStartTile, Tile inTile, Tile inEndTile)
    {
        StartTile = inStartTile;
        Tile = inTile;
        EndTile = inEndTile;
    }
}

public static class GridPathNodeStatics
{
    public static void FindPath(this Tile startingPosition, Tile endingPosition, IEnumerable<Tile>? availableTiles = null, Func<Tile, bool>? predicate = null)
    {
        availableTiles ??= startingPosition.FanOut(predicate);
        predicate ??= ((_) => true);

    }

    public static IEnumerable<Tile> PathFind(this Tile inTile, Func<Tile, bool>? predicate = null)
    {
        predicate ??= ((_) => true);
        HashSet<Tile> searchedTiles = new();
        Queue<Tile> unsearchedTiles = new();
        unsearchedTiles.Enqueue(inTile);

        while (unsearchedTiles.Count > 0)
        {
            Tile searchedTile = unsearchedTiles.Dequeue();
            yield return searchedTile;
            searchedTiles.Add(searchedTile);
            foreach (Tile tile in searchedTile.Circle())
            {
                if (searchedTiles.Contains(tile) == false && unsearchedTiles.Contains(tile) == false)
                {
                    if (predicate.Invoke(tile))
                    {
                        unsearchedTiles.Enqueue(tile);
                    }
                    else
                    {
                        searchedTiles.Add(tile);
                    }
                }
            }
        }
    }
}