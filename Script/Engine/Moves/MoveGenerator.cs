using Godot;
using System;
using System.Collections.Generic;

namespace Chess.Script.Engine.Moves;

public class MoveGenerator
{
   private Board _board;

   public MoveGenerator(Board board)
   {
      this._board = board;
   }

   public List<int> GetLegalMovesSquare()
   {
      List<int> legalSquares = new List<int>();
      
      int pCount = 0;
      for (int i = 0; i < 64; ++i)
      {
         int piece = _board.BoardStatus[i];
         
         if(piece == (int)ColourType.FREE)
            continue;
         
         if(_board.WhiteTurn && piece > (int)ColourType.WHITE_PAWN)
            continue;

         if(!_board.WhiteTurn && piece < (int)ColourType.BLACK_KING)
            continue;

         int p_id = (piece - 1) % 6;

         List<Move> m = new List<Move>();
         switch (p_id)
         {
            case (int)ColourType.WHITE_PAWN - 1:
               m = LegalMoves.Pawn(_board, i);
               break;
            case (int)ColourType.WHITE_KNIGHT - 1:
               m = LegalMoves.Knight(_board, i);
               break;
            case (int)ColourType.WHITE_KING - 1:
               m = LegalMoves.King(_board, i);
               break;
            default:
               m = LegalMoves.SlidingMoves(_board, i, piece);
               break;
         }

         if (m.Count > 0)
         {
            legalSquares.Add(i);
         }
         ++pCount;
         if(pCount == 16)
            break;
      }
      
      return legalSquares;
   }
   
   public List<Move> GenerateMoves()
   {
      List<Move> @out = new List<Move>();
      int pCount = 0;
      for (int i = 0; i < 64; ++i)
      {
         int piece = _board.BoardStatus[i];
         
         if(piece == (int)ColourType.FREE)
            continue;
         
         if(_board.WhiteTurn && piece > (int)ColourType.WHITE_PAWN)
            continue;

         if(!_board.WhiteTurn && piece < (int)ColourType.BLACK_KING)
            continue;

         int p_id = (piece - 1) % 6;

         List<Move> m = new List<Move>();
         switch (p_id)
         {
            case (int)ColourType.WHITE_PAWN - 1:
               m = LegalMoves.Pawn(_board, i);
               break;
            case (int)ColourType.WHITE_KNIGHT - 1:
               m = LegalMoves.Knight(_board, i);
               break;
            case (int)ColourType.WHITE_KING - 1:
               m = LegalMoves.King(_board, i);
               break;
            default:
               m = LegalMoves.SlidingMoves(_board, i, piece);
               break;
         }
         
         @out.AddRange(m);
         ++pCount;
         if(pCount == 16)
            break;
      }
      
      return @out;
   }
   
   
}

