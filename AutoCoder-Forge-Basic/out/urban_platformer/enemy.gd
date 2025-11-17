extends CharacterBody2D

# Enemy script - basic AI for enemies

@export var speed = 100.0
@export var direction = 1  # 1 for right, -1 for left

@onready var player = get_parent().get_node("Player")

func _physics_process(delta):
	# Simple patrol or chase logic
	if player:
		# Chase player
		var direction_to_player = (player.position - position).normalized()
		velocity.x = direction_to_player.x * speed
	else:
		# Patrol
		velocity.x = direction * speed
		if is_on_wall():
			direction *= -1

	# Gravity
	if not is_on_floor():
		velocity.y += ProjectSettings.get_setting("physics/2d/default_gravity") * delta

	move_and_slide()

func _on_body_entered(body):
	if body.name == "Player":
		# Damage player
		body.get_parent().lose_life()
		queue_free()