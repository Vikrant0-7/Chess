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
        Vector2I kpos = Functions.IntToBoardPosition(
            isWhite ? board.WhiteKingPosition : board.BlackKingPosition);

        bool blocked = true;
        int testPos = Functions.BoardPositionToInt(pos.X, pos.Y + modifier);
        int piece = isWhite ? (int)ColourType.WHITE_PAWN : (int)ColourType.BLACK_PAWN;

        bool isPinned = (board.PinnedBitboard & Functions.GetBit(from)) != 0;
        bool isUnderCheck = (board.CurrAttackBitboard & 
                             Functions.GetBit(
                                 isWhite ? board.WhiteKingPosition : board.BlackKingPosition
                             )) != 0;
        
        ulong checkBitboard = board.CheckBitboard;

        if (isUnderCheck && checkBitboard == Bitboard.MultiAttacker)
            return moves;
        
        
        if (boardStatus[testPos] == (int)ColourType.FREE)
        {
            if (isPinned)
            {
                Vector2I newPos = Functions.IntToBoardPosition(testPos);

                if (Functions.MoveIsInDirection(newPos.X - pos.X, newPos.Y - pos.Y, kpos - pos))
                {
                    if(!isUnderCheck || (checkBitboard & Functions.GetBit(testPos)) != 0)
                        AddMove(from, pos, isWhite, moves, piece, testPos);
                }
            }
            else
            {
                if(!isUnderCheck || (checkBitboard & Functions.GetBit(testPos)) != 0)
                    AddMove(from, pos, isWhite, moves, piece, testPos);
            }

            blocked = false;
        }

        if (!blocked && pos.Y == (isWhite ? 6 : 1))
        {
            testPos = Functions.BoardPositionToInt(pos.X, pos.Y + 2 * modifier);
            if (boardStatus[testPos] == (int)ColourType.FREE)
            {
                if (isPinned)
                {
                    Vector2I newPos = Functions.IntToBoardPosition(testPos);

                    if (Functions.MoveIsInDirection(newPos.X - pos.X, newPos.Y - pos.Y, kpos - pos))
                    {
                        if(!isUnderCheck || (checkBitboard & Functions.GetBit(testPos)) != 0)
                            moves.Add(new Move(from, piece,testPos));
                    }
                }
                else
                {
                    if(!isUnderCheck || (checkBitboard & Functions.GetBit(testPos)) != 0)
                        moves.Add(new Move(from, piece,testPos));
                }
            }
        }

        foreach (var item in PrecomputedMoves.PawnAttacks[from])
        {
            testPos = item + modifier * 8;
            
            if (Functions.IsPieceFriendly(isWhite, boardStatus[testPos]) == 0)
            {
                if (isPinned)
                {
                    Vector2I newPos = Functions.IntToBoardPosition(testPos);

                    if (Functions.MoveIsInDirection(newPos.X - pos.X, newPos.Y - pos.Y, kpos - pos))
                    {
                        if(!isUnderCheck || (checkBitboard & Functions.GetBit(testPos)) != 0)
                            AddMove(from, pos, isWhite, moves, piece, testPos);
                    }
                }
                else
                {
                    if(!isUnderCheck || (checkBitboard & Functions.GetBit(testPos)) != 0)
                        AddMove(from, pos, isWhite, moves, piece, testPos);
                }
            }

            if (pos.Y == (isWhite ? 3 : 4) && testPos == enPassantSquare)
            {
                if (Functions.IsPieceFriendly(isWhite, boardStatus[testPos]) == -1)
                {
                    if (isPinned)
                    {
                        Vector2I newPos = Functions.IntToBoardPosition(testPos);

                        if (Functions.MoveIsInDirection(newPos.X - pos.X, newPos.Y - pos.Y, kpos - pos))
                        {
                            if(!isUnderCheck || (checkBitboard & Functions.GetBit(testPos)) != 0)
                                moves.Add(new Move(from, piece,testPos));
                        }
                    }
                    else
                    {
                        if(!isUnderCheck || (checkBitboard & Functions.GetBit(testPos)) != 0)
                            moves.Add(new Move(from, piece,testPos));
                    }
                    
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
        
        bool isPinned = (board.PinnedBitboard & Functions.GetBit(from)) != 0;
        bool isUnderCheck = (board.CurrAttackBitboard & 
                             Functions.GetBit(
                                 isWhite ? board.WhiteKingPosition : board.BlackKingPosition
                             )) != 0;
        
        ulong checkBitboard = board.CheckBitboard;

        Vector2I kpos = Functions.IntToBoardPosition(
            isWhite ? board.WhiteKingPosition : board.BlackKingPosition);
        Vector2I pos = Functions.IntToBoardPosition(from);
        
        if (isUnderCheck && checkBitboard == Bitboard.MultiAttacker)
            return moves;
        
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

                if (isPinned)
                {
                    Vector2I newPos = Functions.IntToBoardPosition(legalPos);

                    if (Functions.MoveIsInDirection(newPos.X - pos.X, newPos.Y - pos.Y, kpos - pos))
                    {
                        if(!isUnderCheck || (checkBitboard & Functions.GetBit(legalPos)) != 0)
                            moves.Add(new Move(from, pieceIdx,legalPos));
                    }
                }
                else
                {
                    if(!isUnderCheck || (checkBitboard & Functions.GetBit(legalPos)) != 0)
                        moves.Add(new Move(from, pieceIdx,legalPos));
                }
                
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

        bool isPinned = (board.PinnedBitboard & Functions.GetBit(from)) != 0;
        bool isUnderCheck = (board.CurrAttackBitboard & 
                             Functions.GetBit(
                                 isWhite ? board.WhiteKingPosition : board.BlackKingPosition
                             )) != 0;
        ulong checkBitboard = board.CheckBitboard;

        if (isPinned)
            return moves;
        
        if (isUnderCheck && checkBitboard == Bitboard.MultiAttacker)
            return moves;
        
        foreach (var item in PrecomputedMoves.KnightMoves[from])
        {
            if(Functions.IsPieceFriendly(isWhite, boardStatus[item]) == 1)
                continue;
            if(!isUnderCheck || (checkBitboard & Functions.GetBit(item)) != 0)
                moves.Add(new Move(from, piece, item));
        }
        
        return moves;
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

        return moves;
    }
    
}
