using Godot;
using System;

public class Move
{

    public int position;
    public ColourType piece;
    public int finalPosition;
    public bool[] whiteCanCastle;
    public bool[] blackCanCastle;
    public ulong[] snapShot;
    public ulong[] board;
    public bool whiteTurn;
    
    public Move(int position, ColourType piece, int finalPosition)
    {
        this.position = position;
        this.piece = piece;
        this.finalPosition = finalPosition;
    }
}
