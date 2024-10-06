using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using Menus;
using Menus.Characters;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;


//[RequireComponent(typeof(InterestMeterController))]

public class GameController : MonoBehaviour
{
    //scene loading
    //score counting
    //
    public uint score = 0;
    public uint highscore;
    private float _timer = 0.0f;
    public float timeModifier = 1.0f;

    public static GameController Instance { get; private set; }
    [SerializeField] private AudioChannel _channel;

    [field: SerializeField] public PlayerPersistentState PlayerPersistence { get; private set; }

    //*******************
    public delegate void GameOverDelegate();

    public event GameOverDelegate OnGameOver;

    public bool isGameOver = false;

    private enum State
    {
        Paused,
        Playing
    }

    private State _state = State.Playing;
    private static string HOUSE_SCENE = "House 1 Colliders";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }

        Instance = this;
        DontDestroyOnLoad(this);

		SceneManager.sceneLoaded += OnHouseSceneLoad;
	}


    // Start is called before the first frame update
    void Start()
    {
    }

    public void GameOver()
    {
        if (score > highscore) highscore = score;
        isGameOver = true;
        OnGameOver?.Invoke();
    }


    // Update is called once per frame
    void Update()
    {
        if (isGameOver)
        {
            return;
        }

        _timer += Time.deltaTime;
        score = (uint)(_timer * timeModifier);
        UIManager.Instance.UpdateScore(score);
        TryTogglePauseMenu();
    }

    private void TryTogglePauseMenu()
    {
        if (SceneManager.GetActiveScene().name == "Start Menu") return;

        // Use ESC or START keys for toggling pause menu
        if (Input.GetKeyDown(KeyCode.Escape)
            || Input.GetKeyDown(KeyCode.Joystick1Button7))
        {
            switch (_state)
            {
                case State.Paused:
                    ResumeGame();
                    break;
                case State.Playing:
                    PauseGame();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public void ExitGame()
    {
        Debug.Log("Exiting game");
        Application.Quit(0);
    }

    public void StartGame()
    {
        Debug.Log("Starting a new game");
        Time.timeScale = 1;
        SceneManager.LoadScene(HOUSE_SCENE);
    }

    private void OnHouseSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != HOUSE_SCENE) return;
		if (_channel.IsAudioPlaying("MainMenuTheme"))
        {
            _channel.StopAudio("MainMenuTheme");
        }
        _channel.PlayAudio("Theme");
    }

    public void PauseGame()
    {
        Debug.Log("Pausing game");
        Time.timeScale = 0;
        _state = State.Paused;
        UIManager.Instance.ShowPauseMenu();
    }

    public void ResumeGame()
    {
        Debug.Log("Resuming game");
        Time.timeScale = 1;
        _state = State.Playing;
        UIManager.Instance.HideMenu();
    }

    public void QuitToStartMenu()
    {
        Debug.Log("Quitting to start menu");
        Time.timeScale = 1; // ensure timeScale is reset in case we're calling this on Pause
        _state = State.Playing;
        SceneManager.LoadScene("Start Menu");
    }
}