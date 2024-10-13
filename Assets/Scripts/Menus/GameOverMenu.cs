using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameOverMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text highScoreText;

    public UnityEvent OnGameOverMenuLoaded;

	private void OnEnable()
	{
        OnGameOverMenuLoaded.Invoke();
	}

	public void SetScore(int score, int highScore, bool isNewHighScore)
    {
        scoreText.text = $"Score: {score}";
        if (isNewHighScore)
        {
            highScoreText.text = $"New High Score: {highScore}";
        }
        else
        {
            highScoreText.text = $"Previous High Score: {highScore}";
        }
    }

    public void StartMenuButtonHandle()
    {
        GameController.Instance.QuitToStartMenu();
    }
    
    public void PlayAgainButtonHandle()
    {
        GameController.Instance.StartGame();
    }
}