using System.Linq;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
	public void Init(EnemyBrain _brain, State _state)
    {
		brain = _brain;
		state = _state;
    }
	
	public enum State
	{
		IDLE,
		ALERT,
		CHASING,
		FLEEING,
	}

	public Transform playerTransform
	{
		get; private set;
	} = null;

	public bool playerInRange
	{
		get; private set;
	} = false;

	[SerializeField] EnemyBrain brain;
	public State state;
	public Animator animator { get; private set; }
	public bool isMoving;

	private void Start()
	{
		getRangeCollider().radius = brain.GetEyesightRange(); 
		animator = GetComponent<Animator>();
	}

	void Update()
	{
		brain.Think(this);
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
	}

	private void MaybeFlipSprite(Vector3 direction)
	{ 
		if (direction == Vector3.zero)
		{
			return;
		}

		GetComponent<SpriteRenderer>().flipX = direction.x < 0;	
	}

}
