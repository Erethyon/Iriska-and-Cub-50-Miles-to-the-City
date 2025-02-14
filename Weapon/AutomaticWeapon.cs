using Godot;
using System;
using System.Runtime.CompilerServices;
using static Constants;

/// <summary>
/// Handles full auto shooting out of the box
/// </summary>
public partial class AutomaticWeapon : AnimatedWeapon 
{
    /// <summary>
    /// Настроить оружие through <c>base._Ready()</c> и проиграть ANIM_IDLE
    /// </summary>
    public override void _Ready()
    {
        base._Ready();
        animatedSprite.Play(ANIM_IDLE);
    }

    /// <summary>
    /// Handle full auto shooting
    /// </summary>
    public override void _PhysicsProcess(double delta) {
        base._PhysicsProcess(delta);
        if (isShooting && timeSinceLastShot >= FireRate)
        {
            Shoot();
            timeSinceLastShot = 0.0f;
        }
        timeSinceLastShot += (float)delta;
    }

    /// <summary>
    /// Outdated. 
    /// Теперь создан Func CalcDirection и функция 
    /// SetCalcDirectionSource в части инициализации
    /// </summary>
    protected override void Shoot(){
        direction = CalcShootingDirection();
        SpawnProjectile(direction);
        base.Shoot();
    }

    /// <summary>
    /// Instantiates projectile and applies an impulse to it
    /// </summary>
    protected override void SpawnProjectile(Vector2 direction){
        Projectile projectile = ProjectileScene.Instantiate<Projectile>(PackedScene.GenEditState.MainInherited);
        projectile.startVelocity = direction.Normalized() * ProjectileSpeed;
        projectile.GlobalPosition = muzzle.GlobalPosition;
        projectile.Rotation = MathF.Atan2(direction.Y, direction.X);
        projectile.DamageValue = ProjectileDamage;
        projectile.CollisionLayer = GetProjectileCollisionLayer();
        projectile.CollisionMask = GetProjectileCollisionMask();
        GetTree().Root.AddChild(projectile);
    }
}

