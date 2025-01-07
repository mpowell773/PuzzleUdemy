using Godot;
using System;

namespace Game;

public partial class Main : Node2D
{
	private Sprite2D sprite;
	private PackedScene buildingScene;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		buildingScene = GD.Load<PackedScene>("res://scenes/building/Building.tscn");
		sprite = GetNode<Sprite2D>("Cursor");
	}

    public override void _UnhandledInput(InputEvent evt)
    {
        if (evt.IsActionPressed("left_click"))
		{
			PlaceBuildingAtMousePosition();
		}
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{	
		Vector2 gridPosition = GetMouseGridCellPosition();
		// Update sprite cursor to snap to grid.
		sprite.GlobalPosition = gridPosition * 64;
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
	}
}
