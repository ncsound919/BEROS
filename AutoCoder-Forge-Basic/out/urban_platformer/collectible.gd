extends Area2D

# Collectible script - items to collect for points

@export var points = 10

func _ready():
	connect("body_entered", Callable(self, "_on_body_entered"))

func _on_body_entered(body):
	if body.name == "Player":
		# Add score
		get_parent().add_score(points)
		# Play sound or animation if needed
		queue_free()