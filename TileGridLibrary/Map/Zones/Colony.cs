using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileGridLibrary.Map.Terrain;

namespace TileGridLibrary.Map.Zones;

public class Colony : ITileContent
{
}

public static class ColonyStatics
{
	public static bool IsValidColonyPlacement(this Tile tile) => tile.Contents.Any((contents) => contents is Plains) /*&& tile.Position.X > tile.Grid.Width / 2 && tile.Position.Y > tile.Grid.Height / 2*/;
}