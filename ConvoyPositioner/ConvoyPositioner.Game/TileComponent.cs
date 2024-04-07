using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Input;
using Stride.Engine;
using TileGridLibrary;
using Stride.Core;

namespace ConvoyPositioner;

public class TileComponent : StartupScript
{
	private SpriteComponent _spriteComponent = default!;

	private Tile _tile = default!;
	public Tile Tile
	{
		get { return _tile; }
		set
		{
			_tile = value;
			_spriteComponent ??= Entity.Get<SpriteComponent>();
			_spriteComponent.Color = _tile.GetTileColor();
		}
	}
}
