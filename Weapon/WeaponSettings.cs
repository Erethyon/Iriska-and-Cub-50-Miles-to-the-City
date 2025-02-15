using Godot;
using System;

/// <summary>
/// Набор параметров для оружия
/// <code>
/// Название пушки, 
/// снаряд, 
/// скорость стрельбы
/// урон и скорость полета снаряда
/// ----
/// Уровни и маски для <c>Player</c> и <c>NPC</c>
/// </code>
/// </summary>
[Tool] 
[GlobalClass] 
public partial class WeaponSettings : Resource
{
    [Export] public string DisplayedWeaponName {get; set;}
    [Export] public PackedScene ProjectileScene {get; set;}
    [Export(PropertyHint.Range, "0, 60, 0, or_greater,or_less")] public float FireRate {get; set;} = 0.5f;
    [Export(PropertyHint.Range, "100, 10000, 0, or_greater,or_less")] public float ProjectileSpeed {get; set;} = 400f;
    [Export(PropertyHint.Range, "-1000, 0, 0, or_greater,or_less")] public float ProjectileDamage {get; set;} = -0.5f;
    
    [ExportGroup("Маски и уровни, если оружие использует игрок")]
    [Export(PropertyHint.Layers2DPhysics)]
    public uint ProjectileCollisionLayer_Player { get; set; }
    [Export(PropertyHint.Layers2DPhysics)]
    public uint ProjectileCollisionMask_Player { get; set; }

    [ExportGroup("Маски и уровни, если оружие использует НИП")]
    [Export(PropertyHint.Layers2DPhysics)]
    public uint ProjectileCollisionLayer_NPC { get; set; }
    [Export(PropertyHint.Layers2DPhysics)]
    public uint ProjectileCollisionMask_NPC { get; set; }
}
