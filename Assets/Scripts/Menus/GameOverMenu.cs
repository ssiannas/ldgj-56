using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text highScoreText;

    public void SetScore(int score, int highScore, bool isNewHighScore)
    {
        scoreText.text = $"Score: {score}";
        highScoreText.text = $"New High Score: {highScore}";
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