using Godot;
using System;

public partial class UILegalMoves : VBoxContainer
{
	[Export] private NodePath _boardPath;
	private BoardVisual _boardVisual;

	private LineEdit _lineEdit;
	private Label _label;
	public override void _Ready()
	{
		_boardVisual = GetNode<BoardVisual>(_boardPath);
		_lineEdit = GetNode<LineEdit>("Attacks2/LineEdit");
		_label = GetNode<Label>("Label2");
		GetNode<Button>("Attacks2/Button").Pressed += () => _OnButtonPressed();
	}

	void _OnButtonPressed()
	{
		int depth = Convert.ToInt32(_lineEdit.Text);
		_label.Text = _boardVisual.board.NumberOfMoves(depth).ToString();
	}
	
}
