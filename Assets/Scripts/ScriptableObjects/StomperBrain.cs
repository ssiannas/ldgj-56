using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.EventSystems.EventTrigger;

[CreateAssetMenu(fileName = "StomperBrain", menuName = "ScriptableObjects/EnemyAI/StomperBrain", order = 1)]
public class StomperBrain : EnemyBrain
{

	[SerializeField] public LayerMask obstacleLayer;
	[SerializeField] private float moveSpeed = 3.5f;

	private Vector2 lastKnownPosition = Vector2.zero;
	private static Int32 COLLISIONS_LAYER_MASK = 1 << 3;
	
	
	[SerializeField] private float stomperEyesightRange = 5.5f;

	private float _waitTime = 1f; // in seconds
	private float _waitCounter = 0f;
	private bool _waiting = false;


	private float _obstacleAvoidanceCooldown;
	private Vector2 _obstacleAvoidanceTargetDirection;
	private RaycastHit2D[] _obstacleCollisions = new RaycastHit2D[10];
	private Vector3 turn;
	private bool _playerInSight = false;

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



	private void HandleChase(EnemyController entity)
	{
		entity.UpdateStuckTimer();
		if (!DistanceToEnemyPosValid(entity.transform.position, lastKnownPosition, 1.3f * GetEyesightRange(), _playerInSight)
			|| entity.IsStuckInSamePosition())
		{
			StopChase(entity);
			return;
		}
		if (!entity.isMoving)
        {
			entity.isMoving = true;
        }

        entity.animator?.SetBool("isChasing", true);
		entity.pathFinding.MoveTowardsPlayer(moveSpeed);
	}

	private void HandleAlert(EnemyController entity) 
	{
		Patrol(entity);
	}

	private void HandleIdle(EnemyController entity) 
	{
		return;
	}

	private void ShootRayTowardsPlayer(EnemyController entity)
	{
		Transform entityTransform = entity.transform;
		_playerInSight = false;
		if (entity.playerTransform != null)
		{
			Vector2 directionToPlayer = (entity.playerTransform.position - entityTransform.position).normalized;
			LayerMask mask = LayerMask.GetMask("Player", "Collisions");
			RaycastHit2D hit = Physics2D.Raycast(entityTransform.position, directionToPlayer, GetEyesightRange(), mask);
			if (hit.collider != null && hit.collider.CompareTag("Player"))
			{
				lastKnownPosition = hit.collider.transform.position;
				_playerInSight = true;
				StateMoveToChase(entity);
			}
		}
	} 

	private void StateMoveToChase(EnemyController entity)
	{
		if (entity.state == EnemyController.State.CHASING) return;
		entity.state = EnemyController.State.CHASING;
		var witchClip = enemySounds.Find(s => s.soundName == "WitchChase");
		entity.pathFinding.StartFollow();
		entity.PlayAudio(witchClip,  UnityEngine.Random.Range(0, witchClip.clip.length));
	}

	// State handling
	private void StopChase(EnemyController entity)
	{
		StateMoveToAlert(entity);
		entity.animator?.SetBool("isChasing", false);
		entity.animator?.SetBool("isWalking", entity.isMoving);
		entity.pathFinding.StopFollow();
		entity.StopAudio(enemySounds.Find(s => s.soundName == "WitchChase"));
	}
	
	private void StateMoveToAlert(EnemyController entity)
	{
		entity.state = EnemyController.State.ALERT;

		entity.ResetPatrolPoints(lastKnownPosition, -1f, 1f);
		// Maybe init timer to go back to IDLE state
	}


	private void StateMoveToIdle(EnemyController entity)
	{
		entity.state = EnemyController.State.IDLE;
		entity.animator?.SetBool("isWalking", false);
		entity.isMoving = false;
	}

	private void Patrol(EnemyController entity)
    {

		Vector2[] waypoints = entity.patrolPoints.ToArray();
		int currentWaypointIndex = entity.currentWaypointIndex;

		Vector2 wp = waypoints[currentWaypointIndex];
		if (Vector2.Distance(entity.transform.position, wp) < 0.01f)
		{
			entity.transform.position = wp;
			_waitCounter = 0f;
			_waiting = true;

			entity.currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
		}
		else
		{
			Vector3 newDirection = (Vector3)((wp - (Vector2)entity.transform.position).normalized);
			entity.Move(newDirection, 0.5f * moveSpeed);
		}
	}
}

