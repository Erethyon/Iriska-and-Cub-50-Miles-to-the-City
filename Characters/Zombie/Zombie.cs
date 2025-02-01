using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using static Constants;

public partial class Zombie : Enemy
{

  	public override void _Ready()
  	{
		try
		{
			base._Ready();
			SetPhysicsProcess(false);	
		}
		finally
		{
			GD.Print("finally");
			ListNullMembersAfter<Zombie>(1.0f);	
		}
  	}

    public override void _PhysicsProcess(double delta)
	{
		// It's ME who for some fucking reason has to query the RayCast2D for collisions with objects!
		base._PhysicsProcess(delta);
		calcVelocity();
		MoveAndSlide();
	}

	protected override void calcVelocityToPatrol(){
		direction = (navAgent.GetNextPathPosition() - GlobalPosition).Normalized();
		Velocity = direction * resource.MoveSpeed * patrolSpeed;
	}

	protected override void calcVelocityToNavigateToPlayer(){
		navAgent.TargetPosition = directTargetNode.Position;
		direction = (navAgent.GetNextPathPosition() - GlobalPosition).Normalized();
		Velocity = direction * resource.MoveSpeed;
	}

	protected override void OnScreenEntered(){
		GD.Print("OnScreenEntered");
		calcVelocity = calcVelocityToNavigateToPlayer;
	}

	protected override void OnScreenExited(){
		GD.Print("OnScreenExited");
		navAgent.TargetPosition = directTargetNode.Position;
		calcVelocity = calcVelocityToGoToTargetPosition;
	}

	protected override void calcVelocityToGoToTargetPosition(){
		direction = (navAgent.GetNextPathPosition() - GlobalPosition).Normalized();
		Velocity = direction * resource.MoveSpeed * patrolSpeed;
	}

	protected override void OnNavigationFinished(){
		GD.Print("OnNavigationFinished");
		Rid mapRid = navAgent.GetNavigationMap();
		navAgent.TargetPosition = NavigationServer2D.MapGetRandomPoint(mapRid, 1, false);
		calcVelocity = calcVelocityToPatrol;
	}

	protected override void OnRayCast2DIsColliding(GodotObject godotObject){
		if (godotObject is Player){
			directTargetNode = (Node2D)godotObject;
			gunNode.StartShooting();
		}
		else{
			gunNode.StopShooting();	
		}
	}

	protected override void OnRayCast2DIsNotColliding(){
		gunNode.StopShooting();
	}
}
