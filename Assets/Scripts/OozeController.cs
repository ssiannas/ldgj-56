using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OozeController : MonoBehaviour
{
    [SerializeField] private float lifetimeSec = 1.5f;

    private void Awake()
    {
        Destroy(gameObject, lifetimeSec);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameController.Instance.GameOver();
        }
    }
}