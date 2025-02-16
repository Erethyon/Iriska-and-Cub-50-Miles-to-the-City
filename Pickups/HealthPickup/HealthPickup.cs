using Godot;
using System;
using System.Linq;


/// <summary>
/// Класс маленького <i>хилбокса</i>, который отхиливает HealthComponent<br/>
/// <i>HealthComponent конечно должен иметь</i> <see cref="HitBoxComponent"/> 
/// <i>поблизости, который настроен отслеживать правильные</i> <see cref="HitBoxComponent.CollisionMask"/>
/// </summary>
public partial class HealthPickup : RigidBody2D, IHealthModifier, IPickable
{
	[Export] private float healthValue;
	[Export] private AnimationPlayer animationPlayer;
	[Export] private Node2D sprite;

    float IHealthModifier.DeltaHealthValue { 
	get{
		OnHealthValueObtained();
		return healthValue;
	}
	set => healthValue = value; }


	private async void OnHealthValueObtained(){
		GD.Print("Test");
		// Disable collisionShape so it will not heal someone else
		var collisionShape = GetChildren().OfType<CollisionShape2D>().FirstOrDefault();
		collisionShape.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);

		// Play an animation and await to AnimationFinished
		animationPlayer.Play("Disappear");
		await ToSignal(animationPlayer, AnimationPlayer.SignalName.AnimationFinished);

		// That's all
		QueueFree();
	}

	/// <summary>
	/// Get bounding rectangle in pixels
	/// </summary>
    public Vector2 GetBoundingRect()
    {
        Vector2 texSize = (sprite as Sprite2D).Texture.GetSize();
		Vector2 scale = sprite.Scale;
		return new(texSize.X * scale.X, texSize.Y * scale.Y);
    }
}
