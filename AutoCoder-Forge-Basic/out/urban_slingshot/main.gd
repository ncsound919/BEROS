extends Node2D

# Main script for Urban Slingshot Fury (Angry Birds-inspired game)

@onready var slingshot = $Slingshot
@onready var projectile_scene = preload("res://projectile.tscn")
@onready var target_scene = preload("res://target.tscn")

var current_projectile
var score = 0
var projectiles_left = 5
var targets_left = 10
var game_over = false

func _ready():
	randomize()
	spawn_targets()
	spawn_projectile()

func _input(event):
	if game_over:
		return

	if event is InputEventMouseButton and event.button_index == MOUSE_BUTTON_LEFT:
		if event.pressed:
			slingshot.start_aim(event.position)
		else:
			if current_projectile:
				launch_projectile(event.position)

func launch_projectile(release_pos):
	var force = slingshot.calculate_force(release_pos)
	current_projectile.apply_impulse(force)
	current_projectile.launched = true
	projectiles_left -= 1
	if projectiles_left > 0:
		call_deferred("spawn_projectile")
	else:
		check_game_over()

func spawn_projectile():
	current_projectile = projectile_scene.instantiate()
	current_projectile.position = slingshot.position
	add_child(current_projectile)
	slingshot.attach_projectile(current_projectile)

func spawn_targets():
	for i in range(targets_left):
		var target = target_scene.instantiate()
		target.position = Vector2(randf_range(200, 1000), randf_range(100, 500))
		add_child(target)
		target.connect("destroyed", Callable(self, "_on_target_destroyed"))

func _on_target_destroyed():
	targets_left -= 1
	score += 100
	update_ui()
	if targets_left <= 0:
		game_over = true
		print("You Win!")

func check_game_over():
	if projectiles_left <= 0 and targets_left > 0:
		game_over = true
		print("Game Over")

func update_ui():
	$ScoreLabel.text = "Score: " + str(score)
	$ProjectilesLabel.text = "Projectiles: " + str(projectiles_left)
	$TargetsLabel.text = "Targets: " + str(targets_left)