using Godot;
using System;
using System.Collections.Generic;

public class AttackBitboard 
{
    static 	ulong GetBit(int bit){
        return (ulong)1 << (bit);
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
                                moves.AddRange(PseudoLegalMove.King(Colour.BLACK,boardStatus,j,true));
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
                                moves.AddRange(PseudoLegalMove.Queen(Colour.BLACK,boardStatus,j,true));
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
                                moves.AddRange(PseudoLegalMove.Rook(Colour.BLACK,boardStatus,j,true));
                                ++c;
                            }
                            if(c == 2)
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
                                moves.AddRange(PseudoLegalMove.Bishop(Colour.BLACK,boardStatus,j,true));
                                ++c;
                            }
                            if(c == 2)
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
                                moves.AddRange(PseudoLegalMove.Knight(Colour.BLACK,boardStatus,j,true));
                                ++c;
                            }
                            if(c == 2)
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
                                moves.AddRange(PseudoLegalMove.Pawn(Colour.BLACK,boardStatus,boardStatus,j,true));
                                ++c;
                            }
                            if(c == 8)
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
                switch (i+6)
                {
                    case 6:
                    {
                        for (int j = 0; j < 64; ++j)
                        {
                            if ((boardStatus[i] & GetBit(j)) != 0)
                            {
                                moves.AddRange(PseudoLegalMove.King(Colour.WHITE,boardStatus,j,true));
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
                                moves.AddRange(PseudoLegalMove.Queen(Colour.WHITE,boardStatus,j,true));
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
                                moves.AddRange(PseudoLegalMove.Rook(Colour.WHITE,boardStatus,j,true));
                                ++c;
                            }
                            if(c == 2)
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
                                moves.AddRange(PseudoLegalMove.Bishop(Colour.WHITE,boardStatus,j,true));
                                ++c;
                            }
                            if(c == 2)
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
                                moves.AddRange(PseudoLegalMove.Knight(Colour.WHITE,boardStatus,j,true));
                                ++c;
                            }
                            if(c == 2)
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
                                moves.AddRange(PseudoLegalMove.Pawn(Colour.WHITE,boardStatus,boardStatus,j,true));
                                ++c;
                            }
                            if(c == 8)
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
}
