using Godot;
using System;

public partial class HealthPickup : RigidBody2D
{
	[Export] private float healthValue;
	public float HealthValue => healthValue;
}
