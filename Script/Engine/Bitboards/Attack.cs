using Godot;
using System;
using System.Collections.Generic;
using Chess.Script.Engine.Moves;


namespace Chess.Script.Engine.Bitboards;


public static partial class Bitboard
{
    public static ulong Attacks(bool whiteAttacks, int[] boardStatus)
    {
        ulong attack = 0;
        for (int i = 0; i < boardStatus.Length; ++i)
        {
            int piece = boardStatus[i];
            if(piece == (int)ColourType.FREE)
                continue;

            piece -= 1;
            if(whiteAttacks && piece > (int)ColourType.WHITE_PAWN - 1 ||
               !whiteAttacks && piece < (int)ColourType.BLACK_KING - 1
               )
                continue;
            switch (piece % 6)
            {
                case (int)ColourType.WHITE_PAWN - 1:
                    attack |= Pawn(whiteAttacks, i);
                    break;
                case (int)ColourType.WHITE_KING - 1:
                    attack |= King(i);
                    break;
                case (int)ColourType.WHITE_KNIGHT - 1:
                    attack |= Knight(i);
                    break;
                default:
                    attack |= SlidingMoves(boardStatus, whiteAttacks, i, piece + 1);
                    break;
            }
        }
        return attack;
    }

    static ulong Pawn(bool isWhite, int from)
    {
        ulong attack = 0;
        int modifier = (isWhite) ? -1 : 1;
        Vector2I pos = Functions.IntToBoardPosition(from);

        foreach (var item in PrecomputedMoves.PawnAttacks[from])
        {
            attack |= Functions.GetBit(item + modifier * 8);
        }

        return attack;
    }
    
    static ulong SlidingMoves(int[] boardStatus, bool isWhite, int from, int pieceIdx)
    {
        ulong attack = 0;
        int piece = (pieceIdx - 1) % 6;
        
        int lastDir = 8;
        int dirIdx = 0;

        if (piece == (int)ColourType.WHITE_BISHOP - 1)
            lastDir -= 4;
        if (piece == (int)ColourType.WHITE_ROOK - 1)
            dirIdx += 4;
        
        for (; dirIdx < lastDir; ++dirIdx)
        {
            int dir = PrecomputedMoves.Direction[dirIdx];
            int legalPos = from + dir;
            if(legalPos < 0 || legalPos > 63)
                continue;
            
            for (int i = 0; i < PrecomputedMoves.DistanceToEdge[from][dirIdx]; ++i)
            {
                int occupation = Functions.IsPieceFriendly(isWhite, boardStatus[legalPos]);

                attack |= Functions.GetBit(legalPos);
                if (occupation != -1)
                {
                    if (isWhite && boardStatus[legalPos] == (int)ColourType.BLACK_KING) { }
                    else if (!isWhite && boardStatus[legalPos] == (int)ColourType.WHITE_KING) { }
                    else
                    {
                        break;
                    }
                }

                legalPos += dir;
            }
        }
        return attack;
    }

    static ulong Knight(int from)
    {
        ulong attack = 0;
        foreach (var item in PrecomputedMoves.KnightMoves[from])
        {
            attack |= Functions.GetBit(item);
        }

        return attack;
    }

    static ulong King(int from)
    {
        ulong attack = 0;

        foreach (var item in PrecomputedMoves.KingMoves[from])
        {
            attack |= Functions.GetBit(item);
        }
        
        return attack;
    }
    
}