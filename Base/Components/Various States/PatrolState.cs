using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PatrolState : LimboState
{	
	private NavigationAgent2D navigationAgent2D;
	private VisibleOnScreenNotifier2D visibilityNotifier;
	private RayCast2D rayCast2D;
	private NPC NPCAgent;

	private bool isPlayerOnScreen;
	private bool isRayCastCollidingWithPlayer;

	public override void _Ready()
	{
		NPCAgent = GetOwner<NPC>();
		Agent = NPCAgent;
		navigationAgent2D = Agent.GetChildren().OfType<NavigationAgent2D>().FirstOrDefault();
		navigationAgent2D.NavigationFinished += OnNavigationFinished;
		visibilityNotifier = Agent.GetNode<VisibleOnScreenNotifier2D>("VisibleOnScreenNotifier2D");
		rayCast2D = Agent.GetNode<RayCast2D>("RayCast2D");
		visibilityNotifier.ScreenEntered += OnScreenEntered;
		visibilityNotifier.ScreenExited += OnScreenExited;
	}

	private void OnScreenEntered(){
		isPlayerOnScreen = true;
	}

	private void OnScreenExited(){
		isPlayerOnScreen = false;
	}

	private void OnNavigationFinished(){
		Rid mapRid = navigationAgent2D.GetNavigationMap();
		navigationAgent2D.TargetPosition = NavigationServer2D.MapGetRandomPoint(mapRid, 1, false);
	}

	/////////////// querying Raycast2D //////////////////
	public override void _PhysicsProcess(double delta)
	{
		// It's ME who for some fucking reason has to query the RayCast2D for collisions with objects!
		rayCast2D.TargetPosition = NPCAgent.TargetNode.GlobalPosition - NPCAgent.Position;
		if (rayCast2D.IsColliding()){
			
			_OnRayCast2DIsColliding(rayCast2D.GetCollider());
		}
		else{
			_OnRayCast2DIsNotColliding();
		}
	}
	
	private void _OnRayCast2DIsColliding(GodotObject godotObject){
		if (godotObject is Player){
			NPCAgent.TargetNode = (Node2D)godotObject;
			isRayCastCollidingWithPlayer = true;
			// Это условие выхода: виден с помощью raycast и игрок враг отображается на экране
			if (isPlayerOnScreen){			
				Dispatch(EVENTFINISHED);
			}
		}
		else{
			isRayCastCollidingWithPlayer = false;
		}
	}

	private void _OnRayCast2DIsNotColliding(){
		isRayCastCollidingWithPlayer = false;
	}
	/////////////////////////////////////////////////////

    public override void _Update(double delta)
    {
		// Move to the next random position on navmap
        Vector2 direction = (navigationAgent2D.GetNextPathPosition() - NPCAgent.GlobalPosition).Normalized();
		NPCAgent.Velocity = direction * NPCAgent.MoveSpeed * NPCAgent.PatrolSpeedMultiplier;
		NPCAgent.MoveAndSlide();
    }

	public override string[] _GetConfigurationWarnings()
    {
        List<string> warnings = new();

		try{GetOwner<CharacterBody2D>().GetChildren().OfType<NavigationAgent2D>().FirstOrDefault();}
		catch (System.Exception)
		{
			warnings.Add($"У владельца сцены отсутствует нод NavigationAgent2D");
		}
		//warnings.Add(GetParent().Name);
		//warnings.Add(GetOwner().Name);
        return warnings.ToArray();
    }
}
