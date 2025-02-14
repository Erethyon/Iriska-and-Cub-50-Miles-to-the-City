#nullable enable
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using static StaticExtensions;


/// <summary>
/// Хранится для какого-нибудь полиморфизма
/// </summary>
public partial class Entity : CharacterBody2D
{
	public override void _Ready()
	{
		try { GetChildren().OfType<HealthComponent>().First(); }		
		catch (System.Exception) { GD.PushWarning($"HealthComponent not found in {Name} Scene Tree");}
	}

	protected async void ListNullMembersAfter<T>(float delay) where T: Entity{
		await ToSignal(GetTree().CreateTimer(delay), "timeout");
    	StaticExtensions.CheckPublicMembersForNull_Node<CharacterBody2D, T>((T)this);
	}
}

