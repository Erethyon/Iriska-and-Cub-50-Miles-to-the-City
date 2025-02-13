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
    [Export] protected WeaponSettings weaponSettings;

    // Для осуществления механики стрельбы
    protected string displayedWeaponName;
    protected bool isShooting = false;
    protected float timeSinceLastShot = 0.0f;
    protected float finalDamageMultiplier = 1.0f;
    protected float finalSpecialDamageMultiplier = 1.0f;
    protected Vector2 direction;

    // public properties для сканирования на null
    public string DisplayedWeaponName => displayedWeaponName;
    public AnimatedSprite2D Sprite => sprite;
    public Node2D Offset => offset;
    public Node2D? OwnerNode => ownerNode;
    public AnimatedSprite2D SpritePublic => sprite;
    public Node2D Muzzle => muzzle;
    public PackedScene ProjectileScene => weaponSettings.ProjectileScene;
    public float FireRate => weaponSettings.FireRate;
    public float ProjectileSpeed => weaponSettings.ProjectileSpeed;
    public float ProjectileDamage => weaponSettings.ProjectileDamage;
    public float FinalDamageMultiplier => finalDamageMultiplier;
    public float FinalSpecialDamageMultiplier => finalSpecialDamageMultiplier;
    //public Vector2 Direction => direction;
    

    /// <summary>
    /// Посчитать направление стрельбы. Разный для Player и NPC классов.
    /// </summary>
    protected Func<Vector2> CalcShootingDirection;
    private Vector2 calcShootingDirection_Player(){
        return (GetGlobalMousePosition() - muzzle.GlobalPosition).Normalized();
    }
    private Vector2 calcShootingDirection_NPC(){
        return direction;
    }

    /// <summary>
    /// Один из способов направить оружие в нужном направлении. <br/>
    /// Другой - <seealso cref="SetFacingDirection"/>
    /// </summary>
    /// <param name="globalPosition">Координаты точки интереса в глодальных координатах</param>
    public void PointWeaponAt(Vector2 globalPosition){
        this.direction = globalPosition - this.GlobalPosition;
        this.direction = this.direction.Normalized();
    }
    /// <summary>
    /// Один из способов направить оружие в нужном направлении. <br/>
    /// Другой - <seealso cref="PointWeaponAt"/>
    /// </summary>
    /// <param name="normDirection">Нормализованный вектор, направленный от <c>GlobalPosition</c> оружия <br/>
    /// к <c>GlobalPosition</c> точки интереса</param>
    public void SetFacingDirection(Vector2 normDirection){
        this.direction = normDirection;
    }


    public override void _Ready()
    {   
        displayedWeaponName = weaponSettings.DisplayedWeaponName;
        flipSprite = flipSpriteDefault;

        ownerNode = GetParent<Entity>();
        if (OwnerNode is Player){
            CalcShootingDirection = calcShootingDirection_Player;
            GetProjectileCollisionLayer = GetProjectileCollisionLayer_Player;
            GetProjectileCollisionMask = GetProjectileCollisionMask_Player;
            UpdateSpriteFlipState = UpdateSpriteFlipState_Player;
        }
        else if (ownerNode is NPC){
            CalcShootingDirection = calcShootingDirection_NPC;
            GetProjectileCollisionLayer = GetProjectileCollisionLayer_NPC;
            GetProjectileCollisionMask = GetProjectileCollisionMask_NPC;
            UpdateSpriteFlipState = UpdateSpriteFlipState_NPC;
        }

        // check for nulls
        StaticExtensions.CheckPublicMembersForNull_Node<Node2D, AnimatedWeapon>(this);
        base._Ready();
    }

    /// <summary>
    /// Получить слои коллизии для снаряда. Разный для обладателя-игрока и обладателя-NPC
    /// </summary>
    protected Func<uint> GetProjectileCollisionLayer;
    private uint GetProjectileCollisionLayer_Player(){
        return weaponSettings.ProjectileCollisionLayer_Player;
    }
    private uint GetProjectileCollisionLayer_NPC(){
        return weaponSettings.ProjectileCollisionLayer_NPC;
    }

    /// <summary>
    /// Получить маски коллизии для снаряда. Разный для обладателя-игрока и обладателя-NPC
    /// </summary>
    protected Func<uint> GetProjectileCollisionMask;
    private uint GetProjectileCollisionMask_Player(){
        return weaponSettings.ProjectileCollisionMask_Player;
    }
    private uint GetProjectileCollisionMask_NPC(){
        return weaponSettings.ProjectileCollisionMask_NPC;
    }


    /// Shooting mechanics ////
    /// <summary>
    /// Зажать курок
    /// </summary>
    public virtual void StartShooting() => isShooting = true;
    /// <summary>
    /// Разжать курок
    /// </summary>
    public virtual void StopShooting() => isShooting = false;

    /// <summary>
    /// Внутренняя механика обработки стрельбы такова, что <br/>
    /// есть <c>bool isShooting</c>, который "зажимает курок". <br/>
    /// Эта функция решает, что должно происходить при каждом нужном выстреле<br/> 
    /// И эта функция позволяет создать какой-нибудь паттерн стрельбы
    /// </summary>
    protected abstract void OnShoot();
    /// <summary>
    /// Функция для инстанцирования сцены, создания и настройки снаряда
    /// </summary>
    /// <param name="direction">Направление полёта снаряда</param>
    protected abstract void SpawnProjectile(Vector2 direction);
    [Obsolete("Эта функция недоделана")]
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
        if ((GlobalPosition + CalcShootingDirection()).X < GlobalPosition.X){
            flipSprite(false);
        }
        else if ((GlobalPosition + CalcShootingDirection()).X > GlobalPosition.X){
            flipSprite(true);
        }
        LookAt(GlobalPosition + CalcShootingDirection());
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