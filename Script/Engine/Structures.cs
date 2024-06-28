using System.Threading.Tasks;
using System.Threading;
using System;

namespace Chess.Script.Engine;
//Todo: To reduce time complexity of move may be store captures so that that piece can be revived.
public struct Move
{

    public int position;
    public int piece;
    public int finalPosition;
    public int promoteTo;
    
    public Move(int position, int piece,int finalPosition, int promoteTo = -1)
    {
        this.position = position;
        this.piece = piece;
        this.finalPosition = finalPosition;
        this.promoteTo = promoteTo;
    }

    public override string ToString()
    {
        return "abcdefgh"[position % 8] + (8 - position / 8).ToString() + "abcdefgh"[finalPosition % 8] + (8 - finalPosition / 8).ToString();
    }
}

public enum ColourType{
    FREE = 0,
    WHITE_KING = 1,
    WHITE_QUEEN = 2,
    WHITE_ROOK = 3,
    WHITE_BISHOP = 4,
    WHITE_KNIGHT = 5,
    WHITE_PAWN = 6,
    BLACK_KING = 7,
    BLACK_QUEEN = 8,
    BLACK_ROOK = 9,
    BLACK_BISHOP = 10,
    BLACK_KNIGHT = 11,
    BLACK_PAWN = 12
}

public struct NoOfMovesThreadData<T>
{
    public Action<T> callback;
    public T parameter;

    public NoOfMovesThreadData(Action<T> callback, T parameter)
    {
        this.callback = callback;
        this.parameter = parameter;
    }
}