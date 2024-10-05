using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject enemy;
    [SerializeField]
    private EnemyBrain grannyBrain;
    [SerializeField]
    private EnemyBrain stomperBrain;
    [SerializeField]
    private float grannySwarmerInterval = 3.5f;
    [SerializeField]
    private float stomperSwarmerInterval = 10.0f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(spawnEnemy(grannySwarmerInterval, enemy, grannyBrain));
        StartCoroutine(spawnEnemy(stomperSwarmerInterval, enemy, stomperBrain));
    }

    // Update is called once per frame
    private IEnumerator spawnEnemy(float interval, GameObject enemy, EnemyBrain brain)
    {
        yield return new WaitForSeconds(interval);
        GameObject newEnemy = Instantiate(enemy, new Vector3(Random.Range(-5f, 5), Random.Range(-6f, 6), 0), Quaternion.identity);
        newEnemy.GetComponent<EnemyController>().Init(brain, EnemyController.State.IDLE);
        StartCoroutine(spawnEnemy(interval, newEnemy, brain));
    }
}