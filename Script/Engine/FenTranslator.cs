using System;

//TODO: Translate Board to FEN string.
public class FenTranslator
{
    private string[] _fenArray;
    
    private ulong[] _boardStatus;
    private bool[] _whiteCastleStatus;
    private bool[] _blackCastleStatus;
    private int _enPassantSquare;
    private bool _whiteTurn;

    public ulong[] BoardStatus => _boardStatus;
    public bool[] WhiteCastleStatus => _whiteCastleStatus;
    public bool[] BlackCastleStatus => _blackCastleStatus;
    public int EnPassantSquare => _enPassantSquare;
    public bool WhiteTurn => _whiteTurn;

    public FenTranslator(string str)
    {
        _fenArray = str.Split(' ');

        if (_fenArray.Length != 6)
            throw new Exception("Invalid Format for Fen String");
        
        _boardStatus = new ulong[12];
        _whiteCastleStatus = new bool[2];
        _blackCastleStatus = new bool[2];
        _enPassantSquare = ChessNotationToInt(_fenArray[3]);
        _whiteTurn = _fenArray[1].Trim() == "w";
        
        ProcessBoard();
        ProcessCastle();
    }
    
    public FenTranslator(ulong[] boardStatus, bool[] whiteCastleStatus, bool[] blackCastleStatus, int enPassantSquare, bool whiteTurn)
    {
        _boardStatus = boardStatus;
        _whiteCastleStatus = whiteCastleStatus;
        _blackCastleStatus = blackCastleStatus;
        _enPassantSquare = enPassantSquare;
        _whiteTurn = whiteTurn;
    }

    void ProcessBoard()
    {
        string position = _fenArray[0].Trim();
        int boardPosition = 0;
        foreach (var pos in position)
        {
            if (pos == '/')
                continue;
            int info = GetIndex(pos);
            if (info != -1)
            {
                _boardStatus[info] |= GetBit(boardPosition);
                ++boardPosition;
            }
            else
            {
                info = ToDigit(pos);
                if (info == -1)
                    throw new Exception("Invalid Fen Format");
                boardPosition += info;
            }
        }
    }

    void ProcessCastle()
    {
        string castle = _fenArray[2].Trim();

        if (castle.Contains('Q'))
            _whiteCastleStatus[0] = true;
        if (castle.Contains('K'))
            _whiteCastleStatus[1] = true;
        
        if (castle.Contains('q'))
            _blackCastleStatus[0] = true;
        if (castle.Contains('k'))
            _blackCastleStatus[1] = true;
    }

    int GetIndex(char c)
    {
        int idx = -1;
        
        switch (c)
        {
            case 'K':
                idx = 0;
                break;
            case 'Q':
                idx = 1;
                break;
            case 'R':
                idx = 2;
                break;
            case 'B':
                idx = 3;
                break;
            case 'N':
                idx = 4;
                break;
            case 'P':
                idx = 5;
                break;
            
            case 'k':
                idx = 6;
                break;
            case 'q':
                idx = 7;
                break;
            case 'r':
                idx = 8;
                break;
            case 'b':
                idx = 9;
                break;
            case 'n':
                idx = 10;
                break;
            case 'p':
                idx = 11;
                break;
        }
        
        return idx;
    }

    ulong GetBit(int pos)
    {
        return (ulong)1 << pos;
    }

    int ToDigit(char c)
    {
        if ("12345678".Contains(c))
        {
            return Convert.ToInt32(c) - Convert.ToInt32('0');
        }

        return -1;
    }

    int ChessNotationToInt(string str)
    {
        str = str.Trim();
        if (str == "-")
            return -1;
        return (8 - Convert.ToInt32(str[1]) + Convert.ToInt32('0')) * 8 + "abcdefgh".IndexOf(str[0]);
    }
    
}