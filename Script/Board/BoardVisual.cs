using Godot;
using System;
using System.Collections.Generic;

public partial class BoardVisual : Node2D
{
	#region Exported Variables
	[Export] PackedScene _quad; //Scene to use as square of chess board
	[Export] PackedScene _text; //Scene to be used to positional text
	[Export] PackedScene _pieceInstance; //Piece Scene


	[ExportCategory("Board Color")]
	[Export] private Color _darkSquares = Colors.Black; //Colour of dark squares
	[Export] private Color _lightSquares = Colors.White; //Colour of Light Squares
	[Export] private Color _PathDarkSquare = Colors.DarkRed;
	[Export] private Color _PathLightSquare = Colors.Red;
	#endregion

	
	Marker2D _positionPlaceHolder; //Child node that acts as position placeholder for board
	Node2D _pieceContainer; //Child node that store all pieces as its childs
	private Node2D _labelContainer;
	private Node2D _squareContainer;
	
	const float size = 50f; //Size of One Square of Chess Board in Px

	public Board board; //Actual Board
	
	const bool _debugNumbering = true;
	private bool _moved = false;
	
	public override void _Ready()
	{
		board = new Board();
		
		_positionPlaceHolder = GetNode<Marker2D>("Marker");
		_pieceContainer = GetNode<Node2D>("Marker/Piece");
		_labelContainer = GetNode<Node2D>("Marker/Labels");
		_squareContainer = GetNode<Node2D>("Marker/Squares");

		_squareContainer.Position -= _positionPlaceHolder.Position;
		_pieceContainer.Position -= _positionPlaceHolder.Position;
		_labelContainer.Position -= _positionPlaceHolder.Position;
		
		Generate();
	}

	//Generates an Chess Board with initial configuration
	void Generate(){
		for(int i = 0; i < 8; ++i){
			for(int j = 0; j < 8; ++j){
				MeshInstance2D mesh = _quad.Instantiate<MeshInstance2D>();
				mesh.Modulate = _lightSquares;
				if(i%2==0 && j%2==1 || i%2==1 && j%2==0){
					mesh.Modulate = _darkSquares;
				}
				mesh.GlobalPosition = (_positionPlaceHolder.GlobalPosition.X + size/2 + size * j) * Vector2.Right + (_positionPlaceHolder.GlobalPosition.Y  + size/2 + size * i) * Vector2.Down;
				if(_debugNumbering){
					Label label = _text.Instantiate<Label>();
					label.Text = (i*8+j).ToString();
					label.GlobalPosition = mesh.GlobalPosition;
					label.ZIndex = 100;
					_labelContainer.CallDeferred("add_child",label);
				}
				_squareContainer.CallDeferred("add_child",mesh);
			}
		}
		board.InitialBoardConfig();
		SetBoard();
	}

	public void SetBoard()
	{
		for (int i = 0; i < _pieceContainer.GetChildCount(); ++i)
		{
			_pieceContainer.GetChild(i).QueueFree();
		}
		for(int i = 0; i < 64; ++i){
			Vector2I boardPosition = IntToBoardPosition(i);
			Vector2 worldPosition = BoardPositionToGlobalPosition(boardPosition);
				
			Piece piece = _pieceInstance.Instantiate<Piece>();
			piece.GlobalPosition = worldPosition;

			if(board.GetPiece((int)ColourType.WHITE_KING,i)){
				piece.Init(ColourType.WHITE_KING, IntToBoardPosition(i), this);
				_pieceContainer.CallDeferred("add_child",piece);
			}
			else if(board.GetPiece((int)ColourType.WHITE_QUEEN,i)){
				piece.Init(ColourType.WHITE_QUEEN, IntToBoardPosition(i), this);
				_pieceContainer.CallDeferred("add_child",piece);
			}
			else if(board.GetPiece((int)ColourType.WHITE_ROOK,i)){
				piece.Init(ColourType.WHITE_ROOK, IntToBoardPosition(i), this);
				_pieceContainer.CallDeferred("add_child",piece);
			}
			else if(board.GetPiece((int)ColourType.WHITE_BISHOP,i)){
				piece.Init(ColourType.WHITE_BISHOP, IntToBoardPosition(i), this);
				_pieceContainer.CallDeferred("add_child",piece);
			}
			else if(board.GetPiece((int)ColourType.WHITE_KNIGHT,i)){
				piece.Init(ColourType.WHITE_KNIGHT, IntToBoardPosition(i), this);
				_pieceContainer.CallDeferred("add_child",piece);
			}
			else if(board.GetPiece((int)ColourType.WHITE_PAWN,i)){
				piece.Init(ColourType.WHITE_PAWN, IntToBoardPosition(i), this);
				_pieceContainer.CallDeferred("add_child",piece);
			}
			else if(board.GetPiece((int)ColourType.BLACK_KING,i)){
				piece.Init(ColourType.BLACK_KING, IntToBoardPosition(i), this);
				_pieceContainer.CallDeferred("add_child",piece);
			}
			else if(board.GetPiece((int)ColourType.BLACK_QUEEN,i)){
				piece.Init(ColourType.BLACK_QUEEN, IntToBoardPosition(i), this);
				_pieceContainer.CallDeferred("add_child",piece);
			}
			else if(board.GetPiece((int)ColourType.BLACK_ROOK,i)){
				piece.Init(ColourType.BLACK_ROOK, IntToBoardPosition(i), this);
				_pieceContainer.CallDeferred("add_child",piece);
			}
			else if(board.GetPiece((int)ColourType.BLACK_BISHOP,i)){
				piece.Init(ColourType.BLACK_BISHOP, IntToBoardPosition(i), this);
				_pieceContainer.CallDeferred("add_child",piece);
			}
			else if(board.GetPiece((int)ColourType.BLACK_KNIGHT,i)){
				piece.Init(ColourType.BLACK_KNIGHT, IntToBoardPosition(i), this);
				_pieceContainer.CallDeferred("add_child",piece);
			}
			else if(board.GetPiece((int)ColourType.BLACK_PAWN,i)){
				piece.Init(ColourType.BLACK_PAWN, IntToBoardPosition(i), this);
				_pieceContainer.CallDeferred("add_child",piece);
			}
		}
	}

	public bool UpdateBoard(int pieceIndex, Vector2I initialPosition, Vector2I finalPosition)
	{
		bool capture = false;
		bool moveValid = false;
		moveValid = board.Move(pieceIndex, BoardPositionToInt(initialPosition), 
			BoardPositionToInt(finalPosition), out capture);
		_moved = true;
		return moveValid;
	}

	public void ShowMoves(int pieceIndex, Vector2I initialPosition)
	{
		List<int> moves = null;
		Colour c = (pieceIndex > 5) ? Colour.BLACK : Colour.WHITE;
		if (pieceIndex == 5 || pieceIndex == 11)
			moves = MoveValidity.Pawn(c, board.BoardStatus, board.BoardSnapshot, BoardPositionToInt(initialPosition));
		if(pieceIndex == 4 || pieceIndex == 10)
			moves = MoveValidity.Knight(c, board.BoardStatus, BoardPositionToInt(initialPosition));

		if (moves != null)
		{
			foreach (var item in moves)
			{
				if (item < 0 || item >= 64)
				{
					GD.Print("Item Out of range");
					continue;
				}
				MeshInstance2D sq = _squareContainer.GetChild<MeshInstance2D>(item);
				if (sq.Modulate == _darkSquares)
					sq.Modulate = _PathDarkSquare;
				else if (sq.Modulate == _lightSquares)
					sq.Modulate = _PathLightSquare;
			}
		}
	}

	public void Reset()
	{
		foreach (var item in _squareContainer.GetChildren())
		{
			
			if ((item as MeshInstance2D).Modulate == _PathDarkSquare)
				(item as MeshInstance2D).Modulate = _darkSquares;
			else if ((item as MeshInstance2D).Modulate == _PathLightSquare)
				(item as MeshInstance2D).Modulate = _lightSquares;
		}
	}

	#region Mapping Functions 
	//Converts global coordinates to coordinates of the chess board
	public Vector2I GlobalPositionToBoardPosition(Vector2 globalPos){
		globalPos -= _positionPlaceHolder.GlobalPosition;
		Vector2I @out = new Vector2I((int)(globalPos.X/50.0f),(int)(globalPos.Y/50f));
		if(@out.X < 0 || @out.X >= 8 || @out.Y < 0 || @out.Y >= 8)
			@out = -1 * Vector2I.One;
		return @out;
	}
	//Converts board's coordinates to global coordinates
	public Vector2 BoardPositionToGlobalPosition(Vector2I globalPos){
		return (_positionPlaceHolder.GlobalPosition.X + size/2 + size * globalPos.X) * Vector2.Right + (_positionPlaceHolder.GlobalPosition.Y  + size/2 + size * globalPos.Y) * Vector2.Down;
	}
	//Convert index of position to board's position
	public Vector2I IntToBoardPosition(int pos){
		return new Vector2I(pos % 8, pos/8);
	}
	//Converts board's position to index of that position
	public int BoardPositionToInt(int x, int y)
	{
		return y * 8 + x;
	}

	public int BoardPositionToInt(Vector2I pos)
	{
		return pos.Y * 8 + pos.X;
	}
	#endregion


	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("ui_accept") || _moved)
		{
			_moved = false;
			SetBoard();
		}
		GD.Print(BoardPositionToInt(GlobalPositionToBoardPosition(GetGlobalMousePosition())));
	}
}