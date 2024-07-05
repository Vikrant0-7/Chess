using Godot;
using System;
using System.Collections.Generic;
using System.Threading;

using Chess.Script.Engine;


public partial class UILegalMoves : VBoxContainer
{
	[Export] private NodePath _boardPath;
	private BoardVisual _boardVisual;

	private LineEdit _lineEdit;
	private Label _label;
	private Label _timeLabel;

	private Queue<MultithreadCallback<int>> queue;

	private double _time;
	
	public override void _Ready()
	{
		_boardVisual = GetNode<BoardVisual>(_boardPath);
		_lineEdit = GetNode<LineEdit>("Attacks2/LineEdit");
		
		_label = GetNode<Label>("Label2");
		_timeLabel = GetNode<Label>("Time/T");
		
		GetNode<Button>("Attacks2/Button").Pressed += () => _OnButtonPressed();
		queue = new Queue<MultithreadCallback<int>>();
	}

	void _OnButtonPressed()
	{
		int depth = Convert.ToInt32(_lineEdit.Text);
		_time = 0;
		//_boardVisual.NumberOfMoves(depth);
		_boardVisual.board.RequestNoOfMoves(depth, OnDataRecieved, queue);
	}

	public override void _PhysicsProcess(double delta)
	{
		_time += delta;
		if (queue.Count > 0)
		{
			MultithreadCallback<int> data = queue.Dequeue();
			data.callback.Invoke(data.parameter);
		}
	}

	void OnDataRecieved(int data)
	{
		_timeLabel.Text = Math.Round(_time,3).ToString() + "s";
		_time = 0;
		_label.Text = data.ToString();
	}
}
