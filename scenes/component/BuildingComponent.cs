using Game.Autoload;
using Godot;


namespace Game.Component;

public partial class BuildingComponent : Node2D
{
	[Export]
	public int BuildableRadius { get; private set; }

	public override void _Ready()
	{
		// This is an insanely smart way to not have to deal with group name strings by just handling groups with code.
		AddToGroup(nameof(BuildingComponent));

		// Deferred Signal
		Callable.From(() => GameEvents.EmitBuildingPlaced(this)).CallDeferred();
	}

	public Vector2I GetGridCellPosition()
	{
		// Get grid coordinates.
		Vector2 gridPosition = GlobalPosition / 64;
		gridPosition = gridPosition.Floor();
		// This cast may become a problem in the future.
		return (Vector2I)gridPosition;
	}

}
