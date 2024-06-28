using Godot;
using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

using Chess.Script.Engine.Bitboards;
using Chess.Script.Engine.Moves;

namespace Chess.Script.Engine;

//TODO: Add check for mate condition.
//TODO: Migrate to hybrid representation of board
/* Ultimate goal is to fully migrate to board oriented representation, which is
 * how we see the chess board. Method Currently in use is piece oriented representation
 * in which bitboard of each piece is stored. Board Oriented is more intutive than piece
 * oriented but is more memory heavy 96 bytes (in piece oriented) vs 256 bytes (in board oriented).
 * Need to migrate is that currently all my algorithms are Board oriented and translation between two is
 * added overhead.
 */
public class Board
{
	private int[] _boardStatus;
	private int _enPassantPosition;

	private bool _whiteTurn = true;

	private int _castleStatus;
	// 0 -> Queen Side Castle
	// 1 -> King Side Castle

	private MoveGenerator _moveGenerator;
	private int _50MoveRule;
	private int _fullMoves;


	private ulong _pinnedBitboard;
	private ulong _attackBitboard;

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

	public ulong PinnedBitboard => _pinnedBitboard;
	public ulong CurrAttackBitboard => _attackBitboard;

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

			_boardStatus[move.position] = (int)ColourType.FREE;
			_boardStatus[move.finalPosition] = move.piece;
			
			return true;
		}

		return false;
	}

	public int GetCastleStatus(bool isWhite)
	{
		if (isWhite)
		{
			return (_castleStatus << 4) >> 4;
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
	}

	public void CleanBoard()
	{
		_boardStatus = new int[64];
		_enPassantPosition = -1;
		_50MoveRule = 0;
		_fullMoves = 1;
		_castleStatus = 0;
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
		_pinnedBitboard = PinBitboard.GetPinnedPositions(_whiteTurn ? 0 : 1, _boardStatus);
		_attackBitboard = AttackBitboard.GetAttackBitBoard(!_whiteTurn, _boardStatus);
		_50MoveRule = fen.Moves[0];
		_fullMoves = fen.Moves[1];
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
		}

		if (valid)
		{
			_attackBitboard = AttackBitboard.GetAttackBitBoard(_whiteTurn, _boardStatus);
			
			_whiteTurn = !_whiteTurn;
			if ((move.piece - 1) % 6 != (int)ColourType.WHITE_PAWN - 1)
			{
				_enPassantPosition = -1;
			}

			_pinnedBitboard = PinBitboard.GetPinnedPositions(_whiteTurn ? 0 : 1, _boardStatus);

		}
		
		return valid;
	}
	
	public void MakeMove(Move move)
	{
		
	}

	public void UnmakeMove(Move move)
	{
		
	}

public void RequestNoOfMoves(int depth, Action<int> callback, Queue<NoOfMovesThreadData<int>> queue)
	{
		ThreadStart start = () => NoOfMovesThread(depth,callback, queue);
		new Thread(start).Start();
	}

	void NoOfMovesThread(int depth, Action<int> callback, Queue<NoOfMovesThreadData<int>> queue)
	{
		int noOfMoves = NumberOfMoves(depth);
		lock (queue)
		{
			queue.Enqueue(new NoOfMovesThreadData<int>(callback,noOfMoves));
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
			MakeMove(move);
			noOfPosition += NumberOfMoves(depth - 1);
			UnmakeMove(move);
		}

		return noOfPosition;
	}
	
}
