using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

[Tool]
public partial class HealthComponent : Node2D
{   
    private float healthValue;
    [Export] private float startValue;
    [Export] private float maxValue;
    [Export] private HitBoxComponent? hitBox;

    public float StartValue => startValue;
    public float MaxValue => maxValue;
    public float HealthValue
    {
        get => healthValue;
        set { 
            EmitSignal(SignalName.HealthChanged, value - healthValue);
            healthValue = value;
            if (value < 0){
                EmitSignal(SignalName.HealthZeroOrBelow);
            }
        }
    }

    public override void _Ready() {
        healthValue = StartValue;
        
        hitBox.BodyEntered += OnBodyEntered;
        
        EmitSignal(SignalName.HealthInitialized, StartValue, MaxValue);
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
    /// Событие, оповещающее об инициализации компонента с параметрами начального значения и максимального
    /// </summary>
    public delegate void HealthInitializedEventHandler(float StartValue, float MaxValue);

    [Signal]
    /// <summary>
    /// Событие, оповещающее об изменении значения компонента
    /// </summary>
    public delegate void HealthChangedEventHandler(float delta);

    [Signal]
    /// <summary>
    /// Событие, оповещающее об падении значения здоровья до 0 или меньше 0
    /// </summary>
    public delegate void HealthZeroOrBelowEventHandler();


    public override string[] _GetConfigurationWarnings()
    {
        List<string> warnings = new();

        if (hitBox == null)
        {
            warnings.Add("HealthComponent требует HitBoxComponent для корректной работы");
        }

        return warnings.ToArray();
    }
}
