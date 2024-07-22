using Godot;
using System;
using Chess.Script.Engine;

public partial class GameEnd : Control
{
	// Called when the node enters the scene tree for the first time.

	private readonly string[] lostMessage =
	{
		"You Lost",
		"You are cooked",
		"You have been eaten by a fish",
		"You really thought you were going to win",
		"What Else you expected",
		"Don't let computers cook for long. They might cook you",
	};

	private readonly string[] wonMessage =
	{
		"You Won",
		"Damn Bro!",
		"Must been hard catching a fish already wounded and stuck in a net",
		"You cooked fish",
		"I dare you to beat my elder brother named \"real stockfish\"",
		"That cannot happen!!!"
	};

	private readonly string[] drawMessage = 
	{
		"Draw",
		"You are barely alive",
		"Fish let you see another sun rise\n(if you managed to wake up early)",
	};

	private Label _msg;
	
	public override void _Ready()
	{
		Hide();
		Board.GameEnd += End;
		_msg = GetNode<Label>("%msg");
	}


	void End(bool isWhite, bool mate)
	{
		GD.Randomize();
		Show();
		if (mate)
		{
			if (isWhite)
			{

				_msg.Text = lostMessage[GD.RandRange(0, lostMessage.Length - 1)];
			}
			else
			{
				_msg.Text = wonMessage[GD.RandRange(0, wonMessage.Length - 1)];
			}
		}
		else
		{
			_msg.Text = drawMessage[GD.RandRange(0, drawMessage.Length - 1)];
		}
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _ExitTree()
	{
		Board.GameEnd -= End;
	}
}
