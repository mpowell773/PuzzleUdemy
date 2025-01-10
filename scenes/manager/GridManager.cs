using Game.Autoload;
using Game.Component;
using Godot;
using System.Collections.Generic;
using System.Linq;

namespace Game.Manager;

public partial class GridManager : Node
{
	private HashSet<Vector2I> validBuildableTiles = new();

	[Export]
	private TileMapLayer highlightTilemapLayer;
	[Export]
	private TileMapLayer baseTerrainTilemapLayer;

	public override void _Ready()
	{
		GameEvents.Instance.BuildingPlaced += OnBuildingPlaced;
	}

	public bool IsTilePositionValid(Vector2I tilePosition)
	{
		var customData = baseTerrainTilemapLayer.GetCellTileData(tilePosition);

		if (customData == null) return false;

		return customData.GetCustomData("buildable").As<bool>();

	}

	public bool IsTilePositionBuildable(Vector2I tilePosition)
	{
		return validBuildableTiles.Contains(tilePosition);
	}

	public void HighlightBuildableTiles()
	{
		foreach (var tilePosition in validBuildableTiles)
		{
			highlightTilemapLayer.SetCell(tilePosition, 0, Vector2I.Zero);
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

	private void UpdateValidBuildableTiles(BuildingComponent buildingComponent)
	{
		Vector2I rootCell = buildingComponent.GetGridCellPosition();

		for (var x = rootCell.X - buildingComponent.BuildableRadius; x <= rootCell.X + buildingComponent.BuildableRadius; x++)
		{
			for (var y = rootCell.Y - buildingComponent.BuildableRadius; y <= rootCell.Y + buildingComponent.BuildableRadius; y++)
			{
				var tilePosition = new Vector2I(x, y);

				if (!IsTilePositionValid(tilePosition)) continue;

				validBuildableTiles.Add(tilePosition);
			}
		}
		validBuildableTiles.Remove(buildingComponent.GetGridCellPosition());
	}

	private void OnBuildingPlaced(BuildingComponent buildingComponent)
	{
		UpdateValidBuildableTiles(buildingComponent);
	}
}