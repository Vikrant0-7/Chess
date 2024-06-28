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
        return (Mathf.IsEqualApprox(angle, 180 / Mathf.Pi) || Mathf.IsZeroApprox(angle)) ;
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
}