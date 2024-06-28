using Godot;
using System;
using System.Collections.Generic;

namespace Chess.Script.Engine.Moves;

//Todo: Using Checkers Bitboard Remove the need of function RemoveIllegalMoves().
//Todo: Add Moves of King.
//Todo: Currently Moves Are Pseudolegal. Generate Legal Moves.


public class LegalMoves
{
    public static List<Move> Pawn(Board board, int from)
    {
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

            blocked = false;
        }

        if (!blocked && pos.Y == (isWhite ? 6 : 1))
        {
            testPos = Functions.BoardPositionToInt(pos.X, pos.Y + 2 * modifier);
            if (boardStatus[testPos] == (int)ColourType.FREE)
            {
                moves.Add(new Move(from, piece,testPos));
            }
        }

        foreach (var item in PrecomputedMoves.PawnAttacks[from])
        {
            testPos = item + modifier * 8;
            
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
        
        return moves;

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
        return moves;
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
        
        return moves;
    }

    public static List<Move> King(Board board, int from)
    {
        List<Move> moves = new List<Move>();
        int[] boardStatus = board.BoardStatus;
        bool isWhite = board.WhiteTurn;
        
        int piece = isWhite ? (int)ColourType.WHITE_KING : (int)ColourType.BLACK_KING;
        
        foreach (var item in PrecomputedMoves.KingMoves[from])
        {
            if(Functions.IsPieceFriendly(isWhite, boardStatus[item]) == 1)
                continue;
            
            moves.Add(new Move(from, piece, item));
        }

        return moves;
    }
    
}
