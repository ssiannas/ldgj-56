using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController: MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    private Vector2 direction = Vector2.zero;

    Rigidbody2D rb;

    private void Awake()
    {

        GameControler.Instance.OnGameOver += onGameOver;
    }
    // Start is called before the first frame update
    void Start()
    {
       rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
		direction.x = Input.GetAxisRaw("Horizontal");
		direction.y = Input.GetAxisRaw("Vertical");
	}

	private void FixedUpdate()
	{
		rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
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
