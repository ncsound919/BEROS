extends Node2D

# Piece script for Urban Block Stack

var shape = []
@onready var sprite = $Sprite2D

func _ready():
	update_visual()

func update_visual():
	# Clear previous visuals
	for child in get_children():
		if child != sprite:
			child.queue_free()

	# Draw the shape
	for i in range(shape.size()):
		for j in range(shape[i].size()):
			if shape[i][j] == 1:
				var block = ColorRect.new()
				block.rect_size = Vector2(32, 32)
				block.position = Vector2(j * 32, i * 32)
				block.color = Color(randf(), randf(), randf(), 1)  # Random color for urban theme
				add_child(block)