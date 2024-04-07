using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileGridLibrary;

public record struct Dimensions(int Width, int Height) : IEnumerable<GridPosition>
{
	public IEnumerator<GridPosition> GetEnumerator()
	{
		for (int width = 0; width < Width; width++)
		{
			for (int height = 0; height < Height; height++)
			{
				yield return new(width, height);
			}
		}
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public static class DimensionsStatics
{
	public static int Max(this Dimensions dimensions) => dimensions.Height > dimensions.Width ? dimensions.Height : dimensions.Width;
}