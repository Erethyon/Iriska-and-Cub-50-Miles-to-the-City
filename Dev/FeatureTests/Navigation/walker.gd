extends CharacterBody2D

@onready var nav_agent = $NavigationAgent2D

@export var move_speed = 100;
var target_pos;
# Called when the node enters the scene tree for the first time.
func _ready():
	nav_agent.connect("target_reached", _on_reached)


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _physics_process(delta):
	if target_pos != null:
		nav_agent.target_position = target_pos;
	var direction = (nav_agent.get_next_path_position() - global_position).normalized();
	velocity = direction * move_speed;
	move_and_slide()

func _input(event):
	if (event is InputEventMouseButton):
		target_pos = get_global_mouse_position();

func _on_reached():
	target_pos = null


func _on_wander_cd_timeout():
	var rid = nav_agent.get_navigation_map()
	target_pos = NavigationServer2D.map_get_random_point(rid, 1, false);
	
