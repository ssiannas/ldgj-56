using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
	public float moveSpeed = 3f; 
	public LayerMask obstacleLayer;
	private Transform player; 
	private Vector2 lastKnownPosition;
	private bool isChasing = false;
	private CircleCollider2D detectionCollider; 

	void Start()
	{
			detectionCollider = GetComponent<CircleCollider2D>();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			player = collision.transform;
			ShootRayTowardsPlayer(); 		}
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			ShootRayTowardsPlayer();
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			player = null;					
		}
	}

	void Update()
	{
		if (isChasing)
		{
			MoveTowardsLastKnownPosition();
		}
	}

	void ShootRayTowardsPlayer()
	{
		if (player != null)
		{
			Vector2 directionToPlayer = (player.position - transform.position).normalized;
			RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, detectionCollider.radius, obstacleLayer);

			if (hit.collider != null && hit.collider.CompareTag("Player"))
			{
				lastKnownPosition = player.position;
				isChasing = true;			}
		}
	}

	void MoveTowardsLastKnownPosition()
	{
		Vector2 directionToTarget = (lastKnownPosition - (Vector2)transform.position).normalized;
		transform.position += (Vector3)(directionToTarget * moveSpeed * Time.deltaTime);

		if (Vector2.Distance(transform.position, lastKnownPosition) < 0.1f) // Adjust the threshold as needed
		{
			isChasing = false;
			// Maybe implement patrol system with range around LKP
		}
	}
}
