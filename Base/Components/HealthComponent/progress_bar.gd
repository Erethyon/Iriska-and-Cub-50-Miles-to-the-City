extends ProgressBar

func _on_health_component_health_initialized(StartValue : float, MaxValue : float):
	max_value = MaxValue;
	value = StartValue;
	


func _on_health_component_health_changed(delta):
	value += delta
