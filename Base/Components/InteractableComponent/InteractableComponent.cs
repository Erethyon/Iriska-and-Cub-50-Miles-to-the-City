using Godot;
using System;
using System.Linq;
using static Constants;

/// <summary>
/// Компонент, который выполняет какое-то действие,
/// если была нажата клавиша <c>Interact</c> (см. InputMap в настройках проекта или в <see cref="Constants"/>). <br/>
/// 
/// Действие разрешено выполнить, если <c>IsAvailable</c> будет возвращать <c>true</c>
/// </summary>
public partial class InteractableComponent : Node2D
{
	/// <summary>
	/// Должен быть выключенный Area2D, обрабатывающий только курсор мыши
	/// </summary>
	[Export] private Area2D mouseCursorArea2D;
	private AnimationPlayer animationPlayer;
	private bool isAvailable;

	/// <summary>
	/// Доступен ли компонент для взаимодействия.<br/>
	/// То есть будет ли вызыван сигнал <see cref="InteractButton"/> при нажатии кнопки INTERACT
	/// </summary>
	public bool IsAvailable => isAvailable;

	[Signal]
	public delegate void InteractButtonPressedEventHandler();


	public override void _Ready() {
		animationPlayer = GetChildren().OfType<AnimationPlayer>().FirstOrDefault();
		animationPlayer.Play("RESET");
		if (mouseCursorArea2D is null){
			GD.PushWarning($"mouseCursorArea2D is not defined for {GetParent().Name}");
		}
		mouseCursorArea2D.MouseEntered += OnMouseEntered;		
		mouseCursorArea2D.MouseExited += OnMouseExited;
	}


	public override void _Input(InputEvent @event) {
		if (@event.IsActionPressed(INTERACT) && isAvailable){
			EmitSignal(SignalName.InteractButtonPressed);
		}
	}
	

	private void OnMouseEntered(){
		animationPlayer.Play("AnimateAppearance");
		isAvailable = true;
	}
	private void OnMouseExited(){
		animationPlayer.PlayBackwards("AnimateAppearance");
		isAvailable = false;
	}
}
