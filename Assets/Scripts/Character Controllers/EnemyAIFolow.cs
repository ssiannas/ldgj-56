using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager;
using UnityEngine;

public class EnemyAIFolow : MonoBehaviour
{
    public float nextWaypointDistance = 1.5f;
    public float linearDrag = 1f;
    Path path;
    int currentWaypoint = 0;

    float followThreshold = 5f;
    Seeker seeker;
    Rigidbody2D rb;
    Transform target = null;
    EnemyController enemyController;
    [field: SerializeField] public bool isFollowing { private set; get; } = false;

    private void Start()
	{
		seeker = GetComponent<Seeker>();
        if (seeker == null) return;
        rb = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        enemyController = GetComponent<EnemyController>();
        InvokeRepeating("UpdatePath", 0, 0.5f);
	}

	private void OnPathComplete(Path p)
    {
        if (p.error) { 
            Debug.LogError(p.error.ToString());    
            return;
        }
        path = p;
        currentWaypoint = 0;
    }

    public void StartFollow()
    {
        if (isFollowing) return;
        isFollowing = true;
    }

    public void StopFollow()
    {
        if (!isFollowing) return;
        isFollowing= false; 
    }

    private void UpdatePath()
    {
        if (!seeker.IsDone()) return;
        seeker.StartPath(rb.position, target.position, OnPathComplete); 
	}

	public void MoveTowardsPlayer(float speed)
    {
        if (path == null || !isFollowing) return;
        if (currentWaypoint >= path.vectorPath.Count)
        {
            return;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 movement = direction;
        enemyController.Move(movement, speed);
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }
}
