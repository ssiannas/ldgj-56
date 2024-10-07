using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

public class TutorialMenu : MonoBehaviour
{
    [SerializeField] private GameObject ObjectiveSlide;
    [SerializeField] private GameObject PlayerSlide;
    [SerializeField] private GameObject EnemiesSlide;

    enum State
    {
        Objective,
        Player,
        Enemies
    }

    private State state;

    private void OnEnable()
    {
        state = State.Objective;
        HideAll();
        ObjectiveSlide.SetActive(true);
    }

    private void HideAll()
    {
        ObjectiveSlide.SetActive(false);
        PlayerSlide.SetActive(false);
        EnemiesSlide.SetActive(false);
    }

    public void PreviousClickHandle()
    {
        switch (state)
        {
            case State.Objective:
                state = State.Objective;
                UIManager.Instance.ShowStartMenu();
                break;
            case State.Player:
                state = State.Objective;
                HideAll();
                ObjectiveSlide.SetActive(true);
                break;
            case State.Enemies:
                state = State.Player;
                HideAll();
                PlayerSlide.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void NextClickHandle()
    {
        switch (state)
        {
            case State.Objective:
                state = State.Player;
                HideAll();
                PlayerSlide.SetActive(true);
                break;
            case State.Player:
                state = State.Enemies;
                HideAll();
                EnemiesSlide.SetActive(true);
                break;
            case State.Enemies:
                state = State.Objective;
                HideAll();
                UIManager.Instance.ShowStartMenu();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
