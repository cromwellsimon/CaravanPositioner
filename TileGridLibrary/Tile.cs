using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OneOf;
using OneOf.Types;

namespace TileGridLibrary;

public class Tile(Grid inGrid, GridPosition inPosition)
{
	public Grid Grid { get; private set; } = inGrid;
	/// <summary> The list of things that are currently on this tile at the moment </summary>
	private HashSet<ITileContent> _contents = new();
	public IEnumerable<ITileContent> Contents => _contents;
	public GridPosition Position { get; private set; } = inPosition;

	public bool AddContent(ITileContent inContent)
	{
		bool result = _contents.Add(inContent);
		if (result == true)
		{
			OnContentsUpdatedEvent?.Invoke();
		}
		return result;
	}

	public void ClearContent()
	{
		_contents.Clear();
		OnContentsUpdatedEvent?.Invoke();
	}

	public delegate void OnContentsUpdatedDelegate();
	public event OnContentsUpdatedDelegate? OnContentsUpdatedEvent;
}

public static class TileStatics
{
	public static Tile? Up(this Tile inTile)
	{
		if (inTile.Position.Y == inTile.Grid.Height - 1)
		{
			return null;
		}
		return inTile.Grid[inTile.Position.X][inTile.Position.Y + 1];
	}
	public static Tile? Down(this Tile inTile)
	{
		if (inTile.Position.Y == 0)
		{
			return null;
		}
		return inTile.Grid.Tiles[inTile.Position.X][inTile.Position.Y - 1];
	}
	public static Tile? Left(this Tile inTile)
	{
		if (inTile.Position.X == 0)
		{
			return null;
		}
		return inTile.Grid[inTile.Position.X - 1][inTile.Position.Y];
	}
	public static Tile? Right(this Tile inTile)
	{
		if (inTile.Position.X == inTile.Grid.Width - 1)
		{
			return null;
		}
		return inTile.Grid[inTile.Position.X + 1][inTile.Position.Y];
	}

	public static IEnumerable<Tile> Circle(this Tile inTile)
	{
		Tile? up = inTile.Up();
		if (up != null)
		{
			yield return up;
		}

		Tile? left = inTile.Left();
		if (left != null)
		{
			yield return left;
		}

		Tile? down = inTile.Down();
		if (down != null)
		{
			yield return down;
		}

		Tile? right = inTile.Right();
		if (right != null)
		{
			yield return right;
		}
	}

	/// <remarks>
	/// In the context of a tree, it's obvious how a breadth-first search works. This is a breadth-first search in the context of a grid structure
	/// </remarks>
	public static IEnumerable<Tile> FanOut(this Tile inTile, Func<Tile, bool>? predicate = null)
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

	/// <remarks>
	/// Note that this is identical to <see cref="TileStatics.FanOut(Tile)"/> except that, instead of a <see cref="Stack{T}"/>, it's a <see cref="Queue{T}"/>.
	/// I had originally done this using recursion but figured that using the <see cref="Stack{T}"/> would be better to prevent StackOverflows.
	/// This is a depth-first search.
	/// </remarks>
	public static IEnumerable<Tile> SnakeOut(this Tile inTile, Func<Tile, bool>? predicate = null)
	{
		predicate ??= ((_) => true);
		HashSet<Tile> searchedTiles = new();
		Stack<Tile> unsearchedTiles = new();
		unsearchedTiles.Push(inTile);

		while (unsearchedTiles.Count > 0)
		{
			Tile searchedTile = unsearchedTiles.Pop();
			yield return searchedTile;
			searchedTiles.Add(searchedTile);
			foreach (Tile tile in searchedTile.Circle())
			{
				if (searchedTiles.Contains(tile) == false && unsearchedTiles.Contains(tile) == false)
				{
					if (predicate.Invoke(tile))
					{
						unsearchedTiles.Push(tile);
						continue;
					}
					else
					{
						searchedTiles.Add(tile);
					}
				}
			}
		}
	}

	public static Tile? GetCenter(this IEnumerable<Tile> inTiles)
	{
		Grid? grid = null;
		// I would usually prefer using LINQ in this instance but that would require querying several times
		int minX = 0;
		int maxX = 0;
		int minY = 0;
		int maxY = 0;
		foreach (Tile tile in inTiles)
		{
			grid ??= tile.Grid;
			if (tile.Position.X < minX)
			{
				minX = tile.Position.X;
			}
			else if (tile.Position.X > maxX)
			{
				maxX = tile.Position.X;
			}
			if (tile.Position.Y < minY)
			{
				minY = tile.Position.Y;
			}
			else if (tile.Position.Y > maxY)
			{
				maxY = tile.Position.Y;
			}
		}
		return grid?[(minX + maxX) / 2][(minY + maxY) / 2];
	}

	/// <remarks>
	/// Behaves similarly to Blender's option to centralize the origin by mass.
	/// There is obviously no concept of mass in this grid, but is usually more logical than getting the actual center
	/// </remarks>
	public static Tile? GetCenterOfMass(this IEnumerable<Tile> inTiles)
	{
		Grid? grid = null;
		int totalX = 0;
		int totalY = 0;
		int tileCount = inTiles.Count();
		foreach (Tile tile in inTiles)
		{
			grid ??= tile.Grid;
			totalX += tile.Position.X;
			totalY += tile.Position.Y;
		}
		return grid?[totalX / tileCount][totalY / tileCount];
	}

	public static int DistanceTo(this Tile inStart, Tile inEnd) => Math.Abs(inStart.Position.X - inEnd.Position.X) + Math.Abs(inStart.Position.Y - inEnd.Position.Y);
}