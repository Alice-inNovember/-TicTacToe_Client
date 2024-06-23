using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Util.EventSystem;
using EventType = Util.EventSystem.EventType;

namespace Game
{
	public class TileBlock : MonoBehaviour, IEventListener
	{
		public List<Tile> Tiles => tiles;
		[SerializeField] private List<Tile> tiles;
		[SerializeField] private GameObject panel;
		[SerializeField] private int id;
		private TMP_Text _text;
		public void OnEvent(EventType eventType, Component sender, object param = null)
		{
			switch(eventType)
			{
				case EventType.ProgramStart:
					break;
				case EventType.ServerConnection:
					break;
				case EventType.GameStart:
					ExecuteCommand(new BlockCommand(id, TileType.Null, true));
					break;
				case EventType.TileClicked:
					break;
				case EventType.TileCommand:
					break;
				case EventType.BlockCommand:
					if (param != null)
						ExecuteCommand((BlockCommand)param);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null);
			}
		}

		public void Start()
		{
			EventManager.Instance.AddListener(EventType.TileCommand, this);
			EventManager.Instance.AddListener(EventType.GameStart, this);

			for (var i = 0; i < 9 ; i++)
			{
				tiles[i].Init(new Vector2Int(id, i));
			}
		}

		private void ExecuteCommand(BlockCommand command)
		{
			if (command.ID != id)
				return;
			gameObject.SetActive(command.Interactable);
			_text.text = command.Type switch
			{
				TileType.Null => " ",
				TileType.O => "O",
				TileType.X => "X",
				_ => throw new ArgumentOutOfRangeException(nameof(command.Type), command.Type, null)
			};
		}
	}
}