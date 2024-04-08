using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace TileGridLibrary;

public class Grid : IEnumerable<Tile>
{
	public List<List<Tile>> Tiles { get; init; } = default!;

	public Grid(Dimensions dimensions)
	{
		Tiles = new(dimensions.Width);
		for (int width = 0; width < dimensions.Width; width++) 
		{
			Tiles.Add(new());
			for (int height = 0; height < dimensions.Height; height++)
			{
				Tiles[width].Add(new(this, new(width, height)));
			}
		}
	}

	public int Width => Tiles.Count;
	public int Height => Tiles[0].Count;
	public List<Tile> this[int inIndex] => Tiles[inIndex];
	public Tile this[int inX, int inY] => Tiles[inX][inY];
	public Tile this[GridPosition inGridPosition] => this[inGridPosition.X, inGridPosition.Y];

	/// <summary>
	/// This traverses the Grid like a 2D array
	/// </summary>
	public IEnumerator<Tile> GetEnumerator()
	{
		int gridWitdh = Width;
		int gridHeight = Height;
		for (int width = 0; width < gridWitdh; width++)
		{
			for (int height = 0; height < gridHeight; height++)
			{
				yield return Tiles[width][height];
			}
		}
	}
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public static class GridStatics
{
	/// <remarks>
	/// There are several off-by-2s in here in order to prevent duplicates in the obtained tiles
	/// </remarks>
	public static IEnumerable<Tile> GetPerimeter(this Grid inGrid)
	{
		int width = inGrid.Width;
		int height = inGrid.Height;
		for (int leftHeight = 0; leftHeight < width; leftHeight++)
		{
			yield return inGrid[0][leftHeight];
		}
		for (int topWidth = 1; topWidth < width; topWidth++)
		{
			yield return inGrid[topWidth][height - 1];
		}
		for (int rightHeight = height - 2; rightHeight >= 0; rightHeight--)
		{
			yield return inGrid[width - 1][rightHeight];
		}
		for (int bottomWidth = width - 2; bottomWidth >= 0; bottomWidth--)
		{
			yield return inGrid[bottomWidth][0];
		}
	}
}
