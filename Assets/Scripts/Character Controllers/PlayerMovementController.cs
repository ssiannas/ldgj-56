using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerMovementController
{
	private readonly Rigidbody2D _rb;
	private Vector2 _direction;
	private readonly PlayerController _playerController;
	private readonly Animator _animator;
	public bool isTaunting;
	public float tauntingTimer;

	private static float TAUNTING_DURATION_S = 0.5f;

	public PlayerMovementController(PlayerController playerController)
	{
		_playerController = playerController;
		_rb = _playerController.GetComponent<Rigidbody2D>();
		_animator = _playerController.GetComponent<Animator>();
	}

	private void MaybeTaunt()
	{
		if (Input.GetKeyDown(KeyCode.Space) && !isTaunting)
		{
			Taunt();
		}
		else if (isTaunting)
		{
			tauntingTimer -= Time.deltaTime;
			if (tauntingTimer <= 0.0)
			{
				StopTaunt();
			}
		}
	} 
	private void MaybeWalk()
	{
		_animator.SetBool("isWalking", _direction != Vector2.zero && !isTaunting);
	}

    public void OnUpdate()
    {
		_direction.x = Input.GetAxisRaw("Horizontal");
		_direction.y = Input.GetAxisRaw("Vertical");

		MaybeTaunt();
		MaybeWalk();
		MaybeFlipSprite();

		var speed = isTaunting ? 0 : _playerController.speed;
		_rb.MovePosition(_rb.position + _direction * speed * Time.fixedDeltaTime);
	}

	private void Taunt()
	{
		isTaunting = true;
		tauntingTimer = TAUNTING_DURATION_S;
		MaybePlayTauntAnimation();
	}


	private void StopTaunt()
	{
		isTaunting = false;
		MaybePlayTauntAnimation();
	}

	private void MaybePlayTauntAnimation()
	{
		_animator.SetBool("isTaunting", isTaunting);
	}

	private void MaybeFlipSprite()
	{
		if (_direction == Vector2.zero || isTaunting)
		{
			return;
		}

		_playerController.GetComponent<SpriteRenderer>().flipX = _direction.x > 0;	
	}	
}
