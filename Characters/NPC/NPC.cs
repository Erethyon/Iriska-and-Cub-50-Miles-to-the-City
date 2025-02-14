using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using static StaticExtensions;

/// <summary>
/// Класс НИПов со способностью ориентироваться в пространстве
/// </summary>
public abstract partial class NPC : Entity
{
	// Общие характеристики
	protected AnimatedWeapon? weaponNode;

	// Ноды логики поведения
	[Export] protected Node2D? targetNode;
	protected Vector2 direction = Vector2.Zero;			// для расчёта направления движения

	// Настройки для поведения
	[Export] protected float moveSpeed = 50;
	protected float patrolSpeedMultiplier = 0.5f;
    protected Action calcVelocity;


	public Node2D TargetNode {get => targetNode; set => targetNode = value;}
	public AnimatedWeapon? WeaponNode => weaponNode;
	public Vector2 Direction => Velocity.Normalized();
	public float PatrolSpeedMultiplier => patrolSpeedMultiplier;
	public float MoveSpeed => moveSpeed;

  	public override void _Ready()
  	{
		base._Ready();		
		try { weaponNode = GetChildren().OfType<AnimatedWeapon>().First(); }
		catch(Exception) {GD.PushWarning($"weaponNode not found in {Name} Scene Tree");}
		weaponNode.SetFacingDirection(direction);
  	}
}
