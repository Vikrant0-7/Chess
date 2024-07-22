using Godot;
using System;
using System.Threading.Tasks;
public partial class PromotePop : Control
{
	// Called when the node enters the scene tree for the first time.
	[Signal]
	public delegate void SetNumberEventHandler();

	private int attr;
	
	public override void _Ready()
	{
		Hide();
		BoardVisual.PromoteTo += GetPromoteTo;
		
		GetNode<TextureButton>("%queen").Pressed += () => SetAttr(1);
		GetNode<TextureButton>("%rook").Pressed += () => SetAttr(2);
		GetNode<TextureButton>("%bishop").Pressed += () => SetAttr(3);
		GetNode<TextureButton>("%knight").Pressed += () => SetAttr(4);
	}

	void SetAttr(int i)
	{
		attr = i;
		EmitSignal(SignalName.SetNumber);
	}
	

	async Task<int> GetPromoteTo(bool isWhite)
	{
		Show();

		int promtesTo = isWhite ? 1 : 7;
		await ToSignal(this, SignalName.SetNumber);
		promtesTo += attr;
		
		Hide();
		return promtesTo;
	}

	public override void _ExitTree()
	{
		BoardVisual.PromoteTo -= GetPromoteTo;
	}
}
