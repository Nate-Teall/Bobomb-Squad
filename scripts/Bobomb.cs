using Godot;
using System;

public partial class Bobomb : CharacterBody2D
{
	public enum BobombState
	{
		Flying,
		Dying
	}

	public BobombState state { get; private set; }
	private const float fallSpeed = 100f;
	private const float deathSpeed = 500f;
	private const float rotationSpeed = 100f;

	private static PackedScene explosion = GD.Load<PackedScene>("res://scenes/explosion.tscn");

	private float targetHeight;

	private GameManager gameManager;

	private Flower target;

	private Vector2 finalVel = new Vector2(0, fallSpeed);

	// Bobombs will curve left/right randomly
	private const double turnRange = Mathf.Pi / 3;
	private const int turnCooldown = 1;
	private double turnCooldownTimer = 0f;
	private float velAngle = 0f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		state = BobombState.Flying;
		targetHeight = GetViewport().GetVisibleRect().Size.Y / 2;
		gameManager = GetParent<GameManager>();

		// Set the bobomb to fall at a fixed rate.
		Velocity = finalVel;
	}

    // Called every frame
    public override void _PhysicsProcess(double delta)
    {
		switch (state)
		{
			case BobombState.Flying:
				// In the top half of the screen, turn randomly every 1 second
				if ( Position.Y <= targetHeight)
				{
					if (turnCooldownTimer >= turnCooldown && Velocity.Equals(finalVel))
					{
						// Reset the timer and the velocity's angle
						turnCooldownTimer = 0;	
						finalVel = finalVel.Rotated(-velAngle);

						// Make a new velocity angle
						velAngle = (float)GD.RandRange(-turnRange, turnRange);
						finalVel = finalVel.Rotated(velAngle);
					}

					// If near the side of the screen, (left or right eighth) slowly move back towards the center
					if (Position.X < 64 || Position.X > 448)
					{
						finalVel = finalVel.Rotated(-velAngle);
						velAngle = Mathf.Pi / 4;
						// Determine whether to turn left or right
						velAngle *= Position.X > 448 ? 1 : -1;
						finalVel = finalVel.Rotated(velAngle);
					}
				}
				// In the bottom half of the screen, lock onto a flower and move towards it.
				else
				{
					if (target == null)
					{
						target = gameManager.GetTarget();
						target.bombs.Add(this);

						// Because I use MoveToward() to slowly change the bobomb's direction,
						// they will be *slightly* off the mark. They still hit 95% of the time,
						// and 100% if its set to instantly snap to the right angle
						finalVel = (target.Position - Position).Normalized() * fallSpeed;
					}
				}

				// Smooth any velocity changes
				Vector2 vel = Velocity;
				vel.X = Mathf.MoveToward(vel.X, finalVel.X, rotationSpeed * (float)delta);
				vel.Y = Mathf.MoveToward(vel.Y, finalVel.Y, rotationSpeed * (float)delta);
				Velocity = vel;
				break;

			case BobombState.Dying:
				// After being hit, bobombs will become affected by gravity and spin out of control
				Velocity += GetGravity() * (float)delta;

				break;
		}

        MoveAndSlide();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		turnCooldownTimer += delta;
		
		switch (state)
		{
			case BobombState.Flying:
				// While flying, rotation of the sprite will always follow it's current velocity
				float angle = Mathf.Atan(Velocity.X / Velocity.Y);
				//angle = Velocity.Angle();
				Rotation = -angle;
				break;
			
			case BobombState.Dying:
				// After being hit, boboms will become affected by gravity and spin out of control
				Rotation += 18 * (float)delta;
				break;
		}
	}

	// Delete bobomb when colliding w/ flower or cannonball
	public void _AreaEntered(Node2D area)
	{
		if ( area.GetParent() is Flower && state == BobombState.Flying )
		{
			QueueFree();
		}
	}

	public void _ScreenExited() { QueueFree(); }

	public void RemoveTarget() { target = null; }

	public void Die(Vector2 vel)
	{
		state = BobombState.Dying;
		Explosion instance = explosion.Instantiate<Explosion>();
		AddSibling(instance);
		instance.Position = Position;
		
		// Inherit the cannonball's velocity
		Velocity = vel.Normalized() * deathSpeed;
	}

	public void Die()
	{
		Die(new Vector2(GD.RandRange(-800, 800), -800));
	}

}
