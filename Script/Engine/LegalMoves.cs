using Godot;
using System;
using System.Collections.Generic;

/* FIXME: Try to optimise search for legal moves
 * Currently search works on making all pseudolegal moves
 * and see if king can be captured, if it can it is illegal.
 */

//Todo: Using Checkers Bitboard Remove the need of function RemoveIllegalMoves().

public class LegalMoves
{
    private static bool _debug = true;
    static 	ulong GetBit(int bit){
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

    static bool MoveIsInDirection(int dirX, int dirY, Vector2I reference)
    {
        float angle = dirX * reference.X + reference.Y * dirY;
        angle /= (Mathf.Sqrt(reference.LengthSquared()) * Mathf.Sqrt(dirX * dirX + dirY * dirY));
        angle = Mathf.Clamp(angle,-1,1);
        
        angle = Mathf.Acos(angle);
        return (Mathf.IsEqualApprox(angle, 180 / Mathf.Pi) || Mathf.IsZeroApprox(angle)) ;
    }

    //Work around: If King is under check use previous method to make sure move is legal
    public static List<int> Pawn(Colour colour, ulong[] boardState, ulong bitboard, ulong attackBitboard, int enPassantPosition, int pos)
    {
        List<int> @out = new List<int>();
        Vector2I pPos = IntToBoardPosition(pos);

        Vector2I kPos = Vector2I.One * -1;

        if (colour == Colour.WHITE)
        {
            for (int i = 63; i >= 0; --i)
            {
                if ((boardState[0] & GetBit(i)) != 0)
                {
                    kPos = IntToBoardPosition(i);
                }
            }
        }
        else
        {
            for (int i = 0; i < 64; ++i)
            {
                if ((boardState[6] & GetBit(i)) != 0)
                {
                    kPos = IntToBoardPosition(i);
                }
            }
        }

        Vector2I direction = pPos - kPos;
        bool isPinned = (bitboard & GetBit(pos)) != 0;
        bool check = (attackBitboard & GetBit(kPos.Y * 8 + kPos.X)) != 0;
        
        if (colour == Colour.WHITE)
        {
            ulong freeSquares = 0; //if any bit is 0 after the for loop that means that position do not have any piece
            ulong blackSquares = 0; //tells if given position is occupied by a black piece.
            for (int i = 0; i < 12; ++i)
            {
                freeSquares |= boardState[i];
                if (i > 5)
                    blackSquares |= boardState[i];
            }

            bool notBlocked = false;

            int legalPos = BoardPositionToInt(pPos.X, pPos.Y - 1);
            //if pawn next Square is Free
            if (pPos.Y > 0 && (freeSquares & GetBit(legalPos)) == 0)
            {
                if (!isPinned || MoveIsInDirection(0, - 1, direction))
                {
                    @out.Add(legalPos);
                    notBlocked = true;
                }
            }

            
            if (pPos.Y == 6) //pawn is moving two steps
            {
                legalPos = BoardPositionToInt(pPos.X, pPos.Y - 2);
                if (notBlocked &&
                    (freeSquares & GetBit(legalPos)) == 0) //if pawn is notBlocked and position is free
                {
                    if(!isPinned || MoveIsInDirection(0, - 2, direction))
                        @out.Add(pos - 16);
                }
            }

            
            if (pPos.X > 0) //if can capture a blackPiece 
            {
                legalPos = BoardPositionToInt(pPos.X - 1, pPos.Y - 1);
                if ((blackSquares & GetBit(legalPos)) != 0)
                {
                    if(!isPinned || MoveIsInDirection(- 1,  - 1, direction))   
                        @out.Add(legalPos);
                }

                if (enPassantPosition == legalPos && (freeSquares & GetBit(legalPos)) == 0)
                {
                    if(!isPinned || MoveIsInDirection( - 1,  - 1, direction))   
                        @out.Add(legalPos);
                }
            }

            if (pPos.X < 7) //if can capture a blackPiece 
            {
                legalPos = BoardPositionToInt(pPos.X + 1, pPos.Y - 1);
                if ((blackSquares & GetBit(legalPos)) != 0)
                {
                    if(!isPinned || MoveIsInDirection(  1,  - 1, direction))   
                        @out.Add(legalPos);
                }

                if (enPassantPosition == legalPos && (freeSquares & GetBit(legalPos)) == 0)
                {
                    if(!isPinned || MoveIsInDirection(  1,  - 1, direction))   
                        @out.Add(legalPos);
                }
            }
        }
        else
        {
            ulong freeSquares = 0; //if any bit is 0 after the for loop that means that position do not have any piece
            ulong whiteSquares = 0; //tells if given position is occupied by a black piece.
            for (int i = 0; i < 12; ++i)
            {
                freeSquares |= boardState[i];
                if (i <= 5)
                    whiteSquares |= boardState[i];
            }
            bool notBlocked = false;
            int legalPos = BoardPositionToInt(pPos.X, pPos.Y + 1);
            //if pawn next Square is Free
            if (pPos.Y > 0 && (freeSquares & GetBit(legalPos)) == 0)
            {
                if (!isPinned || MoveIsInDirection(0, 1, direction))
                {
                    @out.Add(legalPos);
                    notBlocked = true;
                }
            }
            if (pPos.Y == 1) //pawn is moving two steps
            {
                legalPos = BoardPositionToInt(pPos.X, pPos.Y + 2);
                if (notBlocked &&
                    (freeSquares & GetBit(legalPos)) == 0) //if pawn is notBlocked and position is free
                {
                    if (!isPinned || MoveIsInDirection(0, +2, direction))
                        @out.Add(legalPos);
                }
            }
            if (pPos.X > 0) //if can capture a blackPiece 
            {
                legalPos = BoardPositionToInt(pPos.X - 1, pPos.Y + 1);
                if ((whiteSquares & GetBit(legalPos)) != 0)
                {
                    if (!isPinned || MoveIsInDirection(-1, 1, direction))
                        @out.Add(legalPos);
                }

                if (enPassantPosition == legalPos && (freeSquares & GetBit(legalPos)) == 0)
                {
                    if (!isPinned || MoveIsInDirection(-1, 1, direction))
                        @out.Add(legalPos);
                }
            }
            if (pPos.X < 7) //if can capture a blackPiece 
            {
                legalPos = BoardPositionToInt(pPos.X + 1, pPos.Y + 1);
                if ((whiteSquares & GetBit(legalPos)) != 0)
                {
                    if (!isPinned || MoveIsInDirection(1, 1, direction))
                        @out.Add(legalPos);
                }
                if (enPassantPosition == legalPos && (freeSquares & GetBit(legalPos)) == 0)
                {
                    if (!isPinned || MoveIsInDirection(1, 1, direction))
                        @out.Add(legalPos);
                }
            }
        }

        if (check)
            return RemoveIllegalMoves((colour == Colour.WHITE ? 0 : 6) + 5, pos, boardState, @out);
        
        return @out;
    }

    public static List<int> Knight(Colour colour, ulong[] boardState, ulong bitboard, ulong attackBitboard, int pos)
    {
        
        List<int> @out = new List<int>();
        ulong oppositeSquares = 0; //tells if given position is occupied by a black piece.

        if ((bitboard & GetBit(pos)) != 0) //if a knight is pinned. It cannot move
            return @out;
        
        int kPos = -1;

        if (colour == Colour.WHITE)
        {
            for (int i = 63; i >= 0; --i)
            {
                if ((boardState[0] & GetBit(i)) != 0)
                {
                    kPos = i;
                }
            }
        }
        else
        {
            for (int i = 0; i < 64; ++i)
            {
                if ((boardState[6] & GetBit(i)) != 0)
                {
                    kPos = i;
                }
            }
        }
        bool check = (attackBitboard & GetBit(kPos)) != 0;

        
        if (colour == Colour.WHITE)
        {
            for (int i = 0; i <= 5; ++i)
            {
                oppositeSquares |= boardState[i];
            }
        }
        else
        {
            for (int i = 6; i < 12; ++i)
            {
                oppositeSquares |= boardState[i];
            }
        }

        Vector2I position = IntToBoardPosition(pos);
        if (position.X - 2 >= 0)
        {
            if (position.Y - 1 >= 0)
            {
                int legalPosition = BoardPositionToInt(position.X - 2, position.Y - 1);
                if ((oppositeSquares & GetBit(legalPosition)) == 0)
                    @out.Add(legalPosition);
            }

            if (position.Y + 1 < 8)
            {
                int legalPosition = BoardPositionToInt(position.X - 2, position.Y + 1);
                if ((oppositeSquares & GetBit(legalPosition)) == 0)
                    @out.Add(legalPosition);
            }
        }

        if (position.X + 2 < 8)
        {
            if (position.Y - 1 >= 0)
            {
                int legalPosition = BoardPositionToInt(position.X + 2, position.Y - 1);
                if ((oppositeSquares & GetBit(legalPosition)) == 0)
                    @out.Add(legalPosition);
            }

            if (position.Y + 1 < 8)
            {
                int legalPosition = BoardPositionToInt(position.X + 2, position.Y + 1);
                if ((oppositeSquares & GetBit(legalPosition)) == 0)
                    @out.Add(legalPosition);
            }
        }

        if (position.Y - 2 >= 0)
        {
            if (position.X - 1 >= 0)
            {
                int legalPosition = BoardPositionToInt(position.X - 1, position.Y - 2);
                if ((oppositeSquares & GetBit(legalPosition)) == 0)
                    @out.Add(legalPosition);
            }

            if (position.X + 1 < 8)
            {
                int legalPosition = BoardPositionToInt(position.X + 1, position.Y - 2);
                if ((oppositeSquares & GetBit(legalPosition)) == 0)
                    @out.Add(legalPosition);
            }
        }

        if (position.Y + 2 < 8)
        {
            if (position.X - 1 >= 0)
            {
                int legalPosition = BoardPositionToInt(position.X - 1, position.Y + 2);
                if ((oppositeSquares & GetBit(legalPosition)) == 0)
                    @out.Add(legalPosition);
            }

            if (position.X + 1 < 8)
            {
                int legalPosition = BoardPositionToInt(position.X + 1, position.Y + 2);
                if ((oppositeSquares & GetBit(legalPosition)) == 0)
                    @out.Add(legalPosition);
            }
        }
        
        if (check)
            return RemoveIllegalMoves((colour == Colour.WHITE ? 0 : 6) + 4, pos, boardState, @out);

        return @out;
    }

    public static List<int> Bishop(Colour colour, ulong[] boardState, ulong bitboard, ulong attackBitboard, int pos)
    {
        List<int> @return = new List<int>();
        
        Vector2I bPos = IntToBoardPosition(pos);
        Vector2I kPos = Vector2I.One * -1;

        if (colour == Colour.WHITE)
        {
            for (int i = 63; i >= 0; --i)
            {
                if ((boardState[0] & GetBit(i)) != 0)
                {
                    kPos = IntToBoardPosition(i);
                }
            }
        }
        else
        {
            for (int i = 0; i < 64; ++i)
            {
                if ((boardState[6] & GetBit(i)) != 0)
                {
                    kPos = IntToBoardPosition(i);
                }
            }
        }

        Vector2I direction = bPos - kPos;
        bool isPinned = (bitboard & GetBit(pos)) != 0;
        bool check = (attackBitboard & GetBit(kPos.Y * 8 + kPos.X)) != 0;
        
        ulong freeSquares = 0; //if any bit is 0 after the for loop that means that position do not have any piece
        ulong oppositeSquares = 0; //tells if given position is occupied by a black piece.

        if (colour == Colour.WHITE)
        {
            for (int i = 0; i < 12; ++i)
            {
                freeSquares |= boardState[i];
                if (i > 5)
                    oppositeSquares |= boardState[i];
            }
        }
        else
        {
            for (int i = 0; i < 12; ++i)
            {
                freeSquares |= boardState[i];
                if (i <= 5)
                    oppositeSquares |= boardState[i];
            }
        }

        Vector2I position = IntToBoardPosition(pos);
        int x = 1;
        bool stopSearchLeft = false, stopSearchRight = false;

        for (int y = position.Y - 1; y >= 0; --y, ++x)
        {
            int possiblePos = BoardPositionToInt(position.X - x, y);
            if (position.X - x < 0)
            {
                stopSearchLeft = true;
            }

            if (!stopSearchLeft && (freeSquares & GetBit(possiblePos)) == 0)
            {
                if (!isPinned || MoveIsInDirection(-x, y - position.Y, direction))
                    @return.Add(possiblePos);
                else
                    stopSearchLeft = true;
            }
            else if (!stopSearchLeft && (oppositeSquares & GetBit(possiblePos)) != 0)
            {
                if(!isPinned || MoveIsInDirection(-x,y - position.Y, direction))
                    @return.Add(possiblePos);
                stopSearchLeft = true;
            }
            else
            {
                stopSearchLeft = true;
            }

            possiblePos = BoardPositionToInt(position.X + x, y);
            if (position.X + x >= 8)
            {
                stopSearchRight = true;
            }

            if (!stopSearchRight && (freeSquares & GetBit(possiblePos)) == 0)
            {
                if (!isPinned || MoveIsInDirection(x, y - position.Y, direction))
                    @return.Add(possiblePos);
                else
                    stopSearchRight = true;
            }
            else if (!stopSearchRight && (oppositeSquares & GetBit(possiblePos)) != 0)
            {
                if(!isPinned || MoveIsInDirection(x,y - position.Y, direction))
                    @return.Add(possiblePos);
                stopSearchRight = true;
            }
            else
            {
                stopSearchRight = true;
            }
        }

        x = 1;
        stopSearchLeft = false;
        stopSearchRight = false;
        for (int y = position.Y + 1; y < 8; ++y, ++x)
        {
            int possiblePos = BoardPositionToInt(position.X - x, y);
            if (position.X - x < 0)
            {
                stopSearchLeft = true;
            }

            if (!stopSearchLeft && (freeSquares & GetBit(possiblePos)) == 0)
            {
                if(!isPinned || MoveIsInDirection(-x,y - position.Y, direction))
                    @return.Add(possiblePos);
                else
                {
                    stopSearchLeft = true;
                }
            }
            else if (!stopSearchLeft && (oppositeSquares & GetBit(possiblePos)) != 0)
            {
                if(!isPinned || MoveIsInDirection(-x,y - position.Y, direction))
                    @return.Add(possiblePos);
                stopSearchLeft = true;
            }
            else
            {
                stopSearchLeft = true;
            }

            possiblePos = BoardPositionToInt(position.X + x, y);
            if (position.X + x >= 8)
            {
                stopSearchRight = true;
            }

            if (!stopSearchRight && (freeSquares & GetBit(possiblePos)) == 0)
            {
                if(!isPinned || MoveIsInDirection(x,y - position.Y, direction))
                    @return.Add(possiblePos);
                else
                    stopSearchRight = true;
            }
            else if (!stopSearchRight && (oppositeSquares & GetBit(possiblePos)) != 0)
            {
                if(!isPinned || MoveIsInDirection(x,y - position.Y, direction))
                    @return.Add(possiblePos);
                stopSearchRight = true;
            }
            else
            {
                stopSearchRight = true;
            }
        }
        if(check)
            return RemoveIllegalMoves((colour == Colour.WHITE ? 0 : 6) + 3, pos, boardState, @return);
        return @return;
    }

    public static List<int> Rook(Colour colour, ulong[] boardState, ulong bitboard, ulong attackBitboard, int pos)
    {
        
        Vector2I rPos = IntToBoardPosition(pos);
        Vector2I kPos = Vector2I.One * -1;

        if (colour == Colour.WHITE)
        {
            for (int i = 63; i >= 0; --i)
            {
                if ((boardState[0] & GetBit(i)) != 0)
                {
                    kPos = IntToBoardPosition(i);
                }
            }
        }
        else
        {
            for (int i = 0; i < 64; ++i)
            {
                if ((boardState[6] & GetBit(i)) != 0)
                {
                    kPos = IntToBoardPosition(i);
                }
            }
        }

        Vector2I direction = rPos - kPos;
        bool isPinned = (bitboard & GetBit(pos)) != 0;
        bool check = (attackBitboard & GetBit(kPos.Y * 8 + kPos.X)) != 0;
        
        ulong freeSquares = 0;
        ulong oppositeSquares = 0;
        List<int> moves = new List<int>();

        if (colour == Colour.BLACK)
        {
            for (int i = 0; i < boardState.Length; ++i)
            {
                freeSquares |= boardState[i];
                if (i <= 5)
                {
                    oppositeSquares |= boardState[i];
                }
            }
        }
        else
        {
            for (int i = 0; i < boardState.Length; ++i)
            {
                freeSquares |= boardState[i];
                if (i > 5)
                {
                    oppositeSquares |= boardState[i];
                }
            }
        }

        Vector2I position = IntToBoardPosition(pos);

        for (int x = position.X - 1; x >= 0; --x)
        {
            int possiblePosition = BoardPositionToInt(x, position.Y);
            if ((freeSquares & GetBit(possiblePosition)) == 0)
            {
                if(!isPinned || MoveIsInDirection(x-position.X,0,direction))
                    moves.Add(possiblePosition);
                else
                    break;
            }
            else if ((oppositeSquares & GetBit(possiblePosition)) != 0)
            {
                if(!isPinned || MoveIsInDirection(x-position.X,0,direction))
                    moves.Add(possiblePosition);
                break;
            }
            else
            {
                break;
            }
        }

        for (int x = position.X + 1; x < 8; ++x)
        {
            int possiblePosition = BoardPositionToInt(x, position.Y);
            if ((freeSquares & GetBit(possiblePosition)) == 0)
            {
                if(!isPinned || MoveIsInDirection(x-position.X,0,direction))
                    moves.Add(possiblePosition);
                else
                    break;
            }
            else if ((oppositeSquares & GetBit(possiblePosition)) != 0)
            {
                if(!isPinned || MoveIsInDirection(x-position.X,0,direction))
                    moves.Add(possiblePosition);
                break;
            }
            else
            {
                break;
            }
        }

        for (int y = position.Y - 1; y >= 0; --y)
        {
            int possiblePosition = BoardPositionToInt(position.X, y);
            if ((freeSquares & GetBit(possiblePosition)) == 0)
            {
                if (!isPinned || MoveIsInDirection(0, y - position.Y, direction))
                    moves.Add(possiblePosition);
                else 
                    break;
            }
            else if ((oppositeSquares & GetBit(possiblePosition)) != 0)
            {
                if (!isPinned || MoveIsInDirection(0, y - position.Y, direction))
                    moves.Add(possiblePosition);
                break;
            }
            else
            {
                break;
            }
        }

        for (int y = position.Y + 1; y < 8; ++y)
        {
            int possiblePosition = BoardPositionToInt(position.X, y);
            if ((freeSquares & GetBit(possiblePosition)) == 0)
            {
                if (!isPinned || MoveIsInDirection(0, y - position.Y, direction))
                    moves.Add(possiblePosition);
                else 
                    break;
            }
            else if ((oppositeSquares & GetBit(possiblePosition)) != 0)
            {
                if (!isPinned || MoveIsInDirection(0, y - position.Y, direction))
                    moves.Add(possiblePosition);
                break;
            }
            else
            {
                break;
            }
        }
        if(check)
            return RemoveIllegalMoves((colour == Colour.WHITE ? 0 : 6) + 2, pos, boardState, moves);
        return moves;
    }

    public static List<int> Queen(Colour colour, ulong[] boardState, ulong bitboard, ulong attackBitboard, int pos)
    {
                
        Vector2I rPos = IntToBoardPosition(pos);
        Vector2I kPos = Vector2I.One * -1;

        if (colour == Colour.WHITE)
        {
            for (int i = 63; i >= 0; --i)
            {
                if ((boardState[0] & GetBit(i)) != 0)
                {
                    kPos = IntToBoardPosition(i);
                }
            }
        }
        else
        {
            for (int i = 0; i < 64; ++i)
            {
                if ((boardState[6] & GetBit(i)) != 0)
                {
                    kPos = IntToBoardPosition(i);
                }
            }
        }

        Vector2I direction = rPos - kPos;
        bool isPinned = (bitboard & GetBit(pos)) != 0;
        bool check = (attackBitboard & GetBit(kPos.Y * 8 + kPos.X)) != 0;
        
        ulong freeSquares = 0;
        ulong oppositeSquares = 0;
        List<int> moves = new List<int>();

        if (colour == Colour.BLACK)
        {
            for (int i = 0; i < boardState.Length; ++i)
            {
                freeSquares |= boardState[i];
                if (i <= 5)
                {
                    oppositeSquares |= boardState[i];
                }
            }
        }
        else
        {
            for (int i = 0; i < boardState.Length; ++i)
            {
                freeSquares |= boardState[i];
                if (i > 5)
                {
                    oppositeSquares |= boardState[i];
                }
            }
        }

        Vector2I position = IntToBoardPosition(pos);

        for (int x = position.X - 1; x >= 0; --x)
        {
            int possiblePosition = BoardPositionToInt(x, position.Y);
            if ((freeSquares & GetBit(possiblePosition)) == 0)
            {
                if(!isPinned || MoveIsInDirection(x-position.X,0,direction))
                    moves.Add(possiblePosition);
                else
                    break;
            }
            else if ((oppositeSquares & GetBit(possiblePosition)) != 0)
            {
                if(!isPinned || MoveIsInDirection(x-position.X,0,direction))
                    moves.Add(possiblePosition);
                break;
            }
            else
            {
                break;
            }
        }

        for (int x = position.X + 1; x < 8; ++x)
        {
            int possiblePosition = BoardPositionToInt(x, position.Y);
            if ((freeSquares & GetBit(possiblePosition)) == 0)
            {
                if(!isPinned || MoveIsInDirection(x-position.X,0,direction))
                    moves.Add(possiblePosition);
                else
                    break;
            }
            else if ((oppositeSquares & GetBit(possiblePosition)) != 0)
            {
                if(!isPinned || MoveIsInDirection(x-position.X,0,direction))
                    moves.Add(possiblePosition);
                break;
            }
            else
            {
                break;
            }
        }

        for (int y = position.Y - 1; y >= 0; --y)
        {
            int possiblePosition = BoardPositionToInt(position.X, y);
            if ((freeSquares & GetBit(possiblePosition)) == 0)
            {
                if (!isPinned || MoveIsInDirection(0, y - position.Y, direction))
                    moves.Add(possiblePosition);
                else 
                    break;
            }
            else if ((oppositeSquares & GetBit(possiblePosition)) != 0)
            {
                if (!isPinned || MoveIsInDirection(0, y - position.Y, direction))
                    moves.Add(possiblePosition);
                break;
            }
            else
            {
                break;
            }
        }

        for (int y = position.Y + 1; y < 8; ++y)
        {
            int possiblePosition = BoardPositionToInt(position.X, y);
            if ((freeSquares & GetBit(possiblePosition)) == 0)
            {
                if (!isPinned || MoveIsInDirection(0, y - position.Y, direction))
                    moves.Add(possiblePosition);
                else 
                    break;
            }
            else if ((oppositeSquares & GetBit(possiblePosition)) != 0)
            {
                if (!isPinned || MoveIsInDirection(0, y - position.Y, direction))
                    moves.Add(possiblePosition);
                break;
            }
            else
            {
                break;
            }
        }
        
        int X = 1;
        bool stopSearchLeft = false, stopSearchRight = false;

        for (int y = position.Y - 1; y >= 0; --y, ++X)
        {
            int possiblePos = BoardPositionToInt(position.X - X, y);
            if (position.X - X < 0)
            {
                stopSearchLeft = true;
            }

            if (!stopSearchLeft && (freeSquares & GetBit(possiblePos)) == 0)
            {
                if (!isPinned || MoveIsInDirection(-X, y - position.Y, direction))
                    moves.Add(possiblePos);
                else
                    stopSearchLeft = true;
            }
            else if (!stopSearchLeft && (oppositeSquares & GetBit(possiblePos)) != 0)
            {
                if(!isPinned || MoveIsInDirection(-X,y - position.Y, direction))
                    moves.Add(possiblePos);
                stopSearchLeft = true;
            }
            else
            {
                stopSearchLeft = true;
            }

            possiblePos = BoardPositionToInt(position.X + X, y);
            if (position.X + X >= 8)
            {
                stopSearchRight = true;
            }

            if (!stopSearchRight && (freeSquares & GetBit(possiblePos)) == 0)
            {
                if (!isPinned || MoveIsInDirection(X, y - position.Y, direction))
                    moves.Add(possiblePos);
                else
                    stopSearchRight = true;
            }
            else if (!stopSearchRight && (oppositeSquares & GetBit(possiblePos)) != 0)
            {
                if(!isPinned || MoveIsInDirection(X,y - position.Y, direction))
                    moves.Add(possiblePos);
                stopSearchRight = true;
            }
            else
            {
                stopSearchRight = true;
            }
        }

        X = 1;
        stopSearchLeft = false;
        stopSearchRight = false;
        for (int y = position.Y + 1; y < 8; ++y, ++X)
        {
            int possiblePos = BoardPositionToInt(position.X - X, y);
            if (position.X - X < 0)
            {
                stopSearchLeft = true;
            }

            if (!stopSearchLeft && (freeSquares & GetBit(possiblePos)) == 0)
            {
                if(!isPinned || MoveIsInDirection(-X,y - position.Y, direction))
                    moves.Add(possiblePos);
                else
                {
                    stopSearchLeft = true;
                }
            }
            else if (!stopSearchLeft && (oppositeSquares & GetBit(possiblePos)) != 0)
            {
                if(!isPinned || MoveIsInDirection(-X,y - position.Y, direction))
                    moves.Add(possiblePos);
                stopSearchLeft = true;
            }
            else
            {
                stopSearchLeft = true;
            }

            possiblePos = BoardPositionToInt(position.X + X, y);
            if (position.X + X >= 8)
            {
                stopSearchRight = true;
            }

            if (!stopSearchRight && (freeSquares & GetBit(possiblePos)) == 0)
            {
                if(!isPinned || MoveIsInDirection(X,y - position.Y, direction))
                    moves.Add(possiblePos);
                else
                    stopSearchRight = true;
            }
            else if (!stopSearchRight && (oppositeSquares & GetBit(possiblePos)) != 0)
            {
                if(!isPinned || MoveIsInDirection(X,y - position.Y, direction))
                    moves.Add(possiblePos);
                stopSearchRight = true;
            }
            else
            {
                stopSearchRight = true;
            }
        }
        
        if(check)
            return RemoveIllegalMoves((colour == Colour.WHITE ? 0 : 6) + 2, pos, boardState, moves);
        return moves;
    }

    public static List<int> King(Colour colour, ulong[] boardState, int pos, bool[] canCastle)
    {
        ulong sameSquares = 0;
        List<int> moves = new List<int>();

        if (colour == Colour.WHITE)
        {
            for (int i = 0; i < boardState.Length; ++i)
            {
                if (i <= 5)
                {
                    sameSquares |= boardState[i];
                }
            }
        }
        else
        {
            for (int i = 0; i < boardState.Length; ++i)
            {
                if (i > 5)
                {
                    sameSquares |= boardState[i];
                }
            }
        }

        Vector2I postion = IntToBoardPosition(pos);

        for (int y = postion.Y - 1; y < 8 && y <= postion.Y + 1; ++y)
        {
            for (int x = postion.X - 1; x < 8 && x <= postion.X + 1; ++x)
            {
                if (x < 0 || y < 0)
                    continue;
                if (x == postion.X && y == postion.Y)
                    continue;
                int possiblePosition = BoardPositionToInt(x, y);
                if ((sameSquares & GetBit(possiblePosition)) == 0)
                    moves.Add(possiblePosition);

            }
        }

        if (canCastle[0])
            moves.Add(pos - 2);
        if (canCastle[1])
            moves.Add(pos + 2);

        return RemoveIllegalMoves((colour == Colour.WHITE ? 0 : 6), pos, boardState, moves);
    }

    
    public static List<int> RemoveIllegalMoves(int pieceIdx, int pos, ulong[] boardStatus, List<int> moves)
    {
        List<int> lastPlace = new List<int>(moves);
        Colour c = (pieceIdx < 6) ? Colour.WHITE : Colour.BLACK;
        int upper = moves.Count;
        
        if (pieceIdx % 6 == 0) //check if castling can be done.
        {
            ulong a = AttackBitboard.GetAttackBitBoard((pieceIdx < 6) ? Colour.BLACK : Colour.WHITE, boardStatus);
            ulong free = 0;
            for (int i = 0; i < 12; ++i)
            {
                free |= boardStatus[i];
            }
            if (moves.Contains(pos + 2)) //king side castling
            {
                if ((free & (GetBit(pos + 1) | GetBit(pos + 2))) != 0)
                {
                    lastPlace.Remove(pos + 2);
                }
                //king can not castle if it is under check or squares it will pass through to castle are under attack
                else if ((a & (GetBit(pos) | GetBit(pos + 1) | GetBit(pos + 2))) != 0)
                {
                    lastPlace.Remove(pos + 2);
                }

                --upper;
            }
            if (moves.Contains(pos - 2)) //queen side castling
            {
                if ((free & (GetBit(pos - 1) | GetBit(pos - 2) | GetBit(pos - 3))) != 0)
                {
                    lastPlace.Remove(pos - 2);
                }
                else if ((a & (GetBit(pos) | GetBit(pos - 1) | GetBit(pos - 2))) != 0)
                {
                    lastPlace.Remove(pos - 2);
                }
                --upper;
            }
        }

        for (int i = 0; i < upper; i++)
        {
            ulong[] tmpBoardStatus = new ulong[12];
            for (int j = 0; j < 12; ++j)
            {
                tmpBoardStatus[j] = boardStatus[j];
            }
			
            for (int j = (c == Colour.BLACK ? 0 : 6); j < (c == Colour.BLACK ? 6 : 12); ++j)
            {
                if ((tmpBoardStatus[j] & GetBit(moves[i])) != 0)
                {
                    tmpBoardStatus[j] ^= GetBit(moves[i]);
                    break;
                }
            }
            tmpBoardStatus[pieceIdx] ^= (GetBit(pos) | GetBit(moves[i]));
            if (c == Colour.WHITE)
            {
                ulong a = AttackBitboard.GetAttackBitBoard(Colour.BLACK, tmpBoardStatus);
                if ( (a & tmpBoardStatus[0]) != 0)
                {
                    lastPlace.Remove(moves[i]);
                }
            }
            else
            {
                ulong a = AttackBitboard.GetAttackBitBoard(Colour.WHITE, tmpBoardStatus);
                if ((a & tmpBoardStatus[6]) != 0)
                {
                    lastPlace.Remove(moves[i]);
                }
            }
        }
        return lastPlace;
    }
}
