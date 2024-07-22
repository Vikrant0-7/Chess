using System;
using Godot;

namespace Chess.Script.Engine;

public static class Functions
{
    public static ulong GetBit(int bit)
    {
        return (ulong)1 << (bit);
    }

    public static Vector2I IntToBoardPosition(int pos){
        return new Vector2I(pos & 0b111, pos >> 3);
    }
    //Converts board's position to index of that position
    public static int BoardPositionToInt(int x, int y)
    {
        return (y << 3) + x;
    }

    public static int BoardPositionToInt(Vector2I pos)
    {
        return (pos.Y << 3) + pos.X;
    }
    
    public static bool MoveIsInDirection(int dirX, int dirY, Vector2I reference)
    {
        float angle = dirX * reference.X + reference.Y * dirY;
        angle /= (Mathf.Sqrt(reference.LengthSquared()) * Mathf.Sqrt(dirX * dirX + dirY * dirY));
        angle = Mathf.Clamp(angle,-1,1);
        
        angle = Mathf.Acos(angle);
        return (Mathf.IsEqualApprox(angle, Mathf.Pi) || Mathf.IsZeroApprox(angle)) ;
    }
    
    /// <summary>
    /// Tells if given Piece if friendly or not
    /// -1: Empty, 1: Friendly, 0: Enemy
    /// </summary>
    /// <param name="isWhite">true if checking for white</param>
    /// <param name="piece">id of piece to check</param>
    /// <returns></returns>
    public static int IsPieceFriendly(bool isWhite, int piece)
    {
        if (piece == (int)ColourType.FREE)
            return -1;
        
        if (isWhite && piece >= (int)ColourType.WHITE_KING && piece <= (int)ColourType.WHITE_PAWN)
            return 1;
        
        if (!isWhite && piece >= (int)ColourType.BLACK_KING && piece <= (int)ColourType.BLACK_PAWN)
            return 1;
        
        return 0;
    }

    public static int AlgebraToInt(string pos)
    {
        return ((8 - (pos[1] - '0')) << 3) + (pos[0] - 'a');
    }

    public static Move TranslateAlgebraicMove(string algebra, Board board)
    {
        Move move = new Move();

        move.finalPosition = AlgebraToInt(algebra.Substring(2, 2));
        move.position = AlgebraToInt(algebra.Substring(0, 2));
        move.piece = board.BoardStatus[move.position];
        move.promoteTo = -1;
        
        if (algebra.Length == 5)
        {
            if (board.WhiteTurn)
            {
                switch (algebra[4])
                {
                    case 'n':
                        move.promoteTo = (int)ColourType.WHITE_KNIGHT;
                        break;
                    case 'q':
                        move.promoteTo = (int)ColourType.WHITE_QUEEN;
                        break;
                    case 'b':
                        move.promoteTo = (int)ColourType.WHITE_BISHOP;
                        break;
                    case 'r':
                        move.promoteTo = (int)ColourType.WHITE_ROOK;
                        break;
                    default:
                        move.promoteTo = -1;
                        break;
                }
            }
            else
            {
                switch (algebra[4])
                {
                    case 'n':
                        move.promoteTo = (int)ColourType.BLACK_KNIGHT;
                        break;
                    case 'q':
                        move.promoteTo = (int)ColourType.BLACK_QUEEN;
                        break;
                    case 'b':
                        move.promoteTo = (int)ColourType.BLACK_BISHOP;
                        break;
                    case 'r':
                        move.promoteTo = (int)ColourType.BLACK_ROOK;
                        break;
                    default:
                        move.promoteTo = -1;
                        break;
                }
            }
        }
        
        return move;
    }

    public static bool IsEqualApprox(Vector2 v1, Vector2 v2, float tolerence)
    {
        return Mathf.IsEqualApprox(v1.X, v2.X, tolerence) &&
               Mathf.IsEqualApprox(v1.Y, v2.Y, tolerence);
    }

    public static bool CheckBit(ulong bits, int pos)
    {
        return (bits & GetBit(pos)) != 0;
    }
    
}