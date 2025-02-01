using Godot;
using System;

public partial class HitBox : Area2D
{
	public void ScanCollisionLayersForDefaultValues(){
        uint test = CollisionLayer ^ CollisionMask;
        if (test == 0 && CollisionLayer == 1)
        {
            GD.PrintErr($"Проверьте CollisionLayer и CollisionMask у HealthComponent для {GetParent().Name}: в них вставлен только слой №1");
        }
    }
}
