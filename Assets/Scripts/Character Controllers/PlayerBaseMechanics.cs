using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBaseMechanics : MonoBehaviour
{
    public uint score = 0;
    public uint highscore;
    public float timeModifier = 1.0f;
    private Transform player;
    private Vector2 lastKnownPosition;
    private bool isChasing = false;
    private CircleCollider2D detectionCollider;
    private ContactPoint collition;
    private float timer = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Stomper"))
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        if (score > highscore) highscore = score;


        // go to main menu or game over screen <3 also git gud noob
        GameObject.Destroy(this.gameObject);
    }


    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        score = (uint)(timer * timeModifier);
    }
}
