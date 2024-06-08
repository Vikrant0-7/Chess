using Godot;
using System;

public partial class Turn : HBoxContainer
{
    [Export] private Texture2D _white;
    [Export] private Texture2D _black;
    [Export] private NodePath _boardPath;
    
    private TextureRect _rect;
    private BoardVisual _boardVisual;

    public override void _Ready()
    {
        _rect = GetNode<TextureRect>("TextureRect");
        _boardVisual = GetNode<BoardVisual>(_boardPath);
    }

    public override void _Process(double delta)
    {
        if (_boardVisual.board.WhiteTurn && _rect.Texture != _white)
        {
            _rect.Texture = _white;
        }
        else if (!_boardVisual.board.WhiteTurn && _rect.Texture != _black)
        {
            _rect.Texture = _black;
        }
    }
}
