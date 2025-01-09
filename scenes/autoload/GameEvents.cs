using Game.Component;
using Godot;

namespace Game.Autoload;

public partial class GameEvents : Node
{
	public static GameEvents Instance { get; private set; }

	// Signals in the C# implementation of Godot require EventHandler at the end due to Source Generated Code.
	[Signal]
	public delegate void BuildingPlacedEventHandler(BuildingComponent buildingComponent);

	public override void _Notification(int what)
	{
		if (what == NotificationSceneInstantiated)
		{
			// The Instance of the Autoload is set as soon as the scene is instantiated.
			Instance = this;
		}
	}

	public static void EmitBuildingPlaced(BuildingComponent buildingComponent)
	{
		Instance.EmitSignal(SignalName.BuildingPlaced, buildingComponent);
	}
}
