using Godot;
using System;

/// <summary>
/// Класс снаряда, который собирается и настраивается классом оружия
/// В базовой логике не пытается никак никого атаковать, но хранит 
/// в себе значение наносимого урона.
/// </summary>
public partial class Projectile : RigidBody2D, IHealthModifier
{
	[Export] private Timer lifetime;
	[Export(PropertyHint.Range, "-1000, 0, 0, or_greater,or_less")] private float _damageValue = -0.5f;

	/// <summary>
	/// Поле, "сырое" хранящее в себе значение здоровья, которое должно прибавиться <br/> 
	/// HealthComponent
	/// </summary>
	public float DamageValue {get; set; }

	/// <summary>
	/// Скорость в части кода ApplyImpulse(startVelocity) 
	/// </summary>
    public Vector2 startVelocity {get; set;}
    
	float IHealthModifier.DeltaHealthValue{get => _damageValue; set => _damageValue = value;}

    public override void _Ready()
    {
        base._Ready();
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
