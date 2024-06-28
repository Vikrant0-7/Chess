using Godot;
using System.Collections.Generic;

namespace Chess.Script.Engine.Moves;

public static class PrecomputedMoves
{
    public static int[][] DistanceToEdge;
    public static int[] Direction;
    public static List<int>[] KnightMoves;
    public static List<int>[] PawnAttacks;
    public static List<int>[] KingMoves;

    public static void Init()
    {
        DistanceToEdge = new int[64][];
        Direction = new[] { -9, -7, 9, 7, -8, 1, 8, -1 };
        
        KnightMoves = new List<int>[64];
        PawnAttacks = new List<int>[64];
        KingMoves = new List<int>[64];

        var knightDir = new Vector2I[]
        {
            new Vector2I(-2,-1),
            new Vector2I( 2,-1),
            new Vector2I(-2, 1),
            new Vector2I( 2, 1),

            new Vector2I(-1,-2),
            new Vector2I( 1,-2),
            new Vector2I(-1, 2),
            new Vector2I( 1, 2),
        };

        var kingDir = new Vector2I[]
        {
            new Vector2I(-1, -1),
            new Vector2I( 0, -1),
            new Vector2I( 1, -1),
            new Vector2I( 1,  0),
            new Vector2I( 1,  1),
            new Vector2I( 0,  1),
            new Vector2I(-1,  1),
            new Vector2I(-1,  0),
        };
        
        for (int rank = 0; rank < 8; rank++)
        {
            for (int file = 0; file < 8; ++file)
            {
                int idx = rank * 8 + file;
                
                KnightMoves[idx] = new List<int>();
                PawnAttacks[idx] = new List<int>();
                KingMoves[idx] = new List<int>();
                
                Vector2I pos = new Vector2I(file, rank);

                foreach (var item in knightDir)
                {
                    Vector2I newPos = pos + item;
                    if(newPos.X < 0 || newPos.X > 7)
                        continue;
                    if(newPos.Y < 0 || newPos.Y > 7)
                        continue;
                    KnightMoves[idx].Add(Functions.BoardPositionToInt(newPos.X, newPos.Y));
                }

                foreach (var item in kingDir)
                {
                    Vector2I newPos = pos + item;
                    if(newPos.X < 0 || newPos.X > 7)
                        continue;
                    if(newPos.Y < 0 || newPos.Y > 7)
                        continue;
                    KingMoves[idx].Add(Functions.BoardPositionToInt(newPos.X, newPos.Y));
                }
                
                

                if (file > 0)
                {
                    PawnAttacks[idx].Add(Functions.BoardPositionToInt(file - 1, rank));
                }
                if (file < 7)
                {
                    PawnAttacks[idx].Add(Functions.BoardPositionToInt(file + 1, rank));
                }

                
                int north = rank;
                int south = 7 - rank;
                int east = file;
                int west = 7 - file;

                int northEast = Mathf.Min(north, east);
                int northWest = Mathf.Min(north, west);
                int southEast = Mathf.Min(south, east);
                int southWest = Mathf.Min(south, west);

                DistanceToEdge[rank * 8 + file] = new[]
                {
                    northEast,
                    northWest,
                    southWest,
                    southEast,
                    north,
                    west,
                    south,
                    east
                };
            }
        }
    }
}