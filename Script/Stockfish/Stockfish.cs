using System;
using System.Collections.Generic;
using System.Linq;

public class Stockfish
{
    private Engine _engine;
    private const int MAX_TRIES = 100;

    private string _fen;
    private bool _kingUnderCheck;
    
    public string Fen => _fen;
    public bool KingUnderCheck => _kingUnderCheck;
    
    public Stockfish(string path, string start = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1")
    {
        _engine = new Engine(path);
        if (!_engine.IsReady())
            throw new Exception("Stockfish Engine cannot be initialized");

        NewGame(start);
    }

    public void NewGame(string start = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1")
    {
        _engine.Send(Engine.Commands.NEWGAME);
        _engine.Wait(500);

        SetPosition(start);
    }

    bool IsEngineReady()
    {
        int tries = MAX_TRIES;
        bool isReady = false;
        
        _engine.Send(Engine.Commands.IS_READY);
        _engine.Wait();
        while (tries > 0)
        {
            string output = _engine.ReadLine();

            if (output == String.Empty)
            {
                --tries;
                _engine.Wait();
            }
            if (output == "readyok")
            {
                isReady = true;
                break;
            }
            
        }

        return isReady;
    }

    public Dictionary<string,List<string>> LegalMoves()
    {
        Dictionary<string, List<string>> moves = new Dictionary<string, List<string>>();
        
        if (!IsEngineReady())
        {
            Console.WriteLine("Engine Not Ready Yet");
            throw new EngineNotReadyException();
        }
        
        _engine.Send(Engine.Commands.LEGALMOVE);
        _engine.Wait();
        
        while (true)
        {
            string output = _engine.ReadLine();

            if (output == String.Empty)
                break;
            
            if(output.Contains(":"))
            {
                string from = output.Substring(0, 2);
                string to = output.Substring(2, 2);

                if (!moves.Keys.Contains(from))
                    moves.Add(from, new List<string>());
                
                moves[from].Add(to);
            }
        }

        return moves;
    }

    public void MakeMove(string move)
    {

        if (!IsEngineReady())
        {
            Console.WriteLine("Engine Not Ready Yet");
            throw new EngineNotReadyException();

        }
        
        _engine.Send(Engine.Commands.POSITION, _fen, "moves", move);
        _engine.Wait();
        _engine.Send(Engine.Commands.DEBUG);
        _engine.Wait();
        int emptyLines = 4;
        
        while (emptyLines > 0)
        {
            string output = _engine.ReadLine();
            if (output == String.Empty)
                --emptyLines;

            if (output.Contains("Fen"))
            {
                _fen = output.Split(":")[1].Trim();
            }

            if (output.Contains("Checkers"))
            {
                _kingUnderCheck = output.Trim().Split(" ").Length > 1;
            }
        }
    }

    public float GetEvaluation()
    {
        if (!IsEngineReady())
        {
            Console.WriteLine("Engine Not Ready Yet!");
            throw new EngineNotReadyException();
        }

        float eval = 0;

        _engine.Send(Engine.Commands.EVAL);
        _engine.Wait(500);
        
        int emptyLines = 7;
        
        while (emptyLines > 0)
        {
            string output = _engine.ReadLine();
            if (output == String.Empty)
            {
                --emptyLines;
                continue;
            }

            if (output.Contains("Final evaluation"))
            {
                var arr = output.Split(" ");
                var evalStr = "";
                foreach (string item in arr)
                {
                    if(item == "")
                        continue;
                    if (item[0] == '+' || item[0] == '-' || Char.IsDigit(item[0]))
                    {
                        evalStr = item;
                        break;
                    }

                    if (item == "none")
                    {
                        evalStr = "NaN";
                    }
                }
                try
                {
                    eval = Convert.ToSingle(evalStr.Trim());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            
        }
        

        return eval;
    }

    public string Debug()
    {
        string @out = "";
        if (!IsEngineReady())
        {
            Console.WriteLine("Engine Not Ready Yet");
            throw new EngineNotReadyException();
        }
        
        _engine.Send(Engine.Commands.DEBUG);
        _engine.Wait();
        int emptyLines = 4;
        
        while (emptyLines > 0)
        {
            string output = _engine.ReadLine();
            if (output == String.Empty)
            {
                --emptyLines;
                continue;
            }

            @out += output + "\n";
        }

        return @out;
    }

    public void SetPosition(string fen)
    {
        _engine.Send(Engine.Commands.POSITION, fen);
        _engine.Wait();
        
        _engine.Send(Engine.Commands.DEBUG);
        _engine.Wait();
        int emptyLines = 4;
        
        while (emptyLines > 0)
        {
            string output = _engine.ReadLine();
            if (output == String.Empty)
                --emptyLines;

            if (output.Contains("Fen"))
            {
                _fen = output.Split(":")[1].Trim();
            }

            if (output.Contains("Checkers"))
            {
                _kingUnderCheck = output.Trim().Split(":").Length > 1;
            }
        }
    }

    public string GetBestMove(int depth = -1, int movetime = -1)
    {
        if (!IsEngineReady())
        {
            Console.WriteLine("Engine is not ready yet!");
            throw new EngineNotReadyException();
        }

        string extra = "";

        depth = Math.Min(depth, 30);
        movetime = Math.Min(movetime, 30_000);
        
        if (depth > 0)
        {
            extra += "depth " + depth;
        }

        if (movetime > 0)
        {
            extra += " movetime " + movetime;
        }

        extra = extra.Trim();

        if (extra == "")
        {
            Console.WriteLine("Stockfish would search best move indefinately");
        }

        _engine.Send(Engine.Commands.BESTMOVE,extra);
        
        _engine.Wait();

        int tries = MAX_TRIES;
        string output = "";
        while (tries > 0)
        {
            output = _engine.ReadLine();

            if (output == String.Empty)
            {
                --tries;
                _engine.Wait();
            }
            if (output.Contains("bestmove"))
            {
                break;
            }
        }

        if (output.Contains("none"))
        {
            return String.Empty;
        }

        try
        {
            return output.Split(" ")[1];
        }
        catch (IndexOutOfRangeException e)
        {
            Console.WriteLine(e);
            return String.Empty;
        }

    }
    
    public void Dispose()
    {
        _engine.Dispose();
    }
}