using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EzaController : MonoBehaviour
{
    private RectTransform _rectTransform;
    [SerializeField] private float moveStep = 10;
    [SerializeField] private List<RectTransform> waypoints;
    private int _currentWaypointIx = 0;

    private RectTransform CurrentWaypoint => waypoints[_currentWaypointIx];

    // Start is called before the first frame update
    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _rectTransform.anchoredPosition = CurrentWaypoint.anchoredPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(CurrentWaypoint.anchoredPosition, _rectTransform.anchoredPosition) < Mathf.Epsilon)
        {
            _currentWaypointIx += 1;
            _currentWaypointIx %= waypoints.Count;
            var angle = Vector2.SignedAngle(Vector2.up,
                CurrentWaypoint.anchoredPosition - _rectTransform.anchoredPosition);
            var rotation = Quaternion.Euler(0, 0, angle);
            _rectTransform.rotation = rotation;
        }

        var newPos = Vector2.MoveTowards(
            _rectTransform.anchoredPosition,
            CurrentWaypoint.anchoredPosition,
            moveStep);
        _rectTransform.anchoredPosition = newPos;
    }
}