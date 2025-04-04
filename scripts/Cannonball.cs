using Godot;
using System;
using System.Collections.Generic;

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

	private static GameManager gameManager;
	private static AudioStreamPlayer audio;

	private const int speed = 800;

	private Vector2 maxSling;
	private Vector2 minSling;

	private CannonState state;

	private Vector2 defaultPos;

	private Line2D slingString;

	private int bobombsHit = 0; 
	private Queue<Vector2> hitLocations = new Queue<Vector2>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		gameManager = GetNode<GameManager>("../GameManager");
		audio = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		defaultPos = new Vector2(256, 464);

		Vector2 viewPortSize = GetViewport().GetVisibleRect().Size;
		minSling = new Vector2(0, viewPortSize.Y / 2);
		maxSling = new Vector2(viewPortSize.X, viewPortSize.Y);

		slingString = GetNode<Line2D>("../String");

		state = CannonState.Default;
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
				slingString.Hide();
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
			GetNode<Area2D>("Area2D").Monitoring = false;
			GetNode<Area2D>("Area2D").Monitoring = true;
			Velocity = (defaultPos - Position).Normalized() * speed;
		}

		// Mute bobomb hit sound 
		if(Input.IsActionJustPressed("mute"))
		{
			audio.VolumeDb = audio.VolumeDb == -20 ? -80 : -20;
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
		if (area.GetParent() is Bobomb && state == CannonState.Fired)
		{
			if(area.GetParent<Bobomb>().state == Bobomb.BobombState.Flying)
			{
				hitLocations.Enqueue(Position);

				audio.PitchScale = (float)(0.7 + (0.1 * bobombsHit)); 
				audio.Play();
				
				bobombsHit++;
			}
			
			area.GetParent<Bobomb>().Die(Velocity);
		}
		else if (area.GetParent() is Lakitu)
		{
			area.GetParent<Lakitu>().Die();
		}
	}

	public void _ScreenExited() 
	{ 
		state = CannonState.Default; 
		slingString.Show();
		gameManager.CountScore(bobombsHit, hitLocations);
		bobombsHit = 0;
		hitLocations.Clear();
	}
}
