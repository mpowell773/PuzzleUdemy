using Game.Manager;
using Godot;

namespace Game;

public partial class Main : Node
{
	private GridManager gridManager;
	private Sprite2D cursor;
	private PackedScene buildingScene;
	private Button placeBuildingButton;

	private Vector2? hoveredGridCell;

	public override void _Ready()
	{
		buildingScene = GD.Load<PackedScene>("res://scenes/building/Building.tscn");
		gridManager = GetNode<GridManager>("GridManager");
		cursor = GetNode<Sprite2D>("Cursor");
		placeBuildingButton = GetNode<Button>("PlaceBuildingButton");

		cursor.Visible = false;

		placeBuildingButton.Pressed += OnButtonPressed;

		// Below is the alternate way of connecting signals in C#. 
		// placeBuildingButton.Connect(Button.SignalName.Pressed, Callable.From(OnButtonPressed));
	}

	public override void _UnhandledInput(InputEvent evt)
	{
		if (hoveredGridCell.HasValue && evt.IsActionPressed("left_click") && gridManager.IsTilePositionValid(hoveredGridCell.Value))
		{
			PlaceBuildingAtHoveredCellPosition();
			cursor.Visible = false;
		}
	}

	public override void _Process(double delta)
	{
		Vector2 gridPosition = gridManager.GetMouseGridCellPosition();
		// Update sprite cursor to snap to grid.
		cursor.GlobalPosition = gridPosition * 64;

		if (cursor.Visible && (!hoveredGridCell.HasValue || hoveredGridCell.Value != gridPosition))
		{
			hoveredGridCell = gridPosition;
			gridManager.HighlightValidTilesInRadius(hoveredGridCell.Value, 3);
		}
	}


	private void PlaceBuildingAtHoveredCellPosition()
	{
		if (!hoveredGridCell.HasValue) return;

		Node2D building = buildingScene.Instantiate<Node2D>();
		AddChild(building);


		building.GlobalPosition = hoveredGridCell.Value * 64;
		gridManager.MarkTilesAsOccupied(hoveredGridCell.Value);

		// Building is placed so hovered cell can be nullified.
		hoveredGridCell = null;
		gridManager.ClearHighlightedTiles();
	}

	private void OnButtonPressed()
	{
		cursor.Visible = true;
	}

}
