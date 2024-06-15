using Godot;
using System;
using System.Collections.Generic;
using System.Threading;

public partial class UILegalMoves : VBoxContainer
{
	[Export] private NodePath _boardPath;
	private BoardVisual _boardVisual;

	private LineEdit _lineEdit;
	private Label _label;

	private Queue<NoOfMovesThreadData<int>> queue;

	private double _time;
	
	public override void _Ready()
	{
		_boardVisual = GetNode<BoardVisual>(_boardPath);
		_lineEdit = GetNode<LineEdit>("Attacks2/LineEdit");
		_label = GetNode<Label>("Label2");
		GetNode<Button>("Attacks2/Button").Pressed += () => _OnButtonPressed();
		queue = new Queue<NoOfMovesThreadData<int>>();
	}

	void _OnButtonPressed()
	{
		int depth = Convert.ToInt32(_lineEdit.Text);
		_time = 0;
		_boardVisual.board.RequestNoOfMoves(depth, OnDataRecieved, queue);
	}

	public override void _PhysicsProcess(double delta)
	{
		_time += delta;
		if (queue.Count > 0)
		{
			NoOfMovesThreadData<int> data = queue.Dequeue();
			data.callback.Invoke(data.parameter);
		}
	}

	void OnDataRecieved(int data)
	{
		GD.Print(_time);
		_time = 0;
		_label.Text = data.ToString();
	}
}
