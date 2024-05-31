using Godot;
using System;

public class Board
{

	int[] pieceCount;

	ulong[] boardStatus;

	public Board(){
		boardStatus = new ulong[12];
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

}
