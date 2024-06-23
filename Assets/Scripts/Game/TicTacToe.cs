using System;
using System.Collections.Generic;
using UI;
using UnityEngine;
using Util.EventSystem;
using EventType = Util.EventSystem.EventType;

namespace Game
{
	public class TicTacToe : MonoBehaviour, IEventListener
	{
		[SerializeField] private List<TileBlock> tileBlockList;

		public void OnEvent(EventType eventType, Component sender, object param = null)
		{
			switch (eventType)
			{
				case EventType.ProgramStart:
					break;
				case EventType.ServerConnection:
					break;
				case EventType.GameStart:
					TurnSwap();
					break;
				case EventType.TileClicked:
					if (param != null)
						OnTileClick((Vector2Int)param);
					break;
				case EventType.TileCommand:
					break;
				case EventType.BlockCommand:
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null);
			}
		}

		void TurnSwap()
		{
			
		}

		private void OnTileClick(Vector2Int id)
		{
			
		}

		private void Start()
		{
			throw new NotImplementedException();
		}
	}
}