using UnityEngine;
using TMPro;
using System;

public class PlaceHolderTextController : MonoBehaviour
{
    public LeaderBoardAPI leaderBoardAPI;
    // Start is called before the first frame update
    void OnEnable()
    {
        string text = leaderBoardAPI.LastUserName;
        if (string.IsNullOrEmpty(text))
        {
            text = "Enter username";
        }
        GetComponent<TextMeshProUGUI>().text = leaderBoardAPI.LastUserName; 
    }
}
