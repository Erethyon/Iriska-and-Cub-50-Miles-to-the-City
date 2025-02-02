extends ProgressBar

func _on_health_component_health_initialized(StartValue : float, MaxValue : float):
	value = StartValue;
	max_value = MaxValue;
	print(StartValue, MaxValue)
	


func _on_health_component_health_changed(delta):
	value += delta
