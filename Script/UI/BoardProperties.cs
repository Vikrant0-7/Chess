using Godot;
using System;

public partial class BoardProperties : VBoxContainer
{
	[Export]
	public NodePath board;
	// Called when the node enters the scene tree for the first time.
	private BoardVisual _visual;
	public override void _Ready()
	{
		_visual = GetNode<BoardVisual>(board);
		GetNode<CheckButton>("Moves/CheckButton2").Toggled += (on) => _OnMovesToggled(on);
		GetNode<CheckButton>("Labels/CheckButton").Toggled += (on) => _OnLabelsToggled(on);
		GetNode<OptionButton>("Attacks/OptionButton").ItemSelected += (what) => _OnAttacksItemSelected(what);
		GetNode<Button>("Refresh").Pressed += () => _OnRefreshBoardPressed();
		GetNode<Button>("Reset").Pressed += () => _OnResetBoardPressed();
	}

	void _OnMovesToggled(bool toggled)
	{
		if (_visual.showAttacks == 0)
		{
			_visual.showMoves = toggled;
		}
		else
		{
			GetNode<CheckButton>("Moves/CheckButton2").ButtonPressed = false;
		}
	}

	void _OnLabelsToggled(bool toggled)
	{
		_visual.GetNode<Node2D>("%Labels").Visible = toggled;
	}

	void _OnAttacksItemSelected(long item)
	{
		if(!_visual.showMoves)
			_visual.showAttacks = (int)item;
		else
		{
			GetNode<OptionButton>("Attacks/OptionButton").Selected = 0;
		}
	}

	void _OnRefreshBoardPressed()
	{
		_visual.RefreshBoard();
	}

	void _OnResetBoardPressed()
	{
		_visual.Reset();
	}
}
