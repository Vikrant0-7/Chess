using System;
using Godot;
using System.Collections.Generic;

namespace Chess.Script.Engine.Bitboards;

public static class PinBitboard
{
    static ulong Bishop(int colour, int bPos, int kPos, int[] board)
    {
        ulong pinningMoves = 0;
        
        Vector2I kPosVec = Functions.IntToBoardPosition(kPos);
        Vector2I bPosVec = Functions.IntToBoardPosition(bPos);

        Vector2I diff = kPosVec - bPosVec;

        if (Mathf.Abs(diff.X) != Mathf.Abs(diff.Y))
        {
            return pinningMoves;
        }
        
        diff.X = Mathf.Sign(diff.X);
        diff.Y = Mathf.Sign(diff.Y);

        int y = bPosVec.Y + diff.Y;
        int possiblePinnedPosition = -1;
        for (int x = bPosVec.X + diff.X; x < 8 && x >= 0 && y >= 0 && y < 8; x += diff.X, y += diff.Y)
        {
            int pos = Functions.BoardPositionToInt(x, y);
            if(pos == kPos)
                break;
            
            if (board[pos] != (int)ColourType.FREE)
            {
                if (possiblePinnedPosition != -1 || Functions.IsPieceFriendly(colour==0,board[pos]) != 1)
                {
                    possiblePinnedPosition = -1;
                    break;
                }
                
                possiblePinnedPosition = pos;
            }
        }

        if (possiblePinnedPosition >= 0)
        {
            pinningMoves = Functions.GetBit(possiblePinnedPosition);
        }

        return pinningMoves;
    }
    
    static ulong Rook(int colour, int rPos, int kPos, int[] board)
    {
        ulong pinningMoves = 0;

        
        Vector2I kPosVec = Functions.IntToBoardPosition(kPos);
        Vector2I rPosVec = Functions.IntToBoardPosition(rPos);

        Vector2I diff = kPosVec - rPosVec;

        if (Mathf.Abs(diff.X) != 0 && Mathf.Abs(diff.Y) != 0)
        {
            return pinningMoves;
        }
        
        diff.X = Mathf.Sign(diff.X);
        diff.Y = Mathf.Sign(diff.Y);
        
        int possiblePinnedPosition = -1;

        if (diff.Y == 0)
        {
            for (int x = rPosVec.X + diff.X; x < 8 && x >= 0; x += diff.X)
            {
                int pos = Functions.BoardPositionToInt(x, rPosVec.Y);
                if (pos == kPos)
                    break;
                
                if (board[pos] != (int)ColourType.FREE)
                {
                    if (possiblePinnedPosition != -1 || Functions.IsPieceFriendly(colour == 0,board[pos]) != 1)
                    {
                        possiblePinnedPosition = -1;
                        break;
                    }
                
                    possiblePinnedPosition = pos;
                }
            }
        }
        else
        {
            for (int y = rPosVec.Y + diff.Y; y < 8 && y >= 0; y += diff.Y)
            {
                int pos = Functions.BoardPositionToInt(rPosVec.X, y);
                if (pos == kPos)
                {
                    break;
                }

                if (board[pos] != (int)ColourType.FREE)
                {
                    if (possiblePinnedPosition != -1 || Functions.IsPieceFriendly(colour == 0, board[pos]) != 1)
                    {
                        possiblePinnedPosition = -1;
                        break;
                    }
                
                    possiblePinnedPosition = pos;
                }
            }
        }

        if (possiblePinnedPosition >= 0)
        {
            pinningMoves = Functions.GetBit(possiblePinnedPosition);
        }

        return pinningMoves;
    }
    

    public static ulong GetPinnedPositions(int colour, int[] board) //colour 0 means find pinned piece of white
    {
        ulong possiblePin = 0;

        int kPos = -1;
        if (colour == 0)
        {
            for (int i = 63; i >= 0; --i)
            {
                if (board[i] == (int)ColourType.WHITE_KING)
                {
                    kPos = i;
                    break;
                }
            }
            
            for (int i = 63; i >= 0; --i)
            {
                if (board[i] == (int)ColourType.BLACK_ROOK || board[i] == (int)ColourType.BLACK_QUEEN)
                {
                    possiblePin |= Rook(colour, i, kPos, board);
                }
                if (board[i] == (int)ColourType.BLACK_BISHOP || board[i] == (int)ColourType.BLACK_QUEEN)
                {
                    possiblePin |= Bishop(colour, i, kPos, board);
                }
            }
        }
        else
        {
            for (int i = 0; i <= 63; ++i)
            {
                if (board[i] == (int)ColourType.BLACK_KING)
                {
                    kPos = i;
                    break;
                }
            }
            
            for (int i = 63; i >= 0; --i)
            {
                if (board[i] == (int)ColourType.WHITE_ROOK || board[i] == (int)ColourType.WHITE_QUEEN)
                {
                    possiblePin |= Rook(colour, i, kPos, board);
                }
                if (board[i] == (int)ColourType.WHITE_BISHOP || board[i] == (int)ColourType.WHITE_QUEEN)
                {
                    possiblePin |= Bishop(colour, i, kPos, board);
                }
            }
        }
        
        return possiblePin;
    }
}