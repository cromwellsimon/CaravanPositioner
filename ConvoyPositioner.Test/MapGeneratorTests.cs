using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileGridLibrary.Map;
using TileGridLibrary.Convoy;
using TileGridLibrary.Map.Terrain;
using TileGridLibrary.Map.Buildings;

namespace TileGridLibrary.Test;

public class MapGeneratorTests
{
	[Fact]
	public void MapGeneratorDebug()
	{
		Grid test = MapGenerator.GenerateMap(100, 100);
		Assert.True(true);
	}

	[Fact]
	public void ConvoyDebug()
	{
		Grid map = MapGenerator.GenerateMap(100, 100);
		Convoy.Convoy convoy = new(map);
		Tile spawnTile = map[convoy.Position];
		Assert.False(spawnTile.Contents.Any((content) => content is Mountains));
		Assert.False(spawnTile.Contents.Any((content) => content is Building));
		Assert.False(spawnTile.Contents.Any((content) => content is Water));
	}
}
