using Godot;
using System;
using Chess.Script.Engine;

public partial class AI : Node2D
{
    private BoardVisual _visual;
    private StockfishInterface _interface;
    private Board _board;
    private bool AiIsWhite;

    public override async void _Ready()
    {
        _visual = GetParent<BoardVisual>();
        await ToSignal(_visual, "ready");
        
        
        _board = _visual.board;
        AiIsWhite = _visual.AiIsWhite;
        
        _interface = new StockfishInterface(_board);
        _interface.SetPlayingAsWhite(AiIsWhite);
        CallDeferred("add_child", _interface);

        _visual.MoveMade += MoveMade;
        MoveMade(true);
    }

    void MoveMade(bool isWhite)
    {
        if (isWhite != AiIsWhite)
        {
            return;
        }
        _interface.OrderToUpdatePosition(PositionUpdated);
    }

    void PositionUpdated()
    {
        _interface.RequestForBestMove(BestMoveRecieved);
    }

    void BestMoveRecieved(Move move)
    {
        _board.MakeMove(move);
        _visual.RefreshBoard();
    }
    
    public override void _ExitTree()
    {
        _visual.MoveMade -= MoveMade;
    }
}
