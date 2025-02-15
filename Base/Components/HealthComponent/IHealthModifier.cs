using Godot;
using System;

/// <summary>
/// DI-интерфейс, облегчающий работу с HealthComponent и изменением его значения здоровья
/// </summary>
public interface IHealthModifier{
    /// <summary>
    /// Указывает, на какую величину измененяется HealthComponent при столкновениях с ним.
    /// </summary>
    public float DeltaHealthValue {get; protected set;}
}