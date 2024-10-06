using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCamera2D : MonoBehaviour
{

	public float dampTime = 0.15f;
	private Vector3 velocity = Vector3.zero;
	public GameObject target;

	private void Start()
	{
		target = GameObject.FindGameObjectWithTag("Player");
		FollowTqarget(target, false);	
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		if (target)
		{
			FollowTqarget(target);
		}

	}

	private void FollowTqarget(GameObject target, bool smooth = true)
	{
		Vector3 point = GetComponent<Camera>().WorldToViewportPoint(target.transform.position);
		Vector3 delta = target.transform.position - GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)); //(new Vector3(0.5, 0.5, point.z));
		Vector3 destination = transform.position + delta;
		if (smooth)
		{
			transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
		}
		else
		{
			transform.position = destination;
		}
	}
}
