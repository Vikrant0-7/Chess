using System;
using Godot;

namespace Chess.Script.Engine;

public class FenTranslator
{
    private string[] _fenArray;
    
    private int[] _boardStatus;
    private bool[] _whiteCastleStatus;
    private bool[] _blackCastleStatus;
    private int _enPassantSquare;
    private bool _whiteTurn;
    private int[] _moves;
    private string _fenString;
    

    public string FenString => _fenString;
    public int[] BoardStatus => _boardStatus;
    public bool[] WhiteCastleStatus => _whiteCastleStatus;
    public bool[] BlackCastleStatus => _blackCastleStatus;
    public int EnPassantSquare => _enPassantSquare;
    public bool WhiteTurn => _whiteTurn;
    public int[] Moves => _moves;


    public FenTranslator(string str)
    {
        _fenArray = str.Split(' ');

        if (_fenArray.Length != 6)
            throw new Exception("Invalid Format for Fen String");
        
        _boardStatus = new int[64];
        _whiteCastleStatus = new bool[2];
        _blackCastleStatus = new bool[2];
        _enPassantSquare = ChessNotationToInt(_fenArray[3]);
        _whiteTurn = _fenArray[1].Trim() == "w";
        
        _moves = new[]
        {
            Convert.ToInt32(_fenArray[4]),
            Convert.ToInt32(_fenArray[5])
        };
        
        ProcessBoard();
        ProcessCastle();
    }
    
    public FenTranslator(int[] boardStatus, byte castleStatus, int enPassantSquare, bool whiteTurn, int[] moves)
    {
        _boardStatus = boardStatus;
        _whiteCastleStatus = new[] { (castleStatus & 0b10) != 0, (castleStatus & 0b01) != 0 };
        _blackCastleStatus = new[] { ((castleStatus >> 4) & 0b10) != 0, 
                                     ((castleStatus >> 4) & 0b01) != 0 };
        _enPassantSquare = enPassantSquare;
        _whiteTurn = whiteTurn;
        _moves = moves;

        _fenString = "";

        _fenString += ProcessBoardToFen() + " ";
        _fenString += ((_whiteTurn) ? "w" : "b") + " ";
        _fenString += GetCastlingString() + " ";
        _fenString += ((enPassantSquare == -1) ? "-" : GetNotation()) + " ";

        _fenString += _moves[0] + " " + _moves[1];
    }
    string ProcessBoardToFen(){
        string @out = "";
        int freeSquare = 0;

        for(int i = 0; i < 64; ++i)
        {
            int piece = _boardStatus[i];

            if(i % 8 == 0 && i != 0){
                if(freeSquare > 0){
                    @out += freeSquare.ToString();
                    freeSquare = 0;
                }
                @out += "/";
            }
            if(piece == (int)ColourType.FREE){
                ++freeSquare;
                if (i == 63)
                    @out += freeSquare.ToString();
                continue;
            }
            if(freeSquare > 0){
                @out += freeSquare.ToString();
                freeSquare = 0;
            }

            @out += PieceToAlgebra(piece);
            
        }
        return @out;
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
            if (info != (int)ColourType.FREE)
            {
                _boardStatus[boardPosition++] = info;
            }
            else
            {
                info = ToDigit(pos);
                if (info == -1)
                    throw new Exception("Invalid Fen Format");
                for (int i = 0; i < info; ++i)
                {
                    _boardStatus[boardPosition++] = (int)ColourType.FREE;
                }
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
        switch (c)
        {
            case 'K':
                return (int)ColourType.WHITE_KING;
            case 'Q':
                return (int)ColourType.WHITE_QUEEN;
            case 'R':
                return (int)ColourType.WHITE_ROOK;
            case 'B':
                return (int)ColourType.WHITE_BISHOP;
            case 'N':
                return (int)ColourType.WHITE_KNIGHT;
            case 'P':
                return (int)ColourType.WHITE_PAWN;
            
            case 'k':
                return (int)ColourType.BLACK_KING;
            case 'q':
                return (int)ColourType.BLACK_QUEEN;
            case 'r':
                return (int)ColourType.BLACK_ROOK;
            case 'b':
                return (int)ColourType.BLACK_BISHOP;
            case 'n':
                return (int)ColourType.BLACK_KNIGHT;
            case 'p':
                return (int)ColourType.BLACK_PAWN;
            
            default:
                return (int)ColourType.FREE;
        }
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

    char PieceToAlgebra(int piece){
        return " KQRBNPkqrbnp"[piece];
    }
    
    String GetCastlingString()
	{
		String @out = "";
		if (!_whiteCastleStatus[0] && !_blackCastleStatus[0] &&
		    !_whiteCastleStatus[1] && !_blackCastleStatus[1]
		   )
			@out = "-";
		if (_whiteCastleStatus[1])
			@out += "K";
		if (_whiteCastleStatus[0])
			@out += "Q";
		if (_blackCastleStatus[1])
			@out += "k";
		if (_blackCastleStatus[0])
			@out += "q";
		return @out;
	}

    string GetNotation(){
        return "abcdefgh"[_enPassantSquare % 8] + (8 - _enPassantSquare / 8).ToString();
    }
}