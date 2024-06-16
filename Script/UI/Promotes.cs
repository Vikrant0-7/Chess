using Godot;
using System;

public partial class Promotes : VBoxContainer
{
	[Export] private NodePath _boardNodePath;

	private BoardVisual _boardVisual;

	private OptionButton white;

	private OptionButton black;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_boardVisual = GetNode<BoardVisual>(_boardNodePath);
		white = GetNode<OptionButton>("OptionButton");
		black = GetNode<OptionButton>("OptionButton3");

	}


	public override void _PhysicsProcess(double delta)
	{
		_boardVisual.board.WhitePromoteTo = white.Selected + 1;
		_boardVisual.board.BlackPromoteTo = black.Selected + 1;
	}

	void _OnWhite(long id)
	{
		_boardVisual.board.WhitePromoteTo = (int)id + 1;
		GD.Print(_boardVisual.board.WhitePromoteTo);
	}

	void _OnBlack(long id)
	{
		_boardVisual.board.BlackPromoteTo = (int)id + 1;

	}
}
