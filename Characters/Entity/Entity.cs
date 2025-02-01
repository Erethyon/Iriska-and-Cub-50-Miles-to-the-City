#nullable enable
using Godot;
using System;
using System.Linq;
using System.Threading;
using static StaticExtensions;

public partial class Entity : CharacterBody2D
{
	[Export] protected EntitySettings resource;
	protected NavigationAgent2D? navAgent;
	protected HealthComponent? healthComponent;
	protected HitBox? hitBox;

	public EntitySettings Resource => resource;
	public NavigationAgent2D NavAgent => navAgent;
	public HealthComponent HealthComponent => healthComponent;
	public HitBox? HitBox => hitBox;

	public override void _Ready()
	{
		try { navAgent = GetChildren().OfType<NavigationAgent2D>().First();}
		catch (System.Exception){ GD.PushWarning($"NavAgent not found in {Name} Scene Tree");}
    	
		try { healthComponent = GetChildren().OfType<HealthComponent>().First();}		
		catch (System.Exception) { GD.PushWarning($"HealthComponent not found in {Name} Scene Tree");}
				
		try { hitBox = GetChildren().OfType<HitBox>().First(); }
		catch (System.Exception) { GD.PushWarning($"hitBox not found in {Name} Scene Tree");}

		if (healthComponent != null && hitBox == null){
			GD.PrintErr($"{nameof(hitBox)} is null while {nameof(healthComponent)} is not");
		}

		UpdateSettingsFromResource();
	}

	protected void UpdateSettingsFromResource(){
		healthComponent.aliveEntity = this;
    	healthComponent.hitBox = hitBox;
		
		// Import settings from resource
		// Health
		healthComponent.MaxValue = resource.HealthMaxValue;
		healthComponent.StartValue = resource.HealthStartValue;
		
		// Gun
		// ...

		// Physics
		CollisionLayer = resource.CollisionLayer;
		CollisionMask = resource.CollisionMask;
		
		// Hitbox
		hitBox.CollisionLayer = resource.HitboxLayer;
		hitBox.CollisionMask = resource.HitboxMask;
	}

	protected async void ListNullMembersAfter<T>(float delay) where T: Entity{
		await ToSignal(GetTree().CreateTimer(delay), "timeout");
    	StaticExtensions.CheckPublicMembersForNull_Node<CharacterBody2D, T>((T)this);
	}
}

