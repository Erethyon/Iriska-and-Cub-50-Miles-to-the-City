using Godot;
using System;

[Tool]
[GlobalClass]
public partial class WeaponSettings : Resource
{
    [Export] public string DisplayedWeaponName {get; set;}
    [Export] public PackedScene ProjectileScene {get; set;}
    [Export] public float FireRate {get; set;} = 0.5f;
    [Export] public float ProjectileSpeed {get; set;} = 400f;
    [Export] public float ProjectileDamage {get; set;} = 0.5f;
}
