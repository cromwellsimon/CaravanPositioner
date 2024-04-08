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

namespace CaravanPositioner;

public class TileComponent : StartupScript
{
	private SpriteComponent _spriteComponent = default!;

	private Tile _tile = default!;
	public Tile Tile
	{
		get { return _tile; }
		set
		{
			if (_tile != null)
			{
				_tile.OnContentsUpdatedEvent -= OnContentsUpdated;
			}
			_tile = value;
			_tile.OnContentsUpdatedEvent += OnContentsUpdated;
			OnContentsUpdated();
		}
	}

	private void OnContentsUpdated()
	{
		_spriteComponent ??= Entity.Get<SpriteComponent>();
		_spriteComponent.Color = _tile.GetTileColor();
	}
}
