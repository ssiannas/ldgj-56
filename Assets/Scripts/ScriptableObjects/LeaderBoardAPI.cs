using UnityEngine;
using Dan.Main;
using Dan.Models;
using System;

[CreateAssetMenu(fileName = "LeaderBoardAPI", menuName = "ScriptableObjects/LeaderBoardAPI", order = 1)]
public class LeaderBoardAPI : ScriptableObject
{
	private string lbPubKey = "282e85d240147a3ac27cc2f1019ac78ec889ab2d215a67475b28b4657e4b540e";

	private Entry[] entries = null;
	private string lastUserName = null;
	public string LastUserName { 
		private set => lastUserName = value; 
		get => lastUserName != null ? lastUserName : "";
	}
	[field: SerializeField]
	public bool LeaderBoardFailed { get; private set; } = false;
	public void FetchLeaderBoard()
	{
		LeaderboardCreator.GetLeaderboard(lbPubKey, (_entries) => {
			entries = _entries;
			UpdatePlayerName();
			LeaderBoardFailed = false;
		}, (_) => {
			LeaderBoardFailed = true;
			Debug.Log("Failed to fetch!");
		});
	}

	public void FetchLeaderBoard(Action<Entry[]> Callback, Action<string> ErrorCallback)
	{
		if (LeaderBoardFailed) {
			ErrorCallback("");
			return;
		}
		LeaderboardCreator.GetLeaderboard(lbPubKey, (_entries) => {
			entries = _entries;
			Callback(entries);
			UpdatePlayerName();
			LeaderBoardFailed = false;
		}, ErrorCallback); 
    }

	private void UpdatePlayerName()
	{
		if (LeaderBoardFailed) return;
		LeaderboardCreator.GetPersonalEntry(lbPubKey, (entry) =>
		{
			LastUserName = entry.Username;
		});
	}

	public Entry[] GetCurrentEntries()
	{
		if (entries == null) { return new Entry[0]; }
		return entries;
	}

    public void UploadLeaderBoardEntry(string playerName, int playerScore, Action<Entry[]> Callback, Action<string> ErrorCallback = null)
    {
		if (LeaderBoardFailed)
		{
			ErrorCallback?.Invoke("");
			return;
		}
		if (!string.IsNullOrWhiteSpace(playerName))
		{
			LastUserName = playerName;
		}

		LeaderboardCreator.UploadNewEntry(lbPubKey, LastUserName, playerScore, (_) => FetchLeaderBoard(Callback, ErrorCallback));
    }
}
