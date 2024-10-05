using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "StomperBrain", menuName = "ScriptableObjects/EnemyAI/StomperBrain", order = 1)]
public class StomperBrain : EnemyBrain
{

	[SerializeField] public LayerMask obstacleLayer;
	[SerializeField] private float moveSpeed = 4f;

	private Vector2 lastKnownPosition = Vector2.zero;
	private static Int32 COLLISIONS_LAYER_MASK = 1 << 3;
	[SerializeField] private float stomperEyesightRange = 5.5f;


	public void OnEnable()
	{
		obstacleLayer = COLLISIONS_LAYER_MASK;
	}

	public override float GetEyesightRange()
	{
		return stomperEyesightRange;
	}

	public override void Think(EnemyController entity)
	{
		if (entity.playerInRange)
		{
			ShootRayTowardsPlayer(entity);
		}

		switch (entity.state)
		{
			case EnemyController.State.IDLE:
				HandleIdle(entity);
				break;
			case EnemyController.State.ALERT:
				HandleAlert(entity);
				break;
			case EnemyController.State.CHASING:
				HandleChase(entity);
				break;
			default:
				break;
		}
	}

	private void HandleChase(EnemyController entity) {
        if (!entity.isMoving)
        {
			entity.isMoving = true;
        }
        entity.animator?.SetBool("isChasing", true);
		MoveTowardsLastKnownPosition(entity);
	}

	private void HandleAlert(EnemyController entity) 
	{
	}

	private void HandleIdle(EnemyController entity) 
	{
		return;
	}

	private void ShootRayTowardsPlayer(EnemyController entity)
	{
		Transform entityTransform = entity.transform;
		if (entity.playerTransform != null)
		{
			Vector2 directionToPlayer = (entity.playerTransform.position - entityTransform.position).normalized;
			RaycastHit2D hit = Physics2D.Raycast(entityTransform.position, directionToPlayer, GetEyesightRange(), obstacleLayer);

			if (hit.collider != null && hit.collider.CompareTag("Player"))
			{
				lastKnownPosition = entity.playerTransform.position;
				entity.state = EnemyController.State.CHASING;
			}
		}
	} 

	private void MoveTowardsLastKnownPosition(EnemyController entity)
	{
		Transform entityTransform = entity.transform;
		Vector2 directionToTarget = (lastKnownPosition - (Vector2)entityTransform.position).normalized;
		entity.Move((Vector3)(directionToTarget * moveSpeed * Time.deltaTime));

		if (Vector2.Distance(entityTransform.position, lastKnownPosition) < 0.1f) // Adjust the threshold as needed
		{
			StopChase(entity);
			// Maybe implement patrol system with range around LKP
		}
	}


	// State handling
	private void StopChase(EnemyController entity)
	{
		StateMoveToAlert(entity);
		entity.animator?.SetBool("isChasing", false);
		// Maybe init timer to go back to IDLE state
	}
	
	private void StateMoveToAlert(EnemyController entity)
	{
		entity.state = EnemyController.State.ALERT;
	}

	private void StateMoveToIdle(EnemyController entity)
	{
		entity.state = EnemyController.State.IDLE;
		entity.animator?.SetBool("isWalking", false);
		entity.isMoving = false;
	}
}
