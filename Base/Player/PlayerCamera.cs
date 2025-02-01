using Godot;
using System;
using static Constants;

public partial class PlayerCamera : Camera2D
{
	[Export] CharacterBody2D characterBody;
	[Export(PropertyHint.Range, "0.1, 7,")] float startZoom = 3f;
	[Export(PropertyHint.Range, "0.2, 0.5, 0.1")] float ZoomingSpeed = 0.5f;
	Vector2 deltaZoomValue = new Vector2(0.5f, 0.5f); // overridden in _ready

	Vector2 minZoom = new Vector2(1.0f, 1.0f);
	Vector2 maxZoom = new Vector2(5, 5);

    public override void _Ready()
    {
        Zoom = new Vector2(startZoom, startZoom);
		deltaZoomValue = new Vector2(ZoomingSpeed, ZoomingSpeed);
		if (characterBody == null){
			characterBody = new CharacterBody2D();
			characterBody.Position = Position;
			throw new Exception("No characterBody");
		}
    }

    public override void _PhysicsProcess(double delta)
    {
		if (Input.IsActionPressed(ZOOM_IN)){
			Zoom += deltaZoomValue;
		}
		if (Input.IsActionPressed(ZOOM_OUT)){
			Zoom -= deltaZoomValue;
		}
		Zoom = Zoom.Clamp(minZoom, maxZoom);
    }
}
