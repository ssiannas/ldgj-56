using System.Linq;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
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

	private void Awake()
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
