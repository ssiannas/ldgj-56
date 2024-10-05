using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
	public List<Vector2> patrolPoints { get; private set; } = new List<Vector2>();
	public int currentWaypointIndex ; 

	private void Start()
	{
		getRangeCollider().radius = brain.GetEyesightRange(); 
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

}
