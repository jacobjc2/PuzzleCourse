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

	// Called when the node enters the scene tree for the first time.
	// This ready method will be called after the ready method of all of its children
	public override void _Ready()
	{
		buildingScene = GD.Load<PackedScene>("res://scenes/building/Building.tscn");
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
	}

	private void OnButtonPressed()
	{
		cursor.Visible = true;
	}

}
