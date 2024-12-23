class_name HealthComponent
extends Node

signal death

@export var health := 100
@export var max_health := 100


func take_damage(amount: int):
	health = clamp(health - amount, 0, max_health)
	if health == 0:
		death.emit()


func is_alive():
	return health >= 1


func hurt_vfx(anim_sprite: AnimatedSprite2D):
	var tween := create_tween()
	tween.tween_property(anim_sprite, "material:shader_parameter/amount", 1.0, 0.0)
	tween.tween_property(anim_sprite, "material:shader_parameter/amount", 0.0, 0.1).set_delay(0.1)
