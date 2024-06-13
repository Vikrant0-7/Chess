using Godot;
using System;
using System.Collections.Generic;

public class MoveGenerator
{
   private Board _board;

   public MoveGenerator(Board board)
   {
      this._board = board;
   }
   
   public List<Move> GenerateMoves()
   {
      List<Move> @out = new List<Move>();
      for (int i = 0; i < 6; ++i)
      {
            @out.AddRange(GenerateForPiece(_board.WhiteTurn ? 0 : 1, i));
      }

      return @out;
   }
   
   List<Move> GenerateForPiece(int colour, int piece)
   {
      List<Move> @out = new List<Move>();
      ulong occupancy = _board.BoardStatus[piece];
      if (colour > 0)
      {
         occupancy = _board.BoardStatus[6+piece];
      }

      for (int i = 0; i < 64; ++i)
      {
         if ((occupancy & _board.GetBit(i)) != 0)
         {
            List<int> moves = GetMoves(colour, piece, i);
            foreach (var item in moves)
            {
               @out.Add(new Move(i,(ColourType)_board.GetIndex((Type)piece,(Colour)colour),item));
            }
         }
      }

      return @out;
   }

   List<int> GetMoves(int colour, int piece, int pos)
   {
      switch (piece)
      {
         case 1:
            return LegalMoves.Queen((Colour)colour, _board.BoardStatus, pos);
         case 2:
            return LegalMoves.Rook((Colour)colour, _board.BoardStatus, pos);
         case 3:
            return LegalMoves.Bishop((Colour)colour, _board.BoardStatus, pos);
         case 4:
            return LegalMoves.Knight((Colour)colour, _board.BoardStatus, pos);
         case 5:
            return LegalMoves.Pawn((Colour)colour, _board.BoardStatus, _board.BoardStatus,pos);
         default:
            return LegalMoves.King((Colour)colour, _board.BoardStatus, pos, _board.CanCastle(colour == 0 ? 0 : 6));
      }
   }
}

