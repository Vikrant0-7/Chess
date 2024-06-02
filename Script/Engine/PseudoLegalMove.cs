using Godot;
using System;
using System.Collections.Generic;

//There is a lot of scope of optimisation here.
//Todo: Get legal moves from pseudolegal moves.
/* Pseudolegal Move: Moves which satisfy rules defined on how pieces moves and how they caputure
 * Eg Knight moves in L, bishops moves in diagonals,
*/
/* Legal Moves: Pseudolegal moves that do not let the king be captured.
 * Eg. You cannot move a knight pinned with king. If you do so your king can be captured in next move. 
 */

//Todo: Add support for castling
public class PseudoLegalMove
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
    
    public static List<int> Pawn(Colour colour, ulong[] boardState, ulong[] prevPawnState, int pos, bool attacks = false)
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
            bool notBlocked = false;
            if (!attacks)
            {
                //if pawn next Square is Free
                if (pos >= 8 && (freeSquares & GetBit(pos - 8)) == 0)
                {
                    @out.Add(pos - 8);
                    notBlocked = true;
                }

                if (pos >= 48) //pawn is moving two steps
                {
                    if (notBlocked &&
                        (freeSquares & GetBit(pos - 16)) == 0) //if pawn is notBlocked and position is free
                    {
                        @out.Add(pos - 16);
                    }
                }
            }

            if (!((pos+1) % 8 == 0)) //if can capture a blackPiece 
            {
                if(attacks || (blackSquares & GetBit(pos - 7)) != 0)
                    @out.Add(pos-7);                
            }
            if ( !(pos % 8 == 0)) //if can capture a blackPiece 
            {
                if(attacks || (blackSquares & GetBit(pos - 9)) != 0)
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

            if (!attacks)
            {
                bool notBlocked = false;
                //if pawn next Square is Free
                if ((freeSquares & GetBit(pos + 8)) == 0)
                {
                    @out.Add(pos + 8);
                    notBlocked = true;
                }

                if (pos <= 15) //pawn is moving two steps
                {
                    if (pos < 48 && notBlocked && (freeSquares & GetBit(pos + 16)) == 0) //if pawn is notBlocked
                    {
                        @out.Add(pos + 16);
                    }
                }
            }

            if (!(pos % 8 == 0)) //if can capture a blackPiece 
            {
                if(attacks || (whiteSquares & GetBit(pos + 7)) != 0)
                    @out.Add(pos+7);                
            }
            if ( !((pos + 1) % 8 == 0)) //if can capture a blackPiece 
            {
                if(attacks || (whiteSquares & GetBit(pos + 9)) != 0)
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

    public static List<int> Knight(Colour colour, ulong[] boardState, int pos, bool attacks = false)
    {
        List<int> @out = new List<int>();
        ulong oppositeSquares = 0; //tells if given position is occupied by a black piece.
        
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
                    if(attacks || (oppositeSquares & GetBit(legalPosition)) == 0)
                        @out.Add(legalPosition);
                }
                if (position.Y + 1 < 8)
                {
                    int legalPosition = BoardPositionToInt(position.X - 2, position.Y + 1);
                    if(attacks || (oppositeSquares & GetBit(legalPosition)) == 0)
                        @out.Add(legalPosition);
                }
            }
            if (position.X + 2 < 8)
            {
                if (position.Y - 1 >= 0)
                {
                    int legalPosition = BoardPositionToInt(position.X + 2, position.Y - 1);
                    if(attacks || (oppositeSquares & GetBit(legalPosition)) == 0)
                        @out.Add(legalPosition);
                }
                if (position.Y + 1 < 8)
                {
                    int legalPosition = BoardPositionToInt(position.X + 2, position.Y + 1);
                    if(attacks || (oppositeSquares & GetBit(legalPosition)) == 0)
                        @out.Add(legalPosition);
                }
            }
            
            if (position.Y - 2 >= 0)
            {
                if (position.X - 1 >= 0)
                {
                    int legalPosition = BoardPositionToInt(position.X - 1, position.Y - 2);
                    if(attacks || (oppositeSquares & GetBit(legalPosition)) == 0)
                        @out.Add(legalPosition);
                }
                if (position.X + 1 < 8)
                {
                    int legalPosition = BoardPositionToInt(position.X + 1, position.Y - 2);
                    if(attacks|| (oppositeSquares & GetBit(legalPosition)) == 0)
                        @out.Add(legalPosition);
                }
            }
            if (position.Y + 2 < 8)
            {
                if (position.X - 1 >= 0)
                {
                    int legalPosition = BoardPositionToInt(position.X - 1, position.Y + 2);
                    if(attacks|| (oppositeSquares & GetBit(legalPosition)) == 0)
                        @out.Add(legalPosition);
                }
                if (position.X + 1 < 8)
                {
                    int legalPosition = BoardPositionToInt(position.X + 1, position.Y + 2);
                    if(attacks|| (oppositeSquares & GetBit(legalPosition)) == 0)
                        @out.Add(legalPosition);
                }
            }
        return @out;
    }

    public static List<int> Bishop(Colour colour, ulong[] boardState, int pos, bool attacks = false)
    {
        List<int> @return = new List<int>();
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
            
            for (int y = position.Y - 1; y >= 0; --y , ++x)
            {
                int possiblePos = BoardPositionToInt(position.X - x, y);
                if (position.X - x < 0)
                {
                    stopSearchLeft = true;
                }
                if (!stopSearchLeft && (freeSquares & GetBit(possiblePos)) == 0)
                {
                    @return.Add(possiblePos);
                }
                else if (!stopSearchLeft && (oppositeSquares & GetBit(possiblePos)) != 0)
                {
                    @return.Add(possiblePos);
                    stopSearchLeft = true;
                }
                else
                {
                    if (!stopSearchLeft && attacks)
                    {
                        @return.Add(possiblePos);
                    }
                    stopSearchLeft = true;
                }
                possiblePos = BoardPositionToInt(position.X + x, y);
                if (position.X + x >= 8)
                {
                    stopSearchRight = true;
                }
                if (!stopSearchRight && (freeSquares & GetBit(possiblePos)) == 0)
                {
                    @return.Add(possiblePos);
                }
                else if (!stopSearchRight && (oppositeSquares & GetBit(possiblePos)) != 0)
                {
                    @return.Add(possiblePos);
                    stopSearchRight = true;
                }
                else
                {
                    if (!stopSearchRight && attacks)
                    {
                        @return.Add(possiblePos);
                    }
                    stopSearchRight = true;
                }
            }

            x = 1;
            stopSearchLeft = false; stopSearchRight = false;
            for (int y = position.Y + 1; y < 8; ++y , ++x)
            {
                int possiblePos = BoardPositionToInt(position.X - x, y);
                if (position.X - x < 0)
                {
                    stopSearchLeft = true;
                }
                if (!stopSearchLeft && (freeSquares & GetBit(possiblePos)) == 0)
                {
                    @return.Add(possiblePos);
                }
                else if (!stopSearchLeft && (oppositeSquares & GetBit(possiblePos)) != 0)
                {
                    @return.Add(possiblePos);
                    stopSearchLeft = true;
                }
                else
                {
                    if (!stopSearchLeft && attacks)
                    {
                        @return.Add(possiblePos);
                    }
                    stopSearchLeft = true;
                }
                possiblePos = BoardPositionToInt(position.X + x, y);
                if (position.X + x >= 8)
                {
                    stopSearchRight = true;
                }
                if (!stopSearchRight && (freeSquares & GetBit(possiblePos)) == 0)
                {
                    @return.Add(possiblePos);
                }
                else if (!stopSearchRight && (oppositeSquares & GetBit(possiblePos)) != 0)
                {
                    @return.Add(possiblePos);
                    stopSearchRight = true;
                }
                else
                {
                    if (!stopSearchRight && attacks)
                    {
                        @return.Add(possiblePos);
                    }
                    stopSearchRight = true;
                }
            }
        return @return;
    }

    public static List<int> Rook(Colour colour, ulong[] boardState, int pos,  bool attacks = false)
    {
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
                moves.Add(possiblePosition);
            }
            else if ((oppositeSquares & GetBit(possiblePosition)) != 0)
            {
                moves.Add(possiblePosition);
                break;
            }
            else
            {
                if (attacks)
                {
                    moves.Add(possiblePosition);
                }
                break;
            }
        }
        for (int x = position.X + 1; x < 8; ++x)
        {
            int possiblePosition = BoardPositionToInt(x, position.Y);
            if ((freeSquares & GetBit(possiblePosition)) == 0)
            {
                moves.Add(possiblePosition);
            }
            else if ((oppositeSquares & GetBit(possiblePosition)) != 0)
            {
                moves.Add(possiblePosition);
                break;
            }
            else
            {
                if (attacks)
                {
                    moves.Add(possiblePosition);
                }
                break;
            }
        }
        for (int y = position.Y - 1; y >= 0; --y)
        {
            int possiblePosition = BoardPositionToInt(position.X,y);
            if ((freeSquares & GetBit(possiblePosition)) == 0)
            {
                moves.Add(possiblePosition);
            }
            else if ((oppositeSquares & GetBit(possiblePosition)) != 0)
            {
                moves.Add(possiblePosition);
                break;
            }
            else
            {
                if (attacks)
                {
                    moves.Add(possiblePosition);
                }
                break;
            }
        }
        for (int y = position.Y + 1; y < 8; ++y)
        {
            int possiblePosition = BoardPositionToInt(position.X,y);
            if ((freeSquares & GetBit(possiblePosition)) == 0)
            {
                moves.Add(possiblePosition);
            }
            else if ((oppositeSquares & GetBit(possiblePosition)) != 0)
            {
                moves.Add(possiblePosition);
                break;
            }
            else
            {
                if (attacks)
                {
                    moves.Add(possiblePosition);
                }
                break;
            }
        }
        
        return moves;
    }

    public static List<int> Queen(Colour colour, ulong[] boardState, int pos,  bool attacks = false)
    {
        List<int> @out = Rook(colour, boardState, pos,attacks);
        @out.AddRange(Bishop(colour, boardState, pos,attacks));
        return @out;
    }

    public static List<int> King(Colour colour, ulong[] boardState, int pos,  bool attacks = false)
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
                if(x < 0 || y < 0)
                    continue;
                if(x == postion.X && y == postion.Y)
                    continue;
                int possiblePosition = BoardPositionToInt(x, y);
                if((sameSquares & GetBit(possiblePosition)) == 0)
                    moves.Add(possiblePosition);
                else if(attacks)
                        moves.Add(possiblePosition);
            }

        }

        return moves;
    }
}
