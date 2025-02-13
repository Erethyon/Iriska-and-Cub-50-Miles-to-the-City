using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using static StaticExtensions;

/// <summary>
/// Класс с определённой логикой поведения
/// </summary>
public abstract partial class NPC : Entity
{
	// Общие характеристики
	[Export] protected PackedScene gunScene;
	protected AnimatedWeapon? gunNode;

	// Ноды логики поведения
	[Export] protected Node2D? targetNode;
	protected NavigationAgent2D BEH_navigationAgent2D;
	protected Area2D? BEH_playerPresenceDetector;		// для определения присутствия игрока. прямого и косвенного
	protected VisibleOnScreenNotifier2D BEH_visibleOnScreenNotifier;
	protected RayCast2D BEH_playerRaycast;	// в мультиплеере есть много игроков
	protected Vector2 direction = Vector2.Zero;			// для расчёта направления движения
	private Rid navigationMapRid;						// получить 

	// Настройки для поведения
	[Export] protected float moveSpeed = 50;
	protected float patrolSpeedMultiplier = 0.5f;
    protected Action calcVelocity;


	public Node2D TargetNode {get => targetNode; set => targetNode = value;}
	public AnimatedWeapon? GunNode => gunNode;
	public NavigationAgent2D BEH_NavigationAgent2D => BEH_navigationAgent2D;
	public Area2D BEH_PlayerPresenceDetector => BEH_playerPresenceDetector;
	public VisibleOnScreenNotifier2D IsVisibleNotifier => BEH_visibleOnScreenNotifier;
	public RayCast2D PlayerRaycast => BEH_playerRaycast;
	public Vector2 Direction => Velocity.Normalized();
	public float PatrolSpeedMultiplier => patrolSpeedMultiplier;
	public float MoveSpeed => moveSpeed;

  	public override void _Ready()
  	{
		base._Ready();

		// Поебаться с какой-то хуйнёй от NavServer из-за поздней/ранней подгрузки карты я хуй его знает честно
		NavigationServer2D.MapChangedEventHandler OnMapChanged = null;
		OnMapChanged = (Rid map) => { 
			BEH_navigationAgent2D.TargetPosition = NavigationServer2D.MapGetRandomPoint(navigationMapRid, 1, false);
			NavigationServer2D.MapChanged -= OnMapChanged; 
		};
		NavigationServer2D.MapChanged += OnMapChanged;

		// behaviour and ai
		try { navigationMapRid = BEH_navigationAgent2D.GetNavigationMap();}
		catch (Exception) { GD.PushWarning($"navigationMapRid not found in {Name} Scene Tree");}

		try { BEH_navigationAgent2D = GetChildren().OfType<NavigationAgent2D>().First();}
		catch (System.Exception){ GD.PushWarning($"BEH_navigationAgent2D not found in {Name} Scene Tree");}
				
		try { BEH_playerPresenceDetector = GetNode<Area2D>("PlayerPresenceDetector");}
		catch (System.Exception){ GD.PushWarning($"BEH_navigationAgent2D not found in {Name} Scene Tree");}
		
		try {BEH_navigationAgent2D.TargetPosition = NavigationServer2D.MapGetRandomPoint(navigationMapRid, 1, false);}
		catch (Exception){ GD.PushWarning($"Attempt to set TargetPosition was unsuccessful");}

		try { BEH_visibleOnScreenNotifier = GetNode<VisibleOnScreenNotifier2D>("VisibleOnScreenNotifier2D");}
		catch(Exception) { GD.PushWarning($"BEH_visibleOnScreenNotifier not found in {Name} Scene Tree");}


		try { BEH_playerRaycast = GetNode<RayCast2D>("RayCast2D");}
		catch(Exception) { GD.PushWarning($"rayCast2D not found in {Name} Scene Tree");}
		
		try {gunNode = gunScene.Instantiate<AnimatedWeapon>();}
		catch(Exception) { GD.PushWarning($"resource.GunScene not found in {Name} Scene Tree");}
		
		gunNode.Offset.MoveLocalX(0);	
		gunNode.SetFacingDirection(direction);
		AddChild(gunNode);
		MoveChild(gunNode, -1);
  	}
}
