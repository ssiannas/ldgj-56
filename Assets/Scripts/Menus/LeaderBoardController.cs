using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;

public class LeaderBoardController : MonoBehaviour
{
	public LeaderBoardAPI leaderBoardAPI;

	[SerializeField] 
	private TextMeshProUGUI ScoresPrefab;
	[SerializeField]
	private TextMeshProUGUI NamesPrefab;

	private GameObject _namesHolder;
	private GameObject _scoresHolder;

	private void Awake()
	{
		_scoresHolder = GameObject.FindWithTag("LeaderBoardScore");
		_namesHolder = GameObject.FindWithTag("LeaderBoardName");
		MaybeFetchEntries();
	}

	private void MaybeFetchEntries()
	{
		var currentEntries = leaderBoardAPI.GetCurrentEntries();
		if (currentEntries.Length == 0) {
			leaderBoardAPI.FetchLeaderBoard(OnLeaderBoardLoaded);
		} else
		{
			OnLeaderBoardLoaded(currentEntries);
		}

	}

    public void UploadLeaderBoardEntry(TextMeshProUGUI playerName)
    {
		uint score = GameController.Instance.score;
		leaderBoardAPI.UploadLeaderBoardEntry(playerName.text, (int)score, OnLeaderBoardLoaded);
    }

	private void EmptyBoard()
	{
		foreach (Transform child in _namesHolder.transform)
		{
			Destroy(child.gameObject);
		}
		foreach (Transform child in _scoresHolder.transform)
		{
			Destroy(child.gameObject);
		}
	}

    private void OnLeaderBoardLoaded(Dan.Models.Entry[] entry)
    {
		EmptyBoard();
        for (int i = 0; i < entry.Length; ++i)
        {
			var newName = Instantiate(NamesPrefab, _namesHolder.transform);
			newName.GetComponent<TextMeshProUGUI>().text = entry[i].Username;
			var newScore = Instantiate(ScoresPrefab, _scoresHolder.transform);
			newScore.GetComponent<TextMeshProUGUI>().text = entry[i].Score.ToString();
		}
    }
}
