using Godot;
using System;
using System.Linq;
using static Constants;

public partial class Player : Entity
{
  private float moveSpeed;  
  private PackedScene? gunScene;
  private AnimatedWeapon? gunNode;

  private Vector2 direction = Vector2.Zero;

  public PackedScene? GunScene => gunScene;
  public Vector2 Direction => direction;
  public float MoveSpeed => moveSpeed;	

  public override void _Ready()
  {
    base._Ready();
    moveSpeed = resource.MoveSpeed;
    gunScene = resource.GunScene;
    gunNode = gunScene.Instantiate<AnimatedWeapon>();
    gunNode.Scale *= 1;
    AddChild(gunNode);
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
        gunNode.StartShooting();
      }
      else{
        gunNode.StopShooting();
      }
      
      // Special mechanics on RMB
      if (inputEventMouseButton.IsActionPressed(RIGHT_MOUSE_BUTTON)){
        gunNode.OnSpecialMechanic();
      }
    }
    base._Input(@event);
  }
}
