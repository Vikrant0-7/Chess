using Godot;
using System.Threading.Tasks;
using System.Collections.Generic;

using Chess.Script.Engine;
using Chess.Script.Engine.Moves;

public partial class BoardVisual : Node2D
{
	#region Exported Variables
	[Export] PackedScene _quad; //Scene to use as square of chess board
	[Export] PackedScene _text; //Scene to be used to positional text
	[Export] PackedScene _pieceInstance; //Piece Scene
	[Export] private bool _aiIsWhite;


	[ExportCategory("Board Color")]
	[Export] private Color _darkSquares = Colors.Black; //Colour of dark squares
	[Export] private Color _lightSquares = Colors.White; //Colour of Light Squares
	[Export] private Color _pathDarkSquare = Colors.DarkGreen;
	[Export] private Color _pathLightSquare = Colors.Green;
	[Export] private Color _attackDarkSqaure = Colors.DarkRed;
	[Export] private Color _attackLighSquare = Colors.Red;

	[Export] private double _time;
	#endregion
	
	Marker2D _positionPlaceHolder; //Child node that acts as position placeholder for board
	Node2D _pieceContainer; //Child node that store all pieces as its childs
	private Node2D _labelContainer;
	private Node2D _squareContainer;
	
	//very important
	private bool _moved = false; //variable is used to update board once a piece is moved
	public float SIZE = 50f; //Size of One Square of Chess Board in Px
	Board _board; //Actual Board
	private List<Piece> _pieces;

	public bool showMoves = false;
	public int pinAttack = 0;
	public Board board => _board;
	public int WhitePromoteTo = (int)ColourType.WHITE_QUEEN;
	public int BlackPromoteTo = (int)ColourType.BLACK_QUEEN;
	public bool AiIsWhite => _aiIsWhite;
	
	public delegate void MoveMadeEvent(bool isWhite);

	public event MoveMadeEvent MoveMade;
	
	
	//Generates an Chess Board with initial configuration
	void Generate(){
		for(int i = 0; i < 8; ++i){
			for (int j = 0; j < 8; ++j)
			{
				MeshInstance2D mesh = _quad.Instantiate<MeshInstance2D>();
				mesh.Modulate = _lightSquares;
				if (i % 2 == 0 && j % 2 == 1 || i % 2 == 1 && j % 2 == 0)
				{
					mesh.Modulate = _darkSquares;
				}

				mesh.GlobalPosition = (_positionPlaceHolder.GlobalPosition.X + SIZE / 2 + SIZE * j) * Vector2.Right +
									  (_positionPlaceHolder.GlobalPosition.Y + SIZE / 2 + SIZE * i) * Vector2.Down;
				Label label = _text.Instantiate<Label>();
				label.Text = "abcdefgh"[j].ToString() + "87654321"[i];
				label.GlobalPosition = mesh.GlobalPosition;
				label.ZIndex = 100;
				_labelContainer.CallDeferred("add_child", label);

				_squareContainer.CallDeferred("add_child", mesh);
			}
		}

		_board.InitialBoardConfig();
		SetBoard();
	}

	void ShowAttacks()
	{
		if (pinAttack == 1)
		{
			for (int i = 0; i < 64; ++i)
			{
				if ((_board.CurrAttackBitboard & (ulong)1 << i) != 0)
				{
					MeshInstance2D sq = _squareContainer.GetChild<MeshInstance2D>(i);
					if (sq.Modulate == _darkSquares)
						sq.Modulate = _attackDarkSqaure;
					else if (sq.Modulate == _lightSquares)
						sq.Modulate = _attackLighSquare;
				}
			}
		}
	}

	void SetBoard()
	{
		if (_pieces.Count == 0)
		{
			foreach (var item in _pieceContainer.GetChildren())
			{
				item.QueueFree();
			}
			for (int i = 0; i < 32; ++i)
			{
				Piece piece = _pieceInstance.Instantiate<Piece>();
				_pieces.Add(piece);
				_pieceContainer.CallDeferred("add_child", piece);
			}
		}
		int pieceIdx = 0;
		for (int i = 0; i < 64; ++i)
		{
			if (board.BoardStatus[i] != (int)ColourType.FREE)
			{
				Vector2I boardPosition = Functions.IntToBoardPosition(i);

				if (pieceIdx >= _pieces.Count)
				{
					_pieces.Clear();
					SetBoard();
				}
				Piece piece = _pieces[pieceIdx++];

				piece.Init((ColourType)_board.BoardStatus[i], boardPosition, this);
			}
		}
		if (pieceIdx < _pieces.Count)
		{
			for (int i = pieceIdx; pieceIdx < _pieces.Count; ++i)
			{
				if(!_pieces[i].IsQueuedForDeletion())
					_pieces[i].QueueFree();
			}
			_pieces.RemoveRange(pieceIdx, _pieces.Count - pieceIdx);
		}
	}

	public bool HumanMakeMove(int pieceIndex, Vector2I initialPosition, Vector2I finalPosition)
	{
		Move m;
		bool legal = false;
		int promoteTo = -1;
		
		if ((pieceIndex - 1) % 6 == (int)ColourType.WHITE_PAWN - 1)
		{
			
			if (pieceIndex == (int)ColourType.WHITE_PAWN && initialPosition.Y == 1)
				promoteTo = WhitePromoteTo;
			else if (pieceIndex == (int)ColourType.BLACK_PAWN && initialPosition.Y == 6)
				promoteTo = BlackPromoteTo;
		}
		
		m = new Move(
			Functions.BoardPositionToInt(initialPosition),
			pieceIndex,
			Functions.BoardPositionToInt(finalPosition),
			promoteTo
		);
		legal = board.Move(m, out _, true);

		if (legal)
		{
			MoveMade?.Invoke(_board.WhiteTurn);
		}

		_moved = true;
		
		return legal;
	}

	public void ShowMoves(int pieceIndex, Vector2I initialPosition)
	{
		if(_board.WhiteTurn == _aiIsWhite)
			return;
		
		if (!showMoves || (_board.WhiteTurn && pieceIndex > 6) || (!_board.WhiteTurn && pieceIndex <= 6))
		{
			return;
		}

		List<Move> moves = null;
		if ((pieceIndex - 1) % 6 == (int)ColourType.WHITE_PAWN - 1)
		{
			moves = LegalMoves.Pawn(
				_board,
				Functions.BoardPositionToInt(initialPosition)
			);
		}
		else if ((pieceIndex - 1) % 6 >= (int)ColourType.WHITE_QUEEN - 1 &&
		         (pieceIndex - 1) % 6 <= (int)ColourType.WHITE_BISHOP - 1)
		{
			moves = LegalMoves.SlidingMoves(
				_board,
				Functions.BoardPositionToInt(initialPosition),
				pieceIndex
			);
		}
		else if ((pieceIndex - 1) % 6 >= (int)ColourType.WHITE_KNIGHT - 1)
		{
			moves = LegalMoves.Knight(_board, Functions.BoardPositionToInt(initialPosition));
		}
		else
		{
			moves = LegalMoves.King(_board, Functions.BoardPositionToInt(initialPosition));
		}

		if (moves != null)
		{
			foreach (var item in moves)
			{
				MeshInstance2D mesh = _squareContainer.GetChild<MeshInstance2D>(item.finalPosition);
				if (mesh.Modulate == _darkSquares)
					mesh.Modulate = _pathDarkSquare;
				else if(mesh.Modulate == _lightSquares)
					mesh.Modulate = _pathLightSquare;
			}
		}
	}

	public void ResetColors()
	{
		foreach (var item in _squareContainer.GetChildren())
		{
			
			if ((item as MeshInstance2D).Modulate == _pathDarkSquare || 
					(item as MeshInstance2D).Modulate == _attackDarkSqaure)
				(item as MeshInstance2D).Modulate = _darkSquares;
			else if ((item as MeshInstance2D).Modulate == _pathLightSquare ||
					 (item as MeshInstance2D).Modulate == _attackLighSquare)
				(item as MeshInstance2D).Modulate = _lightSquares;
		}
	}
	
	public void RefreshBoard()
	{
		ResetColors();
		SetBoard();
		if (pinAttack != 0)
		{
			ShowAttacks();
		}
	}

	public void Reset()
	{
		board.InitialBoardConfig();
		RefreshBoard();
		SetBoard();
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
		return (_positionPlaceHolder.GlobalPosition.X + SIZE/2 + SIZE * globalPos.X) * Vector2.Right + (_positionPlaceHolder.GlobalPosition.Y  + SIZE/2 + SIZE * globalPos.Y) * Vector2.Down;
	}
	#endregion

	public override void _Ready()
	{
		_board = new Board();
		_pieces = new List<Piece>();
		
		_positionPlaceHolder = GetNode<Marker2D>("Marker");
		_pieceContainer = GetNode<Node2D>("Marker/Piece");
		_labelContainer = GetNode<Node2D>("Marker/Labels");
		_squareContainer = GetNode<Node2D>("Marker/Squares");

		_squareContainer.Position -= _positionPlaceHolder.Position;
		_pieceContainer.Position += Vector2.One * 23;
		_labelContainer.Position -= _positionPlaceHolder.Position;
		
		Generate();
	}

	public override void _Process(double delta)
	{
		if (_moved)
		{
			_moved = false;
			RefreshBoard();
		}
	}
}

