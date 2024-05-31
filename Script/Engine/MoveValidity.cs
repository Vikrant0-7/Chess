using Godot;
using System;

public class MoveValidity
{
    static 	ulong GetBit(int bit){
        return (ulong)1 << (bit);
    }

    static bool _LegalPawnMoves(int pos)
    {
        return (pos == 8 || pos == 16 || pos == 7 || pos == 9);
    }
    
    public static bool Pawn(Colour colour, ulong[] boardState, int posFrom, int posTo)
    {
        if (colour == Colour.WHITE)
        {
            if (!_LegalPawnMoves(posFrom - posTo))
            {
                return false;
            }
            
            ulong freeSquares = 0; //if any bit is 0 after the for loop that means that position do not have any piece
            ulong blackSquares = 0; //tells if given position is occupied by a black piece.
            for (int i = 0; i < 12; ++i)
            {
                freeSquares |= boardState[i];
                if (i > 5)
                    blackSquares |= boardState[i];
            }
            
            bool firstMove = (posFrom >= 48); //check if pawns are at second rank, as the can move 2 steps if they are.
            int pos = posFrom - posTo;

            //if pawn is moving in straight line
            if (pos == 8 || pos == 16)
            {
                //if pawn is blocked
                if ((freeSquares & GetBit(posFrom - 8)) != 0)
                {
                    return false; //move is invalid
                }
                else if(pos == 16) //pawn is moving two steps
                {
                    if (!firstMove) //if pawn is not at 2nd rank
                    {
                        return false; //move is invalid
                    }
                    else
                    {
                        if ((freeSquares & GetBit(posTo)) != 0) //position to which pawn is moving is occupied 
                            return false; //move is invalid
                    }
                }
            }
            //pawn is capturing
            else
            {
                if ((blackSquares & GetBit(posTo)) == 0) //square is either not occupied or occupied by white piece
                    return false; //move is invalid
            }
        }
        else
        {
            
        }

        return true;
    }
}
