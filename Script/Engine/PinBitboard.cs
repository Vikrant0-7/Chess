using System;
using Godot;
using System.Collections.Generic;

public static class PinBitboard
{
    static ulong GetBit(int bit)
    {
        return (ulong)1 << (bit);
    }

    static Vector2I IntToBoardPosition(int pos){
        return new Vector2I(pos % 8, pos/8);
    }
    //Converts board's position to index of that position
    static int BoardPositionToInt(int x, int y)
    {
        return y * 8 + x;
    }
    
    static ulong Bishop(int colour, int bPos, int kPos, ulong[] board)
    {
        ulong pinningMoves = 0;
        ulong freepiece = 0;
        ulong friendlyPiece = 0;
        
        Vector2I kPosVec = IntToBoardPosition(kPos);
        Vector2I bPosVec = IntToBoardPosition(bPos);

        Vector2I diff = kPosVec - bPosVec;

        if (Mathf.Abs(diff.X) != Mathf.Abs(diff.Y))
        {
            return pinningMoves;
        }
        
        diff.X = Mathf.Sign(diff.X);
        diff.Y = Mathf.Sign(diff.Y);

        for (int i = 0; i < 12; ++i)
        {
            freepiece |= board[i];
            if (colour == 0)
            {
                if (i < 6)
                    friendlyPiece |= board[i];
            }
            else
            {
                if (i > 5)
                    friendlyPiece |= board[i];
            }
        }

        int y = bPosVec.Y + diff.Y;
        int possiblePinnedPosition = -1;
        for (int x = bPosVec.X + diff.X; x < 8 && x >= 0 && y >= 0 && y < 8; x += diff.X, y += diff.Y)
        {
            int pos = BoardPositionToInt(x, y);
            if(pos == kPos)
                break;
            
            if ((freepiece & GetBit(pos)) != 0)
            {
                if (possiblePinnedPosition != -1 || (friendlyPiece & GetBit(pos)) == 0)
                {
                    possiblePinnedPosition = -1;
                    break;
                }
                
                possiblePinnedPosition = pos;
            }
        }

        if (possiblePinnedPosition >= 0)
        {
            pinningMoves = GetBit(possiblePinnedPosition);
        }

        return pinningMoves;
    }
    
    static ulong Rook(int colour, int rPos, int kPos, ulong[] board)
    {
        ulong pinningMoves = 0;
        ulong freepiece = 0;
        ulong friendlyPiece = 0;
        
        Vector2I kPosVec = IntToBoardPosition(kPos);
        Vector2I rPosVec = IntToBoardPosition(rPos);

        Vector2I diff = kPosVec - rPosVec;

        if (Mathf.Abs(diff.X) != 0 && Mathf.Abs(diff.Y) != 0)
        {
            return pinningMoves;
        }
        
        diff.X = Mathf.Sign(diff.X);
        diff.Y = Mathf.Sign(diff.Y);

        for (int i = 0; i < 12; ++i)
        {
            freepiece |= board[i];
            if (colour == 0)
            {
                if (i < 6)
                    friendlyPiece |= board[i];
            }
            else
            {
                if (i > 5)
                    friendlyPiece |= board[i];
            }
        }
        
        int possiblePinnedPosition = -1;

        if (diff.Y == 0)
        {
            for (int x = rPosVec.X + diff.X; x < 8 && x >= 0; x += diff.X)
            {
                int pos = BoardPositionToInt(x, rPosVec.Y);
                if (pos == kPos)
                    break;
                
                if ((freepiece & GetBit(pos)) != 0)
                {
                    if (possiblePinnedPosition != -1 || (friendlyPiece & GetBit(pos)) == 0)
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
                int pos = BoardPositionToInt(rPosVec.X, y);
                if (pos == kPos)
                {
                    break;
                }

                if ((freepiece & GetBit(pos)) != 0)
                {
                    if (possiblePinnedPosition != -1 || (friendlyPiece & GetBit(pos)) == 0)
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
            pinningMoves = GetBit(possiblePinnedPosition);
        }

        return pinningMoves;
    }
    

    public static ulong GetPinnedPositions(int colour, ulong[] board) //colour 0 means find pinned piece of white
    {
        ulong possiblePin = 0;

        int kPos = -1;
        if (colour == 0)
        {
            for (int i = 63; i >= 0; --i)
            {
                if ((board[0] & GetBit(i)) != 0)
                {
                    kPos = i;
                    break;
                }
            }
            
            for (int i = 63; i >= 0; --i)
            {
                if ((board[8] & GetBit(i)) != 0)
                {
                    possiblePin |= Rook(colour, i, kPos, board);
                }
                else if ((board[9] & GetBit(i)) != 0)
                {
                    possiblePin |= Bishop(colour, i, kPos, board);
                }

                else if ((board[7] & GetBit(i)) != 0)
                {
                    possiblePin |= Bishop(colour, i, kPos, board);
                    possiblePin |= Rook(colour, i, kPos, board);
                }
            }
        }
        else
        {
            for (int i = 0; i <= 63; ++i)
            {
                if ((board[6] & GetBit(i)) != 0)
                {
                    kPos = i;
                    break;
                }
            }
            
            for (int i = 63; i >= 0; --i)
            {
                if ((board[2] & GetBit(i)) != 0)
                {
                    possiblePin |= Rook(colour, i, kPos, board);
                }
                else if ((board[3] & GetBit(i)) != 0)
                {
                    possiblePin |= Bishop(colour, i, kPos, board);
                }
                else if ((board[1] & GetBit(i)) != 0)
                {
                    possiblePin |= Bishop(colour, i, kPos, board);
                    possiblePin |= Rook(colour, i, kPos, board);
                }
            }
        }
        
        return possiblePin;
    }
}