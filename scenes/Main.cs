using Godot;
using System;
using System.Runtime.CompilerServices;

namespace Game;

public partial class Main : Node2D
{
	private Sprite2D cursor;
	// A packed scene is effectively the data needed to create a scene
	private PackedScene buildingScene;
	private Button placeBuildingButton;
	private TileMapLayer highlightTileMapLayer;

	private Vector2? hoveredGridCell;

	// Called when the node enters the scene tree for the first time.
	// This ready method will be called after the ready method of all of its children
	public override void _Ready()
	{
		buildingScene = GD.Load<PackedScene>("res://scenes/building/Building.tscn");
		// Define the sprite as the Node we setup in Godot
		cursor = GetNode<Sprite2D>("Cursor");
		placeBuildingButton = GetNode<Button>("PlaceBuildingButton");
		highlightTileMapLayer = GetNode<TileMapLayer>("HighlightTileMapLayer");

		cursor.Visible = false;

		placeBuildingButton.Pressed += OnButtonPressed;
		// Alternate way to connect to signals
		// placeBuildingButton.Connect(Button.SignalName.Pressed, Callable.From(OnButtonPressed));
	}

	public override void _UnhandledInput(InputEvent evt)
	{
		if (evt.IsActionPressed("left_click") && cursor.Visible)
		{
			PlaceBuildingAtMousePosition();
			cursor.Visible = false;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var gridPosition = GetMouseGridCellPosition();
		// Global Position = position of the Node2d in the world space
		// Position = relative position to its parent
		cursor.GlobalPosition = gridPosition * 64;
		if (cursor.Visible && (!hoveredGridCell.HasValue || hoveredGridCell.Value != gridPosition))
		{
			hoveredGridCell = gridPosition;
			UpdateHighlightTileMapLayer();
		}
	}

	private Vector2 GetMouseGridCellPosition()
	{
		var mousePosition = GetGlobalMousePosition();
		var gridPosition = mousePosition / 64;
		gridPosition = gridPosition.Floor();
		return gridPosition;
	}

	private void PlaceBuildingAtMousePosition()
	{
		var building = buildingScene.Instantiate<Node2D>();
		AddChild(building);

		var gridPosition = GetMouseGridCellPosition();
		building.GlobalPosition = gridPosition * 64;
		// Reset hover when clicked and reset tilemap
		hoveredGridCell = null;
		UpdateHighlightTileMapLayer();
	}

	private void UpdateHighlightTileMapLayer()
	{
		// Clear all tiles in the tilemap when the cursor moves before re-painting
		highlightTileMapLayer.Clear();
		// Safety check that the hovered cell actually has a value
		if (!hoveredGridCell.HasValue)
		{
			return;
		}

		for (var x = hoveredGridCell.Value.X - 3; x <= hoveredGridCell.Value.X + 3; x++)
		{
			for (var y = hoveredGridCell.Value.Y - 3; y <= hoveredGridCell.Value.Y + 3; y++)
			{
				highlightTileMapLayer.SetCell(new Vector2I((int)x, (int)y), 0, Vector2I.Zero);
			}
		}
		
		

	}

	private void OnButtonPressed()
	{
		cursor.Visible = true;
	}

}
