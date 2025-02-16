using Godot;

/// <summary>
/// Интерфейс всех предметов, которые могут лежать на полу и быть подняты
/// </summary>
public interface IPickable{
    /// <summary>
    /// Используется для LootSpawner, чтобы на основе BoundingRect расположить <br/>
    /// предметы на земле так, чтобы они не пересекались (или пересекались минимально)
    /// </summary>
    public Vector2 GetBoundingRect();
}