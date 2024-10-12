using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

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
		if (Vector2.Distance(lastKnownPosition, entity.transform.position) <= 0.15f)
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
		if (entity.playerTransform != null)
		{
			Vector2 directionToPlayer = (entity.playerTransform.position - entityTransform.position).normalized;
			LayerMask mask = LayerMask.GetMask("Player", "Collisions");
			var normalizedRange = GetEyesightRange();
			RaycastHit2D hit = Physics2D.Raycast(entityTransform.position, directionToPlayer, normalizedRange, mask);
			if (hit.collider != null && hit.collider.CompareTag("Player"))
			{
				lastKnownPosition = hit.collider.transform.position;
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
		entity.pathFinding.StopFollow();
		entity.StopAudio(enemySounds.Find(s => s.soundName == "WitchChase"));
	}
	
	private void StateMoveToAlert(EnemyController entity)
	{
		entity.state = EnemyController.State.ALERT;
		ResetPatrolPoints(entity, -1, 1);
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


	private void ResetPatrolPoints(EnemyController entity, float minDist, float maxDist)
    {
		Vector2 currCenter = entity.transform.position;
		entity.patrolPoints.Clear();
		entity.patrolPoints.Add(lastKnownPosition);
		//Add two random points (i - 1)	
		for (int i = 0; i < 3; i++)
		{
			Vector2 newPt = GetRandomPoint(currCenter, 3);

			while (!IsPointValid(currCenter, newPt))
			{

				newPt = GetRandomPoint(currCenter, 3);
 
			}
			entity.patrolPoints.Add(newPt);
		}
		
	}

	private bool IsPointValid(Vector2 origin, Vector2 point)
    {
		Vector2 direction = (point - origin).normalized;
		RaycastHit2D hit = Physics2D.Raycast(origin, direction, Vector2.Distance(point, origin), obstacleLayer);
		if (hit.collider != null) { return false; }
		return true;
    }

	private static Vector2 GetRandomPoint(Vector2 center,float radius)
    {
		return UnityEngine.Random.insideUnitCircle * radius + center;

	}
}

