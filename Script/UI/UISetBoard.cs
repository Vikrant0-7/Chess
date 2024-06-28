using Godot;
using System;
using Chess.Script.Engine;
public partial class UISetBoard : VBoxContainer
{
    [Export] private NodePath boardNodePath;
    [Export] private Texture2D[] textures;
    private int _colour = 0;
    private int _type = -1;

    private CheckBox settingBoard;
    private CheckBox whiteTurn;
    private CheckBox[] blackCastle;
    private CheckBox[] whiteCastle;
    private TextureRect _textureRect;
    
    private BoardVisual _boardVisual;
    
    
    public override void _Ready()
    {
        _textureRect = GetNode<TextureRect>("texture");

        SetType(-1);
        GetNode<Button>("%k").Pressed += () => SetType(0);
        GetNode<Button>("%q").Pressed += () => SetType(1);
        GetNode<Button>("%r").Pressed += () => SetType(2);
        GetNode<Button>("%b").Pressed += () => SetType(3);
        GetNode<Button>("%n").Pressed += () => SetType(4);
        GetNode<Button>("%p").Pressed += () => SetType(5);
        GetNode<Button>("Set").Pressed += () => _OnSetPressed();
        GetNode<Button>("Clean").Pressed += () => _OnCleanPressed();

        
        GetNode<OptionButton>("colour").ItemSelected += (idx) => _colour = (int)idx;
        

        settingBoard = GetNode<CheckBox>("startsetup");
        whiteTurn = GetNode<CheckBox>("whiteturn");

        _boardVisual = GetNode<BoardVisual>(boardNodePath);
        
        whiteCastle = new[]
        {
            GetNode<CheckBox>("%CQ"),
            GetNode<CheckBox>("%CK")
        };

        blackCastle = new[]
        {
            GetNode<CheckBox>("%bcq"),
            GetNode<CheckBox>("%bck")
        };

    }

    void SetType(int type)
    {
        if (type != -1)
            _textureRect.Texture = textures[_colour * 6 + type];
        else
            _textureRect.Texture = null;
        _type = type + 1;
    }
    
    void _OnSetPressed()
    {
        if(!settingBoard.ButtonPressed)
            return;
        _boardVisual.board.SetCastlingState(
        true, whiteCastle[0].ButtonPressed, whiteCastle[1].ButtonPressed
        );
        
        _boardVisual.board.SetCastlingState(
            false,blackCastle[0].ButtonPressed, blackCastle[1].ButtonPressed
            );
        
        _boardVisual.board.WhiteTurn = whiteTurn.ButtonPressed;

        SetType(-1);
        
        whiteCastle[0].ButtonPressed = false;
        whiteCastle[1].ButtonPressed = false;
        blackCastle[0].ButtonPressed = false;
        blackCastle[1].ButtonPressed = false;
        whiteTurn.ButtonPressed = false;
        
        settingBoard.ButtonPressed = false;
    }

    void _OnCleanPressed()
    {
        if(!settingBoard.ButtonPressed)
            return;
        _boardVisual.board.CleanBoard();            
        _boardVisual.RefreshBoard();
    }
    
    public override void _Process(double delta)
    {
        if(!settingBoard.ButtonPressed)
            return;

        if (Input.IsActionJustPressed("left_click"))
        {
            if(_colour < 0 || _type < 0)
                return;
            
            Vector2I position = _boardVisual.GlobalPositionToBoardPosition(GetGlobalMousePosition());
            if(position.X < 0 || position.X > 7)
                return;
            if(position.Y < 0 || position.Y > 7)
                return;
            int ipos = Functions.BoardPositionToInt(position);
            
            _boardVisual.board.SetBoardPosition(ipos,_colour * 6 + _type);
            _boardVisual.RefreshBoard();
        }
        
    }
}
