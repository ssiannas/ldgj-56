using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerMovementController
{
	private readonly Rigidbody2D _rb;
	private Vector2 _direction;
	private readonly PlayerController _playerController;
	private readonly Animator _animator;

	public PlayerMovementController(PlayerController playerController)
	{
		_playerController = playerController;
		_rb = _playerController.GetComponent<Rigidbody2D>();
		_animator = _playerController.GetComponent<Animator>();
	}


	public void OnFixedUpdate()
	{
		var speed = _playerController.speed;
		MaybeWalk();
		_rb.MovePosition(_rb.position + _direction * speed * Time.fixedDeltaTime);
	}

	private void MaybeWalk()
	{
		if (_direction != Vector2.zero)
		{
			_animator.SetBool("isWalking", true);
		} else
		{
			_animator.SetBool("isWalking", false);
		}
	}

    public void OnUpdate()
    {
		_direction.x = Input.GetAxisRaw("Horizontal");
		_direction.y = Input.GetAxisRaw("Vertical");
		MaybeFlipSprite();
	}

	private void MaybeFlipSprite()
	{
		if (_direction == Vector2.zero)
		{
			return;
		}

		_playerController.GetComponent<SpriteRenderer>().flipX = _direction.x > 0;	
	}	
}
