using Godot;
using System;
using Chess.Script.Engine;

public partial class Promotes : VBoxContainer
{
	[Export] private NodePath _boardNodePath;

	private BoardVisual _boardVisual;

	private OptionButton _white;

	private OptionButton _black;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_boardVisual = GetNode<BoardVisual>(_boardNodePath);
		_white = GetNode<OptionButton>("OptionButton");
		_black = GetNode<OptionButton>("OptionButton3");
	}


	public override void _PhysicsProcess(double delta)
	{
		if (_white.Selected == 0)
		{
			_boardVisual.WhitePromoteTo = (int)ColourType.WHITE_QUEEN;
		}
		else if (_white.Selected == 1)
		{
			_boardVisual.WhitePromoteTo = (int)ColourType.WHITE_ROOK;
		}
		else if (_white.Selected == 2)
		{
			_boardVisual.WhitePromoteTo = (int)ColourType.WHITE_BISHOP;
		}
		else if (_white.Selected == 3)
		{
			_boardVisual.WhitePromoteTo = (int)ColourType.WHITE_KNIGHT;
		}
		
		if (_black.Selected == 0)
		{
			_boardVisual.BlackPromoteTo = (int)ColourType.BLACK_QUEEN;
		}
		else if (_black.Selected == 1)
		{
			_boardVisual.BlackPromoteTo = (int)ColourType.BLACK_ROOK;
		}
		else if (_black.Selected == 2)
		{
			_boardVisual.BlackPromoteTo = (int)ColourType.BLACK_BISHOP;
		}
		else if (_black.Selected == 3)
		{
			_boardVisual.BlackPromoteTo = (int)ColourType.BLACK_KNIGHT;
		}
	}
	
}
