extends Label

@onready var anim_player = $"../AnimationPlayer";

func _on_health_component_health_changed(delta):
	anim_player.stop()
	text = str(-delta)
	anim_player.play("LabelBump")
	
