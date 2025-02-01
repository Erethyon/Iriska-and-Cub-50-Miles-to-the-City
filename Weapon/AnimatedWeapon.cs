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
    
    public Vector2 Direction {get; set;}
    public bool? isUsingCursorPosition;
    

    public override void _Ready()
    {   
        // setup
        displayedWeaponName = weaponSettings.DisplayedWeaponName;
        flipSpriteAction = flipSprite;

        ownerNode = GetParent<Entity>();

        // check for nulls
        StaticExtensions.CheckPublicMembersForNull_Node<Node2D, AnimatedWeapon>(this);
        base._Ready();
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


    // Shooting mechanics
    public virtual void StartShooting() => isShooting = true;
    public virtual void StopShooting() => isShooting = false;
    protected abstract void OnShoot();
    protected abstract void SpawnProjectile(Vector2 direction);
    public virtual void OnSpecialMechanic(){}


    // sprite control stuff
    protected void UpdateSpriteFlipState(){
        if (ownerNode.GlobalPosition.X < GetGlobalMousePosition().X){
            flipSpriteAction(false);
        }
        else if (ownerNode.GlobalPosition.X > GetGlobalMousePosition().X){
            flipSpriteAction(true);
        }
        LookAt(isUsingCursorPosition == true ? GetGlobalMousePosition() : GlobalPosition + Direction);
    }
    

    // Почему нужно обязательно придумывать какой-то "контейнер" для функции?
    // Разве она не будет просто универсальной для всех типов оружия?
    private Action<bool> flipSpriteAction;
    private void flipSprite(bool isFlipped) => sprite.FlipV = isFlipped;
}