using Godot;
using System;

using Chess.Script.Engine;


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
	private int _boardPosInt = -1;
	private Sprite2D _sprite;

	private bool isWhite;

	private bool _moved;

	private Vector2 worldPosition = Vector2.Zero;

	public Vector2I BoardPosition
	{
		get => _boardPosition;
	}

	public int IntBoardPosition
	{
		get => _boardPosInt;
	}
	
	// Called when the node enters the scene tree for the first time.
	public void Init(ColourType colourType, Vector2I _boardPosition, BoardVisual _boardVisual)
	{
		Name = new StringName(Functions.BoardPositionToInt(this._boardPosition).ToString());
		this._boardPosition = _boardPosition;
		this._boardVisual = _boardVisual;
		this.colourType = colourType;
		_sprite = GetNode<Sprite2D>("Sprite2D");
		
		_moved = false;
		if (_boardPosInt != -1)
		{
			_moved = true;
		}
		
		this._boardPosInt = Functions.BoardPositionToInt(this._boardPosition);
		_sprite.Texture = textures[(int)colourType - 1];
		
		worldPosition = new Vector2(_boardPosition.X, _boardPosition.Y) * _boardVisual.SIZE;

		if (!_moved)
			Position = worldPosition;
		
		isWhite = (int)this.colourType <= (int)ColourType.WHITE_PAWN;
	}

	

	public override void _Ready()
	{
		GetNode<Area2D>("Area2D").MouseEntered += () => _OnMouseEnter();
		GetNode<Area2D>("Area2D").MouseExited += () => _OnMouseExit();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public async override void _Process(double delta)
	{

		if (_moved)
		{
			if (Functions.IsEqualApprox(Position, worldPosition, 10))
			{
				Position = worldPosition;
				_moved = false;
			}
			else
			{
				Position = Position.Lerp(worldPosition, 0.3f);
			}
		}
		
		if(_boardVisual.board.WhiteTurn != isWhite)
			return;
		
		if(_boardVisual.AiIsWhite == isWhite)
			return;
		
		if (_isDragging && _mouseOver)
		{
			if (!_dragginStart)
			{
				_prevPosition = GlobalPosition;
				_dragginStart = true;
			}
			GlobalPosition = GetGlobalMousePosition() - 40 * Vector2.One;
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
				Vector2I finalPos = _boardVisual.GlobalPositionToBoardPosition(_sprite.GlobalPosition);

				if (!await _boardVisual.HumanMakeMove((int)colourType, _boardPosition, finalPos))
				{
					_moved = true;
				}
				
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
