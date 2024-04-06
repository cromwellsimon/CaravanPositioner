using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TileGridLibrary.Map.Buildings;
using TileGridLibrary.Map.Terrain;

namespace TileGridLibrary.Convoy;

public class Convoy
{
	public Vector2 Position { get; set; }

	public Convoy(Grid inGrid)
	{
		Position = ConvoyStatics.GetConvoySpawnLocation(inGrid).Position.ToSystemVector2();
	}
}

public static class ConvoyStatics
{
	public static Tile GetConvoySpawnLocation(this Grid inGrid)
	{
		Tile[] perimeter = inGrid.GetPerimeter().ToArray();
		Random.Shared.Shuffle(perimeter);
		return perimeter.First((tile) => tile.IsValidConvoySpawn());
	}

	public static bool IsValidConvoySpawn(this Tile tile)
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
