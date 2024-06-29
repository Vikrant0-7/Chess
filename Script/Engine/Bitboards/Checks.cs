namespace Chess.Script.Engine.Bitboards;
using Godot;
using Chess.Script.Engine.Moves;

public static partial class Bitboard
{
    public const ulong MultiAttacker = 0xFFFF_FFFF_FFFF_FFFF;

    public static ulong Checks(bool checksForWhite, int[] boardStatus)
    {
        ulong attack = MultiAttacker;

        int kpos = 0;
        if (checksForWhite)
        {
            for (int i = 0; i < 64; ++i)
            {
                if (boardStatus[i] == (int)ColourType.WHITE_KING)
                {
                    kpos = i;
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < 64; ++i)
            {
                if (boardStatus[i] == (int)ColourType.BLACK_KING)
                {
                    kpos = i;
                    break;
                }
            }
        }

        ulong slide = SlidingAttacks(checksForWhite, kpos, boardStatus);
        ulong knight = KnightAttacks(checksForWhite, kpos, boardStatus);
        ulong pawn = PawnAttacks(checksForWhite, kpos, boardStatus);

        if (slide == MultiAttacker || knight == MultiAttacker || pawn == MultiAttacker)
        {
            return attack;
        }

        if ((slide | knight) == 0)
            attack = pawn;
        else if ((slide | pawn) == 0)
            attack = knight;
        else if ((pawn | knight) == 0)
            attack = slide;

        return attack;
    }

    static ulong SlidingAttacks(bool isWhite, int from, int[] boardStatus)
    {
        ulong checks = 0;

        int lastDir = 8;
        int dirIdx = 0;

        int breakReason = 0;
        int checkers = 0;
        
        for (; dirIdx < lastDir; ++dirIdx)
        {
            int dir = PrecomputedMoves.Direction[dirIdx];
            int legalPos = from + dir;
            if(legalPos < 0 || legalPos > 63)
                continue;

            ulong c = 0;
            breakReason = 0;
            for (int i = 0; i < PrecomputedMoves.DistanceToEdge[from][dirIdx]; ++i)
            {
                int occupation = Functions.IsPieceFriendly(isWhite, boardStatus[legalPos]);
                if (occupation == 1)
                {
                    breakReason = 1;
                    break;
                }

                c |= Functions.GetBit(legalPos);

                if (occupation == 0)
                {
                    breakReason = 2;
                    break;
                }

                legalPos += dir;
            }

            if (breakReason == 2)
            {
                if (dirIdx < 4 &&
                    ((boardStatus[legalPos] - 1) % 6 == (int)ColourType.WHITE_BISHOP - 1 ||
                     (boardStatus[legalPos] - 1) % 6 == (int)ColourType.WHITE_QUEEN - 1 
                    ))
                {
                    ++checkers;
                    checks |= c;
                }
                else if (dirIdx >= 4 &&
                         ((boardStatus[legalPos] - 1) % 6 == (int)ColourType.WHITE_ROOK - 1 ||
                          (boardStatus[legalPos] - 1) % 6 == (int)ColourType.WHITE_QUEEN - 1
                         ))
                {
                    ++checkers;
                    checks |= c;
                }
            }

            if (checkers > 1)
            {
                checks = MultiAttacker;
                break;
            }
        }
        return checks;
    }
    
    public static ulong KnightAttacks(bool isWhite, int from, int [] boardStatus)
    {
        int checkers = 0;
        ulong checks = 0;
        
        foreach (var item in PrecomputedMoves.KnightMoves[from])
        {
            if (Functions.IsPieceFriendly(isWhite, boardStatus[item]) == 0)
            {
                if ((boardStatus[item] - 1) % 6 == (int)ColourType.WHITE_KNIGHT - 1)
                {
                    checks |= Functions.GetBit(item);
                    ++checkers;
                }
            }

            if (checkers > 1)
            {
                checks = MultiAttacker;
                break;
            }
        }
        
        return checks;
    }

    public static ulong PawnAttacks(bool isWhite, int from, int[] boardStatus)
    {
        ulong check = 0;
        int checkers = 0;

        int modifier = (isWhite ? -1 : 1);

        foreach (var item in PrecomputedMoves.PawnAttacks[from])
        {
            int pos = item + 8 * modifier;
            if(pos < 0 || pos > 63)
                continue;
            if (Functions.IsPieceFriendly(isWhite, boardStatus[pos]) == 0)
            {
                if ((boardStatus[item + 8 * modifier] - 1) % 6 == (int)ColourType.WHITE_PAWN - 1)
                {
                    check |= Functions.GetBit(item + 8 * modifier);
                    ++checkers;
                }
            }
            
            if (checkers > 1)
            {
                check = MultiAttacker;
                break;
            }
        }
        
        return check;
    }
    
}