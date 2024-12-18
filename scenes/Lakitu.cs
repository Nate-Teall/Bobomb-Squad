using Godot;
using System;

public partial class Lakitu : Sprite2D
{
	private int speed = 300;
	private const float pauseTime = 0.5f;
	private float timer = 0f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Position = new Vector2(25, 160);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// When reaching the side of the screen, start the timer and don't move
		if ( (Position.X < 25 || Position.X > 487) && timer < pauseTime )
		{
			timer += (float)delta;
			return;
		}
		// When timer is finished, stop it and turn around
		else if (timer >= pauseTime)
		{
			speed *= -1;
			timer = 0;
			FlipH = !FlipH;
		}
		
		Vector2 pos = Position;
		pos.X += speed * (float)delta;
		Position = pos;
	}
}
