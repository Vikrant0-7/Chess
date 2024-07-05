using Godot;
using System;
using System.Collections.Generic;

using Chess.Script.Engine.Bitboards;

namespace Chess.Script.Engine.Moves;


//Todo: Add Castling

//BUG: 8/2p5/3p4/KP5r/1R2Pp1k/8/6P1/8 b - e3 0 1



public class LegalMoves
{
    public static List<Move> Pawn(Board board, int from)
    {
        void AddMove(int i, Vector2I vector2I, bool b, List<Move> list, int piece1, int testPos1)
        {
            if (vector2I.Y != (b ? 1 : 6))
            {
                list.Add(new Move(i, piece1,testPos1));
            }
            else
            {
                list.Add(new Move(i, piece1,testPos1,
                    (b ? 0 : 6) + (int)ColourType.WHITE_QUEEN));
                list.Add(new Move(i, piece1,testPos1,
                    (b ? 0 : 6) + (int)ColourType.WHITE_ROOK));
                list.Add(new Move(i, piece1,testPos1,
                    (b ? 0 : 6) + (int)ColourType.WHITE_BISHOP));
                list.Add(new Move(i, piece1,testPos1,
                    (b ? 0 : 6) + (int)ColourType.WHITE_KNIGHT));

            }
        }

        int[] boardStatus = board.BoardStatus;
        bool isWhite = board.WhiteTurn;
        int enPassantSquare = board.EnPassantPosition; 
        
        List<Move> moves = new List<Move>();
        int modifier = (isWhite) ? -1 : 1;
        Vector2I pos = Functions.IntToBoardPosition(from);

        bool blocked = true;
        int testPos = Functions.BoardPositionToInt(pos.X, pos.Y + modifier);
        int piece = isWhite ? (int)ColourType.WHITE_PAWN : (int)ColourType.BLACK_PAWN;
        
        
        
        if (boardStatus[testPos] == (int)ColourType.FREE)
        {
            AddMove(from, pos, isWhite, moves, piece, testPos);
            
            blocked = false;
        }

        if (!blocked && pos.Y == (isWhite ? 6 : 1))
        {
            testPos = Functions.BoardPositionToInt(pos.X, pos.Y + 2 * modifier);
            if (boardStatus[testPos] == (int)ColourType.FREE)
            {
                moves.Add(new Move(from, piece, testPos));
            }
        }

        foreach (var item in PrecomputedMoves.PawnAttacks[from])
        {
            testPos = item + modifier * 8;

            if (Functions.IsPieceFriendly(isWhite, boardStatus[testPos]) == 0)
            {
                AddMove(from, pos, isWhite, moves, piece, testPos);
            }

            if (pos.Y == (isWhite ? 3 : 4) && testPos == enPassantSquare)
            {
                if (Functions.IsPieceFriendly(isWhite, boardStatus[testPos]) == -1)
                {
                    moves.Add(new Move(from, piece, testPos));
                }
            }
        }
        
/*
        if (pos.X > 0)
        {
            testPos = Functions.BoardPositionToInt(pos.X - 1, pos.Y + modifier);
            if (Functions.IsPieceFriendly(isWhite, boardStatus[testPos]) == 0)
            {
                if (pos.Y != (isWhite ? 1 : 6))
                {
                    moves.Add(new Move(from, piece,testPos));
                }
                else
                {
                    moves.Add(new Move(from, piece,testPos,
                        (isWhite ? 0 : 6) + (int)ColourType.WHITE_QUEEN));
                    moves.Add(new Move(from, piece,testPos,
                        (isWhite ? 0 : 6) + (int)ColourType.WHITE_ROOK));
                    moves.Add(new Move(from, piece,testPos,
                        (isWhite ? 0 : 6) + (int)ColourType.WHITE_BISHOP));
                    moves.Add(new Move(from, piece,testPos,
                        (isWhite ? 0 : 6) + (int)ColourType.WHITE_KNIGHT));

                }
            }

            if (pos.Y == (isWhite ? 3 : 4) && testPos == enPassantSquare)
            {
                if (Functions.IsPieceFriendly(isWhite, boardStatus[testPos]) == -1)
                {
                    moves.Add(new Move(from, piece,testPos));
                }
            }
        }
        
        if (pos.X < 7)
        {
            testPos = Functions.BoardPositionToInt(pos.X + 1, pos.Y + modifier);
            if (Functions.IsPieceFriendly(isWhite, boardStatus[testPos]) == 0)
            {
                if (pos.Y != (isWhite ? 1 : 6))
                {
                    moves.Add(new Move(from, piece,testPos));
                }
                else
                {
                    moves.Add(new Move(from, piece,testPos,
                        (isWhite ? 0 : 6) + (int)ColourType.WHITE_QUEEN));
                    moves.Add(new Move(from, piece,testPos,
                        (isWhite ? 0 : 6) + (int)ColourType.WHITE_ROOK));
                    moves.Add(new Move(from, piece,testPos,
                        (isWhite ? 0 : 6) + (int)ColourType.WHITE_BISHOP));
                    moves.Add(new Move(from, piece,testPos,
                        (isWhite ? 0 : 6) + (int)ColourType.WHITE_KNIGHT));

                }
            }

            if (pos.Y == (isWhite ? 3 : 4) && testPos == enPassantSquare)
            {
                if (Functions.IsPieceFriendly(isWhite, boardStatus[testPos]) == -1)
                {
                    moves.Add(new Move(from, piece,testPos));
                }
            }
        }
        */
        
        return RemoveIllegal(moves, board);
    }

    public static List<Move> SlidingMoves(Board board, int from, int pieceIdx)
    {
        int piece = (pieceIdx - 1) % 6;
        int[] boardStatus = board.BoardStatus;
        bool isWhite = board.WhiteTurn;

        List<Move> moves = new List<Move>();

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
                if (occupation == 1) 
                    break;
                
                moves.Add(new Move(from, pieceIdx,legalPos));
                
                
                if(occupation == 0)
                    break;

                legalPos += dir;
            }
        }
        return RemoveIllegal(moves, board);
    }

    public static List<Move> Knight(Board board, int from)
    {
        int[] boardStatus = board.BoardStatus;
        bool isWhite = board.WhiteTurn;

        List<Move> moves = new List<Move>();

        int piece = isWhite ? (int)ColourType.WHITE_KNIGHT : (int)ColourType.BLACK_KNIGHT;

        
        foreach (var item in PrecomputedMoves.KnightMoves[from])
        {
            if(Functions.IsPieceFriendly(isWhite, boardStatus[item]) == 1)
                continue;
            moves.Add(new Move(from, piece, item));
        }
        
        return RemoveIllegal(moves, board);
    }

    public static List<Move> King(Board board, int from)
    {
        List<Move> moves = new List<Move>();
        int[] boardStatus = board.BoardStatus;
        bool isWhite = board.WhiteTurn;
        ulong attack = board.CurrAttackBitboard;

        
        int piece = isWhite ? (int)ColourType.WHITE_KING : (int)ColourType.BLACK_KING;

        
        foreach (var item in PrecomputedMoves.KingMoves[from])
        {
            if(Functions.IsPieceFriendly(isWhite, boardStatus[item]) == 1)
                continue;
            if((attack & Functions.GetBit(item)) == 0)
                moves.Add(new Move(from, piece, item));
        }

        int castle = board.GetCastleStatus(isWhite);

        bool kingSide = (castle & 0b01) != 0;
        bool queenSide = (castle & 0b10) != 0;
        

        if (kingSide)
        {
            if (boardStatus[from + 1] == 0 && boardStatus[from + 2] == 0)
            {
                if((attack & ( Functions.GetBit(from) | Functions.GetBit(from + 1) | Functions.GetBit(from + 2) )) == 0)
                {
                    if (boardStatus[from + 3] == (isWhite ? (int)ColourType.WHITE_ROOK : (int)ColourType.BLACK_ROOK))
                    {
                        moves.Add(new Move(from, piece, from + 2));
                    }
                }
            }
        }

        if (queenSide)
        {
            if (boardStatus[from - 1] == 0 && boardStatus[from - 2] == 0 && boardStatus[from - 3] == 0)
            {
                if((attack & ( Functions.GetBit(from) | Functions.GetBit(from - 1) | Functions.GetBit(from - 2) )) == 0)
                {
                    if (boardStatus[from - 4] == (isWhite ? (int)ColourType.WHITE_ROOK : (int)ColourType.BLACK_ROOK))
                    {
                        moves.Add(new Move(from, piece, from - 2));
                    }
                }
            }
        }

        
        
        return RemoveIllegal(moves, board);
    }
    
    static List<Move> RemoveIllegal(List<Move> moves, Board board)
    {
        List<Move> @out = new List<Move>(moves);
        bool isWhite = board.WhiteTurn;
        
        foreach (Move move in moves)
        {
            int[] tmpBoard = (int[])board.BoardStatus.Clone();
            
            
            tmpBoard[move.position] = (int)ColourType.FREE;

            if ((move.piece - 1) % 6 == (int)ColourType.WHITE_PAWN - 1)
            {
                if (tmpBoard[move.finalPosition] == (int)ColourType.FREE && move.finalPosition == board.EnPassantPosition)
                {
                    tmpBoard[move.finalPosition + (isWhite ? 8 : -8)] = (int)ColourType.FREE;
                }
            }
            
            tmpBoard[move.finalPosition] = move.piece;

            int kpos = isWhite ? board.WhiteKingPosition : board.BlackKingPosition;

            if (move.piece == (int)ColourType.WHITE_KING ||
                move.piece == (int)ColourType.BLACK_KING)
            {
                kpos = move.finalPosition;
            }

            ulong attack = Bitboard.Attacks(!isWhite, tmpBoard);

            if ((attack & Functions.GetBit(kpos)) != 0)
            {
                @out.Remove(move);
            }
        }

        return @out;
    }
    
}
