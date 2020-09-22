using UnityEngine;

public abstract class InputSource {

	public Vector2 MoveVector { get; protected set; }
	public Vector2 CursorPosition { get; protected set; }
	public bool BasicAttackPressed { get; protected set; }
	public bool BasicAttackHeld { get; protected set; }
	public bool Attack2Pressed { get; protected set; }
	public bool Attack2Held { get; protected set; }
	public bool DashKeyPressed { get; protected set; }

	public void Reset(){
		MoveVector = Vector2.zero;
		CursorPosition = Vector2.zero;
		BasicAttackPressed = false;
		BasicAttackHeld = false;
		Attack2Held = false;
		DashKeyPressed = false;
	}

	public InputSource(){
		Reset();
	}

	public abstract void UpdateInput();
}