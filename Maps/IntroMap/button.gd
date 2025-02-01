extends Button

func _on_pressed():
	$"../Enemy1".directTargetNode = $"../Player"
