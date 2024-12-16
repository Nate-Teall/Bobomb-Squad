using Godot;
using System;

public partial class Bobomb : CharacterBody2D
{
	private const float fallSpeed = 75;

	private float targetHeight;

	private GameManager gameManager;

	private Flower target;

	private float finalRotation = 0f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		targetHeight = GetViewport().GetVisibleRect().Size.Y / 2;
		gameManager = GetParent<GameManager>();

		// Set the bobomb to fall at a fixed rate.
		Velocity = new Vector2(0, fallSpeed);
	}

    // Called every frame(?)
    public override void _PhysicsProcess(double delta)
    {
		if ( (Position.Y > targetHeight) && target == null)
		{ 
			target = gameManager.GetTarget();
			target.bombs.Add(this);
			UpdateXVel();
			UpdateRotation();
		}

        MoveAndSlide();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		Rotation = Mathf.MoveToward(Rotation, finalRotation, (Mathf.Pi / 3) * (float)delta);
	}

	// Delete bobomb when colliding w/ flower or cannonball
	public void _AreaEntered(Node2D area)
	{
		if ( !(area.GetParent() is Bobomb) )
			QueueFree();
	}

	public void RemoveTarget() { target = null; }

	// When getting a new target, determine how fast the bobomb needs to move to reach the flower.
	private void UpdateXVel() 
	{
		Vector2 vel = Velocity;

		Vector2 distToTarget = new Vector2(
			target.Position.X - Position.X,
			target.Position.Y - Position.Y
		);
		vel.X = distToTarget.X / (distToTarget.Y / fallSpeed);

		Velocity = vel;
	}

	private void UpdateRotation() 
	{
		Vector2 distToTarget = new Vector2(
			target.Position.X - Position.X,
			target.Position.Y - Position.Y
		);

		float angle = Mathf.Atan(distToTarget.X / distToTarget.Y);
		finalRotation = -angle;
	}
}
