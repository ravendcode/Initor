extends Node


func hide_actor(actor: Node, has_collision: bool = true) -> void:
	actor.hide()
	if has_collision:
		actor.get_node("CollisionShape2D").set_deferred("disabled", true)
