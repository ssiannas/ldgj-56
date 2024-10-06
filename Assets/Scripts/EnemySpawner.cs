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
    private GameObject[] spawnPoints;

    [SerializeField] private float INITIAL_SPAWNRATE = 2f;
    public float MIN_SPAWNRATE = 0.1f;
    public float difficultyScale = 0.01f;
    private float currentSpawnRate;
    private float roundStart = 0;

    // Start is called before the first frame update
    void Start()
    {
        currentSpawnRate = INITIAL_SPAWNRATE;
        roundStart = Time.time;
        StartCoroutine(spawnEnemy(currentSpawnRate, enemy, enemyBrain, GetRandomSpawnPoit()));
    }

    // Update is called once per frame
    private void Update()
    {
        currentSpawnRate = INITIAL_SPAWNRATE - ((Time.time - roundStart) * difficultyScale);
        currentSpawnRate = Mathf.Max(currentSpawnRate, MIN_SPAWNRATE);
    }

    private GameObject GetRandomSpawnPoit()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }

    private IEnumerator spawnEnemy(float interval, GameObject enemy, EnemyBrain brain, GameObject spawnPoint)
    {
        yield return new WaitForSeconds(interval);
        GameObject newEnemy = Instantiate(enemy, spawnPoint.transform.position, Quaternion.identity);
        newEnemy.GetComponent<EnemyController>().Init(brain, EnemyController.State.IDLE);
        StartCoroutine(spawnEnemy(interval, newEnemy, brain, GetRandomSpawnPoit()));
    }
}