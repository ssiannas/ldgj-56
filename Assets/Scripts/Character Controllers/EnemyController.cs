using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class EnemyController : MonoBehaviour
{
    public void Init(EnemyBrain _brain, State _state)
    {
        brain = _brain;
        state = _state;
    }

    [SerializeField] private GameObject OozeTargetPrefab;
    [SerializeField] private LineRenderer OozeTargetLine;
    [SerializeField] private GameObject OozePrefab;
    private GameObject targetIndicator;

    [SerializeField] public UnityEvent OnWarmupSpray = new();
    [SerializeField] public UnityEvent OnShootSpray = new();

    [field: SerializeField] public GameObject reaction { get; private set; }
    [SerializeField] private float reactionDurationSec = 3f;

    [SerializeField] private uint tauntReward = 20;
    
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

    [SerializeField] EnemyBrain brain;
    public State state;

    public Animator animator { get; private set; }
    public bool isMoving;

    public List<Vector2> patrolPoints { get; private set; } = new List<Vector2>();
    public int currentWaypointIndex;

    private void Start()
    {
        getRangeCollider().radius = brain.GetEyesightRange();
        animator = GetComponent<Animator>();
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