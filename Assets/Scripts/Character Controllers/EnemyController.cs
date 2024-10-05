using UnityEngine;

public class EnemyController : MonoBehaviour
{
	public enum State
	{
		IDLE,
		ALERT,
		CHASING,
		RUNNING,
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

	void Update()
	{
		brain.Think(this);
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
