using UnityEngine;
public class KeyboardInputSource : InputSource {

	public override void UpdateInput(){
		Vector2 vec = MoveVector;
		vec.x = (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0);
		vec.y = (Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0);
		MoveVector = vec;
		BasicAttackHeld = Input.GetMouseButton(0);
		Attack2Held = Input.GetMouseButton(1);
		DashKeyPressed = Input.GetKey(KeyCode.LeftShift);
		CursorPosition = CameraController.GetCursorPositionAdjustedToScreen();
	}
}