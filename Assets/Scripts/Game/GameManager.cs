using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Game;
using Network;
using UI;
using UnityEngine;
using Util.EventSystem;
using Util.SingletonSystem;
using EventType = Util.EventSystem.EventType;

public enum EGameState
{
	PreLogin,
	Lobby,
	MatchMaking,
	PreGame,
	InGame,
	PostGame,
}

public class GameManager : MonoBehaviourSingleton<GameManager>, IEventListener
{
	public EGameState State { get; private set; }
	public TileType PlayerTileType { get; private set; }
	public TileType turn;
	private CancellationTokenSource _matchTimeCountCancelToken;

	private void Start()
	{
		Application.runInBackground = true;
		State = EGameState.PreLogin;
		PlayerTileType = TileType.Null;
	}
	public async void StartMachMaking()
	{
		if (State != EGameState.Lobby)
			return;
		State = EGameState.MatchMaking;
		MatchMakingTimeCount();
		await NetworkManager.Instance.Send(new Message(EMessageType.MT_MATCHQ_JOIN, ""));
	}
	public void MatchFound(string matchInfo)
	{
		var arg = matchInfo.Split('|');
		var playerType = arg[0];
		var enemyName = arg[1];
		_matchTimeCountCancelToken?.Cancel();
		PlayerTileType = playerType switch
		{
			"O" => TileType.O,
			"X" => TileType.X,
			_ => PlayerTileType
		};
		UIManager.Instance.SetUI(EuiState.InGame);
		Debug.Log("My type : " + playerType + " | EnemyName : " + enemyName);
		State = EGameState.PreGame;
		StartCoroutine(GameStart());
	}
	IEnumerator GameStart()
	{
		turn = TileType.O;
		yield return new WaitForSeconds(3);
		EventManager.Instance.PostNotification(EventType.GameStart, this);
		State = EGameState.InGame;
	}
	private void MatchStop()
	{
		_matchTimeCountCancelToken?.Cancel();
		State = EGameState.Lobby;
	}
	public void GameOver(string arg)
	{
		State = EGameState.PostGame;
		switch (arg)
		{
			case "Enemy escaped" :
				UIManager.Instance.SetUI(EuiState.Result, true);
				break;
			case "O WIN" : 
				UIManager.Instance.SetUI(EuiState.Result, PlayerTileType == TileType.O);
				break;
			case "X WIN" : 
				UIManager.Instance.SetUI(EuiState.Result, PlayerTileType == TileType.X);
				break;
		}
	}
	private async void MatchMakingTimeCount()
	{
		Debug.Log("MatchMakingTimeCount");
		_matchTimeCountCancelToken?.Cancel();
		_matchTimeCountCancelToken = new CancellationTokenSource();
		var startTime = Time.time;
		try
		{
			while (true)
			{
				_matchTimeCountCancelToken.Token.ThrowIfCancellationRequested();
				await NetworkManager.Instance.Send(new Message(EMessageType.MT_ACTIVE_USER, ""));
				UIManager.Instance.SetMatchMakingText((int)(Time.time - startTime));
				await Task.Delay(1000);
			}
		}
		catch (OperationCanceledException)
		{
			UIManager.Instance.SetMatchMakingText(00);
		}
		finally
		{
			_matchTimeCountCancelToken.Dispose();
			_matchTimeCountCancelToken = null;
		}
	}
	public void OnEvent(EventType eventType, Component sender, object param = null)
	{
		switch (eventType)
		{
			case EventType.ProgramStart:
				break;
			case EventType.ServerConnection:
				if (param != null)
					ServerConnectionAction((EConnectResult)param);
				break;
			case EventType.GameStart:
				break;
			case EventType.TileClicked:
				break;
			case EventType.TileCommand:
				break;
			case EventType.BlockCommand:
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null);
		}
	}
	private void ServerConnectionAction(EConnectResult connectResult)
	{
		switch (connectResult)
		{
			case EConnectResult.Success:
				State = EGameState.Lobby;
				break;
			case EConnectResult.TimeOut:
				State = EGameState.PreLogin;
				break;
			case EConnectResult.Disconnect:
				State = EGameState.PreLogin;
				MatchStop();
				break;
			case EConnectResult.Error:
				State = EGameState.PreLogin;
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(connectResult), connectResult, null);
		}
	}
}