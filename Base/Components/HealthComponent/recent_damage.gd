extends Label

@onready var anim_player = $"../AnimationPlayer";
## если стоит true, то показывает только получение урона. иначе - ещё и отхил
@export var is_displaying_damage_only : bool = true; 
#@export var damage_color : Color
#@export var heal_color : Color

func _on_health_component_health_changed(delta):
	if (delta <= 0 and !is_displaying_damage_only):
		return;	
	anim_player.stop()
	text = str(delta)
	anim_player.play("LabelBump")
	
