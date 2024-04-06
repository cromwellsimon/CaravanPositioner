using System;
using System.Collections.Generic;
using System.Linq;
using OneOf;
using OneOf.Types;

namespace TileGridLibrary;

public class Tile(Grid inGrid, GridPosition inPosition)
{
	public Grid Grid { get; private set; } = inGrid;
	/// <summary> The list of things that are currently on this tile at the moment </summary>
	public HashSet<ITileContent> Contents { get; private set; } = new();
	public GridPosition Position { get; private set; } = inPosition;
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
	public static IEnumerable<Tile> FanOut(this Tile inTile)
	{
		HashSet<Tile> fannedTiles = new();
		Queue<Tile> toFan = new();
		toFan.Enqueue(inTile);

		while (toFan.Count > 0)
		{
			Tile fannedTile = toFan.Dequeue();
			yield return fannedTile;
			fannedTiles.Add(fannedTile);
			foreach (Tile tile in fannedTile.Circle())
			{
				if (fannedTiles.Contains(tile) == false && toFan.Contains(tile) == false)
				{
					toFan.Enqueue(tile);
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
}