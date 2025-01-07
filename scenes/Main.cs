using Godot;
using System;
using System.Collections.Generic;

namespace Game;

public partial class Main : Node2D
{
	private Sprite2D cursor;
	private PackedScene buildingScene;
	private Button placeBuildingButton;
	private TileMapLayer highlightTilemapLayer;

	private Vector2? hoveredGridCell;
	private HashSet<Vector2> occupiedCells = new();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		buildingScene = GD.Load<PackedScene>("res://scenes/building/Building.tscn");
		cursor = GetNode<Sprite2D>("Cursor");
		placeBuildingButton = GetNode<Button>("PlaceBuildingButton");
		highlightTilemapLayer = GetNode<TileMapLayer>("HighlightTileMapLayer");

		cursor.Visible = false;

		placeBuildingButton.Pressed += OnButtonPressed;

		// Below is the alternate way of connecting signals in C#. 
		// placeBuildingButton.Connect(Button.SignalName.Pressed, Callable.From(OnButtonPressed));
	}

	public override void _UnhandledInput(InputEvent evt)
	{
		if (cursor.Visible && evt.IsActionPressed("left_click") && !occupiedCells.Contains(GetMouseGridCellPosition()))
		{
			PlaceBuildingAtMousePosition();
			cursor.Visible = false;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2 gridPosition = GetMouseGridCellPosition();
		// Update sprite cursor to snap to grid.
		cursor.GlobalPosition = gridPosition * 64;

		if (cursor.Visible && (!hoveredGridCell.HasValue || hoveredGridCell.Value != gridPosition))
		{
			hoveredGridCell = gridPosition;
			UpdateHighlightTileMapLayer();
		}
	}

	private Vector2 GetMouseGridCellPosition()
	{
		// Get grid coordinates.
		Vector2 mousePosition = GetGlobalMousePosition();
		Vector2 gridPosition = mousePosition / 64;
		gridPosition = gridPosition.Floor();
		return gridPosition;
	}

	private void PlaceBuildingAtMousePosition()
	{
		Node2D building = buildingScene.Instantiate<Node2D>();
		AddChild(building);

		Vector2 gridPosition = GetMouseGridCellPosition();
		building.GlobalPosition = gridPosition * 64;
		occupiedCells.Add(gridPosition);

		// Building is placed so hovered cell can be nullified.
		hoveredGridCell = null;
		UpdateHighlightTileMapLayer();
	}

	private void UpdateHighlightTileMapLayer()
	{
		highlightTilemapLayer.Clear();
		if (!hoveredGridCell.HasValue)
		{
			return;
		}

		for (var x = hoveredGridCell.Value.X - 3; x <= hoveredGridCell.Value.X + 3; x++)
		{
			for (var y = hoveredGridCell.Value.Y - 3; y <= hoveredGridCell.Value.Y + 3; y++)
			{
				highlightTilemapLayer.SetCell(new Vector2I((int)x, (int)y), 0, Vector2I.Zero);
			}
		}
	}

	private void OnButtonPressed()
	{
		cursor.Visible = true;
	}

}
