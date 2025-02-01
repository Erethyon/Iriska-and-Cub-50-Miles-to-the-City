using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using static Constants;

public partial class Enemy1 : Entity
{
	[Export] protected Node2D? directTargetNode;
	private Timer stateTimer;
	private Area2D playerPresenceDetector;
	protected Vector2 direction = Vector2.Zero;
	private AnimatedWeapon gunNode;
	private float patrolSpeed = 0.5f;
	private VisibleOnScreenNotifier2D isVisibleNotifier;
	private Rid mapRid;
	private RayCast2D rayCast2D;

	public Node2D DirectTargetNode {get => directTargetNode; set => directTargetNode = value;}
	public Timer StateTimer => stateTimer;
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
		stateTimer = GetNode<Timer>("StateTimer");
		//stateTimer.Timeout += OnStateTimerTimeout;
		playerPresenceDetector = GetNode<Area2D>("PlayerPresenceDetector");
		navAgent.TargetPosition = NavigationServer2D.MapGetRandomPoint(mapRid, 1, false);
		calcVelocity += calcVelocityToPatrol;
		navAgent.NavigationFinished += OnNavigationFinished;
		isVisibleNotifier = GetNode<VisibleOnScreenNotifier2D>("VisibleOnScreenNotifier2D");
		isVisibleNotifier.ScreenEntered += OnScreenEntered;
		isVisibleNotifier.ScreenExited += OnScreenExited;
		mapRid = navAgent.GetNavigationMap();

		rayCast2D = GetNode<RayCast2D>("RayCast2D");
		
		gunNode = resource.GunScene.Instantiate<AnimatedWeapon>();
		gunNode.Scale *= 1;
		gunNode.Offset.MoveLocalX(0);	
		gunNode.Direction = Direction;
		AddChild(gunNode);
		MoveChild(gunNode, -1);
		//gunNode.StartShooting();

		// Поебаться с какой-то хуйнёй от NavServer из-за поздней/ранней подгрузки карты
		NavigationServer2D.MapChangedEventHandler OnMapChanged = null;
		OnMapChanged = (Rid map) => { SetPhysicsProcess(true); NavigationServer2D.MapChanged -= OnMapChanged; };
		NavigationServer2D.MapChanged += OnMapChanged;
		
		StaticExtensions.CheckPublicMembersForNull_Node<Node2D, Entity>(this);
  	}

	public override void _Process(double delta)
    {
		gunNode.Direction = Direction;
        base._Process(delta);
    }

	private Action calcVelocity;
    public override void _PhysicsProcess(double delta)
	{
		// It's ME who for some fucking reason has to query the RayCast2D for collisions with objects!
		base._PhysicsProcess(delta);
		rayCast2D.TargetPosition = directTargetNode.GlobalPosition - Position;
		if (rayCast2D.IsColliding()){
			
			OnRayCast2DIsColliding(rayCast2D.GetCollider());
		}
		else{
			OnRayCast2DIsNotColliding();
		}
		calcVelocity();
		MoveAndSlide();
	}

	// Navigation and simple ai stuff
	private Random random = new Random();
	private List<Vector2> randomPointsOnPatrolArea = new List<Vector2>();
	
	public void OnStateTimerTimeout(){
		float randomValue = random.NextSingle();
		randomValue = 0.5f;
		randomPointsOnPatrolArea.Clear();
		indexOfLastRandomPoint = 0;
		// patrol state
		if (randomValue < 0.8f){
			GD.Print("patrol state");
			mapRid = navAgent.GetNavigationMap();
			GD.Print(mapRid.IsValid);
			randomPointsOnPatrolArea.Add(NavigationServer2D.MapGetRandomPoint(mapRid, 1, false));
			randomPointsOnPatrolArea.Add(NavigationServer2D.MapGetRandomPoint(mapRid, 1, false));
			randomPointsOnPatrolArea.Add(NavigationServer2D.MapGetRandomPoint(mapRid, 1, false));		
			string msg = "";
			foreach (Vector2 vec in randomPointsOnPatrolArea){
				msg += $"{vec.X} {vec.Y}; ";
			}
			GD.Print(msg, mapRid.Id.ToString());
			calcVelocity = calcVelocityToPatrol;
		}

		// "move to player" state
		else{
			GD.Print("move to player");
			calcVelocity = calcVelocityToNavigateToPlayer;
		}
	}


	private int indexOfLastRandomPoint = 0;
	private void calcVelocityToPatrol(){
		direction = (navAgent.GetNextPathPosition() - GlobalPosition).Normalized();
		Velocity = direction * resource.MoveSpeed * patrolSpeed;
	}

	private void calcVelocityToNavigateToPlayer(){
		navAgent.TargetPosition = directTargetNode.Position;
		direction = (navAgent.GetNextPathPosition() - GlobalPosition).Normalized();
		Velocity = direction * resource.MoveSpeed;
	}

	private void OnScreenEntered(){
		GD.Print("OnScreenEntered");
		calcVelocity = calcVelocityToNavigateToPlayer;
	}

	private void OnScreenExited(){
		GD.Print("OnScreenExited");
		navAgent.TargetPosition = directTargetNode.Position;
		calcVelocity = calcVelocityToGoToTargetPosition;
	}

	private void calcVelocityToGoToTargetPosition(){
		direction = (navAgent.GetNextPathPosition() - GlobalPosition).Normalized();
		Velocity = direction * resource.MoveSpeed * patrolSpeed;
	}

	private void OnNavigationFinished(){
		GD.Print("OnNavigationFinished");
		Rid mapRid = navAgent.GetNavigationMap();
		navAgent.TargetPosition = NavigationServer2D.MapGetRandomPoint(mapRid, 1, false);
		calcVelocity = calcVelocityToPatrol;
	}

	private void OnRayCast2DIsColliding(GodotObject godotObject){
		if (godotObject is Player){
			directTargetNode = (Node2D)godotObject;
			gunNode.StartShooting();
		}
		else{
			gunNode.StopShooting();	
		}
	}

	private void OnRayCast2DIsNotColliding(){
		gunNode.StopShooting();
	}
}
