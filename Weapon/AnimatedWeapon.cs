using Godot;
using System;
using static Constants;

public abstract partial class AnimatedWeapon : Node2D
{
    [ExportGroup("Required Nodes")]
    [Export] public Entity? ownerNode;
    [Export] protected AnimatedSprite2D sprite;
    [Export] protected Node2D offset;
    [Export] protected Node2D muzzle;
    [ExportGroup("Gun Characteristics")]
    [Export] WeaponSettings weaponSettings;

    protected string displayedWeaponName;
    protected bool isShooting = false;
    protected float timeSinceLastShot = 0.0f;
    protected float finalDamageMultiplier = 1.0f;
    protected float finalSpecialDamageMultiplier = 1.0f;

    //
    public string DisplayedWeaponName { get => displayedWeaponName; }
    public AnimatedSprite2D Sprite => sprite;
    public Node2D Offset => offset;
    public Node2D? OwnerNode => ownerNode;
    public AnimatedSprite2D SpritePublic => sprite;
    public Node2D Muzzle => muzzle;
    public PackedScene ProjectileScene => weaponSettings.ProjectileScene;
    public float FireRate => weaponSettings.FireRate;
    public float ProjectileSpeed => weaponSettings.ProjectileSpeed;
    public float ProjectileDamage => weaponSettings.ProjectileDamage;
    public bool IsShooting => isShooting;
    public float FinalDamageMultiplier => finalDamageMultiplier;
    public float FinalSpecialDamageMultiplier => finalSpecialDamageMultiplier;
    
    protected Vector2 direction;
    public Vector2 Direction {get => direction; set => direction = value;}
    
    protected Func<Vector2> CalcDirection;
    private Vector2 calcDirection_Player(){
        return (GetGlobalMousePosition() - muzzle.GlobalPosition).Normalized();
    }
    private Vector2 calcDirection_NPC(){
        return Direction;
    }

    public override void _Ready()
    {   
        displayedWeaponName = weaponSettings.DisplayedWeaponName;
        flipSprite = flipSpriteDefault;

        ownerNode = GetParent<Entity>();
        if (OwnerNode is Player){
            CalcDirection = calcDirection_Player;
            UpdateSpriteFlipState = UpdateSpriteFlipState_Player;
        }
        else if (ownerNode is NPC){
            CalcDirection = calcDirection_NPC;
            UpdateSpriteFlipState = UpdateSpriteFlipState_NPC;
        }

        // check for nulls
        StaticExtensions.CheckPublicMembersForNull_Node<Node2D, AnimatedWeapon>(this);
        base._Ready();
    }
    
    //// Shooting mechanics ////
    public virtual void StartShooting() => isShooting = true;
    public virtual void StopShooting() => isShooting = false;
    //
    protected abstract void OnShoot();
    protected abstract void SpawnProjectile(Vector2 direction);
    public virtual void OnSpecialMechanic(){}


    //// Sprite control stuff ////
    private Action<bool> flipSprite;
    private void flipSpriteDefault(bool isFlipped) => sprite.FlipV = isFlipped;

    private Action UpdateSpriteFlipState;
    protected void UpdateSpriteFlipState_Player(){
        if (ownerNode.GlobalPosition.X < GetGlobalMousePosition().X){
            flipSprite(false);
        }
        else if (ownerNode.GlobalPosition.X > GetGlobalMousePosition().X){
            flipSprite(true);
        }
        LookAt(GetGlobalMousePosition());
    }

    protected void UpdateSpriteFlipState_NPC(){
        if ((GlobalPosition + Direction).X < GetGlobalMousePosition().X){
            flipSprite(false);
        }
        else if ((GlobalPosition + Direction).X > GetGlobalMousePosition().X){
            flipSprite(true);
        }
        LookAt(GlobalPosition + Direction);
    }


    public override void _Process(double delta)
    {
        UpdateSpriteFlipState();
        if (isShooting && timeSinceLastShot >= FireRate)
        {
            OnShoot();
            timeSinceLastShot = 0.0f;
        }

        timeSinceLastShot += (float)delta;
    }
}

/*
    /// <summary>
    /// Функция оружия на нажимаемую левую кнопку мыши (например, обычная стрельба)
    /// </summary>
    public abstract void OnLeftMouseButton();
    /// <summary>
    /// Функция оружия на нажимаемую правую кнопку мыши (например, специальная механика)
    /// </summary>
    public abstract void OnRightMouseButton();
    */