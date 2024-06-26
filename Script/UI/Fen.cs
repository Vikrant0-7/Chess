using Godot;
using System;

using Chess.Script.Engine;

public partial class Fen : VBoxContainer
{
	[Export] private NodePath _boardNodePath;

	private BoardVisual _boardVisual;
	private LineEdit _input;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_boardVisual = GetNode<BoardVisual>(_boardNodePath);
		_input = GetNode<LineEdit>("LineEdit");

		GetNode<Button>("Button/Fen").Pressed += () => _OnSetBoardPressed();
		GetNode<Button>("Button/Board").Pressed += () => _OnGetFenPressed();
	}

	void _OnSetBoardPressed()
	{
		string str = _input.Text.Trim();
		try
		{
			FenTranslator translator = new FenTranslator(str);
			_boardVisual.board.BoardConfig(translator);
			_boardVisual.RefreshBoard();
		}
		catch (Exception e)
		{
			GD.Print(e);
		}
	}

	void _OnGetFenPressed()
	{
		Board board = _boardVisual.board;

		FenTranslator translator = new FenTranslator(board.BoardStatus, 
			(byte)(board.GetCastleStatus(true) | 
			          (board.GetCastleStatus(false) << 4)), 
			board.EnPassantPosition, board.WhiteTurn, new []{board.FiftyMoveRule, board.FullMoves});
		
		_input.Text = translator.FenString;

	}
	
}
