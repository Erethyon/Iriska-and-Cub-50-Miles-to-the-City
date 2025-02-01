using Godot;
using System;
using System.Linq;


public partial class HealthComponent : Node2D
{
    [Export] public float StartValue{get; set;}
    [Export] public float MaxValue {get;set;}
    private ProgressBar progressBar;

    private float _healthValue;

    public CharacterBody2D? aliveEntity;
    public HitBox hitBox;

    public float HealthValue
    {
        get { return _healthValue; }
        set { EmitSignal(SignalName.HealthChanged, _healthValue - value);
        _healthValue = value;
        progressBar.Value = _healthValue; }
    }

    public override void _Ready() {
        if (aliveEntity is null){
            //throw new Exception("Этот Health Component не имеет привязки к CharacterBody2D");
        }
        _healthValue = StartValue;

        hitBox = GetParent().GetChildren().OfType<HitBox>().First();
        hitBox.ScanCollisionLayersForDefaultValues();
        hitBox.BodyEntered += OnBodyEntered;
        
        progressBar = GetChildren().OfType<ProgressBar>().First();
        progressBar.MaxValue = MaxValue;
        progressBar.Value = StartValue;
        base._Ready();
    }

    static uint collisionsCount = 0;
    private void OnBodyEntered(Node2D node2d){
        collisionsCount++;
        GD.Print($"{collisionsCount}) {GetParent().Name} collided with {node2d.Name} so my hp is {HealthValue}");
        if (node2d is Projectile projectile){
            HealthValue -= projectile.DamageValue;
        }
    }

    [Signal]
    /// <summary>
    /// Событие, оповещающее об изменении значения компонента
    /// </summary>
    public delegate void HealthChangedEventHandler(float delta);
}
