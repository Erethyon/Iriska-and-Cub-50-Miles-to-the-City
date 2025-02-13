#nullable enable
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using static StaticExtensions;

public partial class Entity : CharacterBody2D
{
	public override void _Ready()
	{
		try { GetChildren().OfType<HealthComponent>().First(); }		
		catch (System.Exception) { GD.PushWarning($"HealthComponent not found in {Name} Scene Tree");}
		//try { hitBox = GetChildren().OfType<HitBoxComponent>().First(); }
		//catch (System.Exception) { GD.PushWarning($"hitBox not found in {Name} Scene Tree");}

		//if (healthComponent != null && hitBox == null){
			//GD.PrintErr($"{nameof(hitBox)} is null while {nameof(healthComponent)} is not");
		//}
	}

	protected async void ListNullMembersAfter<T>(float delay) where T: Entity{
		await ToSignal(GetTree().CreateTimer(delay), "timeout");
    	StaticExtensions.CheckPublicMembersForNull_Node<CharacterBody2D, T>((T)this);
	}
}

