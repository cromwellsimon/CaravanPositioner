using SimplexNoise;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileGridLibrary.Map.Buildings;
using TileGridLibrary.Map.Terrain;
using TileGridLibrary.Map.Zones;
using static TileGridLibrary.Map.MapGenerator;

namespace TileGridLibrary.Map;

public static class MapGenerator
{
	private const float scale = 0.1f;

    public static Grid GenerateMap(Dimensions dimensions)
    {
        Grid grid = new(dimensions);
		grid.GenerateTerrain().GenerateZones().GenerateBuildings();
        return grid;
	}

	public static Grid GenerateTerrain(this Grid grid)
	{
		Noise.Seed = DateTimeOffset.UtcNow.Nanosecond;
		float[,] terrainElevation = Noise.Calc2D(grid.Width, grid.Height, scale);
		foreach (Tile tile in grid)
		{
			tile.AddContent(ParseElevation(terrainElevation[tile.Position.X, tile.Position.Y]));
		}
		return grid;
	}

	public static Grid GenerateZones(this Grid grid)
	{
		Noise.Seed = DateTimeOffset.UtcNow.Microsecond;
		float[,] zoneAssignment = Noise.Calc2D(grid.Width, grid.Height, scale);
		foreach (Tile tile in grid)
		{
			ITileContent? zone = ParseZone(tile, zoneAssignment[tile.Position.X, tile.Position.Y]);
			if (zone != null)
			{
				tile.AddContent(zone);
			}
		}
		return grid;
	}

	public static Grid GenerateBuildings(this Grid grid)
	{
		Noise.Seed = DateTimeOffset.UtcNow.Millisecond;
		float[,] buildingPlacement = Noise.Calc2D(grid.Width, grid.Height, scale);
		foreach (Tile tile in grid)
		{
			ITileContent? building = ParseBuilding(tile, buildingPlacement[tile.Position.X, tile.Position.Y]);
			if (building != null)
			{
				tile.AddContent(building);
			}
		}
		return grid;
	}

    public static ITileContent ParseElevation(float noiseValue) => noiseValue switch
	{
		<= 40 => new Water(),
		>= 40 and <= 215 => new Plains(),
		_ => new Mountains(),
	};

    public static ITileContent? ParseZone(Tile tile, float noiseValue)
	{
		if (noiseValue >= 150 && tile.IsValidColonyPlacement())
		{
			return new Colony();
		}
		return null;
	}

    public static ITileContent? ParseBuilding(Tile tile, float noiseValue)
	{
		if (noiseValue >= 200 && tile.IsValidBuildingPlacement())
		{
			return new Building();
		}
		return null;
	}
}
