using Godot;
using System;

public partial class ScoreLabel : RichTextLabel
{
	private static int offset = 30;
	private static int speed = 30;
	private float finalPos;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		finalPos = Position.Y - offset;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Position.Y <= finalPos)
			QueueFree();
		
		Vector2 pos = Position;
		pos.Y -= speed * (float)delta;
		Position = pos;
	}
}
