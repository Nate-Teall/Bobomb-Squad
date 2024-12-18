using Godot;
using System;
using System.Collections.Generic;

public partial class Flower : CharacterBody2D
{
	private GameManager gameManager;
	private AnimatedSprite2D sprite2D;

	public List<Bobomb> bombs { get; private set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		gameManager = GetParent<GameManager>();
		sprite2D = GetNode<AnimatedSprite2D>("Sprite2D");
		sprite2D.Animation = "default";
		bombs = new List<Bobomb>();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// Called when any area collides with the flower
	public void _AreaEntered(Area2D area)
	{
		Node parent = area.GetParent();
		if ( parent is Bobomb ) 
		{
			Bobomb attacker = (Bobomb)parent;
			if (attacker.state == Bobomb.BobombState.Flying)
			{
				gameManager.RemoveFlower(this);
				foreach( Bobomb bobomb in bombs)
				{
					bobomb.RemoveTarget();
				}
				sprite2D.Animation = "explode";
				GetNode<Area2D>("Area2D").QueueFree();
			}
		}
	}
}
