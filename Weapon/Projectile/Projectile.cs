using Godot;
using System;

/// <summary>
/// Класс снаряда, который собирается и настраивается классом оружия
/// В базовой логике не пытается никак никого атаковать, но хранит 
/// в себе значение наносимого урона.
/// </summary>
public partial class Projectile : RigidBody2D
{
	[Export] private Node2D sprite;
	[Export] private Timer lifetime;
	[Export] private float _damageValue = 0.5f;

	/// <summary>
	/// Поле, "сырое" хранящее в себе значение урона, которое должно нанестись 
	/// HealthComponent
	/// </summary>
	public float DamageValue {get; set; }

	/// <summary>
	/// Скорость в части кода ApplyImpulse(startVelocity) 
	/// </summary>
    public Vector2 startVelocity {get; set;}

    public override void _Ready()
    {
        base._Ready();
		sprite = GetNode<Node2D>("Sprite2D");
		lifetime = GetNode<Timer>("Lifetime");
		lifetime.Timeout += OnLifetimeEnded;
		lifetime.Start();
		ApplyImpulse(startVelocity);		
  	}
	
    /// <summary>
    /// Executes after lifetime timer has timed out
    /// Queues a node for deletion
    /// </summary>
    protected virtual void OnLifetimeEnded(){
		QueueFree();
	}
}
