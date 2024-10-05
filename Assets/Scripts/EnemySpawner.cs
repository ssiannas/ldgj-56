using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject enemy;
    [SerializeField]
    private EnemyBrain enemyBrain;
    [SerializeField]
    private float enemySwarmerInterval = 3.5f;

    public static float INITIAL_SPAWNRATE = 2f;
    public float MIN_SPAWNRATE = 0.1f;
    public float difficultyScale = 0.01f;
    private float currentSpawnRate = INITIAL_SPAWNRATE;
    private float roundStart = 0;

    // Start is called before the first frame update
    void Start()
    {
        roundStart = Time.time;
        StartCoroutine(spawnEnemy(currentSpawnRate, enemy, enemyBrain));
    }

    // Update is called once per frame
    private void Update()
    {
        currentSpawnRate = INITIAL_SPAWNRATE - ((Time.time - roundStart) * difficultyScale);
        currentSpawnRate = Mathf.Max(currentSpawnRate, MIN_SPAWNRATE);
    }

    private IEnumerator spawnEnemy(float interval, GameObject enemy, EnemyBrain brain)
    {
        yield return new WaitForSeconds(interval);
        GameObject newEnemy = Instantiate(enemy, new Vector3(Random.Range(-5f, 5), Random.Range(-6f, 6), 0), Quaternion.identity);
        newEnemy.GetComponent<EnemyController>().Init(brain, EnemyController.State.IDLE);
        StartCoroutine(spawnEnemy(interval, newEnemy, brain));
    }
}