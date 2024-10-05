using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GrannyBrain", menuName = "ScriptableObjects/EnemyAI/GrannyBrain", order = 1)]
public class GrannyBrain : EnemyBrain
{
	[SerializeField] public LayerMask obstacleLayer;
	[SerializeField] private float moveSpeed = 4f;

	private Vector2 lastKnownPosition = Vector2.zero;
	private static Int32 COLLISIONS_LAYER_MASK = 1 << 3;
	[SerializeField] private float grannyEyesightRange = 2.0f;

	public void OnEnable()
	{
		obstacleLayer = COLLISIONS_LAYER_MASK;
	}

	public override float GetEyesightRange()
	{
		return grannyEyesightRange;
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
				break;
			case EnemyController.State.ALERT:
				break;
			case EnemyController.State.FLEEING:
				MoveAwayFromPlayer(entity);
				break;
			default:
				break;
		}
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
				entity.state = EnemyController.State.FLEEING;
			}
		}
	} 

	private void MoveAwayFromPlayer(EnemyController entity)
	{
		Transform entityTransform = entity.transform;
		Vector2 directionToTarget = (lastKnownPosition - (Vector2)entityTransform.position).normalized;
		entityTransform.position -= (Vector3)(directionToTarget * moveSpeed * Time.deltaTime);

		if (Vector2.Distance(entityTransform.position, lastKnownPosition) > 2*GetEyesightRange()) // Adjust the threshold as needed
		{
			StopChase(entity);
			// Maybe implement patrol system with range around LKP
		}
	}

	private void StopChase(EnemyController entity)
	{
		entity.state = EnemyController.State.ALERT;
		// Maybe init timer to go back to IDLE state
	}

}
