using Godot;
using System;

public partial class Toad : AnimatedSprite2D
{
	private float speed = 125f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2 pos = Position;
		pos.X += speed * (float)delta;
		Position = pos;
	}

	public void _AnimationLooped()
	{
		FlipH = !FlipH;
		speed *= -1;
	}
}
