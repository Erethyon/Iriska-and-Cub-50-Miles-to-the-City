using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using static StaticExtensions;


public abstract partial class Enemy : Entity
{
	[Export] protected Node2D? directTargetNode;
	protected Area2D? playerPresenceDetector;
	protected Vector2 direction = Vector2.Zero;
	protected AnimatedWeapon? gunNode;
	protected float patrolSpeed = 0.5f;
	protected VisibleOnScreenNotifier2D isVisibleNotifier;
	protected Rid mapRid;
	protected RayCast2D rayCast2D;
    protected Action calcVelocity;

	public Node2D DirectTargetNode {get => directTargetNode; set => directTargetNode = value;}
	public Area2D PlayerPresenceDetector => playerPresenceDetector;
	public Vector2 Direction => direction;
	public float MoveSpeed => resource.MoveSpeed;
	public float PatrolSpeed => patrolSpeed;
	public VisibleOnScreenNotifier2D IsVisibleNotifier => isVisibleNotifier;
	public RayCast2D RayCast2D => rayCast2D;

  	public override void _Ready()
  	{
		base._Ready();
		SetPhysicsProcess(false);

		// behaviour and ai
		try { playerPresenceDetector = GetNode<Area2D>("PlayerPresenceDetector");}
		catch (System.Exception){ GD.PushWarning($"NavAgent not found in {Name} Scene Tree");}
		
		navAgent.TargetPosition = NavigationServer2D.MapGetRandomPoint(mapRid, 1, false);
		calcVelocity += calcVelocityToPatrol;
		navAgent.NavigationFinished += OnNavigationFinished;

		try { isVisibleNotifier = GetNode<VisibleOnScreenNotifier2D>("VisibleOnScreenNotifier2D");}
		catch(Exception) { GD.PushWarning($"isVisibleNotifier not found in {Name} Scene Tree");}
		isVisibleNotifier.ScreenEntered += OnScreenEntered;
		isVisibleNotifier.ScreenExited += OnScreenExited;

		try { mapRid = navAgent.GetNavigationMap();}
		catch (Exception) { GD.PushWarning($"mapRid not found in {Name} Scene Tree");}

		try { rayCast2D = GetNode<RayCast2D>("RayCast2D");}
		catch(Exception) { GD.PushWarning($"rayCast2D not found in {Name} Scene Tree");}
		
		try {gunNode = resource.GunScene.Instantiate<AnimatedWeapon>();}
		catch(Exception) { GD.PushWarning($"resource.GunScene not found in {Name} Scene Tree");}
		gunNode.Scale *= 1;
		gunNode.Offset.MoveLocalX(0);	
		gunNode.isUsingCursorPosition = false;
		gunNode.Direction = Direction;
		AddChild(gunNode);
		MoveChild(gunNode, -1);
		//gunNode.StartShooting();

		// Поебаться с какой-то хуйнёй от NavServer из-за поздней/ранней подгрузки карты
		NavigationServer2D.MapChangedEventHandler OnMapChanged = null;
		OnMapChanged = (Rid map) => { SetPhysicsProcess(true); NavigationServer2D.MapChanged -= OnMapChanged; };
		NavigationServer2D.MapChanged += OnMapChanged;
  	}

	public override void _Process(double delta)
    {
		gunNode.Direction = Direction;
        base._Process(delta);
    }

	public override void _PhysicsProcess(double delta)
	{
		// It's ME who for some fucking reason has to query the RayCast2D for collisions with objects!
		base._PhysicsProcess(delta);
		if (directTargetNode == null){
			return;
		}
		rayCast2D.TargetPosition = directTargetNode.GlobalPosition - Position;
		if (rayCast2D.IsColliding()){
			
			OnRayCast2DIsColliding(rayCast2D.GetCollider());
		}
		else{
			OnRayCast2DIsNotColliding();
		}
	}

	// Navigation and simple ai stuff
	protected abstract void calcVelocityToPatrol();

	protected abstract void calcVelocityToNavigateToPlayer();

	protected abstract void OnScreenEntered();

	protected abstract void OnScreenExited();

	protected abstract void calcVelocityToGoToTargetPosition();

	protected abstract void OnNavigationFinished();

	protected abstract void OnRayCast2DIsColliding(GodotObject godotObject);

	protected abstract void OnRayCast2DIsNotColliding();
}
