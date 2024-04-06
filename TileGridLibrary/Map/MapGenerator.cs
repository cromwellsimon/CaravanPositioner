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
    public static Grid GenerateMap(int width, int height)
    {
        Grid grid = new(width, height);
        MapElements mapElements = new(width, height);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y].Contents.Add(ParseElevation(mapElements.TerrainElevation[x, y]));
                ITileContent? zone = ParseZone(mapElements.ZoneAssignment[x, y]);
                if (zone != null)
				{
					grid[x, y].Contents.Add(zone);
				}
                ITileContent? building = ParseBuilding(mapElements.BuildingPlacement[x, y]);
                if (building != null)
				{
					grid[x, y].Contents.Add(building);
				}
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

    public static ITileContent? ParseZone(float noiseValue) => noiseValue switch
    {
        >= 200 => new Colony(),
        _ => null
    };

    public static ITileContent? ParseBuilding(float noiseValue) => noiseValue switch
    {
        >= 200 => new Building(),
        _ => null
    };
}
