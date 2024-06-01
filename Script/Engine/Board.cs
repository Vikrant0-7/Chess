using Godot;
using System;
using System.Collections.Generic;


//Todo: Make bitboard of currently square currently under attack by opposition.
public class Board
{

	int[] pieceCount;

	ulong[] boardStatus;
	private ulong[] boardSnapshot;
	public ulong[] BoardStatus
	{
		get => boardStatus;
	}

	public ulong[] BoardSnapshot
	{
		get => boardSnapshot;
	}
	
	public Board(){
		boardStatus = new ulong[12];
		boardSnapshot = new ulong[12];
		pieceCount = new int[]{1,1,2,2,2,8,1,1,2,2,2,8};
	}

	ulong GetBit(int bit){
		return (ulong)1 << (bit);
	}

	public void InitialBoardConfig(){
		boardStatus[GetIndex(Type.KING,Colour.BLACK)] = GetBit(4);
		boardStatus[GetIndex(Type.QUEEN,Colour.BLACK)] = GetBit(3);
		boardStatus[GetIndex(Type.ROOK,Colour.BLACK)] = GetBit(0) | GetBit(7);
		boardStatus[GetIndex(Type.BISHOP,Colour.BLACK)] = GetBit(2) | GetBit(5);
		boardStatus[GetIndex(Type.KNIGHT,Colour.BLACK)] = GetBit(1) | GetBit(6);
		for(int i = 8; i <= 15; ++i){
			boardStatus[GetIndex(Type.PAWN,Colour.BLACK)] |= GetBit(i);
		}

		boardStatus[GetIndex(Type.KING,Colour.WHITE)] = GetBit(60);
		boardStatus[GetIndex(Type.QUEEN,Colour.WHITE)] = GetBit(59);
		boardStatus[GetIndex(Type.ROOK,Colour.WHITE)] = GetBit(56) | GetBit(63);
		boardStatus[GetIndex(Type.BISHOP,Colour.WHITE)] = GetBit(58) | GetBit(61);
		boardStatus[GetIndex(Type.KNIGHT,Colour.WHITE)] = GetBit(62) | GetBit(57);
		for(int i = 48; i <= 55; ++i){
			boardStatus[GetIndex(Type.PAWN,Colour.WHITE)] |= GetBit(i);
		}

		GetSnapshot();
	}

	private void GetSnapshot()
	{
		for (int i = 0; i < boardStatus.Length; ++i)
		{
			boardSnapshot[i] = boardStatus[i];
		}
	}

	public int GetIndex(Type type, Colour colour){
		return ((colour == Colour.WHITE) ? 0 : 1) * 6 + (int)type;
	}
	

	public bool GetPiece(int pieceIndex, int position){
		if(position < 0 || position >= 64){
			throw new IndexOutOfRangeException("Position should be in range [0,63]: " + position.ToString());
		}
		return (boardStatus[pieceIndex] & GetBit(position)) != 0; 
	}

	public bool Move(int pieceIdx, int initialPos, int finalPos, out bool caputure)
	{
		caputure = false;
		if (pieceIdx == 5 || pieceIdx == 11)
		{
			return MovePawn(pieceIdx, initialPos, finalPos, out caputure);
		}

		if (pieceIdx == 4 || pieceIdx == 10)
		{
			return MoveKnight(pieceIdx, initialPos, finalPos, out caputure);
		}

		if (pieceIdx == 3 || pieceIdx == 9)
		{
			return MoveBishop(pieceIdx, initialPos, finalPos, out caputure);
		}
		
		if (pieceIdx == 2 || pieceIdx == 8)
		{
			return MoveRook(pieceIdx, initialPos, finalPos, out caputure);
		}
		
		if (pieceIdx == 1 || pieceIdx == 7)
		{
			return MoveQueen(pieceIdx, initialPos, finalPos, out caputure);
		}
		
		if (pieceIdx == 0 || pieceIdx == 6)
		{
			return MoveKing(pieceIdx, initialPos, finalPos, out caputure);
		}

		return false;
	}

	//Execpt MovePawn() method, every other Move<Piece> is same
	//Todo: Merge Merge MoveKnight, MoveBishop, MoveQueen, MoveRook, MoveKing 
	bool MovePawn(int pieceIdx, int initialPos, int finalPos, out bool caputure)
	{
		caputure = false;
		Colour c = (pieceIdx < 6) ? Colour.WHITE : Colour.BLACK;
		List<int> moves = PseudoLegalMove.Pawn(c, boardStatus, boardSnapshot, initialPos);

		if (moves.Contains(finalPos))
		{
			GetSnapshot();
			if (Mathf.Abs(initialPos - finalPos) % 2 == 1)
			{
				for (int i = (c == Colour.BLACK ? 0 : 6); i < (c == Colour.BLACK ? 6 : 12); ++i)
				{
					if ((boardStatus[i] & GetBit(finalPos)) != 0)
					{
						boardStatus[i] ^= GetBit(finalPos);
						caputure = true;
						break;
					}
				}
				//handles capures by en passente
				if (c == Colour.WHITE && !caputure)
				{
					if ((BoardStatus[11] & GetBit(finalPos)) == 0) //not need but just is case
					{
						boardStatus[11] ^= GetBit(finalPos + 8);
					}
				}
				else if (c == Colour.BLACK && !caputure)
				{
					if ((BoardStatus[5] & GetBit(finalPos)) == 0) //not needed but just in case
					{
						boardStatus[5] ^= GetBit(finalPos - 8);
					}
				}

				caputure = true;
			}
			boardStatus[pieceIdx] ^= (GetBit(initialPos) | GetBit(finalPos));
			return true;
		}
		return false;
	}

	bool MoveKnight(int pieceIdx, int initialPos, int finalPos, out bool caputure)
	{
		caputure = false;
		Colour c = (pieceIdx < 6) ? Colour.WHITE : Colour.BLACK;
		List<int> moves = PseudoLegalMove.Knight(c, boardStatus, initialPos);
		if (moves.Contains(finalPos))
		{
			GetSnapshot();
			for (int i = (c == Colour.BLACK ? 0 : 6); i < (c == Colour.BLACK ? 6 : 12); ++i)
			{
				if ((boardStatus[i] & GetBit(finalPos)) != 0)
				{
					caputure = true;
					boardStatus[i] ^= GetBit(finalPos);
					break;
				}
			}
			boardStatus[pieceIdx] ^= (GetBit(initialPos) | GetBit(finalPos));
			return true;
		}

		return false;
	}
	
	bool MoveBishop(int pieceIdx, int initialPos, int finalPos, out bool caputure)
	{
		caputure = false;
		Colour c = (pieceIdx < 6) ? Colour.WHITE : Colour.BLACK;
		List<int> moves = PseudoLegalMove.Bishop(c, boardStatus, initialPos);
		if (moves.Contains(finalPos))
		{
			GetSnapshot();
			for (int i = (c == Colour.BLACK ? 0 : 6); i < (c == Colour.BLACK ? 6 : 12); ++i)
			{
				if ((boardStatus[i] & GetBit(finalPos)) != 0)
				{
					caputure = true;
					boardStatus[i] ^= GetBit(finalPos);
					break;
				}
			}
			boardStatus[pieceIdx] ^= (GetBit(initialPos) | GetBit(finalPos));
			return true;
		}
		return false;
	}
	
	bool MoveQueen(int pieceIdx, int initialPos, int finalPos, out bool caputure)
	{
		caputure = false;
		Colour c = (pieceIdx < 6) ? Colour.WHITE : Colour.BLACK;
		List<int> moves = PseudoLegalMove.Queen(c, boardStatus, initialPos);
		if (moves.Contains(finalPos))
		{
			GetSnapshot();
			for (int i = (c == Colour.BLACK ? 0 : 6); i < (c == Colour.BLACK ? 6 : 12); ++i)
			{
				if ((boardStatus[i] & GetBit(finalPos)) != 0)
				{
					caputure = true;
					boardStatus[i] ^= GetBit(finalPos);
					break;
				}
			}
			boardStatus[pieceIdx] ^= (GetBit(initialPos) | GetBit(finalPos));
			return true;
		}
		return false;
	}
	
	bool MoveRook(int pieceIdx, int initialPos, int finalPos, out bool caputure)
	{
		caputure = false;
		Colour c = (pieceIdx < 6) ? Colour.WHITE : Colour.BLACK;
		List<int> moves = PseudoLegalMove.Rook(c, boardStatus, initialPos);
		if (moves.Contains(finalPos))
		{
			GetSnapshot();
			for (int i = (c == Colour.BLACK ? 0 : 6); i < (c == Colour.BLACK ? 6 : 12); ++i)
			{
				if ((boardStatus[i] & GetBit(finalPos)) != 0)
				{
					caputure = true;
					boardStatus[i] ^= GetBit(finalPos);
					break;
				}
			}
			boardStatus[pieceIdx] ^= (GetBit(initialPos) | GetBit(finalPos));
			return true;
		}
		return false;
	}
	
	bool MoveKing(int pieceIdx, int initialPos, int finalPos, out bool caputure)
	{
		caputure = false;
		Colour c = (pieceIdx < 6) ? Colour.WHITE : Colour.BLACK;
		List<int> moves = PseudoLegalMove.King(c, boardStatus, initialPos);
		if (moves.Contains(finalPos))
		{
			GetSnapshot();
			for (int i = (c == Colour.BLACK ? 0 : 6); i < (c == Colour.BLACK ? 6 : 12); ++i)
			{
				if ((boardStatus[i] & GetBit(finalPos)) != 0)
				{
					caputure = true;
					boardStatus[i] ^= GetBit(finalPos);
					break;
				}
			}
			boardStatus[pieceIdx] ^= (GetBit(initialPos) | GetBit(finalPos));
			return true;
		}
		return false;
	}
}
