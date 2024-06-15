using Godot;
using System;
using System.Threading;
using System.Collections.Generic;

//Todo: Add check for mate condition.
//Todo: Implement translation of FEN to board positions.
//Todo: Migrate moves to Move class.
//Todo: Handle Pawn Promotions
public class Board
{
	private ulong[] _boardStatus;
	private int _enPassantPosition;
    
	private bool _whiteTurn = true;
	
	private bool[] _whiteCanCastle;
	private bool[] _blackCanCastle;
	// 0 -> Queen Side Castle
	// 1 -> King Side Castle
	
	private Stack<Move> _movesMade;
	private MoveGenerator _moveGenerator;
	private int _halfMoves;
	private int _fullMoves;
	
	public int EnPassantPosition => _enPassantPosition;
	public ulong[] BoardStatus => _boardStatus;
	
	public bool WhiteTurn
	{
		set => _whiteTurn = value;
		get => _whiteTurn;
	}
	
	public Board(){
		_boardStatus = new ulong[12];
		_enPassantPosition = -1;
		_halfMoves = _fullMoves = 0;
		_whiteCanCastle = new bool[2];
		_blackCanCastle = new bool[2];
		_movesMade = new Stack<Move>();
		_moveGenerator = new MoveGenerator(this);
	}

	
	bool MovePawn(int pieceIdx, int initialPos, int finalPos, bool checkLegal = true)
	{
		bool caputure = false;
		Colour c = (pieceIdx < 6) ? Colour.WHITE : Colour.BLACK;
		
		List<int> moves = new List<int>();
		if(checkLegal)
			moves = LegalMoves.Pawn(c, _boardStatus, _enPassantPosition, initialPos);

		if (!checkLegal || moves.Contains(finalPos))
		{
			//Pawn has moved 2 places then, square just behind is _enPassantPosition
			if (Mathf.Abs(initialPos - finalPos) == 16)
			{
				if (c == Colour.WHITE)
				{
					_enPassantPosition = finalPos + 8;
				}
				else
				{
					_enPassantPosition = finalPos - 8;
				}
			}
			else //pawn has moves 1 step.
			{ 
				_enPassantPosition = -1;
			}
			//if piece is moving diagonally the it is capturing some other piece
			/*
			 * For white 1 step ahead is initialPos - 8, thus initialPos - 9 and initialPos - 7 are diagonals
			 * thus if final position is intialPos - 7/9 and initialPos is subtracted from it we will get -7 or -9
			 * both give 1 with mod 2 operation of absolute value.
			 * If pawn moved forward this value would have be +- 8 and would have given 0 with mod 2 operation.
			 */
			if (Mathf.Abs(initialPos - finalPos) % 2 == 1)
			{
				for (int i = (c == Colour.BLACK ? 0 : 6); i < (c == Colour.BLACK ? 6 : 12); ++i)
				{
					if ((_boardStatus[i] & GetBit(finalPos)) != 0)
					{
						_boardStatus[i] ^= GetBit(finalPos);
						caputure = true;
						break;
					}
				}
				//if any piece wasn't captured by previous for loop that means position isn't occupied by any other
				//piece thus if pawn is capturing an empty square that means it is case of en passante.
				//Handling En Passant.
				if (!caputure && c == Colour.WHITE)
				{
					//11 is index of blackPawn.
					//In En passant, a pawn has actually moved two steps but pawn can capture on first move
					//as if pawn has just moved one. If Pawn has moved 2 steps that mean it is adjacent to 
					//initial position of capturing pawn of just behind the final postion of capturing pawn.
					//A pawn at index finalPos + 8 is just behind the capturing pawn.
					_boardStatus[11] ^= GetBit(finalPos + 8);
				}
				else if (!caputure && c == Colour.BLACK)
				{
					//5 is index of WhitePawn
					//A pawn at index finalPos - 8 is just behind the capturing pawn.
					_boardStatus[5] ^= GetBit(finalPos - 8);

				}
			}
			_boardStatus[pieceIdx] ^= GetBit(initialPos) | GetBit(finalPos); //acutally update the position of pawn
			return true;
		}
		return false;
	}

	bool MoveQRBN(int pieceIdx, int initialPos, int finalPos, bool checkLegal = true) //moves queen, rook, bishop and knight
	{
		Colour c = (pieceIdx < 6) ? Colour.WHITE : Colour.BLACK;

		List<int> moves = new List<int>();
		if (checkLegal)
		{
			switch (pieceIdx % 6)
			{
				case 1: //1 and 7 are queens
					moves = LegalMoves.Queen(c, _boardStatus, initialPos);
					break;
				case 2: //2 and 8 are rooks
					moves = LegalMoves.Rook(c, _boardStatus, initialPos);
					break;
				case 3: //3 and 9 and bishops
					moves = LegalMoves.Bishop(c, _boardStatus, initialPos);
					break;
				default:
					moves = LegalMoves.Knight(c, _boardStatus, initialPos);
					break;
			}
		}

		if (!checkLegal || moves.Contains(finalPos))
		{
			for (int i = (c == Colour.BLACK ? 0 : 6); i < (c == Colour.BLACK ? 6 : 12); ++i)
			{
				if ((_boardStatus[i] & GetBit(finalPos)) != 0) //check if final position is occupied by any other piece
				{
					_boardStatus[i] ^= GetBit(finalPos); //if it does, XORing two 1 bits results in 0 at that bit.
					break;
				}
			}
			_boardStatus[pieceIdx] ^= (GetBit(initialPos) | GetBit(finalPos)); //moves the piece
			return true;
		}

		return false;
	}
	
	bool MoveKing(int pieceIdx, int initialPos, int finalPos, bool checkLegal = true)
	{
		Colour c = (pieceIdx < 6) ? Colour.WHITE : Colour.BLACK;
		List<int> moves = new List<int>();
		if(checkLegal)
			moves = LegalMoves.King(c, _boardStatus, initialPos, CanCastle(pieceIdx));
		
		if (!checkLegal || moves.Contains(finalPos))
		{
			if (Mathf.Abs(finalPos - initialPos) == 2) //king is castling
			{
				_boardStatus[pieceIdx] ^= (GetBit(initialPos) | GetBit(finalPos));
				if (finalPos == initialPos - 2) //king is castling on queen side
				{
					_boardStatus[pieceIdx + 2] ^= (GetBit(initialPos - 4) | GetBit(finalPos + 1));
				}
				else
				{
					_boardStatus[pieceIdx + 2] ^= (GetBit(initialPos + 3) | GetBit(finalPos - 1));
				}
			}
			else
			{
				for (int i = (c == Colour.BLACK ? 0 : 6); i < (c == Colour.BLACK ? 6 : 12); ++i)
				{
					if ((_boardStatus[i] & GetBit(finalPos)) != 0)
					{
						_boardStatus[i] ^= GetBit(finalPos);
						break;
					}
				}

				_boardStatus[pieceIdx] ^= (GetBit(initialPos) | GetBit(finalPos));
			}

			return true;
		}
		return false;
	}

	public int GetIndex(Type type, Colour colour){
		return ((colour == Colour.WHITE) ? 0 : 1) * 6 + (int)type;
	}
	
	public int GetIndex(int type, int colour){
		return colour * 6 + type;
	}

	public void CleanBoard()
	{
		_boardStatus = new ulong[12];
		_enPassantPosition = -1;
		_halfMoves = _fullMoves = 0;

		_whiteCanCastle = new bool[2];
		_blackCanCastle = new bool[2];
		_movesMade = new Stack<Move>();
	}

	public void SetCastlingState(bool[] white, bool[] black)
	{
		_whiteCanCastle = white;
		_blackCanCastle = black;
	}

	public void SetBoardPosition(int position, int piece)
	{
		if (piece == -1) //If piece is -1, Remove any piece from board at that position.
		{
			for (int i = 0; i < 12; ++i)
			{
				if ((_boardStatus[i] & GetBit(position)) != 0)
				{
					_boardStatus[i] ^= GetBit(position);
				}
			}
		}
		else
		{
			_boardStatus[piece] ^= GetBit(position);
			for (int i = 0; i < 12; ++i)
			{
				if(i == piece)
					continue;
				if ((_boardStatus[i] & GetBit(position)) != 0)
				{
					_boardStatus[i] ^= GetBit(position);
				}
			}
		}
	}
	
	public bool GetPiece(int pieceIndex, int position){
		if(position < 0 || position >= 64){
			throw new IndexOutOfRangeException("Position should be in range [0,63]: " + position.ToString());
		}
		return (_boardStatus[pieceIndex] & GetBit(position)) != 0; 
	}

	public bool Move(int pieceIdx, int initialPos, int finalPos, bool checkLegal = true)
	{
		if ((_whiteTurn && pieceIdx > 5) || (!_whiteTurn && pieceIdx <= 5)) //not your turn to move.
		{
			return false;
		}
		bool valid = false;
		if (pieceIdx % 6 == 5) //if piece index is 5 or 11
		{
			valid =  MovePawn(pieceIdx, initialPos, finalPos, checkLegal);
		}
		else if (pieceIdx % 6 >= 1 && pieceIdx % 6 <= 4) //if piece index is 1,2,3,4,7,8,9,10
		{
			valid = MoveQRBN(pieceIdx, initialPos, finalPos, checkLegal);
		}
		else
		{
			valid =  MoveKing(pieceIdx, initialPos, finalPos, checkLegal);
			if (valid && pieceIdx == 0)
			{
				_whiteCanCastle[0] = false;
				_whiteCanCastle[1] = false;
			}
			else if(valid)
			{
				_blackCanCastle[0] = false;
				_whiteCanCastle[1] = false;
			}
		}

		if (valid)
		{
			++_halfMoves;
			_whiteTurn = !_whiteTurn;
			if (_halfMoves % 2 == 0)
			{
				++_fullMoves;
			}
			
			//if move is not a pawn moves reset enPassant pawn position
			if (pieceIdx % 6 != 5)
			{
				_enPassantPosition = -1;
			}
			
			/*
			 * If rook has been captured or moved even once, their respective variable will become false and will remain
			 * so till board is reset, ensuring castling can only be done when rook is not yet moved.
			 */
			
			if (_whiteCanCastle[0] && (_boardStatus[2] & GetBit(56)) == 0)
				_whiteCanCastle[0] = false;
			if (_whiteCanCastle[1] && (_boardStatus[2] & GetBit(63)) == 0)
				_whiteCanCastle[1] = false;
			
			if (_blackCanCastle[0] && (_boardStatus[8] & GetBit(0)) == 0)
				_blackCanCastle[0] = false;
			if (_blackCanCastle[1] && (_boardStatus[8] & GetBit(7)) == 0)
				_blackCanCastle[1] = false;
		}

		return valid;
	}
	
	public ulong GetBit(int bit){
		return (ulong)1 << (bit);
	}

	//Initializes board to its initial config.
	public void InitialBoardConfig(){
		_boardStatus[GetIndex(Type.KING,Colour.BLACK)] = GetBit(4);
		_boardStatus[GetIndex(Type.QUEEN,Colour.BLACK)] = GetBit(3);
		_boardStatus[GetIndex(Type.ROOK,Colour.BLACK)] = GetBit(0) | GetBit(7);
		_boardStatus[GetIndex(Type.BISHOP,Colour.BLACK)] = GetBit(2) | GetBit(5);
		_boardStatus[GetIndex(Type.KNIGHT,Colour.BLACK)] = GetBit(1) | GetBit(6);
		_boardStatus[GetIndex(Type.PAWN, Colour.BLACK)] = 0;
		for(int i = 8; i <= 15; ++i){
			_boardStatus[GetIndex(Type.PAWN,Colour.BLACK)] |= GetBit(i);
		}

		_boardStatus[GetIndex(Type.KING,Colour.WHITE)] = GetBit(60);
		_boardStatus[GetIndex(Type.QUEEN,Colour.WHITE)] = GetBit(59);
		_boardStatus[GetIndex(Type.ROOK,Colour.WHITE)] = GetBit(56) | GetBit(63);
		_boardStatus[GetIndex(Type.BISHOP,Colour.WHITE)] = GetBit(58) | GetBit(61);
		_boardStatus[GetIndex(Type.KNIGHT,Colour.WHITE)] = GetBit(62) | GetBit(57);
		_boardStatus[GetIndex(Type.PAWN, Colour.WHITE)] = 0;
		for(int i = 48; i <= 55; ++i){
			_boardStatus[GetIndex(Type.PAWN,Colour.WHITE)] |= GetBit(i);
		}

		_whiteTurn = true;

		_whiteCanCastle[0] = true;
		_whiteCanCastle[1] = true;
		_blackCanCastle[0] = true;
		_blackCanCastle[1] = true;
		_enPassantPosition = -1;
		_halfMoves = _fullMoves = 0;

	}

	public bool[] CanCastle(int pieceIndex)
	{
		if (pieceIndex <= 5)
		{
			return _whiteCanCastle;
		}
		else
		{
			return _blackCanCastle;
		}
	}

	public String GetCastlingString()
	{
		String @out = "";
		if (!_whiteCanCastle[0] && !_blackCanCastle[0] &&
		    !_whiteCanCastle[1] && !_blackCanCastle[1]
		   )
			@out = "-";
		if (_whiteCanCastle[1])
			@out += "K";
		if (_whiteCanCastle[0])
			@out += "Q";
		if (_blackCanCastle[1])
			@out += "k";
		if (_blackCanCastle[0])
			@out += "q";
		return @out;
	}
	

	public void MakeMove(Move move)
	{ 
		move.whiteCanCastle = (bool[])_whiteCanCastle.Clone();
		move.blackCanCastle = (bool[])_blackCanCastle.Clone();
		move.whiteTurn = _whiteTurn;
		move.data[0] = _halfMoves;
		move.data[1] = _fullMoves;
		move.data[2] = _enPassantPosition;
		move.board = (ulong[])_boardStatus.Clone();
		
		this.Move(move.piece, move.position, move.finalPosition, false);
		_movesMade.Push(move);
	}

	public void UnmakeMove(Move move)
	{
		_whiteCanCastle = move.whiteCanCastle;
		_blackCanCastle = move.blackCanCastle;
		_whiteTurn = move.whiteTurn;
		_halfMoves = move.data[0];
		_fullMoves = move.data[1];
		_enPassantPosition = move.data[2];

		_boardStatus = move.board;
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
		List < global::Move > moves = _moveGenerator.GenerateMoves();
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

public struct NoOfMovesThreadData<T>
{
	public Action<T> callback;
	public T parameter;

	public NoOfMovesThreadData(Action<T> callback, T parameter)
	{
		this.callback = callback;
		this.parameter = parameter;
	}
}
