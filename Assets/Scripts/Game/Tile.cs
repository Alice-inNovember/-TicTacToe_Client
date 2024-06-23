using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Util.EventSystem;
using EventType = Util.EventSystem.EventType;

namespace Game
{
	public class Tile : MonoBehaviour, IEventListener
	{
		public TileType Type { get; private set; }
		private Vector2Int _id;
		private Button _button;
		private TMP_Text _text;
		
		public void OnEvent(EventType eventType, Component sender, object param = null)
		{
			switch (eventType)
			{
				case EventType.ProgramStart:
					break;
				case EventType.ServerConnection:
					break;
				case EventType.GameStart:
					ExecuteCommand(new TileCommand(_id, TileType.Null, true));
					break;
				case EventType.TileClicked:
					break;
				case EventType.TileCommand:
					if (param != null)
						ExecuteCommand((TileCommand)param);
					break;
				case EventType.BlockCommand:
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null);
			}
		}

		private void Start()
		{
			EventManager.Instance.AddListener(EventType.TileCommand, this);
			EventManager.Instance.AddListener(EventType.GameStart, this);
		}

		public void Init(Vector2Int id)
		{
			_id = id;
			_button = GetComponent<Button>();
			_button.onClick.AddListener(() =>
			{
				EventManager.Instance.PostNotification(EventType.TileClicked, this, _id);
			});
		}

		private void ExecuteCommand(TileCommand command)
		{
			if (command.ID != _id)
				return;
			_button.interactable = command.Interactable;
			_text.text = command.Type switch
			{
				TileType.Null => " ",
				TileType.O => "O",
				TileType.X => "X",
				_ => throw new ArgumentOutOfRangeException(nameof(command.Type), command.Type, null)
			};
			Type = command.Type;
		}
	}
}