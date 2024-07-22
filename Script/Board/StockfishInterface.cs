
using Godot;
using System;
using System.Collections;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

using Chess.Script.Engine;


public partial class StockfishInterface : Node
{
	private Stockfish _fish;

	private int _depth = 2;
	private int _movetime = 10_000;

	private Queue<MultithreadCallback<Move>> _bestMoveCallbacks;
	private Queue<Action> _updatePositionCallbacks;
	private Board _board;

	private bool _playingAsWhite;

	public bool PlayingAsWhite => _playingAsWhite;
	
	public StockfishInterface(Board b, int depth, int movetime, bool playingAsWhite)
	{
		string path = "";
		if (OS.HasFeature("editor"))
		{
			path = ProjectSettings.GlobalizePath("res://stockfish");
		}
		else
		{
			path = OS.GetExecutablePath().GetBaseDir() + "/stockfish";
		}

		_bestMoveCallbacks = new Queue<MultithreadCallback<Move>>();
		_updatePositionCallbacks = new Queue<Action>();
		_board = b;
		_fish = new Stockfish(path);
		
		_playingAsWhite = playingAsWhite;
		_depth = depth;
		_movetime = movetime;
	}

	public void OrderToUpdatePosition(Action callback)
	{
		ThreadStart start = delegate
		{
			UpdatePosition(callback);
		};

		new Thread(start).Start();
	}
	
	public void RequestForBestMove(Action<Move> callback)
	{
		ThreadStart start = delegate
		{
			GetBestMove(callback);
		};

		new Thread(start).Start();
	}

	void GetBestMove(Action<Move> callback)
	{
		string bestMove = _fish.GetBestMove(_depth, _movetime);

		Move m = Functions.TranslateAlgebraicMove(bestMove, _board);
		
		lock (_bestMoveCallbacks)
		{
			_bestMoveCallbacks.Enqueue(new MultithreadCallback<Move>(callback, m));
		}
	}

	void UpdatePosition(Action callback)
	{
		FenTranslator translator = new FenTranslator(_board.BoardStatus, 
			(byte)(_board.GetCastleStatus(true) | 
			       (_board.GetCastleStatus(false) << 4)), 
			_board.EnPassantPosition, _board.WhiteTurn, new []{_board.FiftyMoveRule, _board.FullMoves});
		_fish.SetPosition(translator.FenString);
		lock (_updatePositionCallbacks)
		{
			_updatePositionCallbacks.Enqueue(callback);
		}
	}

	public override void _Process(double delta)
	{
		if (_bestMoveCallbacks.Count > 0)
		{
			var data = _bestMoveCallbacks.Dequeue();
			data.callback.Invoke(data.parameter);
		}
		if (_updatePositionCallbacks.Count > 0)
		{
			var data = _updatePositionCallbacks.Dequeue();
			data.Invoke();
		}
	}
}
