using Godot;
using System;

public partial class Piece : Node2D
{
	[Export]
	Texture2D[] textures;

	[Export]
	public ColourType colourType;

	private bool _isDragging = false;
	private bool _dragginStart = false;
	private bool _mouseOver = false;

	private Vector2 _prevPosition;

	private Vector2I _boardPosition;
	private BoardVisual _boardVisual;
	
	// Called when the node enters the scene tree for the first time.
	public void Init(ColourType colourType, Vector2I _boardPosition, BoardVisual _boardVisual)
	{
		this._boardPosition = _boardPosition;
		this._boardVisual = _boardVisual;
		this.colourType = colourType;
		GetNode<Sprite2D>("Sprite2D").Texture = textures[(int)colourType];
	}

	
	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton)
		{
			_isDragging = (@event as InputEventMouseButton).Pressed;
			if ((@event as InputEventMouseButton).IsReleased())
			{
				_dragginStart = false;
				_boardVisual.UpdateBoard((int)this.colourType,_boardPosition,_boardVisual.GlobalPositionToBoardPosition(GlobalPosition));
			}
		}
	}

	public override void _Ready()
	{
		GetNode<Area2D>("Area2D").MouseEntered += () => _OnMouseEnter();
		GetNode<Area2D>("Area2D").MouseExited += () => _OnMouseExit();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_isDragging && _mouseOver)
		{
			if (!_dragginStart)
			{
				_prevPosition = GlobalPosition;
				_dragginStart = true;
			}
			GlobalPosition = GetGlobalMousePosition();
		}
	}
	

	void _OnMouseEnter()
	{
		_mouseOver = true;
	}

	void _OnMouseExit()
	{
		_mouseOver = false;
	}
}
