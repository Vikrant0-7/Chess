using Godot;
using System;
using Chess.Script.Engine;

public partial class AI : Node2D
{
    private BoardVisual _visual;
    private StockfishInterface _interface;
    private Board _board;
    
    private bool AiIsWhite;

    [Export] private Resource _aiSettings;
    
    public override async void _Ready()
    {
        _visual = GetParent<BoardVisual>();
        await ToSignal(_visual, "ready");
        
        _board = _visual.board;
        
        _visual.MoveMade += MoveMade;
        
        MoveMade(true);

        if (_aiSettings.Get("depth").VariantType == Variant.Type.Nil)
        {
            throw new Exception(
                "Resource Assigned to _aiSettings should be of class AiSettings defined in ai_settings.gd\n"+_aiSettings.GetClass());
        }

        AiIsWhite = _aiSettings.Get("ai_is_white").AsBool();
        int depth = _aiSettings.Get("depth").AsInt32();
        int movetime = _aiSettings.Get("movetime").AsInt32();
        
        _interface = new StockfishInterface(_board, depth, movetime, AiIsWhite);
        CallDeferred("add_child", _interface);
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
        // _visual.RefreshBoard();
    }
    
    public override void _ExitTree()
    {
        _visual.MoveMade -= MoveMade;
    }
}
