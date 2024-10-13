using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Dan.Main;
using System;

public class LeaderBoardController : MonoBehaviour
{
    [field: SerializeField] private List<TextMeshProUGUI> _leaderBoardScores;
    [field: SerializeField] private List<TextMeshProUGUI> _leaderBoardNames;
	private TextMeshProUGUI _scoreText;

	public static LeaderBoardController Instance { get;  private set; }

    private string lbPubKey = "282e85d240147a3ac27cc2f1019ac78ec889ab2d215a67475b28b4657e4b540e";
	public string lastUserName = null;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(this);
		}
		Instance = this;
		DontDestroyOnLoad(gameObject);
		
		foreach (Transform child in transform)
		{
			TextMeshProUGUI textMeshPro = child.GetComponent<TextMeshProUGUI>();
			if (textMeshPro != null)
			{
				if (child.CompareTag("LeaderBoardScore"))
				{
					_leaderBoardScores.Add(textMeshPro);
				}
				else if (child.CompareTag("LeaderBoardName"))
				{
					_leaderBoardNames.Add(textMeshPro);
				}
			}
		}
		_scoreText = GameObject.FindGameObjectWithTag("ScoreText").GetComponent<TextMeshProUGUI>();
		LeaderboardCreator.GetPersonalEntry(lbPubKey, (entry) => lastUserName = entry.Username);
	}

	public void GetLeaderBoard()
    {
        LeaderboardCreator.GetLeaderboard(lbPubKey, OnLeaderBoardLoaded);
    }

    public void UploadLeaderBoardEntry(TextMeshProUGUI playerName)
    {
		uint score = GameController.Instance.score;
		lastUserName = playerName.text;
		LeaderboardCreator.UploadNewEntry(lbPubKey, playerName.text, (int)score, (_) => GetLeaderBoard());
    }

    private void OnLeaderBoardLoaded(Dan.Models.Entry[] entry)
    {
        for (int i = 0; i <  entry.Length; ++i)
        {
            _leaderBoardNames[i].text = entry[i].Username;
            _leaderBoardScores[i].text = entry[i].Score.ToString();
        }
    }
     
	public string GetUserName()
	{
		return lastUserName;
	}
}
