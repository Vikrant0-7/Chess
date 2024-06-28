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
   
   public List<Move> GenerateMoves()
   {
      List<Move> @out = new List<Move>();
      
      return @out;
   }
   
}

