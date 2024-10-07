using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerMovementController
{
    private readonly Rigidbody2D _rb;
    private Vector2 _direction;
    private readonly PlayerController _playerController;
    private readonly Animator _animator;
    public bool isTaunting;
    public float tauntingTimer;

    private static float TAUNTING_DURATION_S = 0.5f;
    private AudioChannel _audioChannel;

    public PlayerMovementController(PlayerController playerController, AudioChannel ac)
    {
        _playerController = playerController;
        _rb = _playerController.GetComponent<Rigidbody2D>();
        _animator = _playerController.GetComponent<Animator>();
        _audioChannel = ac;
    }

    private void MaybeTaunt()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isTaunting)
        {
            Taunt();
        }
        else if (isTaunting)
        {
            tauntingTimer -= Time.deltaTime;
            if (tauntingTimer <= 0.0)
            {
                StopTaunt();
            }
        }
    }

    private void MaybeWalk()
    {
        _animator.SetBool("isWalking", _direction != Vector2.zero && !isTaunting);
    }

    public void OnUpdate()
    {
        _direction.x = Input.GetAxisRaw("Horizontal");
        _direction.y = Input.GetAxisRaw("Vertical");

        MaybeTaunt();
        MaybeWalk();
        MaybeFlipSprite();

        var speed = isTaunting ? 0 : _playerController.speed;
        _rb.MovePosition(_rb.position + _direction.normalized * speed * Time.fixedDeltaTime);
    }

    private void Taunt()
    {
        isTaunting = true;
        tauntingTimer = TAUNTING_DURATION_S;
        MaybePlayTauntAnimation();

        // Taunt enemies
        LayerMask mask = LayerMask.GetMask("Enemies"); //LayerMask.NameToLayer("Enemies"));
        var enemiesHit = Physics2D.OverlapCircleAll(_playerController.transform.position, _playerController.TauntRadius,
            mask);
        foreach (var enemy in enemiesHit)
        {
            CheckEnemyTaunt(enemy);
        }
    }

    private void CheckEnemyTaunt(Collider2D enemy)
    {
        var hits = Physics2D.LinecastAll(
            _playerController.transform.position,
            enemy.transform.position,
            LayerMask.GetMask("Collisions"));
        var shouldTaunt =
            hits.All(hit => hit.collider.CompareTag("Player")) || hits.Length == 0; // only collision is with player.
        if (!shouldTaunt)
        {
            return;
        }

        enemy.GetComponent<EnemyController>().TriggerReaction();
    }


    private void StopTaunt()
    {
        isTaunting = false;
        MaybePlayTauntAnimation();
    }

    private void MaybePlayTauntAnimation()
    {
        _animator.SetBool("isTaunting", isTaunting);
    }

    private void MaybeFlipSprite()
    {
        if (_direction == Vector2.zero || isTaunting)
        {
            return;
        }

        _playerController.GetComponent<SpriteRenderer>().flipX = _direction.x > 0;
    }
}