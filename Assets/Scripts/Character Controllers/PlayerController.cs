using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController: MonoBehaviour
{
    [SerializeField] public float speed
    {
         get; private set;
    } = 5f;

    private PlayerMovementController movementController;
  
    public void Awake()
    {
        movementController = new PlayerMovementController(this);
        GameControler.Instance.OnGameOver += onGameOver;
    }

    public void FixedUpdate()
    {
        movementController.OnFixedUpdate();
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
            GameControler.Instance.GameOver();
        }
    }
}
