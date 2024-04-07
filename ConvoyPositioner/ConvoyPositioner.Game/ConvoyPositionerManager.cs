using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Input;
using Stride.Engine;
using TileGridLibrary;
using TileGridLibrary.Map;
using TileGridLibrary.Map.Terrain;
using TileGridLibrary.Map.Buildings;
using TileGridLibrary.Map.Zones;
using TileGridLibrary.Convoy;
using TileGridLibrary.Pathfinding.BreadthFirst;

namespace ConvoyPositioner;

public class ConvoyPositionerManager : StartupScript
{
	public Dimensions Dimensions { get; init; } = new(100, 100);
	public Prefab TilePrefab { get; init; } = default!;
	public Entity Camera { get; private set; } = default!;

	public override void Start()
	{
		Camera ??= Entity.Scene.Entities.First((entity) => entity.Name == nameof(Camera));
		Camera.Get<CameraComponent>().OrthographicSize = Dimensions.Max();
		TransformComponent cameraTransform = Camera.Get<TransformComponent>();
		cameraTransform.Position.X = Dimensions.Width / 2;
		cameraTransform.Position.Y = Dimensions.Height / 2;
		Game.Window.SetSize(new(Dimensions.Width, Dimensions.Height));
		Game.Window.AllowUserResizing = true;

		Grid map = MapGenerator.GenerateMap(Dimensions);
		foreach (Tile tile in map)
		{
			List<Entity> gridElement = TilePrefab.Instantiate();
			Entity.Scene.Entities.AddRange(gridElement);
			foreach (Entity entity in gridElement)
			{
				entity.Transform.Position = new(tile.Position.X, tile.Position.Y, 0);
				TileComponent tileComponent = entity.Get<TileComponent>();
				tileComponent.Tile = tile;
			}
		}

		Tile? caravanSpawnLocation = null;
		Tile? colonyCenter = map.Where((tile) => tile.Contents.Any((contents) => contents.Is<Colony>())).GetCenterOfMass();
		if (colonyCenter != null)
		{
			caravanSpawnLocation = map.GetCaravanSpawnLocation(colonyCenter);
			if (caravanSpawnLocation == null)
			{
				foreach (Tile potentialPosition in colonyCenter.FanOut())
				{
					if (potentialPosition.IsValidColonyPlacement() == false)
					{
						continue;
					}
					caravanSpawnLocation = map.GetCaravanSpawnLocation(potentialPosition);
					if (caravanSpawnLocation != null)
					{
						colonyCenter = potentialPosition;
						break;
					}
				}
			}
			List<Tile>? path = NodeStatics.GetPath(caravanSpawnLocation!, colonyCenter, (tile) => tile.IsValidCaravanSpawn());
			//List<Tile> validTiles = colonyCenter.FanOut((tile) => tile.IsValidCaravanSpawn()).ToList();
			List<TileComponent> tiles = Entity.Scene.Entities.Where((entity) =>
			{
				TileComponent? tileComponent = entity.Get<TileComponent>();
				if (tileComponent != null && /*validTiles*/path.Contains(tileComponent.Tile))
				{
					return true;
				}
				return false;
			}).Select((entity) => entity.Get<TileComponent>()).ToList();
			foreach (TileComponent tile in tiles)
			{
				SpriteComponent spriteComponent = tile.Entity.Get<SpriteComponent>();
				if (tile.Tile == caravanSpawnLocation)
				{
					spriteComponent.Color = Color.Red;
				}
				else if (tile.Tile == colonyCenter)
				{
					spriteComponent.Color = Color.Yellow;
				}
				else
				{
					spriteComponent.Color = new(spriteComponent.Color.R * 0.5f, spriteComponent.Color.G * 0.5f, spriteComponent.Color.B * 0.5f);
				}
			}
		}
	}
}

public static class MapGeneratorStatics
{
	public static Color GetTileColor(this Tile inTile)
	{
		if (inTile.Contents.Any(Is<Water>))
		{
			return Color.Blue;
		}
		if (inTile.Contents.Any(Is<Mountains>))
		{
			return Color.Gray;
		}
		if (inTile.Contents.Any(Is<Plains>))
		{
			if (inTile.Contents.Any(Is<Colony>))
			{
				if (inTile.Contents.Any(Is<Building>))
				{
					return new(61, 12, 2);
				}
				return Color.HotPink;
			}
			if (inTile.Contents.Any(Is<Building>))
			{
				return Color.Black;
			}
			return Color.Green;
		}
		return Color.Purple;
	}

	public static bool Contains<InputType, QueryType>(this IEnumerable<InputType> values) => values.Any((value) => value is QueryType);
	public static bool Is<T>(this object inObject) => inObject is T;
}