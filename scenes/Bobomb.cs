using Godot;
using System;

public partial class Bobomb : CharacterBody2D
{
	private const float fallSpeed = 75f;
	private const float rotationSpeed = 100f;

	private float targetHeight;

	private GameManager gameManager;

	private Flower target;

	private Vector2 finalVel = new Vector2(0, fallSpeed);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		targetHeight = GetViewport().GetVisibleRect().Size.Y / 2;
		gameManager = GetParent<GameManager>();

		// Set the bobomb to fall at a fixed rate.
		Velocity = finalVel;
	}

    // Called every frame(?)
    public override void _PhysicsProcess(double delta)
    {
		if ( (Position.Y > targetHeight) && target == null)
		{ 
			target = gameManager.GetTarget();
			target.bombs.Add(this);
			UpdateXVel();
		}

		// Smoothly change velocity towards the target
		Vector2 vel = Velocity;
		vel.X = Mathf.MoveToward(vel.X, finalVel.X, rotationSpeed * (float)delta);
		Velocity = vel;

        MoveAndSlide();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		// Rotation of the sprite will always follow it's current velocity
		float angle = Mathf.Atan(Velocity.X / Velocity.Y);
		Rotation = -angle;
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

		finalVel.X = vel.X;
	}

	// Outdated
	/*private void UpdateRotation() 
	{
		Vector2 distToTarget = new Vector2(
			target.Position.X - Position.X,
			target.Position.Y - Position.Y
		);

		float angle = Mathf.Atan(distToTarget.X / distToTarget.Y);
		finalRotation = -angle;
	} */
}
