using Game.Manager;
using Godot;

namespace Game;

public partial class Main : Node
{
	private GridManager gridManager;
	private Sprite2D cursor;
	// A packed scene is effectively the data needed to create a scene
	private PackedScene buildingScene;
	private Button placeBuildingButton;
	private Vector2? hoveredGridCell;
	/*
		HashSet - A type of datastructure in which each element is unique
			- Un-ordered
			- Will automatically prevent duplicate entries
	*/
	

	// Called when the node enters the scene tree for the first time.
	// This ready method will be called after the ready method of all of its children
	public override void _Ready()
	{
		buildingScene = GD.Load<PackedScene>("res://scenes/building/Building.tscn");
		gridManager = GetNode<GridManager>("GridManager");
		// Define the sprite as the Node we setup in Godot
		cursor = GetNode<Sprite2D>("Cursor");
		placeBuildingButton = GetNode<Button>("PlaceBuildingButton");

		cursor.Visible = false;

		placeBuildingButton.Pressed += OnButtonPressed;
		// Alternate way to connect to signals
		// placeBuildingButton.Connect(Button.SignalName.Pressed, Callable.From(OnButtonPressed));
	}

	public override void _UnhandledInput(InputEvent evt)
	{
		if (evt.IsActionPressed("left_click") && hoveredGridCell.HasValue && gridManager.IsTilePositionValid(hoveredGridCell.Value))
		{
			PlaceBuildingAtHoveredCellPosition();
			cursor.Visible = false;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var gridPosition = gridManager.GetMouseGridCellPosition();
		// Global Position = position of the Node2d in the world space
		// Position = relative position to its parent
		cursor.GlobalPosition = gridPosition * 64;
		if (cursor.Visible && (!hoveredGridCell.HasValue || hoveredGridCell.Value != gridPosition))
		{
			hoveredGridCell = gridPosition;
			gridManager.HighlightValidTilesInRadius(hoveredGridCell.Value, 3);
		}
	}

	

	private void PlaceBuildingAtHoveredCellPosition()
	{
		if (!hoveredGridCell.HasValue)
		{
			return;
		}

		var building = buildingScene.Instantiate<Node2D>();
		AddChild(building);

		building.GlobalPosition = hoveredGridCell.Value * 64;
		gridManager.MarkTileAsOccupied(hoveredGridCell.Value);

		// Reset hover when clicked and reset tilemap
		hoveredGridCell = null;
		gridManager.ClearHighlightedTiles();
	}

	private void OnButtonPressed()
	{
		cursor.Visible = true;
	}

}
