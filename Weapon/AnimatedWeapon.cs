using Godot;
using System;
using static Constants;

/// <summary>
/// Внутренняя механика обработки стрельбы такова, что <br/>
/// есть <c>bool isShooting</c>, который "зажимает курок". <br/>
/// То есть мы получаем подобие <b>авто-атаки</b> из террарии.<br/>
/// <br/>
/// Использует <c>Node2D._PhysicsProcess</c> для обработки стрельбы и<br/>
/// при работе с <c>timeSinceLastShot</c> для реализации <b>"зажатого курка"</b> <br/>
/// </summary>
public abstract partial class AnimatedWeapon : Node2D
{
    [ExportGroup("Required Nodes")]    
    [Export] protected AnimatedSprite2D animatedSprite;
    [Export] protected Node2D offset;
    [Export] protected Node2D muzzle;
    [ExportGroup("Gun Characteristics")]
    [Export] protected WeaponSettings weaponSettings;
    [Export] protected float finalDamageMultiplier = 1.0f;
    [Export] protected float finalSpecialDamageMultiplier = 1.0f;

    // Для работы механики стрельбы

    /// <summary>
    /// состояние, отображающее "зажат" ли "курок" оружия
    /// </summary>
    protected bool isShooting = false;
    /// <summary>
    /// при зажатом курке оружие пытается создать новый инстанс снаряда
    /// </summary>
    protected float timeSinceLastShot = 0.0f;
    /// <summary>
    /// Направление оружия и направление, в котором создаётся снаряд
    /// </summary>
    protected Vector2 direction;
    protected Entity ownerNode;

    // public properties

    /// <summary>
    /// Value from a <see cref="WeaponSettings"/>
    /// </summary>
    public string DisplayedWeaponName => weaponSettings.DisplayedWeaponName;
    /// <summary>
    /// Value from a <see cref="WeaponSettings"/>
    /// </summary>
    public PackedScene ProjectileScene => weaponSettings.ProjectileScene;
    /// <summary>
    /// Value from a <see cref="WeaponSettings"/>
    /// </summary>
    public float FireRate => weaponSettings.FireRate;
    /// <summary>
    /// Value from a <see cref="WeaponSettings"/>
    /// </summary>
    public float ProjectileSpeed => weaponSettings.ProjectileSpeed;
    /// <summary>
    /// Value from a <see cref="WeaponSettings"/>
    /// </summary>
    public float ProjectileDamage => weaponSettings.ProjectileDamage;
    /// <summary>
    /// Instance-based damage multiplier
    /// </summary>
    public float FinalDamageMultiplier => finalDamageMultiplier;
    /// <summary>
    /// Instance-based damage multiplier for special attack
    /// </summary>
    public float FinalSpecialDamageMultiplier => finalSpecialDamageMultiplier;
    /// <summary>
    /// Типовые настройки для класса оружия
    /// </summary>
    public WeaponSettings WeaponSettings => weaponSettings;
    
    /// <summary>
    /// The weapon created and configured the projectile. <br/>
    /// <b>Should be invoked at the end of the <see cref="Shoot"/></b>
    /// </summary>
    [Signal] public delegate void WeaponShootedEventHandler();

    /// <summary>
    /// Оружие начало стрелять. На оружии <b>зажали</b> курок.
    /// </summary>
    [Signal] public delegate void WeaponStartedShootingEventHandler();

    /// <summary>
    /// Оружие перестало стрелять. На оружии <b>отжали</b> курок.
    /// </summary>
    [Signal] public delegate void WeaponStoppedShootingEventHandler();

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

    /// <summary>
    /// В зависимости от типа Parent-нода (Player or NPC) выставляет функции
    /// <code>
    /// CalcShootingDirection, GetProjectileCollisionLayer, GetProjectileCollisionMask, UpdateSpriteFlipState
    /// но
    /// flipSprite = flipSpriteDefault;
    /// </code>
    /// </summary>
    public override void _Ready()
    {   
        ownerNode = GetParent<Entity>();
        flipSprite = flipSpriteDefault;

        if (ownerNode is Player){
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
    }
    
    /// <summary>
    /// Update sprite flip state <br/>
    /// </summary>
    public override void _PhysicsProcess(double delta)
    {
        UpdateSpriteFlipState();
    }

    //////////////////////// Owner's type based actions ////////////////////////

    /// <summary>
    /// Посчитать направление стрельбы. Разный для Player и NPC классов.
    /// </summary>
    protected Func<Vector2> CalcShootingDirection {get; private set;}
    private Vector2 calcShootingDirection_Player(){
        return (GetGlobalMousePosition() - muzzle.GlobalPosition).Normalized();
    }
    private Vector2 calcShootingDirection_NPC(){
        return direction;
    }

    /// <summary>
    /// Получить слои коллизии для снаряда. Разный для Player и NPC классов.
    /// </summary>
    protected Func<uint> GetProjectileCollisionLayer {get; private set;}
    private uint GetProjectileCollisionLayer_Player(){
        return weaponSettings.ProjectileCollisionLayer_Player;
    }
    private uint GetProjectileCollisionLayer_NPC(){
        return weaponSettings.ProjectileCollisionLayer_NPC;
    }

    /// <summary>
    /// Получить маски коллизии для снаряда. Разный для Player и NPC классов.
    /// </summary>
    protected Func<uint> GetProjectileCollisionMask {get; private set;}
    private uint GetProjectileCollisionMask_Player(){
        return weaponSettings.ProjectileCollisionMask_Player;
    }
    private uint GetProjectileCollisionMask_NPC(){
        return weaponSettings.ProjectileCollisionMask_NPC;
    }


    /////////////////////////// Shooting mechanics ////////////////////////
    /// <summary>
    /// Зажать курок
    /// </summary>
    public virtual void StartShooting() {
        isShooting = true;
        EmitSignal(SignalName.WeaponStartedShooting);
    }
    /// <summary>
    /// Разжать курок
    /// </summary>
    public virtual void StopShooting(){
        isShooting = false;
        EmitSignal(SignalName.WeaponStoppedShooting);
    }

    /// <summary>
    /// Описывает то, что должно происходить при нажатом курке<br/>
    /// <code>
    /// Например,
    /// Посчитано направление для спавна снаряда
    /// Создана задержка или
    /// Проиграна анимация
    /// Вызвана функция SpawnProjectile
    /// </code>
    /// <b>Вызывает сигнал <see cref="WeaponShootedEventHandler"/></b>
    /// </summary>
    protected virtual void Shoot(){
        EmitSignal(SignalName.WeaponShooted);
    }
    /// <summary>
    /// Функция для инстанцирования сцены, создания и настройки снаряда
    /// </summary>
    /// <param name="direction">Направление полёта снаряда</param>
    protected abstract void SpawnProjectile(Vector2 direction);
    [Obsolete("Эта функция недоделана")]
    public virtual void OnSpecialMechanic(){}


    //////////////////////// Sprite control stuff ////////////////////////
    
    /// <summary>
    /// Функция отзеркаливания спрайта для левой полуокружности значений вектора <see cref="direction"/> <br/>
    /// Будет записано <see cref="flipSpriteDefault"/> по-умолчанию
    /// </summary>
    protected Action<bool> flipSprite;
    private void flipSpriteDefault(bool isFlipped) => animatedSprite.FlipV = isFlipped;

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
            flipSprite(true);
        }
        else if ((GlobalPosition + CalcShootingDirection()).X > GlobalPosition.X){
            flipSprite(false);
        }
        LookAt(GlobalPosition + CalcShootingDirection());
    }
}