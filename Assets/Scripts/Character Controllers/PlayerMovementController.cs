using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerMovementController
{
	private readonly Rigidbody2D _rb;
	private Vector2 _direction;
	private readonly PlayerController _playerController;

	public PlayerMovementController(PlayerController playerController)
	{
		_playerController = playerController;
		_rb = _playerController.GetComponent<Rigidbody2D>();
	}


	public void OnFixedUpdate()
	{
		var speed = _playerController.speed;
		_rb.MovePosition(_rb.position + _direction * speed * Time.fixedDeltaTime);
	}

    public void OnUpdate()
    {
		_direction.x = Input.GetAxisRaw("Horizontal");
		_direction.y = Input.GetAxisRaw("Vertical");
		MaybeFlipSprite();
	}

	private void MaybeFlipSprite()
	{
		_playerController.GetComponent<SpriteRenderer>().flipX = _direction.x > 0;	
	}	
}
