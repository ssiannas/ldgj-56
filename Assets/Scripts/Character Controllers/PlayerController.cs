using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public float speed { get; private set; } = 5f;

    private PlayerMovementController movementController;

    [field: SerializeField] public PlayerPersistentState PersistentState { get; private set; }

    [SerializeField] private AudioChannel _channel;

    [field: SerializeField] public float TauntRadius { get; private set; } = 4f;

    private void Awake()
    {
        Debug.Log($"Selected Character is: {PersistentState.CharacterChoice.name}");
        var animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = PersistentState.CharacterChoice.animations;
    }

    public void Start()
    {
        movementController = new PlayerMovementController(this, _channel);
        GameController.Instance.OnGameOver += onGameOver;
    }

    public void Update()
    {
        movementController.OnUpdate();
    }

    private void onGameOver()
    {
        Destroy(this.gameObject);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Stomper"))
        {
            GameController.Instance.GameOver();
        }
    }
}