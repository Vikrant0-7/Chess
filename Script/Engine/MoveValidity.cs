using Godot;
using System;
using System.Collections.Generic;
public class MoveValidity
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

    static int BoardPositionToInt(Vector2I pos)
    {
        return pos.Y * 8 + pos.X;
    }
    
    public static List<int> Pawn(Colour colour, ulong[] boardState, ulong[] prevPawnState, int pos)
    {
        List<int> @out = new List<int>();
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
            GD.Print("Freesquares = " + freeSquares);
            GD.Print("BlackSquare = " + blackSquares);
            bool notBlocked = false;
            //if pawn next Square is Free
            if (pos >= 8 && (freeSquares & GetBit(pos-8)) == 0)
            {
                if (_debug)
                    GD.Print("Next Square is Free");
                @out.Add(pos-8);
                notBlocked = true;
            }
            if (pos >= 48) //pawn is moving two steps
            {
                if (notBlocked && (freeSquares & GetBit(pos - 16)) == 0) //if pawn is notBlocked and position is free
                {
                    if (_debug)
                        GD.Print("First move for pawn, can move 2 steps");
                    @out.Add(pos-16);
                }
            }

            //TODO: possible error here!
            if (!((pos+1) % 8 == 0) && (blackSquares & GetBit(pos - 7)) != 0) //if can capture a blackPiece 
            {
                @out.Add(pos-7);                
            }
            if ( !(pos % 8 == 0) && (blackSquares & GetBit(pos - 9)) != 0) //if can capture a blackPiece 
            {
                @out.Add(pos-9);                
            }
            
            //Unoptimised en passente' 
            if (pos >= 24 && pos <= 31)
            {
               
                if (pos > 24 && (boardState[11] & GetBit(pos - 1)) != 0)
                {
                    if ((prevPawnState[11] & GetBit(pos - 16 - 1)) != 0)
                    {
                        @out.Add(pos-9);  
                    }
                }
                if (pos < 31 && (boardState[11] & GetBit(pos + 1)) != 0)
                {
                    if ((prevPawnState[11] & GetBit(pos - 16 + 1)) != 0)
                    {
                        @out.Add(pos-7);  
                    }
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
            //if pawn next Square is Free
            if ((freeSquares & GetBit(pos+8)) == 0)
            {
                if (_debug)
                    GD.Print("Next Square is Free");
                @out.Add(pos+8);
                notBlocked = true;
            }
            if (pos <= 15) //pawn is moving two steps
            {
                if (pos < 48 && notBlocked && (freeSquares & GetBit(pos + 16)) == 0) //if pawn is notBlocked
                {
                    if (_debug)
                        GD.Print("First move for pawn, can move 2 steps");
                    @out.Add(pos+16);
                }
            }

            //TODO: possible error here!
            if (!(pos % 8 == 0) && (whiteSquares & GetBit(pos + 7)) != 0) //if can capture a blackPiece 
            {
                @out.Add(pos+7);                
            }
            if ( !((pos + 1) % 8 == 0) && (whiteSquares & GetBit(pos + 9)) != 0) //if can capture a blackPiece 
            {
                @out.Add(pos+9);                
            }
            
            if (pos >= 32 && pos <= 39)
            {
               
                if (pos > 32 && (boardState[5] & GetBit(pos - 1)) != 0)
                {
                    if ((prevPawnState[5] & GetBit(pos + 16 - 1)) != 0)
                    {
                        @out.Add(pos+7);  
                    }
                }
                if (pos < 39 && (boardState[5] & GetBit(pos + 1)) != 0)
                {
                    if ((prevPawnState[5] & GetBit(pos + 16 + 1)) != 0)
                    {
                        @out.Add(pos+9);  
                    }
                }

            }
        }

        return @out;
    }

    public static List<int> Knight(Colour colour, ulong[] boardState, int pos)
    {
        List<int> @out = new List<int>();
        if (colour == Colour.WHITE)
        {
            ulong whiteSquares = 0; //tells if given position is occupied by a black piece.
            for (int i = 0; i <= 5; ++i)
            { 
                whiteSquares |= boardState[i];
            }

            Vector2I position = IntToBoardPosition(pos);
            if (position.X - 2 >= 0)
            {
                if (position.Y - 1 >= 0)
                {
                    int legalPosition = BoardPositionToInt(position.X - 2, position.Y - 1);
                    if((whiteSquares & GetBit(legalPosition)) == 0)
                        @out.Add(legalPosition);
                }
                if (position.Y + 1 < 8)
                {
                    int legalPosition = BoardPositionToInt(position.X - 2, position.Y + 1);
                    if((whiteSquares & GetBit(legalPosition)) == 0)
                        @out.Add(legalPosition);
                }
            }
            if (position.X + 2 < 8)
            {
                if (position.Y - 1 >= 0)
                {
                    int legalPosition = BoardPositionToInt(position.X + 2, position.Y - 1);
                    if((whiteSquares & GetBit(legalPosition)) == 0)
                        @out.Add(legalPosition);
                }
                if (position.Y + 1 < 8)
                {
                    int legalPosition = BoardPositionToInt(position.X + 2, position.Y + 1);
                    if((whiteSquares & GetBit(legalPosition)) == 0)
                        @out.Add(legalPosition);
                }
            }
            
            if (position.Y - 2 >= 0)
            {
                if (position.X - 1 >= 0)
                {
                    int legalPosition = BoardPositionToInt(position.X - 1, position.Y - 2);
                    if((whiteSquares & GetBit(legalPosition)) == 0)
                        @out.Add(legalPosition);
                }
                if (position.X + 1 < 8)
                {
                    int legalPosition = BoardPositionToInt(position.X + 1, position.Y - 2);
                    if((whiteSquares & GetBit(legalPosition)) == 0)
                        @out.Add(legalPosition);
                }
            }
            if (position.Y + 2 < 8)
            {
                if (position.X - 1 >= 0)
                {
                    int legalPosition = BoardPositionToInt(position.X - 1, position.Y + 2);
                    if((whiteSquares & GetBit(legalPosition)) == 0)
                        @out.Add(legalPosition);
                }
                if (position.X + 1 < 8)
                {
                    int legalPosition = BoardPositionToInt(position.X + 1, position.Y + 2);
                    if((whiteSquares & GetBit(legalPosition)) == 0)
                        @out.Add(legalPosition);
                }
            }
        }
        else
        {
            ulong blackSquare = 0; //tells if given position is occupied by a black piece.
            for (int i = 6; i < 12; ++i)
            { 
                blackSquare |= boardState[i];
            }

            Vector2I position = IntToBoardPosition(pos);
            if (position.X - 2 >= 0)
            {
                if (position.Y - 1 >= 0)
                {
                    int legalPosition = BoardPositionToInt(position.X - 2, position.Y - 1);
                    if((blackSquare & GetBit(legalPosition)) == 0)
                        @out.Add(legalPosition);
                }
                if (position.Y + 1 < 8)
                {
                    int legalPosition = BoardPositionToInt(position.X - 2, position.Y + 1);
                    if((blackSquare & GetBit(legalPosition)) == 0)
                        @out.Add(legalPosition);
                }
            }
            if (position.X + 2 < 8)
            {
                if (position.Y - 1 >= 0)
                {
                    int legalPosition = BoardPositionToInt(position.X + 2, position.Y - 1);
                    if((blackSquare & GetBit(legalPosition)) == 0)
                        @out.Add(legalPosition);
                }
                if (position.Y + 1 < 8)
                {
                    int legalPosition = BoardPositionToInt(position.X + 2, position.Y + 1);
                    if((blackSquare & GetBit(legalPosition)) == 0)
                        @out.Add(legalPosition);
                }
            }
            
            if (position.Y - 2 >= 0)
            {
                if (position.X - 1 >= 0)
                {
                    int legalPosition = BoardPositionToInt(position.X - 1, position.Y - 2);
                    if((blackSquare & GetBit(legalPosition)) == 0)
                        @out.Add(legalPosition);
                }
                if (position.X + 1 < 8)
                {
                    int legalPosition = BoardPositionToInt(position.X + 1, position.Y - 2);
                    if((blackSquare & GetBit(legalPosition)) == 0)
                        @out.Add(legalPosition);
                }
            }
            if (position.Y + 2 < 8)
            {
                if (position.X - 1 >= 0)
                {
                    int legalPosition = BoardPositionToInt(position.X - 1, position.Y + 2);
                    if((blackSquare & GetBit(legalPosition)) == 0)
                        @out.Add(legalPosition);
                }
                if (position.X + 1 < 8)
                {
                    int legalPosition = BoardPositionToInt(position.X + 1, position.Y + 2);
                    if((blackSquare & GetBit(legalPosition)) == 0)
                        @out.Add(legalPosition);
                }
            }
        }
        return @out;
    }

}
