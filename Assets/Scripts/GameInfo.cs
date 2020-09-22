using System;
using UnityEngine;

public static class GameInfo {

	public enum ActionType {
		None,
		Move,
		Attack
	}

	public static int ActionTypesLength = Enum.GetNames(typeof(ActionType)).Length;	

	public enum Stats {
		MaxHealth,
		MoveSpeed,
		FlatDamageReduction
	}

	public static int StatsLength = Enum.GetNames(typeof(Stats)).Length;

	public enum Messages {
		Undefined,
		ChangeState,
		Collision
	}

	public static LayerMask EnemiesLayer = LayerMask.GetMask("Enemies");
	public static LayerMask PlayersLayer = LayerMask.GetMask("Players");
	public static LayerMask WallsLayer = LayerMask.GetMask("LowWalls", "HighWalls");
	public static LayerMask HighWallsLayer = LayerMask.GetMask("HighWalls");
}