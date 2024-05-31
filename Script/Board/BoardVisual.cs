using Godot;
using System;
using System.Collections.Generic;

public partial class BoardVisual : Node2D
{
	#region Exported Variables
	[Export]
	PackedScene quad; //Scene to use as square of chess board
	[Export]
	PackedScene text; //Scene to be used to positional text
	[Export]
	PackedScene pieceInstance; //Piece Scene

	[Export]
	NodePath topLeftPosition; //Position place holder for board
	[Export]
	NodePath pieceContainer; //Holds are the pieces

	[ExportCategory("Board Color")]
	[Export]
	Color darkSquares = Colors.Black; //Colour of dark squares
	[Export]
	Color lightSquares = Colors.White; //Colour of Light Squares
	#endregion

	
	Marker2D positionPlaceHolder; //Child node that acts as position placeholder for board
	Node2D _piece_container; //Child node that store all pieces as its childs
	
	const float size = 50f; //Size of One Square of Chess Board in Px

	Board board; //Actual Board
	
	const bool _debugNumbering = false;
	private List<Piece> _pieces;
	
	public override void _Ready()
	{
		board = new Board();
		_pieces = new List<Piece>();
		
		positionPlaceHolder = GetNode<Marker2D>(topLeftPosition);
		_piece_container = GetNode<Node2D>(pieceContainer);

		Generate();
	}

	//Generates an Chess Board with initial configuration
	void Generate(){
		for(int i = 0; i < 8; ++i){
			for(int j = 0; j < 8; ++j){
				MeshInstance2D mesh = quad.Instantiate<MeshInstance2D>();
				mesh.Modulate = lightSquares;
				if(i%2==0 && j%2==1 || i%2==1 && j%2==0){
					mesh.Modulate = darkSquares;
				}
				mesh.GlobalPosition = (positionPlaceHolder.GlobalPosition.X + size/2 + size * j) * Vector2.Right + (positionPlaceHolder.GlobalPosition.Y  + size/2 + size * i) * Vector2.Down;
				if(_debugNumbering){
					Label label = text.Instantiate<Label>();
					label.Text = (i*8+j).ToString();
					label.GlobalPosition = mesh.GlobalPosition;
					label.ZIndex = 100;
					CallDeferred("add_child",label);
				}
				CallDeferred("add_child",mesh);
			}
		}
		board.InitialBoardConfig();
		for(int i = 0; i < 64; ++i){
				Vector2I boardPosition = IntToBoardPosition(i);
				Vector2 worldPosition = BoardPositionToGlobalPosition(boardPosition);
				
				Piece piece = pieceInstance.Instantiate<Piece>();
				piece.GlobalPosition = worldPosition;

				if(board.GetPiece((int)ColourType.WHITE_KING,i)){
					piece.Init(ColourType.WHITE_KING, IntToBoardPosition(i), this);
                    _pieces.Add(piece);
					_piece_container.CallDeferred("add_child",piece);
				}
				else if(board.GetPiece((int)ColourType.WHITE_QUEEN,i)){
					piece.Init(ColourType.WHITE_QUEEN, IntToBoardPosition(i), this);
                    _pieces.Add(piece);
					_piece_container.CallDeferred("add_child",piece);
				}
				else if(board.GetPiece((int)ColourType.WHITE_ROOK,i)){
					piece.Init(ColourType.WHITE_ROOK, IntToBoardPosition(i), this);
                    _pieces.Add(piece);
					_piece_container.CallDeferred("add_child",piece);
				}
				else if(board.GetPiece((int)ColourType.WHITE_BISHOP,i)){
					piece.Init(ColourType.WHITE_BISHOP, IntToBoardPosition(i), this);
                    _pieces.Add(piece);
					_piece_container.CallDeferred("add_child",piece);
				}
				else if(board.GetPiece((int)ColourType.WHITE_KNIGHT,i)){
					piece.Init(ColourType.WHITE_KNIGHT, IntToBoardPosition(i), this);
                    _pieces.Add(piece);
					_piece_container.CallDeferred("add_child",piece);
				}
				else if(board.GetPiece((int)ColourType.WHITE_PAWN,i)){
					piece.Init(ColourType.WHITE_PAWN, IntToBoardPosition(i), this);
                    _pieces.Add(piece);
					_piece_container.CallDeferred("add_child",piece);
				}
				else if(board.GetPiece((int)ColourType.BLACK_KING,i)){
					piece.Init(ColourType.BLACK_KING, IntToBoardPosition(i), this);
                    _pieces.Add(piece);
					_piece_container.CallDeferred("add_child",piece);
				}
				else if(board.GetPiece((int)ColourType.BLACK_QUEEN,i)){
					piece.Init(ColourType.BLACK_QUEEN, IntToBoardPosition(i), this);
                    _pieces.Add(piece);
					_piece_container.CallDeferred("add_child",piece);
				}
				else if(board.GetPiece((int)ColourType.BLACK_ROOK,i)){
					piece.Init(ColourType.BLACK_ROOK, IntToBoardPosition(i), this);
                    _pieces.Add(piece);
					_piece_container.CallDeferred("add_child",piece);
				}
				else if(board.GetPiece((int)ColourType.BLACK_BISHOP,i)){
					piece.Init(ColourType.BLACK_BISHOP, IntToBoardPosition(i), this);
                    _pieces.Add(piece);
					_piece_container.CallDeferred("add_child",piece);
				}
				else if(board.GetPiece((int)ColourType.BLACK_KNIGHT,i)){
					piece.Init(ColourType.BLACK_KNIGHT, IntToBoardPosition(i), this);
                    _pieces.Add(piece);
					_piece_container.CallDeferred("add_child",piece);
				}
				else if(board.GetPiece((int)ColourType.BLACK_PAWN,i)){
					piece.Init(ColourType.BLACK_PAWN, IntToBoardPosition(i), this);
                    _pieces.Add(piece);
					_piece_container.CallDeferred("add_child",piece);
				}
			}
	}

	public void UpdateBoard(int pieceIndex, Vector2I initialPosition, Vector2I finalPosition)
	{
		
	}

	#region Mapping Functions 
	//Converts global coordinates to coordinates of the chess board
	public Vector2I GlobalPositionToBoardPosition(Vector2 globalPos){
		globalPos -= positionPlaceHolder.GlobalPosition;
		Vector2I @out = new Vector2I((int)(globalPos.X/50.0f),(int)(globalPos.Y/50f));
		if(@out.X < 0 || @out.X >= 8 || @out.Y < 0 || @out.Y >= 8)
			@out = -1 * Vector2I.One;
		return @out;
	}
	//Converts board's coordinates to global coordinates
	public Vector2 BoardPositionToGlobalPosition(Vector2I globalPos){
		return (positionPlaceHolder.GlobalPosition.X + size/2 + size * globalPos.X) * Vector2.Right + (positionPlaceHolder.GlobalPosition.Y  + size/2 + size * globalPos.Y) * Vector2.Down;
	}
	//Convert index of position to board's position
	public Vector2I IntToBoardPosition(int pos){
		return new Vector2I(pos % 8, pos/8);
	}
	//Converts board's position to index of that position
	public int BoardPositionToInt(int x, int y)
	{
		return x * 8 + y;
	}
	#endregion
	
	
}
