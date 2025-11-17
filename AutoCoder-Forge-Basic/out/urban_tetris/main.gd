extends Node2D

# Main script for Urban Block Stack (Tetris-inspired game)

@onready var grid = $Grid
@onready var piece_scene = preload("res://piece.tscn")
@onready var next_piece_display = $NextPiece

var current_piece
var next_piece
var score = 0
var level = 1
var lines_cleared = 0
var game_over = false

const GRID_WIDTH = 10
const GRID_HEIGHT = 20
const DROP_TIME = 1.0  # seconds

var drop_timer = 0.0

func _ready():
	randomize()
	spawn_new_piece()
	update_ui()

func _process(delta):
	if game_over:
		return

	drop_timer += delta
	if drop_timer >= DROP_TIME / level:
		drop_timer = 0.0
		if not move_piece(0, 1):
			lock_piece()
			check_lines()
			spawn_new_piece()
			if not is_valid_position(current_piece, current_piece.position):
				game_over = true
				print("Game Over")

func _input(event):
	if game_over:
		return

	if event.is_action_pressed("ui_left"):
		move_piece(-1, 0)
	elif event.is_action_pressed("ui_right"):
		move_piece(1, 0)
	elif event.is_action_pressed("ui_down"):
		if move_piece(0, 1):
			score += 1
		else:
			lock_piece()
			check_lines()
			spawn_new_piece()
	elif event.is_action_pressed("ui_accept"):  # Rotate
		rotate_piece()

func move_piece(dx, dy):
	var new_pos = current_piece.position + Vector2(dx * 32, dy * 32)
	if is_valid_position(current_piece, new_pos):
		current_piece.position = new_pos
		return true
	return false

func rotate_piece():
	var rotated_shape = rotate_shape(current_piece.shape)
	if is_valid_position_for_shape(rotated_shape, current_piece.position):
		current_piece.shape = rotated_shape
		current_piece.update_visual()
		return true
	return false

func is_valid_position(piece, pos):
	return is_valid_position_for_shape(piece.shape, pos)

func is_valid_position_for_shape(shape, pos):
	for i in range(shape.size()):
		for j in range(shape[i].size()):
			if shape[i][j] == 1:
				var x = pos.x / 32 + j
				var y = pos.y / 32 + i
				if x < 0 or x >= GRID_WIDTH or y >= GRID_HEIGHT or (y >= 0 and grid.is_occupied(int(x), int(y))):
					return false
	return true

func lock_piece():
	for i in range(current_piece.shape.size()):
		for j in range(current_piece.shape[i].size()):
			if current_piece.shape[i][j] == 1:
				var x = current_piece.position.x / 32 + j
				var y = current_piece.position.y / 32 + i
				grid.set_occupied(int(x), int(y), true)

func check_lines():
	var lines_to_clear = []
	for y in range(GRID_HEIGHT):
		if grid.is_line_full(y):
			lines_to_clear.append(y)

	if lines_to_clear.size() > 0:
		clear_lines(lines_to_clear)
		lines_cleared += lines_to_clear.size()
		score += lines_to_clear.size() * 100 * level
		level = lines_cleared / 10 + 1
		update_ui()

func clear_lines(lines):
	for y in lines:
		grid.clear_line(y)
		for yy in range(y, 0, -1):
			grid.copy_line(yy - 1, yy)

func spawn_new_piece():
	current_piece = next_piece
	if current_piece == null:
		current_piece = create_random_piece()
	current_piece.position = Vector2(GRID_WIDTH / 2 * 32, 0)
	add_child(current_piece)

	next_piece = create_random_piece()
	next_piece.position = Vector2(400, 100)  # Position for next piece display
	add_child(next_piece)

func create_random_piece():
	var piece = piece_scene.instantiate()
	piece.shape = get_random_shape()
	piece.update_visual()
	return piece

func get_random_shape():
	var shapes = [
		[[1,1,1,1]],  # I
		[[1,1],[1,1]],  # O
		[[0,1,0],[1,1,1]],  # T
		[[1,0,0],[1,1,1]],  # J
		[[0,0,1],[1,1,1]],  # L
		[[1,1,0],[0,1,1]],  # S
		[[0,1,1],[1,1,0]]   # Z
	]
	return shapes[randi() % shapes.size()]

func rotate_shape(shape):
	var new_shape = []
	for j in range(shape[0].size()):
		var row = []
		for i in range(shape.size() - 1, -1, -1):
			row.append(shape[i][j])
		new_shape.append(row)
	return new_shape

func update_ui():
	$ScoreLabel.text = "Score: " + str(score)
	$LevelLabel.text = "Level: " + str(level)
	$LinesLabel.text = "Lines: " + str(lines_cleared)