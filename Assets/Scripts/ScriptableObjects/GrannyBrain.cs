using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;
using static Unity.VisualScripting.Member;

[CreateAssetMenu(fileName = "GrannyBrain", menuName = "ScriptableObjects/EnemyAI/GrannyBrain", order = 1)]
public class GrannyBrain : EnemyBrain
{
    [SerializeField] public LayerMask obstacleLayer;
    [SerializeField] private float moveSpeed = 4f;

    private Vector2 lastKnownPosition = Vector2.zero;
    private static Int32 COLLISIONS_LAYER_MASK = 1 << 3;
    [SerializeField] private float grannyEyesightRange = 1.5f;

    [SerializeField] private AudioChannel _audioChannel;

    private System.Random _random = new System.Random();
    [SerializeField] private GameObject kidFaintPrefab;
    private int tauntCountToFaint = 5;

    public void OnEnable()
    {
        obstacleLayer = COLLISIONS_LAYER_MASK;
    }

    public override float GetEyesightRange()
    {
        return grannyEyesightRange;
    }

    public Sound GetRandomScream()
    {
        return enemySounds[_random.Next(0, enemySounds.Count - 2)];
    }

    public override void Think(EnemyController entity)
    {
        if (entity.playerInRange)
        {
            ShootRayTowardsPlayer(entity);
        }

        if (lastKnownPosition != Vector2.zero)
        {
            Debug.DrawLine(entity.transform.position, lastKnownPosition, Color.red);
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
            LayerMask mask = obstacleLayer | GetPlayerLayer();
            RaycastHit2D hit = Physics2D.Raycast(entityTransform.position, directionToPlayer, GetEyesightRange(), mask);

            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                lastKnownPosition = entity.playerTransform.position;
                MoveStateToFleeing(entity);
            }
        }
    }

    private void MoveStateToFleeing(EnemyController entity)
    {
        if (entity.state == EnemyController.State.FLEEING) return;
        entity.state = EnemyController.State.FLEEING;
        entity.animator?.SetBool("isFleeing", true);
        entity.PlayScream();
    }


    private void MoveAwayFromPlayer(EnemyController entity)
    {
        Transform entityTransform = entity.transform;
        Vector2 directionToTarget = (lastKnownPosition - (Vector2)entityTransform.position).normalized;
        entity.Move(-directionToTarget, moveSpeed);


        if (Vector2.Distance(entityTransform.position, lastKnownPosition) >
            2 * GetEyesightRange()) // Adjust the threshold as needed
        {
            StopFleeing(entity);
            // Maybe implement patrol system with range around LKP
        }
    }

    private void StopFleeing(EnemyController entity)
    {
        entity.state = EnemyController.State.IDLE;
        entity.animator?.SetBool("isFleeing", false);
    }

    public override void OnTaunt(EnemyController entity, int tauntCount)
    {
        if (tauntCount == tauntCountToFaint)
        {
            Vector3 pos = entity.transform.position;
            entity.PlayAudio(enemySounds.Find(s => s.soundName == "Faint"));
            var kid = Instantiate(kidFaintPrefab, pos, Quaternion.identity);
            var kidScale = kid.transform.localScale;
            kidScale.x = entity.transform.localScale.x < 0
                ? -Mathf.Abs(kid.transform.localScale.x)
                : Mathf.Abs(kid.transform.localScale.x);
            kid.transform.localScale = kidScale;
            PlayFaintOnKid(kid);
            entity.StopCoroutine(entity.ShowReaction());
            Destroy(entity.gameObject);
        }
    }

    public void PlayFaintOnKid(GameObject kid)
    {
        var source = kid.AddComponent<AudioSource>();
        var s = enemySounds.Find(s => s.soundName == "Faint");
        if (s is null) return;
        source.clip = s.clip;
        source.volume = s.volume;
        source.pitch = s.pitch;
        source.loop = s.loop;
        s.source = source;
        source.Play();
    }
}