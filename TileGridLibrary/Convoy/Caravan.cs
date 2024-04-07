using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TileGridLibrary.Map.Buildings;
using TileGridLibrary.Map.Terrain;

namespace TileGridLibrary.Convoy;

public class Caravan
{
	public Vector2 Position { get; set; }

	public Caravan(Grid inGrid, Tile inCenterTile)
	{
		Tile? spawnTile = inGrid.GetCaravanSpawnLocation(inCenterTile);
		if (spawnTile != null)
		{
			Position = spawnTile.Position.ToSystemVector2();
		}
	}
}

public static class ConvoyStatics
{
	public static Tile? GetCaravanSpawnLocation(this Grid inGrid, Tile inDesiredTile)
	{
		List<Tile> validTiles = inDesiredTile.FanOut((tile) => tile.IsValidCaravanSpawn()).ToList();
		Tile[] perimeter = inGrid.GetPerimeter().ToArray();
		Random.Shared.Shuffle(perimeter);
		return perimeter.Intersect(validTiles).FirstOrDefault(/*(tile) => tile.IsValidCaravanSpawn()*/);
	}

	public static bool IsValidCaravanSpawn(this Tile tile)
	{
		foreach (ITileContent content in tile.Contents)
		{
			if (content is Mountains || content is Building || content is Water)
			{
				return false;
			}
		}
		return true;
	}
}
