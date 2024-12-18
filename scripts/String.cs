using Godot;
using System;

public partial class String : Line2D
{
	private Cannonball cannonball;

	private Vector2 defaultPos; // Should match cannonball's defaultPos (256, 464)

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		cannonball = GetNode<Cannonball>("../Cannonball");

		defaultPos = Position;
		
		AddPoint(new Vector2(-36, -10));
		AddPoint(Vector2.Zero);
		AddPoint(new Vector2(+36, -10));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		SetPointPosition(1, cannonball.Position - defaultPos);
	}
}
