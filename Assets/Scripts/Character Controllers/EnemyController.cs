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
    SpriteRenderer sr;
    private float proximityRewardTimer = 0;

    private int tauntCount = 0;

    public void WarmupSpray()
    {
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
        // Create an Ooze object in scene
        var ooze = Instantiate(OozePrefab);
        ooze.transform.position = targetIndicator.transform.position;
        Destroy(targetIndicator);
        OozeTargetLine.enabled = false;

        animator.SetBool("isSpraying", false);
        isMoving = true;
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
    public Sound scream { get; private set; }
    public EnemyAIFolow pathFinding {  get; private set; }
    private Sound fxPlaying;
    Rigidbody2D rigidbody;

	private static float MAX_STUCK_TIME_S = 3f; // in seconds
	private static float POS_STUCK_THRESHOLD_DIST = 0.005f;
	private Vector2 previousPosition;
	private float _stuckTimer = 0f;


    private void Start()
    {   
        enemyAudioEffects = gameObject.AddComponent<AudioSource>();
        pathFinding = gameObject.AddComponent<EnemyAIFolow>();
        rigidbody = GetComponent<Rigidbody2D>();
		getRangeCollider().radius = brain.GetEyesightRange();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        // HUGE HACK!!!
        if (brain.GetType() == typeof(GrannyBrain))
        {
            scream = ((GrannyBrain)brain).GetRandomScream();
        }
        previousPosition = rigidbody.position;
    }

    public void PlayAudio(Sound s, float startTime = 0)
    {
        if (s == null) return;
        fxPlaying = s;
        enemyAudioEffects.pitch = s.pitch;
        enemyAudioEffects.time = startTime;
        s.source = enemyAudioEffects;
        enemyAudioEffects.PlayOneShot(s.clip, s.volume);
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

    public void Move(Vector3 direction, float speed)
    {
        var newVelocity = Vector2.Lerp(rigidbody.velocity, direction * speed, 0.3f);
		rigidbody.velocity = newVelocity;
        MaybeFlipSprite(newVelocity);
        MaybeWalkAnimation(newVelocity);
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
        var sign = direction.x < 0 ? -1 : 1;
		transform.localScale = new Vector2(sign*Mathf.Abs(transform.localScale.x),
                                                          transform.localScale.y);
    }

    public void TriggerReaction()
    {
        if (!isActiveAndEnabled) return;
        GameController.Instance.AddScore(tauntReward);
        tauntCount += 1;
        brain.OnTaunt(this, tauntCount);
        StartCoroutine(ShowReaction());
    }

    public void UpdateStuckTimer()
    {
    	Vector2 currentPos = rigidbody.position;
		float dist = Vector2.Distance(currentPos, previousPosition);
        previousPosition = currentPos;
        if (dist < POS_STUCK_THRESHOLD_DIST)
        {
            _stuckTimer += Time.deltaTime;
        } else
        {
            _stuckTimer = 0f;
        }
    }
    
	public bool IsStuckInSamePosition() {
        return _stuckTimer >= MAX_STUCK_TIME_S;
    }

    public IEnumerator ShowReaction()
    {
        reaction.SetActive(true); // Set the object active

        yield return new WaitForSeconds(reactionDurationSec); // Wait for reactionDurationSec seconds

        reaction.SetActive(false); // Set the object inactive 
        if (transform.localScale == Vector3.zero )
        {
           gameObject.SetActive(false);
        }
    }
	 public void ResetPatrolPoints(Vector2 lastKnownPosition, float minDist, float maxDist)
	{
		Vector2 currCenter = transform.position;
		patrolPoints.Clear();
		if (IsPointValid(lastKnownPosition, currCenter, patrolPoints))
		{

			patrolPoints.Add(lastKnownPosition);
		}
		//Add two random points (i - 1)	
		for (int i = 0; i < 3; i++)
		{
			Vector2 newPt = GetRandomPoint(currCenter, 3);

			while (!IsPointValid(currCenter, newPt, patrolPoints))
			{
				newPt = GetRandomPoint(currCenter, 3);
			}
			patrolPoints.Add(newPt);
		}

		patrolPoints.OrderBy(p => Mathf.Atan2(p.y - currCenter.y, p.x - currCenter.x)).ToList();
	}

	private bool IsPointValid(Vector2 origin, Vector2 point, List<Vector2> previousPoints)
	{
		Vector2 direction = (point - origin).normalized;
		RaycastHit2D hit = Physics2D.Raycast(origin, direction, Vector2.Distance(point, origin), 1 << LayerMask.NameToLayer("Collisions"));
		if (hit.collider != null)
		{
			return false;
		}
		foreach (var p in previousPoints)
		{
			if (Vector2.Distance(p, point) < 1)
			{
				return false;
			}
		}

		return true;
	}
	private static Vector2 GetRandomPoint(Vector2 center, float radius)
	{
		return UnityEngine.Random.insideUnitCircle * radius + center;

	}
}