using SimplexNoise;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileGridLibrary.Map.Buildings;
using TileGridLibrary.Map.Terrain;
using TileGridLibrary.Map.Zones;

namespace TileGridLibrary.Map;

public static class MapGenerator
{
    public static Grid GenerateMap(Dimensions dimensions)
    {
        Grid grid = new(dimensions);
        MapElements mapElements = new(dimensions.Width, dimensions.Height);
        foreach (GridPosition pixel in dimensions)
		{
			Tile tile = grid[pixel.X, pixel.Y];
			tile.Contents.Add(ParseElevation(mapElements.TerrainElevation[pixel.X, pixel.Y]));
			ITileContent? zone = ParseZone(tile, mapElements.ZoneAssignment[pixel.X, pixel.Y]);
			if (zone != null)
			{
				grid[pixel.X, pixel.Y].Contents.Add(zone);
			}
			ITileContent? building = ParseBuilding(tile, mapElements.BuildingPlacement[pixel.X, pixel.Y]);
			if (building != null)
			{
				grid[pixel.X, pixel.Y].Contents.Add(building);
			}
		}
        return grid;
    }

    public record MapElements
    {
        public float[,] TerrainElevation { get; private set; } = default!;
        public float[,] ZoneAssignment { get; private set; } = default!;
        public float[,] BuildingPlacement { get; private set; } = default!;
		public MapElements(int width, int height)
		{
			Noise.Seed = DateTimeOffset.UtcNow.Nanosecond;
			TerrainElevation = Noise.Calc2D(width, height, 1.0f);
			Noise.Seed = DateTimeOffset.UtcNow.Microsecond;
			ZoneAssignment = Noise.Calc2D(width, height, 1.0f);
			Noise.Seed = DateTimeOffset.UtcNow.Millisecond;
			BuildingPlacement = Noise.Calc2D(width, height, 1.0f);
		}
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
