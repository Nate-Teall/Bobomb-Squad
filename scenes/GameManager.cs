using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class GameManager : Node
{
	// Screen dimensions
	private float screenWidth;
	private float screenHeight;

	// Instances
	private PackedScene bobomb = GD.Load<PackedScene>("res://scenes/bobomb.tscn");
	private PackedScene flower = GD.Load<PackedScene>("res://scenes/flower.tscn");
	
	// Bomb spawning variables
	private const double maxTimer = 2;
	private const double minTimer = 0.5;
	private double timeToNextSpawn = 1;
	private double timeElapsed = 0;

	// Flower related variables
	private int flowerCount = 4;
	private List<Flower> flowers;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		screenWidth = GetViewport().GetVisibleRect().Size.X;
		screenHeight = GetViewport().GetVisibleRect().Size.Y;

		flowers = new List<Flower>
        {
            CreateFlower(100, 708),
            CreateFlower(200, 708),
            CreateFlower(300, 708),
            CreateFlower(400, 708)
        };

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		timeElapsed += delta;

		if (timeElapsed >= timeToNextSpawn)
		{
			CreateBobomb();
			timeToNextSpawn = GD.RandRange(minTimer, maxTimer);
			timeElapsed = 0;
		}
	}

	public void RemoveFlower(Flower flower) 
	{
		flowerCount -= 1;
		flowers.Remove(flower);
		// GameOver(); ???
	}

	public Flower GetTarget()
	{
		int i = (int)(GD.Randi() % flowers.Count);
		return flowers[i];
	}

	private void CreateBobomb()
	{
		Bobomb instance = bobomb.Instantiate<Bobomb>();
		AddChild(instance);
		instance.Position = new Vector2((float)GD.RandRange(0, screenWidth), -32);
	}

	private Flower CreateFlower(int x, int y)
	{
		Flower instance = flower.Instantiate<Flower>();
		AddChild(instance);
		instance.Position = new Vector2(x, y);

		return instance;
	}
}
