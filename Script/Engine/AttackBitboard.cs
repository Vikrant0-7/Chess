using Godot;
using System;
using System.Collections.Generic;

public class AttackBitboard
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
    
    
    public static ulong GetAttackBitBoard(Colour colour, ulong[] boardStatus)
    {
        ulong attacks = 0;
        List<int> moves = new List<int>();
        if (colour == Colour.BLACK)
        {
            for (int i = 6; i <= 11; ++i)
            {
                switch (i)
                {
                    case 6:
                    {
                        for (int j = 0; j < 64; ++j)
                        {
                            if ((boardStatus[i] & GetBit(j)) != 0)
                            {
                                moves.AddRange(King(Colour.BLACK, boardStatus, j));
                                break;
                            }
                        }
                        break;
                    }
                    case 7:
                    {
                        for (int j = 0; j < 64; ++j)
                        {
                            if ((boardStatus[i] & GetBit(j)) != 0)
                            {
                                moves.AddRange(Queen(Colour.BLACK, boardStatus, j));
                                break;
                            }
                        }

                        break;
                    }
                    case 8:
                    {
                        int c = 0;
                        for (int j = 0; j < 64; ++j)
                        {
                            if ((boardStatus[i] & GetBit(j)) != 0)
                            {
                                moves.AddRange(Rook(Colour.BLACK, boardStatus, j));
                                ++c;
                            }

                            if (c == 2)
                                break;
                        }

                        break;
                    }
                    case 9:
                    {
                        int c = 0;
                        for (int j = 0; j < 64; ++j)
                        {
                            if ((boardStatus[i] & GetBit(j)) != 0)
                            {
                                moves.AddRange(Bishop(Colour.BLACK, boardStatus, j));
                                ++c;
                            }

                            if (c == 2)
                                break;
                        }

                        break;
                    }
                    case 10:
                    {
                        int c = 0;
                        for (int j = 0; j < 64; ++j)
                        {
                            if ((boardStatus[i] & GetBit(j)) != 0)
                            {
                                moves.AddRange(Knight(Colour.BLACK, boardStatus, j));
                                ++c;
                            }

                            if (c == 2)
                                break;
                        }

                        break;
                    }
                    case 11:
                    {
                        int c = 0;
                        for (int j = 0; j < 64; ++j)
                        {
                            if ((boardStatus[i] & GetBit(j)) != 0)
                            {
                                moves.AddRange(Pawn(Colour.BLACK, boardStatus, j));
                                ++c;
                            }

                            if (c == 8)
                                break;
                        }

                        break;
                    }
                }

                foreach (var item in moves)
                {
                    attacks |= GetBit(item);
                }

                moves.Clear();
            }
        }
        else
        {
            for (int i = 0; i <= 5; ++i)
            {
                switch (i + 6)
                {
                    case 6:
                    {
                        for (int j = 0; j < 64; ++j)
                        {
                            if ((boardStatus[i] & GetBit(j)) != 0)
                            {
                                moves.AddRange(King(Colour.WHITE, boardStatus, j));
                                break;
                            }
                        }

                        break;
                    }
                    case 7:
                    {
                        for (int j = 0; j < 64; ++j)
                        {
                            if ((boardStatus[i] & GetBit(j)) != 0)
                            {
                                moves.AddRange(Queen(Colour.WHITE, boardStatus, j));
                                break;
                            }
                        }

                        break;
                    }
                    case 8:
                    {
                        int c = 0;
                        for (int j = 0; j < 64; ++j)
                        {
                            if ((boardStatus[i] & GetBit(j)) != 0)
                            {
                                moves.AddRange(Rook(Colour.WHITE, boardStatus, j));
                                ++c;
                            }

                            if (c == 2)
                                break;
                        }

                        break;
                    }
                    case 9:
                    {
                        int c = 0;
                        for (int j = 0; j < 64; ++j)
                        {
                            if ((boardStatus[i] & GetBit(j)) != 0)
                            {
                                moves.AddRange(Bishop(Colour.WHITE, boardStatus, j));
                                ++c;
                            }

                            if (c == 2)
                                break;
                        }

                        break;
                    }
                    case 10:
                    {
                        int c = 0;
                        for (int j = 0; j < 64; ++j)
                        {
                            if ((boardStatus[i] & GetBit(j)) != 0)
                            {
                                moves.AddRange(Knight(Colour.WHITE, boardStatus, j));
                                ++c;
                            }

                            if (c == 2)
                                break;
                        }

                        break;
                    }
                    case 11:
                    {
                        int c = 0;
                        for (int j = 0; j < 64; ++j)
                        {
                            if ((boardStatus[i] & GetBit(j)) != 0)
                            {
                                moves.AddRange(Pawn(Colour.WHITE, boardStatus, j));
                                ++c;
                            }

                            if (c == 8)
                                break;
                        }

                        break;
                    }
                }

                foreach (var item in moves)
                {
                    attacks |= GetBit(item);
                }

                moves.Clear();
            }
        }

        return attacks;
    }

    static List<int> Pawn(Colour colour, ulong[] boardState, int pos)
    {
        List<int> @out = new List<int>();
        if (colour == Colour.WHITE)
        {
            if (!((pos + 1) % 8 == 0)) //if can capture a blackPiece 
            {
                @out.Add(pos - 7);
            }

            if (!(pos % 8 == 0)) //if can capture a blackPiece 
            {
                @out.Add(pos - 9);
            }
        }
        else
        {

            if (!(pos % 8 == 0)) //if can capture a white piece 
            {
                @out.Add(pos + 7);
            }

            if (!((pos + 1) % 8 == 0)) //if can capture a white piece 
            {
                @out.Add(pos + 9);
            }
        }

        return @out;
    }

    static List<int> Knight(Colour colour, ulong[] boardState, int pos)
    {
        List<int> @out = new List<int>();
        Vector2I position = IntToBoardPosition(pos);
        if (position.X - 2 >= 0)
        {
            if (position.Y - 1 >= 0)
            {
                int legalPosition = BoardPositionToInt(position.X - 2, position.Y - 1);
                @out.Add(legalPosition);
            }

            if (position.Y + 1 < 8)
            {
                int legalPosition = BoardPositionToInt(position.X - 2, position.Y + 1);
                @out.Add(legalPosition);
            }
        }

        if (position.X + 2 < 8)
        {
            if (position.Y - 1 >= 0)
            {
                int legalPosition = BoardPositionToInt(position.X + 2, position.Y - 1);
                @out.Add(legalPosition);
            }

            if (position.Y + 1 < 8)
            {
                int legalPosition = BoardPositionToInt(position.X + 2, position.Y + 1);
                @out.Add(legalPosition);
            }
        }

        if (position.Y - 2 >= 0)
        {
            if (position.X - 1 >= 0)
            {
                int legalPosition = BoardPositionToInt(position.X - 1, position.Y - 2);
                @out.Add(legalPosition);
            }

            if (position.X + 1 < 8)
            {
                int legalPosition = BoardPositionToInt(position.X + 1, position.Y - 2);
                @out.Add(legalPosition);
            }
        }

        if (position.Y + 2 < 8)
        {
            if (position.X - 1 >= 0)
            {
                int legalPosition = BoardPositionToInt(position.X - 1, position.Y + 2);
                @out.Add(legalPosition);
            }

            if (position.X + 1 < 8)
            {
                int legalPosition = BoardPositionToInt(position.X + 1, position.Y + 2);
                @out.Add(legalPosition);
            }
        }
        return @out;
    }

    static List<int> Bishop(Colour colour, ulong[] boardState, int pos)
    {
        List<int> moves = new List<int>();
        ulong freeSquares = 0; //if any bit is 0 after the for loop that means that position do not have any piece

        for (int i = 0; i < 12; ++i)
        {
            freeSquares |= boardState[i];
        }

        Vector2I position = IntToBoardPosition(pos);

        int i1 = 1;
        bool stopSearchLeft = false, stopSearchRight = false;

        for (int y = position.Y - 1; y >= 0; --y, ++i1)
        {
            int possiblePos = BoardPositionToInt(position.X - i1, y);
            if (position.X - i1 < 0)
            {
                stopSearchLeft = true;
            }

            if (position.X + i1 >= 8)
            {
                stopSearchRight = true;
            }

            if (!stopSearchLeft)
                moves.Add(possiblePos);

            if (!stopSearchLeft && (freeSquares & GetBit(possiblePos)) != 0)
            {
                stopSearchLeft = true;
            }

            possiblePos = BoardPositionToInt(position.X + i1, y);

            if (!stopSearchRight)
            {
                moves.Add(possiblePos);
            }

            if (!stopSearchRight && (freeSquares & GetBit(possiblePos)) != 0)
            {
                stopSearchRight = true;
            }
        }

        i1 = 1;
        stopSearchLeft = false;
        stopSearchRight = false;
        for (int y = position.Y + 1; y < 8; ++y, ++i1)
        {
            int possiblePos = BoardPositionToInt(position.X - i1, y);
            if (position.X - i1 < 0)
            {
                stopSearchLeft = true;
            }

            if (position.X + i1 >= 8)
            {
                stopSearchRight = true;
            }

            if (!stopSearchLeft)
            {
                moves.Add(possiblePos);
            }

            if (!stopSearchLeft && (freeSquares & GetBit(possiblePos)) != 0)
            {
                stopSearchLeft = true;
            }


            possiblePos = BoardPositionToInt(position.X + i1, y);
            if (!stopSearchRight)
            {
                moves.Add(possiblePos);
            }

            if (!stopSearchRight && (freeSquares & GetBit(possiblePos)) != 0)
            {
                stopSearchRight = true;
            }
        }

        return moves;
    }

    static List<int> Rook(Colour colour, ulong[] boardState, int pos)
    {
        ulong freeSquares = 0;
        List<int> moves = new List<int>();

        for (int i = 0; i < boardState.Length; ++i)
        {
            freeSquares |= boardState[i];
        }

        Vector2I position = IntToBoardPosition(pos);

        for (int x = position.X - 1; x >= 0; --x)
        {
            int possiblePosition = BoardPositionToInt(x, position.Y);
            moves.Add(possiblePosition);

            if ((freeSquares & GetBit(possiblePosition)) != 0)
            {
                break;
            }
        }

        for (int x = position.X + 1; x < 8; ++x)
        {
            int possiblePosition = BoardPositionToInt(x, position.Y);
            moves.Add(possiblePosition);

            if ((freeSquares & GetBit(possiblePosition)) != 0)
            {
                break;
            }
        }

        for (int y = position.Y - 1; y >= 0; --y)
        {
            int possiblePosition = BoardPositionToInt(position.X, y);
            moves.Add(possiblePosition);

            if ((freeSquares & GetBit(possiblePosition)) != 0)
            {
                break;
            }
        }

        for (int y = position.Y + 1; y < 8; ++y)
        {
            int possiblePosition = BoardPositionToInt(position.X, y);
            moves.Add(possiblePosition);
            if ((freeSquares & GetBit(possiblePosition)) != 0)
            {
                break;
            }
        }

        return moves;
    }

    static List<int> Queen(Colour colour, ulong[] boardState, int pos)
    {
        ulong freeSquares = 0;
        List<int> moves = new List<int>();

        for (int i = 0; i < boardState.Length; ++i)
        {
            freeSquares |= boardState[i];
        }

        Vector2I position = IntToBoardPosition(pos);

        for (int x = position.X - 1; x >= 0; --x)
        {
            int possiblePosition = BoardPositionToInt(x, position.Y);
            moves.Add(possiblePosition);

            if ((freeSquares & GetBit(possiblePosition)) != 0)
            {
                break;
            }
        }

        for (int x = position.X + 1; x < 8; ++x)
        {
            int possiblePosition = BoardPositionToInt(x, position.Y);
            moves.Add(possiblePosition);

            if ((freeSquares & GetBit(possiblePosition)) != 0)
            {
                break;
            }
        }

        for (int y = position.Y - 1; y >= 0; --y)
        {
            int possiblePosition = BoardPositionToInt(position.X, y);
            moves.Add(possiblePosition);

            if ((freeSquares & GetBit(possiblePosition)) != 0)
            {
                break;
            }
        }

        for (int y = position.Y + 1; y < 8; ++y)
        {
            int possiblePosition = BoardPositionToInt(position.X, y);
            moves.Add(possiblePosition);
            if ((freeSquares & GetBit(possiblePosition)) != 0)
            {
                break;
            }
        }

        int i1 = 1;
        bool stopSearchLeft = false, stopSearchRight = false;

        for (int y = position.Y - 1; y >= 0; --y, ++i1)
        {
            int possiblePos = BoardPositionToInt(position.X - i1, y);
            if (position.X - i1 < 0)
            {
                stopSearchLeft = true;
            }

            if (position.X + i1 >= 8)
            {
                stopSearchRight = true;
            }

            if (!stopSearchLeft)
                moves.Add(possiblePos);

            if (!stopSearchLeft && (freeSquares & GetBit(possiblePos)) != 0)
            {
                stopSearchLeft = true;
            }

            possiblePos = BoardPositionToInt(position.X + i1, y);

            if (!stopSearchRight)
            {
                moves.Add(possiblePos);
            }

            if (!stopSearchRight && (freeSquares & GetBit(possiblePos)) != 0)
            {
                stopSearchRight = true;
            }
        }

        i1 = 1;
        stopSearchLeft = false;
        stopSearchRight = false;
        for (int y = position.Y + 1; y < 8; ++y, ++i1)
        {
            int possiblePos = BoardPositionToInt(position.X - i1, y);
            if (position.X - i1 < 0)
            {
                stopSearchLeft = true;
            }

            if (position.X + i1 >= 8)
            {
                stopSearchRight = true;
            }

            if (!stopSearchLeft)
            {
                moves.Add(possiblePos);
            }

            if (!stopSearchLeft && (freeSquares & GetBit(possiblePos)) != 0)
            {
                stopSearchLeft = true;
            }


            possiblePos = BoardPositionToInt(position.X + i1, y);
            if (!stopSearchRight)
            {
                moves.Add(possiblePos);
            }

            if (!stopSearchRight && (freeSquares & GetBit(possiblePos)) != 0)
            {
                stopSearchRight = true;
            }
        }

        return moves;
    }

    static List<int> King(Colour colour, ulong[] boardState, int pos)
    {
        List<int> moves = new List<int>();
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
                moves.Add(possiblePosition);
            }
        }

        return moves;
    }

}