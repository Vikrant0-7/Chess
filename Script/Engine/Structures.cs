//Todo: To reduce time complexity of move may be store captures so that that piece can be revived.
public class Move
{

    public int position;
    public int piece;
    public int finalPosition;
    public int promoteTo;
    
    public bool[] whiteCanCastle;
    public bool[] blackCanCastle;
    public int[] data;
    public ulong[] board;
    public bool whiteTurn;
    public ulong attack;
    public ulong pin;
    
    public Move(int position, int piece, int finalPosition, int promoteTo = -1)
    {
        this.position = position;
        this.piece = piece;
        this.finalPosition = finalPosition;
        data = new int[3];
        this.promoteTo = promoteTo;

        this.board = new ulong[12];
        this.whiteTurn = false;
        this.whiteCanCastle = new bool[2];
        this.blackCanCastle = new bool[2];
    }

    public override string ToString()
    {
        return "abcdefgh"[position % 8] + (8 - position / 8).ToString() + "abcdefgh"[finalPosition % 8] + (8 - finalPosition / 8).ToString();
    }
}