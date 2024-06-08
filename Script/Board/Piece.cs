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
	private bool _clickedWhileHovering = false;

	private Vector2 _prevPosition;

	private Vector2I _boardPosition;
	private BoardVisual _boardVisual;

	public Vector2I BoardPosition
	{
		get => _boardPosition;
	}
	
	// Called when the node enters the scene tree for the first time.
	public void Init(ColourType colourType, Vector2I _boardPosition, BoardVisual _boardVisual)
	{
		this._boardPosition = _boardPosition;
		this._boardVisual = _boardVisual;
		this.colourType = colourType;
		GetNode<Sprite2D>("Sprite2D").Texture = textures[(int)colourType];
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

		if (_mouseOver)
		{
			if (Input.IsActionJustPressed("left_click") && !_clickedWhileHovering)
			{
				_clickedWhileHovering = true;
				_boardVisual.ShowMoves((int)colourType, _boardPosition);
			}

			if (_clickedWhileHovering)
			{
				_isDragging = Input.IsActionPressed("left_click");
			}

			if (Input.IsActionJustReleased("left_click"))
			{
				_dragginStart = false;
				_clickedWhileHovering = false;
				Vector2I finalPos = _boardVisual.GlobalPositionToBoardPosition(GlobalPosition);
				
				_boardVisual.UpdateBoard((int)colourType, _boardPosition, finalPos);
				_boardVisual.ResetColors();
			}
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
