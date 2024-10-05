using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[RequireComponent(typeof(InterestMeterController))]

public class GameControler : MonoBehaviour
{
    //scene loading
    //score counting
    //
    public uint score = 0;
    public uint highscore;
    private float _timer = 0.0f;
    public float timeModifier = 1.0f;

    public static GameControler Instance { get; private set; }


    //*******************
    public delegate void GameOverDelegate();
    public event GameOverDelegate OnGameOver;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //go to main menu
    }

    public void GameOver()
    {
        if (score > highscore) highscore = score;
        OnGameOver?.Invoke();
    }


    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;
        score = (uint)(_timer * timeModifier);
    }
}
