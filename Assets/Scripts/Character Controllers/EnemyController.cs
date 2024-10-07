using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public class EnemyController : MonoBehaviour
{
    public void Init(EnemyBrain _brain, State _state)
    {
        brain = _brain;
        state = _state;
    }

    [Header("Choleras Settings")]
    [SerializeField] private GameObject OozeTargetPrefab;
    [SerializeField] private LineRenderer OozeTargetLine;
    [SerializeField] private GameObject OozePrefab;
    private GameObject targetIndicator;
    [SerializeField] public UnityEvent OnWarmupSpray = new();
    [SerializeField] public UnityEvent OnShootSpray = new();

    [field: SerializeField] public GameObject reaction { get; private set; }
    [SerializeField] private float reactionDurationSec = 3f;

    [Header("Reward Settings")]
    [SerializeField] private uint tauntReward = 20;
    [SerializeField] private uint proximityRewardPerSec = 10;
    [SerializeField] private float proximityRewardDistance = 4f;
    private float proximityRewardTimer = 0;
    public void WarmupSpray()
    {
        Debug.Log("Ramping Up spray....");
        targetIndicator = Instantiate(OozeTargetPrefab);
        targetIndicator.transform.position = playerTransform.position;
        var startPos = Vector3.Lerp(transform.position, playerTransform.position, 0.5f);
        OozeTargetLine.enabled = true;
        OozeTargetLine.SetPosition(0, startPos);
        OozeTargetLine.SetPosition(1, playerTransform.position);
        animator.SetBool("isSpraying", true);
        OnWarmupSpray.Invoke();
    }

    public void ShootSpray()
    {
        Debug.Log("Spray!");

        // Create an Ooze object in scene
        var ooze = Instantiate(OozePrefab);
        ooze.transform.position = targetIndicator.transform.position;
        Destroy(targetIndicator);
        OozeTargetLine.enabled = false;

        animator.SetBool("isSpraying", false);
        OnShootSpray.Invoke();
    }

    public enum State
    {
        IDLE,
        ALERT,
        CHASING,
        FLEEING,
        SPRAY_WARMUP,
    }

    public Transform playerTransform { get; private set; } = null;

    public bool playerInRange { get; private set; } = false;

    [Header("AI Settings")]
    [SerializeField] EnemyBrain brain;
    public State state;
    [field: SerializeField] public float _rotationSpeed { get; private set; }
    [field: SerializeField] public float _obstacleCheckCircleRadius { get; private set; }
    [field: SerializeField] public float _obstacleCheckDistance { get; private set; }



    public Animator animator { get; private set; }
    public bool isMoving;

    public List<Vector2> patrolPoints { get; private set; } = new List<Vector2>();
    public int currentWaypointIndex;

    private AudioSource enemyAudioEffects;
    public Sound scream;
    private Sound fxPlaying;

    private void Start()
    {   
        enemyAudioEffects = gameObject.AddComponent<AudioSource>();
		getRangeCollider().radius = brain.GetEyesightRange();
        animator = GetComponent<Animator>();
        // HUGE HACK!!!
        if (brain.GetType() == typeof(GrannyBrain))
        {
            scream = ((GrannyBrain)brain).GetRandomScream();
        }
    }

    public void PlayAudio(Sound s, float startTime = 0)
    {
        if (s == null) return;
        fxPlaying = s;
        enemyAudioEffects.pitch = s.pitch;
        enemyAudioEffects.name = s.soundName;
        enemyAudioEffects.time = startTime;
        enemyAudioEffects.PlayOneShot(s.clip, s.volume);
        s.source = enemyAudioEffects;
    }

    public void StopAudio(Sound s)
    {
        if (s == null) return;
        if (fxPlaying == null) return;
        if (fxPlaying.soundName == s.soundName)
        {
            fxPlaying = null;
            enemyAudioEffects.Stop();
        }
    }

    public void PlayScream()
    {
        PlayAudio(scream);
    }

    void Update()
    {
        brain.Think(this);

        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                TriggerReaction();
            }
        }

        if (playerTransform)
        {
            TryProximityRewardPlayer();
        }
    }

    private void TryProximityRewardPlayer()
    {
        var distance = Vector3.Distance(transform.position, playerTransform.position);
        var hits = Physics2D.LinecastAll(transform.position,
            playerTransform.position,
            LayerMask.GetMask("Collisions"));
        var canSeePlayer = hits.All(hit => hit.collider.CompareTag("Player"));

        if (!(distance < proximityRewardDistance) || !canSeePlayer) return;

        proximityRewardTimer += Time.deltaTime;
        if (proximityRewardTimer < 1f) return;

        GameController.Instance.AddScore(proximityRewardPerSec);
        proximityRewardTimer = 0;
    }

    CircleCollider2D getRangeCollider()
    {
        return GetComponents<CircleCollider2D>().Where(c => c.isTrigger).First();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerTransform = collision.transform;
            playerInRange = true;
            proximityRewardTimer = 0;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    public void Move(Vector3 direction)
    {
        this.transform.position += direction;
        MaybeFlipSprite(direction);
        MaybeWalkAnimation(direction);
    }

    
    private void MaybeWalkAnimation(Vector3 direction)
    {
        animator.SetBool("isWalking", (direction != Vector3.zero));
    }

    private void MaybeFlipSprite(Vector3 direction)
    {
        if (direction == Vector3.zero)
        {
            return;
        }

        GetComponent<SpriteRenderer>().flipX = direction.x < 0;
    }

    public void TriggerReaction()
    {
        Debug.Log($"{gameObject.name} is angery!");
        StartCoroutine(ShowReaction());
        GameController.Instance.AddScore(tauntReward);
    }

    IEnumerator ShowReaction()
    {
        reaction.SetActive(true); // Set the object active

        yield return new WaitForSeconds(reactionDurationSec); // Wait for reactionDurationSec seconds

        reaction.SetActive(false); // Set the object inactive 
    }
   
}