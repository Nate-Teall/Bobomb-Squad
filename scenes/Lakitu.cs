using Godot;
using System;

public partial class Lakitu : Sprite2D
{
	private int speed = 300;
	private const float pauseTime = 0.5f;
	private float timer = 0f;

	// Lakitu will leave the screen on his third time hitting the edge.
	private int turns = 0;

	private GameManager gameManager;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		gameManager = GetParent<GameManager>();
		// Randomize which side of the screen lakitu starts on
		if (GD.Randf() <= 0.5f)
		{
			Position = new Vector2(25, 160);
		}
		else
		{
			Position = new Vector2(487, 160);
			speed *= -1;
			FlipH = true;
		}
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// When reaching the side of the screen, start the timer and don't move
		if ( (Position.X < 25 || Position.X > 487) && timer < pauseTime && turns < 2 )
		{
			timer += (float)delta;
			return;
		}
		// When timer is finished, stop it and turn around
		else if (timer >= pauseTime)
		{
			turns++;
			speed *= -1;
			timer = 0;
			FlipH = !FlipH;
		}
		
		Vector2 pos = Position;
		pos.X += speed * (float)delta;
		Position = pos;
	}

	public void _ScreenExited() { QueueFree(); }

	public void Die()
	{
		gameManager.Clear();
		QueueFree();
	}
}
