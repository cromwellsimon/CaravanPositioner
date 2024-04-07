using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileGridLibrary.Map.Terrain;

namespace TileGridLibrary.Map.Buildings;

public class Building : ITileContent
{
}

public static class BuildingStatics
{
	public static bool IsValidBuildingPlacement(this Tile tile) => tile.Contents.Any((contents) => contents is Plains);
}
