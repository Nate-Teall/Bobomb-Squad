using Godot;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

public partial class Cannonball : CharacterBody2D
{
	private enum CannonState
	{
		// The default state of the cannon ball. It is sitting in the slingshot, not being dragged.
		Default,

		// The cannonball is being dragged by the user, it has not been let go yet.
		Aiming,
		
		// The cannonball has been let go, and is now traveling in a straight line
		// until it reaches the edge of the screen.
		Fired
	}

	private Vector2 maxSling;
	private Vector2 minSling;

	private CannonState state;

	private Vector2 defaultPos;

	private int speed;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		defaultPos = new Vector2(256, 464);

		Vector2 viewPortSize = GetViewport().GetVisibleRect().Size;
		minSling = new Vector2(0, viewPortSize.Y / 2);
		maxSling = new Vector2(viewPortSize.X, viewPortSize.Y);

		state = CannonState.Default;

		speed = 400;
	}

    public override void _PhysicsProcess(double delta)
    {
        MoveAndSlide();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		switch (state)
		{
			case CannonState.Default:
				Position = defaultPos;
				break;

			case CannonState.Aiming:
				Position = GetViewport().GetMousePosition().Clamp(minSling, maxSling);
				break;

			case CannonState.Fired:
				break;
		}
	}

	// Called every frame, holds *all* input from the previous frame
    public override void _Input(InputEvent @event)
    {
		// Start firing the cannonball	 only after left click is released from the ball
        if(Input.IsActionJustReleased("click") && state == CannonState.Aiming)
		{
			state = CannonState.Fired;
			Velocity = (defaultPos - Position).Normalized() * speed;
		}
    }

    // Called every time an input event occurs within the cannonball's area
    public void _InputEvent(Node viewport, InputEvent inputEvent, int shapeIdx)
	{
		// Start dragging the cannonball when the ball itself is clickeds
		if(inputEvent is InputEventMouseButton)
		{
			if (inputEvent.IsPressed() && state == CannonState.Default)
			{
				state = CannonState.Aiming;
			}
		}
	}

    public void _AreaEntered(Area2D area)
	{
		if (area.GetParent() is Bobomb)
		{
			// Do something to increase the score...
		}
	}
}