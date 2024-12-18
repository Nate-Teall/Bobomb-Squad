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
	private static PackedScene bobomb = GD.Load<PackedScene>("res://scenes/bobomb.tscn");
	private static PackedScene flower = GD.Load<PackedScene>("res://scenes/flower.tscn");
	private static PackedScene lakitu = GD.Load<PackedScene>("res://scenes/lakitu.tscn");
	
	// Bomb spawning variables
	private const double maxTimer = 2;
	private const double minTimer = 0.5;
	private double timeToNextSpawn = 1;
	private double timeElapsed = 0;

	// Flower related variables
	private int flowerCount = 4;
	private List<Flower> flowers;

	private const int baseScore = 100;
	private int totalScore = 0;

	private const double maxLakituTimer = 10;
	private const double minLakituTimer = 7;
	private double timeToNextLakitu = 10;
	private double lakituTimer = 0;

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
		lakituTimer += delta;

		if (timeElapsed >= timeToNextSpawn)
		{
			CreateBobomb();
			timeToNextSpawn = GD.RandRange(minTimer, maxTimer);
			timeElapsed = 0;
		}

		if (lakituTimer >= timeToNextLakitu)
		{
			// Only spawn 1 lakitu at a time
			if (!HasNode("Lakitu"))
				CreateLakitu();
			timeToNextLakitu = GD.RandRange(minLakituTimer, maxLakituTimer);
			lakituTimer = 0;
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

	public void CountScore(int bobombsHit)
	{
		int score = 0;

		for (int i=0; i<bobombsHit; i++)
		{
			score += baseScore * (int)Mathf.Pow(2, i);
			GD.Print("+" + (baseScore * (int)Mathf.Pow(2, i)).ToString());
		}

		totalScore += score;
		GD.Print("New score: " + totalScore.ToString());
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

	private void CreateLakitu()
	{
		Lakitu instance = lakitu.Instantiate<Lakitu>();
		AddChild(instance);
	}
}
