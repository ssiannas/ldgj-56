using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "PestControlBrain", menuName = "ScriptableObjects/EnemyAI/PestControlBrain", order = 1)]
public class PestControlBrain : EnemyBrain
{
    [SerializeField] public LayerMask obstacleLayer;
    [SerializeField] private float moveSpeed = 4.8f;

    private Vector2 lastKnownPosition = Vector2.zero;
    private static Int32 COLLISIONS_LAYER_MASK = 1 << 3;


    [SerializeField] private float eyesightRange = 5.5f;
    [SerializeField] private float minSprayDistance = 2.5f;
    [SerializeField] private float sprayWaitTimeSec = 2f;


    private float _waitTime = 1f; // in seconds
    private float _waitCounter = 0f;
    private bool _waiting = false;
	private bool _playerInSight = false;

	[SerializeField] AudioChannel _audioChannel;

    public void OnEnable()
    {
        obstacleLayer = COLLISIONS_LAYER_MASK;
    }

    public override float GetEyesightRange()
    {
        return eyesightRange;
    }

    public override void Think(EnemyController entity)
    {
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
            case EnemyController.State.SPRAY_WARMUP:
                HandleSprayWarmup(entity);
                break;
            default:
                break;
        }
    }

    private void HandleSprayWarmup(EnemyController entity)
    {
        entity.Move(Vector3.zero, 0);
        entity.isMoving = false;
        entity.animator?.SetBool("isChasing", false);
    }

    private IEnumerator WarmupAndSpray(EnemyController entity)
    {
        entity.WarmupSpray();

        yield return new WaitForSeconds(sprayWaitTimeSec);

        entity.ShootSpray();
        MoveToStateChasing(entity, playAudio: false);
    }

    private void HandleChase(EnemyController entity)
    {
		if (entity.playerInRange)
        {
            ShootRayTowardsPlayer(entity);
        }
		entity.UpdateStuckTimer();
		if (!DistanceToEnemyPosValid(entity.transform.position, lastKnownPosition, GetEyesightRange(), _playerInSight)
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
        if (entity.playerInRange)
        {
            ShootRayTowardsPlayer(entity);
        }

        Patrol(entity);
    }

    private void HandleIdle(EnemyController entity)
    {
        if (entity.playerInRange)
        {
            ShootRayTowardsPlayer(entity);
        }

        return;
    }

    private void ShootRayTowardsPlayer(EnemyController entity)
    {
        Transform entityTransform = entity.transform;
		_playerInSight = false;
		if (entity.playerTransform != null)
        {
            Vector2 directionToPlayer = (entity.playerTransform.position - entityTransform.position).normalized;
            LayerMask mask = obstacleLayer | GetPlayerLayer();
            RaycastHit2D hit = Physics2D.Raycast(entityTransform.position, directionToPlayer, GetEyesightRange(),
                mask);

            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
				lastKnownPosition = hit.collider.transform.position;
				_playerInSight = true;
				var distanceToPlayer = Vector3.Distance(entity.playerTransform.position, entity.transform.position);
                if (distanceToPlayer < minSprayDistance)
                {

                    MoveToStateWarmup(entity);
                }
                else
                {
                    MoveToStateChasing(entity);
                }
            }
        }
    }
    private void MoveToStateChasing(EnemyController entity, bool playAudio = true)
    {
        if (entity.state == EnemyController.State.CHASING) return; 
		entity.state = EnemyController.State.CHASING;
        if (playAudio) _audioChannel.PlayAudio("Alarm");
		entity.pathFinding.StartFollow();
	}

	private void MoveToStateWarmup(EnemyController entity)
    {
        entity.state = EnemyController.State.SPRAY_WARMUP;
        entity.pathFinding.StopFollow();
        entity.StartCoroutine(WarmupAndSpray(entity));
    }

    private void MoveTowardsLastKnownPosition(EnemyController entity)
    {
        Transform entityTransform = entity.transform;
        Vector2 directionToTarget = (lastKnownPosition - (Vector2)entityTransform.position).normalized;
        entity.Move(directionToTarget, moveSpeed);

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
		entity.pathFinding.StopFollow();
		entity.animator?.SetBool("isChasing", false);
	}

	private void StateMoveToAlert(EnemyController entity)
    {
        entity.state = EnemyController.State.ALERT;
        entity.ResetPatrolPoints(lastKnownPosition, -1, 1);
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
            Vector3 newDirection =
                (Vector3)((wp - (Vector2)entity.transform.position).normalized);
            entity.Move(newDirection, moveSpeed * 0.5f);
        }
    }
}