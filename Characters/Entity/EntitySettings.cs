using Godot;
using System.Runtime.CompilerServices;

[Tool]
[GlobalClass]
public partial class EntitySettings : Resource
{
    // Characteristics
    [ExportCategory("Characteristics")]
    [Export]
    public float HealthStartValue { get; set; } = 100;

    [Export]
    public float HealthMaxValue { get; set; } = 100;

    [Export]
    public float MoveSpeed { get; set; } = 200;

    // Gun
    [ExportCategory("Gun")]
    [Export]
    public PackedScene GunScene { get; set; }

    [Export]
    public float GunDamageMultiplier { get; set; } = 1.0f;

    [Export]
    public float GunSpecialDamageMultiplier { get; set; } = 1.0f;

    [Export(PropertyHint.Layers2DPhysics)]
    public uint ProjectileCollisionLayer { get; set; }

    [Export(PropertyHint.Layers2DPhysics)]
    public uint ProjectileCollisionMask { get; set; }

    // Physics
    [ExportCategory("Physics")]
    [Export(PropertyHint.Layers2DPhysics)]
    public uint CollisionLayer { get; set; }

    [Export(PropertyHint.Layers2DPhysics)]
    public uint CollisionMask { get; set; }

    // Health Hitbox
    [ExportCategory("Health Hitbox")]
    [Export(PropertyHint.Layers2DPhysics)]
    public uint HitboxLayer { get; set; }

    [Export(PropertyHint.Layers2DPhysics)]
    public uint HitboxMask { get; set; }


    public void ValidateValues(){
        StaticExtensions.CheckPublicMembersForNull_GodotObject<Resource, EntitySettings>(this);
    }
}
