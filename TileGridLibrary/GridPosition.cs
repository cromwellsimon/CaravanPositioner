using System.Numerics;

namespace TileGridLibrary;

/// <summary> Used to represent a single position on the grid. Essentially equivalent to a Vector2i in Godot </summary>
public record struct GridPosition(int X, int Y);

public static class GridPositionStatics
{
	public static Vector2 ToSystemVector2(this GridPosition position) => new(position.X, position.Y);
}