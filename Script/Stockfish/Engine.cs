using System.Diagnostics;
using System;
public class Engine
{

    public enum Commands 
    {
        POSITION,
        DEBUG,
        IS_READY,
        LEGALMOVE,
        NEWGAME,
        EVAL,
        BESTMOVE,
        PONDERHIT,
    }

    private readonly string[] command_translate = new string[]
    {
        "position fen",
        "d",
        "isready",
        "go perft 2",
        "ucinewgame",
        "eval",
        "go",
        "ponderhit"
    };
    
    private Process _engine;
    public bool producesOutput;
    public Engine(string path)
    {
        _engine = new Process();
        _engine.StartInfo.RedirectStandardError = true;
        _engine.StartInfo.RedirectStandardOutput = true;
        _engine.StartInfo.RedirectStandardInput = true;

        _engine.StartInfo.UseShellExecute = false;
        _engine.StartInfo.CreateNoWindow = true;

        _engine.StartInfo.FileName = path;


        if (!_engine.Start())
        {
            throw new Exception("Cannot Start Process");
        }
        
        
        _engine.StandardOutput.ReadLine();
    }

    public void Wait(int estimatedTime = 100)
    {
        _engine.WaitForExit(estimatedTime);
    }

    public bool Send(Commands com, params object[] args)
    {
        
        int limit = 4;
        while (limit > 0)
        {
            if(ReadLine() == String.Empty)
            {
                --limit;
            }
        }
        
        string str = command_translate[(int)com];
        foreach (string o in args)
        {
            str += " " + o;
        }
        _engine.StandardInput.WriteLine(str);
        _engine.StandardInput.Flush();

        producesOutput = true;
        
        return producesOutput;
    }

    public string ReadLine()
    {
        if (!producesOutput)
            return String.Empty;
        
        string? output = _engine.StandardOutput.ReadLine();
        if (String.IsNullOrEmpty(output))
            return String.Empty;

        if (output.Trim() == "LF")
        {
            producesOutput = false;
            return String.Empty;
        }

        return output;

    }

    public bool IsReady()
    {
        Send(Commands.IS_READY);
        
        Wait();

        string output = ReadLine();
        
        if (output == "")
            return false;

        return output == "readyok";
    }

    public void Dispose()
    {
        _engine.StandardInput.Close();
        _engine.StandardInput.Dispose();
        
        _engine.StandardOutput.Close();
        _engine.StandardOutput.Dispose();
        
        _engine.StandardError.Close();
        _engine.StandardError.Dispose();
        
        _engine.Dispose();
    }
}

public class EngineNotReadyException : Exception
{
    public EngineNotReadyException() : base("Engine is not ready to accept Inputs")
    {
        
    }
}