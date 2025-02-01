using Godot;
using System;
using System.Linq;
using System.Collections.Generic;


[Tool]
public partial class HitBoxComponent : Area2D
{
	public override string[] _GetConfigurationWarnings()
    {
        List<string> warnings = new List<string>();
        // Существуют ли CollisionObject2D's вообще?
        try
        {
            var collisionObject2D = GetChildren().OfType<CollisionObject2D>().First();
        }
        catch (System.Exception)
        {
            return warnings.ToArray();
        }

        if (CollisionLayer == 1 && CollisionMask == 1)
        {
            warnings.Add($"Проверьте CollisionLayer и CollisionMask у HealthComponent для в них вставлен только слой №1");
        }
        return warnings.ToArray();
    }
}
