using System.Collections;
using System.Collections.Generic;
using Menus.Characters;
using UnityEngine;
using UnityEngine.Serialization;


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
}
