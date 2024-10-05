using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StomperBrain", menuName = "ScriptableObjects/EnemyAI/StomperBrain", order = 1)]
public class StomperBrain : EnemyBrain
{

	[SerializeField] private float eyesightRange = 5f;
	[SerializeField] public LayerMask obstacleLayer;
	[SerializeField] private float moveSpeed = 4f;

	private Vector2 lastKnownPosition = Vector2.zero;

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
			case EnemyController.State.CHASING:
				MoveTowardsLastKnownPosition(entity);
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
			RaycastHit2D hit = Physics2D.Raycast(entityTransform.position, directionToPlayer, eyesightRange, obstacleLayer);

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
		entityTransform.position += (Vector3)(directionToTarget * moveSpeed * Time.deltaTime);

		if (Vector2.Distance(entityTransform.position, lastKnownPosition) < 0.1f) // Adjust the threshold as needed
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
