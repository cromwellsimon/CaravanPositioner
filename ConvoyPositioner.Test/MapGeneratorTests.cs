using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileGridLibrary.Map;
using TileGridLibrary.Convoy;
using TileGridLibrary.Map.Terrain;
using TileGridLibrary.Map.Buildings;
using TileGridLibrary.Map.Zones;

namespace TileGridLibrary.Test;

public class MapGeneratorTests
{
	[Fact]
	public void MapGeneratorDebug()
	{
		Grid test = MapGenerator.GenerateMap(new(100, 100));
		Assert.True(true);
	}

	[Fact]
	public void ConvoyDebug()
	{
		Grid map = MapGenerator.GenerateMap(new(100, 100));
		Tile? colonyCenter = map.Where((tile) => tile.Contents.Any((contents) => contents is Colony)).GetCenterOfMass();
		if (colonyCenter != null)
		{
			Caravan convoy = new(map, colonyCenter);
			Tile spawnTile = map[convoy.Position];
			Assert.False(spawnTile.Contents.Any((content) => content is Mountains));
			Assert.False(spawnTile.Contents.Any((content) => content is Building));
			Assert.False(spawnTile.Contents.Any((content) => content is Water));
		}
	}
}
