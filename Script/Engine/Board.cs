using Godot;
using System;
using System.Collections.Generic;

//Todo: Implement translation of FEN to board positions.
public class Board
{
	private ulong[] _boardStatus;
	private ulong[] _boardSnapshot;

	private bool _whiteTurn = true;

	public ulong[] BoardSnapshot => _boardSnapshot;
	public ulong[] BoardStatus => _boardStatus;
	public bool WhiteTurn => _whiteTurn;
	
	public Board(){
		_boardStatus = new ulong[12];
		_boardSnapshot = new ulong[12];
	}

	ulong GetBit(int bit){
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
		
		GetSnapshot();
	}

	private void GetSnapshot()
	{
		for (int i = 0; i < _boardStatus.Length; ++i)
		{
			_boardSnapshot[i] = _boardStatus[i];
		}
	}

	private void RevertToSnapshot()
	{
		for (int i = 0; i < _boardStatus.Length; ++i)
		{
			_boardStatus[i] = _boardSnapshot[i];
		}
	}
	

	public int GetIndex(Type type, Colour colour){
		return ((colour == Colour.WHITE) ? 0 : 1) * 6 + (int)type;
	}
	
	public bool GetPiece(int pieceIndex, int position){
		if(position < 0 || position >= 64){
			throw new IndexOutOfRangeException("Position should be in range [0,63]: " + position.ToString());
		}
		return (_boardStatus[pieceIndex] & GetBit(position)) != 0; 
	}

	public bool Move(int pieceIdx, int initialPos, int finalPos, out bool caputure)
	{
		caputure = false;
		if ((_whiteTurn && pieceIdx > 5) || (!_whiteTurn && pieceIdx <= 5)) //not your turn to move.
		{
			return false;
		}
		bool valid = false;
		if (pieceIdx % 6 == 5) //if piece index is 5 or 11
		{
			valid =  MovePawn(pieceIdx, initialPos, finalPos, out caputure);
		}
		else if (pieceIdx % 6 >= 1 && pieceIdx % 6 <= 4) //if piece index is 1,2,3,4,7,8,9,10
		{
			valid =  MoveQRBN(pieceIdx, initialPos, finalPos, out caputure);
		}
		else
		{
			valid =  MoveKing(pieceIdx, initialPos, finalPos, out caputure);
		}

		if (valid)
			_whiteTurn = !_whiteTurn;
		
		return valid;
	}

	bool MovePawn(int pieceIdx, int initialPos, int finalPos, out bool caputure)
	{
		caputure = false;
		Colour c = (pieceIdx < 6) ? Colour.WHITE : Colour.BLACK;
		List<int> moves = global::LegalMoves.Pawn(c, _boardStatus, _boardSnapshot, initialPos);

		if (moves.Contains(finalPos))
		{
			GetSnapshot();
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
				caputure = true;
			}
			_boardStatus[pieceIdx] ^= GetBit(initialPos) | GetBit(finalPos); //acutally update the position of pawn
			return true;
		}
		return false;
	}

	bool MoveQRBN(int pieceIdx, int initialPos, int finalPos, out bool caputure) //moves queen, rook, bishop and knight
	{
		caputure = false;
		Colour c = (pieceIdx < 6) ? Colour.WHITE : Colour.BLACK;

		List<int> moves;
		switch (pieceIdx % 6)
		{
			case 1: //1 and 7 are queens
				moves = LegalMoves.Queen(c, _boardStatus, initialPos);
				break;
			case 2: //2 and 8 are rooks
				moves = LegalMoves.Queen(c, _boardStatus, initialPos);
				break;
			case 3: //3 and 9 and bishops
				moves = LegalMoves.Bishop(c, _boardStatus, initialPos);
				break;
			default:
				moves = LegalMoves.Knight(c, _boardStatus, initialPos);
				break;
		}
		
		if (moves.Contains(finalPos))
		{
			GetSnapshot();
			for (int i = (c == Colour.BLACK ? 0 : 6); i < (c == Colour.BLACK ? 6 : 12); ++i)
			{
				if ((_boardStatus[i] & GetBit(finalPos)) != 0) //check if final position is occupied by any other piece
				{
					caputure = true;
					_boardStatus[i] ^= GetBit(finalPos); //if it does, XORing two 1 bits results in 0 at that bit.
					break;
				}
			}
			_boardStatus[pieceIdx] ^= (GetBit(initialPos) | GetBit(finalPos)); //moves the piece
			return true;
		}

		return false;
	}
	
	//Todo: Add code to handle castling
	bool MoveKing(int pieceIdx, int initialPos, int finalPos, out bool caputure)
	{
		caputure = false;
		Colour c = (pieceIdx < 6) ? Colour.WHITE : Colour.BLACK;
		List<int> moves = global::LegalMoves.King(c, _boardStatus, initialPos);
		if (moves.Contains(finalPos))
		{
			GetSnapshot();
			for (int i = (c == Colour.BLACK ? 0 : 6); i < (c == Colour.BLACK ? 6 : 12); ++i)
			{
				if ((_boardStatus[i] & GetBit(finalPos)) != 0)
				{
					caputure = true;
					_boardStatus[i] ^= GetBit(finalPos);
					break;
				}
			}
			_boardStatus[pieceIdx] ^= (GetBit(initialPos) | GetBit(finalPos));
			return true;
		}
		return false;
	}
	
}
