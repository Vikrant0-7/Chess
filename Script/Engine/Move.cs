using Godot;
using System;

//Todo: To reduce time complexity of move may be store captures so that that piece can be revived.
public class Move
{

    public int position;
    public int piece;
    public int finalPosition;
    public bool[] whiteCanCastle;
    public bool[] blackCanCastle;
    public int[] data;
    public ulong[] board;
    public bool whiteTurn;
    
    public Move(int position, int piece, int finalPosition)
    {
        this.position = position;
        this.piece = piece;
        this.finalPosition = finalPosition;
        data = new int[3];
    }

    public override string ToString()
    {
        return "abcdefgh"[position % 8] + (position / 8).ToString() + "abcdefgh"[finalPosition % 8] + (finalPosition / 8).ToString();
    }
}
