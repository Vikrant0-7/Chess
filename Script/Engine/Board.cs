using Godot;
using System;
using System.Threading;
using System.Collections.Generic;

using Chess.Script.Engine.Bitboards;
using Chess.Script.Engine.Moves;

namespace Chess.Script.Engine;

//TODO: Add check for mate condition.

public class Board
{
	private int[] _boardStatus;
	private int _enPassantPosition;
	private bool _whiteTurn = true;
	private int _castleStatus;

	private MoveGenerator _moveGenerator;
	private int _50MoveRule;
	private int _fullMoves;
	private int _whiteKingPosition;
	private int _blackKingPosition;

	private List<int> _squaresWithMoves;

	private ulong _attackBitboard;

	public delegate void MoveMadeEventHandler(Move move, bool capture);

	public delegate void CheckEventHandler(bool isWhite);

	public delegate void GameEndEventHandler(bool isWhite, bool isMate);
	
	public static MoveMadeEventHandler MoveMade;
	public static event CheckEventHandler Check;
	public static event GameEndEventHandler GameEnd;

	public int EnPassantPosition => _enPassantPosition;
	public int[] BoardStatus => _boardStatus;

	public int FiftyMoveRule
	{
		set => _50MoveRule = value;
		get => _50MoveRule;
	}

	public int FullMoves
	{
		set => _fullMoves = value;
		get => _fullMoves;
	}

	public bool WhiteTurn
	{
		set => _whiteTurn = value;
		get => _whiteTurn;
	}

	public int WhiteKingPosition => _whiteKingPosition;
	public int BlackKingPosition => _blackKingPosition;

	public ulong CurrAttackBitboard => _attackBitboard;

	public List<int> SquaresWithMoves => _squaresWithMoves;
	
	public Board()
	{
		PrecomputedMoves.Init();
		
		_boardStatus = new int[64];
		_enPassantPosition = -1;
		_50MoveRule = 0;
		_fullMoves = 1;
		_castleStatus = 0;
		_moveGenerator = new MoveGenerator(this);
	}

	bool MovePawn(Move move, out bool capture, bool checkLegal)
	{
		capture = false;
		List<Move> legal = new List<Move>();
		if (checkLegal)
			legal = LegalMoves.Pawn(this, move.position);
		if (!checkLegal || legal.Contains(move))
		{
			_boardStatus[move.position] = (int)ColourType.FREE;
			if (_boardStatus[move.finalPosition] != (int)ColourType.FREE)
			{
				capture = true;
			}

			if (!capture && move.finalPosition == _enPassantPosition)
			{
				_boardStatus[move.finalPosition + (_whiteTurn ? 8 : -8)] = (int)ColourType.FREE;
				capture = true;
			}

			if (move.promoteTo != -1)
			{
				_boardStatus[move.finalPosition] = move.promoteTo;
			}
			else
			{
				_boardStatus[move.finalPosition] = move.piece;
			}
			
			if (Mathf.Abs(move.finalPosition - move.position) == 16)
			{
				_enPassantPosition = move.finalPosition + (_whiteTurn ? 8 : -8);
			}
			else
			{
				_enPassantPosition = -1;
			}
			
			return true;
		}

		return false;
	}

	bool MoveQRBN(Move move, out bool capture, bool checkLegal)
	{
		capture = false;
		List<Move> moves = new List<Move>();
		if (checkLegal)
		{
			if ((move.piece - 1) % 6 != (int)ColourType.WHITE_KNIGHT - 1)
				moves = LegalMoves.SlidingMoves(this, move.position, move.piece);
			else
				moves = LegalMoves.Knight(this, move.position);
		}

		if (!checkLegal || moves.Contains(move))
		{
			if (_boardStatus[move.finalPosition] != (int)ColourType.FREE)
				capture = true;

			_boardStatus[move.finalPosition] = move.piece;
			_boardStatus[move.position] = 0;

			return true;
		}
		
		return false;
	}

	bool MoveKing(Move move, out bool capture, bool checkLegal)
	{
		capture = false;
		List<Move> moves = new List<Move>();
		if (checkLegal)
		{
			moves = LegalMoves.King(this, move.position);
		}

		if (!checkLegal || moves.Contains(move))
		{
			if (_boardStatus[move.finalPosition] != (int)ColourType.FREE)
				capture = true;

			if (Mathf.Abs(move.finalPosition - move.position) == 2)
			{
				int modifier = move.finalPosition < move.position ? 1 : -1;

				_boardStatus[move.finalPosition + modifier] =
					(int)(_whiteTurn ? ColourType.WHITE_ROOK : ColourType.BLACK_ROOK);

				if (modifier == -1)
				{
					_boardStatus[move.position + 3] = (int)ColourType.FREE;
				}
				else
				{
					_boardStatus[move.position - 4] = (int)ColourType.FREE;

				}
			}
			_boardStatus[move.position] = (int)ColourType.FREE;
			_boardStatus[move.finalPosition] = move.piece;

			if (move.piece == (int)ColourType.WHITE_KING)
				_whiteKingPosition = move.finalPosition;
			else
				_blackKingPosition = move.finalPosition;
			
			return true;
		}

		return false;
	}

	public int GetCastleStatus(bool isWhite)
	{
		if (isWhite)
		{
			return (byte)(_castleStatus << 4) >> 4;
		}
		else
		{
			return _castleStatus >> 4;
		}
	}

	public String GetCastlingString()
	{
		String @out = "";
		if (_castleStatus == 0)
			@out = "-";
		int castle = GetCastleStatus(true);
		if ((castle & 0b01) != 0)
			@out += "K";
		if ((castle & 0b10) != 0)
			@out += "Q";
		
		castle = GetCastleStatus(false);
		if ((castle & 0b01) != 0)
			@out += "k";
		if ((castle & 0b10) != 0)
			@out += "q";
		return @out;
	}

	public void SetCastlingState(bool isWhite, bool queen = false, bool king = false)
	{
		int castle = 0;
		if (king)
		{
			castle |= 0b01;
		}

		if (queen)
		{
			castle |= 0b10;
		}

		if (isWhite)
		{
			_castleStatus = ((_castleStatus >> 4) << 4) | castle;
		}
		else
		{
			_castleStatus = ((byte)(_castleStatus << 4) >> 4) | (castle << 4);
		}

		_castleStatus = (_castleStatus << 16) >> 16;
	}

	public void SetBoardPosition(int position, int piece)
	{
		if (_boardStatus[position] == piece)
			_boardStatus[position] = (int)ColourType.FREE;
		else
			_boardStatus[position] = piece;
		
		_attackBitboard = Bitboard.Attacks(!_whiteTurn, _boardStatus);
	}

	public void CleanBoard()
	{
		_boardStatus = new int[64];
		_enPassantPosition = -1;
		_50MoveRule = 0;
		_fullMoves = 1;
		_castleStatus = 0;
		_squaresWithMoves = _moveGenerator.GetLegalMovesSquare();
	}


	//Initializes board to its initial config.
	public void InitialBoardConfig()
	{
		FenTranslator fen = new FenTranslator(
			"rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"
		);
		BoardConfig(fen);
	}

	public void BoardConfig(FenTranslator fen)
	{
		_boardStatus = fen.BoardStatus;
		SetCastlingState(true,fen.WhiteCastleStatus[0],fen.WhiteCastleStatus[1]);
		SetCastlingState(false,fen.BlackCastleStatus[0],fen.BlackCastleStatus[1]);
		_whiteTurn = fen.WhiteTurn;
		_enPassantPosition = fen.EnPassantSquare;
		_attackBitboard = Bitboard.Attacks(!_whiteTurn, _boardStatus);
		_50MoveRule = fen.Moves[0];
		_fullMoves = fen.Moves[1];

		int b = 0;
		for (int i = 0; i < 64; ++i)
		{
			if (_boardStatus[i] == (int)ColourType.WHITE_KING)
			{
				_whiteKingPosition = i;
				++b;
			}
			else if (_boardStatus[i] == (int)ColourType.BLACK_KING)
			{
				_blackKingPosition = i;
				++b;
			}
			
			if(b == 2)
				break;
		}
		_squaresWithMoves = _moveGenerator.GetLegalMovesSquare();
	}

	public bool Move(Move move, out bool capture, bool checkLegal = true)
	{
		capture = false;
		bool valid = false;
		if (Functions.IsPieceFriendly(_whiteTurn, _boardStatus[move.position]) == 0)
			return false;

		if ((move.piece - 1) % 6 == (int)ColourType.WHITE_PAWN - 1)
		{
			valid = MovePawn(move, out capture, checkLegal);
		}
		else if ((move.piece - 1) % 6 >= (int)ColourType.WHITE_QUEEN - 1 &&
		         (move.piece - 1) % 6 <= (int)ColourType.WHITE_KNIGHT - 1)
		{
			valid = MoveQRBN(move, out capture, checkLegal);
		}
		else
		{
			valid = MoveKing(move, out capture, checkLegal);
			if (valid)
			{
				SetCastlingState(_whiteTurn);
			}
		}

		if (valid)
		{
			MoveMade?.Invoke(move, capture);
			_whiteTurn = !_whiteTurn;
			if ((move.piece - 1) % 6 != (int)ColourType.WHITE_PAWN - 1)
			{
				_enPassantPosition = -1;
			}

			++_50MoveRule;
			if (move.piece == (int)ColourType.WHITE_PAWN ||
			    move.piece == (int)ColourType.BLACK_PAWN ||
			    capture)
			{
				_50MoveRule = 0;
			}

			if (_whiteTurn)
				++FullMoves;
			
			_attackBitboard = Bitboard.Attacks(!_whiteTurn, _boardStatus);


			if ((_castleStatus & 0b01) != 0 && _boardStatus[63] != (int)ColourType.WHITE_ROOK)
			{
				SetCastlingState(true, (_castleStatus & 0b10) != 0, false);
			}
			if ((_castleStatus & 0b10) != 0 && _boardStatus[56] != (int)ColourType.WHITE_ROOK)
			{
				SetCastlingState(true, false, (_castleStatus & 0b01) != 0);
			}
			
			if ((_castleStatus & 0b01_0000) != 0 && _boardStatus[7] != (int)ColourType.BLACK_ROOK)
			{
				SetCastlingState(false, (_castleStatus & 0b10_0000) != 0, false);
			}
			if ((_castleStatus & 0b10_0000) != 0 && _boardStatus[0] != (int)ColourType.BLACK_ROOK)
			{
				SetCastlingState(false, false, (_castleStatus & 0b01_0000) != 0);
			}

			_squaresWithMoves = _moveGenerator.GetLegalMovesSquare();

			bool check = false;
			if (Functions.CheckBit
			    (_attackBitboard,
				    _whiteTurn ? _whiteKingPosition : _blackKingPosition)
			   )
			{
				Check?.Invoke(_whiteTurn);
				check = true;
			}

			if (_squaresWithMoves.Count == 0)
			{
				GameEnd?.Invoke(_whiteTurn, check);
			}
			
		}
		
		return valid;
	}
	
	public MoveData MakeMove(Move move, bool checkLegal = false)
	{
		MoveData d = new MoveData();
		d.move = move;
		d.boardStatus = (int[])_boardStatus.Clone();

		d.bitboards = new[] { _attackBitboard };
		d.data = new[]
		{
			_enPassantPosition,
			_castleStatus,
			_50MoveRule,
			_fullMoves,
			_whiteKingPosition,
			_blackKingPosition
		};

		d.whiteTurn = WhiteTurn;
		
		Move(move, out _, checkLegal);
		return d;

	}

	public void UnmakeMove(MoveData move)
	{
		_boardStatus = move.boardStatus;
		_attackBitboard = move.bitboards[0];
		_whiteTurn = move.whiteTurn;
		(
			_enPassantPosition,
			_castleStatus,
			_50MoveRule,
			_fullMoves,
			_whiteKingPosition,
			_blackKingPosition
		) = (
			move.data[0],
			move.data[1],
			move.data[2],
			move.data[3],
			move.data[4],
			move.data[5]
		);
	}

public void RequestNoOfMoves(int depth, Action<int> callback, Queue<MultithreadCallback<int>> queue)
	{
		ThreadStart start = () => NoOfMovesThread(depth,callback, queue);
		new Thread(start).Start();
	}

	void NoOfMovesThread(int depth, Action<int> callback, Queue<MultithreadCallback<int>> queue)
	{
		int noOfMoves = NumberOfMoves(depth);
		lock (queue)
		{
			queue.Enqueue(new MultithreadCallback<int>(callback,noOfMoves));
		}
	}
	
	public int NumberOfMoves(int depth)
	{
		if (depth == 0)
			return 1;
		
		List <Move> moves = _moveGenerator.GenerateMoves();
		int noOfPosition = 0;
		
		
		foreach (var move in moves)
		{
			MoveData m = MakeMove(move);
			// GD.Print(move);
			noOfPosition += NumberOfMoves(depth - 1);
			UnmakeMove(m);
		}

		return noOfPosition;
	}
	
}
