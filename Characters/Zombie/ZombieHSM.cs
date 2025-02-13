using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class ZombieHSM : Godot.LimboHsm
{
	[Export] PatrolState patrolState;
	[Export] HuntState huntState;
	
	public override void _Ready()
	{
		InitialState = patrolState;
		AddTransition(patrolState, huntState, patrolState.EVENTFINISHED);
		AddTransition(huntState, patrolState, huntState.EVENTFINISHED);
		Initialize(GetOwner<NPC>());
		SetActive(true);
	}
}
