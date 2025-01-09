using Game.Component;
using Godot;
using System.Collections.Generic;
using System.Linq;

namespace Game.Manager;

public partial class GridManager : Node
{
	private HashSet<Vector2I> occupiedCells = new();

	[Export]
	private TileMapLayer highlightTilemapLayer;
	[Export]
	private TileMapLayer baseTerrainTilemapLayer;

	public bool IsTilePositionValid(Vector2I tilePosition)
	{
		var customData = baseTerrainTilemapLayer.GetCellTileData(tilePosition);

		if (customData == null) return false;
		if (!customData.GetCustomData("buildable").As<bool>()) return false;

		return !occupiedCells.Contains(tilePosition);
	}

	public void MarkTilesAsOccupied(Vector2I tilePosition)
	{
		occupiedCells.Add(tilePosition);
	}

	public void HighlightBuildableTiles()
	{
		ClearHighlightedTiles();

		var buildingComponents = GetTree().GetNodesInGroup(nameof(BuildingComponent)).Cast<BuildingComponent>();
		foreach (var buildingComponent in buildingComponents)
		{
			HighlightValidTilesInRadius(buildingComponent.GetGridCellPosition(), buildingComponent.BuildableRadius);
		}
	}

	public void ClearHighlightedTiles()
	{
		highlightTilemapLayer.Clear();
	}

	public Vector2I GetMouseGridCellPosition()
	{
		// Get grid coordinates.
		Vector2 mousePosition = highlightTilemapLayer.GetGlobalMousePosition();
		Vector2 gridPosition = mousePosition / 64;
		gridPosition = gridPosition.Floor();
		// This cast may become a problem in the future.
		return (Vector2I)gridPosition;
	}

	private void HighlightValidTilesInRadius(Vector2I rootCell, int radius)
	{

		for (var x = rootCell.X - radius; x <= rootCell.X + radius; x++)
		{
			for (var y = rootCell.Y - radius; y <= rootCell.Y + radius; y++)
			{
				var tilePosition = new Vector2I(x, y);
				if (!IsTilePositionValid(tilePosition)) continue;
				highlightTilemapLayer.SetCell(tilePosition, 0, Vector2I.Zero);
			}
		}
	}

}