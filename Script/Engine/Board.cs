using Godot;
using System;
using System.Collections.Generic;

public class Board
{

	int[] pieceCount;

	ulong[] boardStatus;
	private ulong[] boardSnapshot;
	public ulong[] BoardStatus
	{
		get => boardStatus;
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

		return false;
	}

	bool MovePawn(int pieceIdx, int initialPos, int finalPos, out bool caputure)
	{
		caputure = false;
		Colour c = (pieceIdx < 6) ? Colour.WHITE : Colour.BLACK;
		List<int> moves = MoveValidity.Pawn(c, boardStatus, initialPos);

		if (moves.Contains(finalPos))
		{
			if (Mathf.Abs(initialPos - finalPos) % 2 == 1)
			{
				caputure = true;
				for (int i = (c == Colour.BLACK ? 0 : 6); i < (c == Colour.BLACK ? 6 : 12); ++i)
				{
					if ((boardStatus[i] & GetBit(finalPos)) != 0)
					{
						boardStatus[i] ^= GetBit(finalPos);
						break;
					}
				}
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
		List<int> moves = MoveValidity.Knight(c, boardStatus, initialPos);
		if (moves.Contains(finalPos))
		{
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
