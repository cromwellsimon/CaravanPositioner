using OneOf.Types;
using System.Collections.Generic;
using System.Linq;

namespace TileGridLibrary.Test;

public class TileGridTests
{
	[Fact]
	public void CreatingGrid_CreatesExpectedSize()
	{
		Grid grid = new(new(100, 100));
		Assert.Equal(100, grid.Width);
		Assert.Equal(100, grid.Height);
	}

	[Fact]
	public void TileDirections_ReturnsExpectedTiles()
	{
		Grid grid = new(new(100, 100));
		Tile middleTile = grid[49, 49];
		Assert.Equal(50, middleTile.Up()!.Position.Y);
		Assert.Equal(48, middleTile.Down()!.Position.Y);
		Assert.Equal(48, middleTile.Left()!.Position.X);
		Assert.Equal(50, middleTile.Right()!.Position.X);

		Assert.Null(grid[0, 0].Down());
	}

	[Fact]
	public void TileFanOut_ReturnsExpectedTiles()
	{
		Grid grid = new(new(100, 100));
		List<Tile> test = grid[49, 49].FanOut().ToList();
		Assert.Equal(10000, test.Count);
	}

	[Fact]
	public void GetCenter_ReturnsExpectedValue()
	{
		Grid grid = new(new(100, 100));
		List<Tile> tiles = new()
		{
			{ grid[0, 0] },
			{ grid[0, 99] },
			// Even offsetting the center of mass shouldn't offset the returned tile
			{ grid[0, 98] },
			{ grid[99, 0] },
			{ grid[99, 99] }
		};
		Tile? foundTile = tiles.GetCenter();
		Assert.NotNull(foundTile);
		Assert.Equal(new GridPosition(49, 49), foundTile.Position);
	}

	[Fact]
	public void GetCenterOfMass_ReturnsExpectedValue()
	{
		Grid grid = new(new(100, 100));
		List<Tile> tiles = new()
		{
			{ grid[0, 0] },
			{ grid[0, 99] },
			// Offsetting the center of mass should offset the returned tile
			{ grid[0, 98] },
			{ grid[99, 0] },
			{ grid[99, 99] }
		};
		Tile? foundTile = tiles.GetCenterOfMass();
		Assert.NotNull(foundTile);
		Assert.Equal(new GridPosition(39, 59), foundTile.Position);
	}

	[Fact]
	public void GridGetPerimeter_ReturnsExpectedValues()
	{
		Grid grid = new(new(100, 100));
		List<Tile> gridPerimeter = grid.GetPerimeter().ToList();
		// This should be 397, not 400 because there are no duplicates for the corners with this like there would usually be
		Assert.Equal(397, gridPerimeter.Count);
	}

	[Fact]
	public void GridEnumerator_ReturnsExpectedValues()
	{
		Grid grid = new(new(100, 100));
		Assert.Equal(10000, grid.Count());
	}

	[Fact]
	public void TileEnumerator_ReturnsExpectedValues()
	{
		Grid grid = new(new(100, 100));
		List<Tile> tiles = grid.ToList();
		Assert.Equal(10000, grid.Count());
	}
}