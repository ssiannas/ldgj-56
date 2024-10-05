using System.Collections;
using System.Collections.Generic;
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

    // PlayerCharacter holds the currently selected character (eg cockroach, mouse, ghost, etc)
    // It's updated when a new character is selected in the start menu
    [SerializeField] private CharacterChoiceSO playerCharacter;


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

    private void SetPlayerCharacter(CharacterChoiceSO choice)
    {
        playerCharacter = choice;
        Debug.Log($"Selected new character: {choice.name}");
    }

    // Start is called before the first frame update
    void Start()
    {
        Managers.MenuManager.Instance
            .OnCharacterSelected.AddListener(SetPlayerCharacter);
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

    public void ExitGame()
    {
        Debug.Log("Exiting game");
        Application.Quit(0);
    }

    public void StartGame()
    {
        Debug.Log("Starting a new game");
        SceneManager.LoadScene("AlgorithmsPlayground");
    }

    public void PauseGame()
    {
        Debug.Log("Pausing game");
        Time.timeScale = 0;
        Managers.MenuManager.Instance.ShowPauseMenu();
    }

    public void ResumeGame()
    {
        Debug.Log("Resuming game");
        Time.timeScale = 1;
        Managers.MenuManager.Instance.HideMenu();
    }

    public void QuitToStartMenu()
    {
        Debug.Log("Quitting to start menu");
        // Time.timeScale = 1; // ensure timeScale is reset in case we're calling this on Pause
        SceneManager.LoadScene("Start Menu");
    }
}