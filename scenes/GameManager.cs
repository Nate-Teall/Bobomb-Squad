using Godot;
using Godot.Collections;
using System;
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
	private static PackedScene scoreLabel = GD.Load<PackedScene>("res://scenes/score_label.tscn");
	
	// Bomb spawning variables
	private const double maxTimer = 2;
	private const double minTimer = 0.5;
	private double timeToNextSpawn = 1;
	private double timeElapsed = 0;

	// Flower related variables
	private List<Flower> flowers;

	// Score related variables
	private const int baseScore = 100;
	private int totalScore = 0;
	private RichTextLabel totalScoreLabel;

	// Lakitu spawning variables
	private const double maxLakituTimer = 20;
	private const double minLakituTimer = 15;
	private double timeToNextLakitu = 10;
	private double lakituTimer = 0;

	// Game state variables
	private bool gameStarted = false;
	private RichTextLabel startLabel;
	private const string endText = "  * Game Over *\n\n Press Space to \n     Restart\n\n Score: ";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		totalScoreLabel = GetNode<RichTextLabel>("../TotalScore");
		startLabel = GetNode<RichTextLabel>("../StartLabel");

		screenWidth = GetViewport().GetVisibleRect().Size.X;
		screenHeight = GetViewport().GetVisibleRect().Size.Y;

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Only spawn objects if the game is started
		if (gameStarted == false)
			return;

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
			CreateLakitu();
			timeToNextLakitu = GD.RandRange(minLakituTimer, maxLakituTimer);
			lakituTimer = 0;
		}
	}

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("start") && gameStarted == false)
		{
			gameStarted = true;
			totalScore = 0;
			totalScoreLabel.Text = "Score: 0";
			startLabel.Hide();

			flowers = new List<Flower>
        	{
            	CreateFlower(100, 708),
            	CreateFlower(200, 708),
            	CreateFlower(300, 708),
            	CreateFlower(400, 708)
        	};
		}
    }

    public void RemoveFlower(Flower flower) 
	{
		flowers.Remove(flower);
		
		if (flowers.Count == 0)
		{
			gameStarted = false;
			foreach (Node child in GetChildren())
			{
				child.QueueFree();
			}

			startLabel.Text = endText + totalScore.ToString();
			startLabel.Show();
		}
	}

	public Flower GetTarget()
	{
		int i = (int)(GD.Randi() % flowers.Count);
		return flowers[i];
	}

	public void CountScore(int bobombsHit, Queue<Vector2> positions)
	{
		int score = 0;

		for (int i=0; i<bobombsHit; i++)
		{
			score += baseScore * (int)Mathf.Pow(2, i);
			CreateLabel(positions.Dequeue(), baseScore * (int)Mathf.Pow(2, i));
		}

		totalScore += score;
		totalScoreLabel.Text = "Score: " + totalScore.ToString();
		GD.Print("New score: " + totalScore);
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

	public void Clear() 
	{
		// There may be an easier way to get a list of *only* the Bobombs
		Array<Node> children = GetChildren();
		foreach (Node child in children)
		{
			if (child is Bobomb)
			{
				Bobomb childBobomb = (Bobomb)child;

				if (childBobomb.state == Bobomb.BobombState.Flying)
				{
					CreateLabel(childBobomb.Position, baseScore);
					childBobomb.Die();
					totalScore += baseScore;
				}
				
			}
		}

		GD.Print("New score: " + totalScore);
	}

	private void CreateLabel(Vector2 pos, int score)
	{
		ScoreLabel instance = scoreLabel.Instantiate<ScoreLabel>();
		AddSibling(instance);
		instance.Position = pos;
		// _Ready normally will be called before setting the position
		// Would be nice if we could make a constructor for the ScoreLabel, 
		// 	but I don't think that's possible
		instance._Ready();
		instance.Text = score.ToString(); 
	}
}
