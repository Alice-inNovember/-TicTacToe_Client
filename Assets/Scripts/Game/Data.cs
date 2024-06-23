using UnityEngine;

namespace Game
{
	public struct TileCommand
	{
		public TileCommand(Vector2Int id, TileType type, bool interactable)
		{
			ID = id;
			Type = type;
			Interactable = interactable;
		}

		public Vector2Int ID;
		public TileType Type;
		public bool Interactable;
	}
	public enum TileType
	{
		Null = 0,
		O,
		X
	}
	public struct BlockCommand
	{
		public BlockCommand(int id, TileType type, bool interactable)
		{
			ID = id;
			Type = type;
			Interactable = interactable;
		}

		public int ID;
		public TileType Type;
		public bool Interactable;
	}
}