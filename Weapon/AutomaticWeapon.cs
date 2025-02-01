using Godot;
using System;
using System.Runtime.CompilerServices;
using static Constants;

public partial class AutomaticWeapon : AnimatedWeapon 
{
    public override void _Ready()
    {
        ownerNode = GetOwner<Entity>();
        sprite.Play(ANIM_IDLE);
        base._Ready();
    }

    /// <summary>
    /// Outdated. 
    /// Теперь создан Func CalcDirection и функция 
    /// SetCalcDirectionSource в части инициализации
    /// </summary>
    protected override void OnShoot(){
        direction = CalcDirection();
        SpawnProjectile(Direction);
    }

    /// <summary>
    /// Instantiates projectile and applies an impulse to it
    /// </summary>
    protected override void SpawnProjectile(Vector2 direction){
        Projectile projectile = ProjectileScene.Instantiate<Projectile>(PackedScene.GenEditState.MainInherited);
        projectile.startVelocity = direction * ProjectileSpeed;
        projectile.GlobalPosition = muzzle.GlobalPosition;
        projectile.Rotation = MathF.Atan2(direction.Y, direction.X);
        projectile.DamageValue = ProjectileDamage;
        projectile.CollisionLayer = ownerNode.Resource.ProjectileCollisionLayer;
        projectile.CollisionMask = ownerNode.Resource.ProjectileCollisionMask;
        GetTree().Root.AddChild(projectile);
    }
}

