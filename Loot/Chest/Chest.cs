using Godot;
using System;
using System.Linq;

public partial class Chest : AnimatedSprite2D
{
	private InteractableComponent interactableComponent;
	
	public override void _Ready() {
		interactableComponent = GetChildren().OfType<InteractableComponent>().FirstOrDefault();
		interactableComponent.InteractButtonPressed += OnInteractButtonPressed;
	}

	private async void OnInteractButtonPressed(){
		Play("Open");
		var timer = GetTree().CreateTimer(4);
		await ToSignal(timer, Timer.SignalName.Timeout);
		QueueFree();
	}
}
