using System;

[Serializable]
public struct BuffPair {

	public enum BuffType {
		Flat,
		Multiplier
	}

	public GameInfo.Stats ModifiedStat;
	public BuffType ModifierType;
	public float Amount;
}