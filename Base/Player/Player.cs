using Godot;
using System;
using System.Linq;
using static Constants;

public partial class Player : Entity
{
  [Export] protected float moveSpeed = 80;
  private AnimatedWeapon? weaponNode;
  private Vector2 direction = Vector2.Zero;

  public Vector2 Direction => direction;
  public float MoveSpeed => moveSpeed;	

  public override void _Ready()
  {
    base._Ready();
    try { weaponNode = GetChildren().OfType<AnimatedWeapon>().First(); }
		catch(Exception) {GD.PushWarning($"weaponNode not found in {Name} Scene Tree");}
		weaponNode.SetFacingDirection(direction);
  }


  public override void _PhysicsProcess(double delta) {
    direction = Input.GetVector(INPUT_MOVE_LEFT, INPUT_MOVE_RIGHT, INPUT_MOVE_UP, INPUT_MOVE_DOWN);
    Velocity = direction * MoveSpeed;

    MoveAndSlide();
    base._PhysicsProcess(delta);
  }


  public override void _Input(InputEvent @event)
  {
    if (@event is InputEventMouseButton inputEventMouseButton){
      // Shoot
      if (inputEventMouseButton.IsActionPressed(LEFT_MOUSE_BUTTON)){
        weaponNode.StartShooting();
      }
      else{
        weaponNode.StopShooting();
      }
      
      // Special mechanics on RMB
      if (inputEventMouseButton.IsActionPressed(RIGHT_MOUSE_BUTTON)){
        weaponNode.OnSpecialMechanic();
      }
    }
    base._Input(@event);
  }
}
