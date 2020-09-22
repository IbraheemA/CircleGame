using UnityEngine;
using System;

public static class CameraController {

	static float _cursorScalingQuantity;

	static Func<Vector3?> _callback = () => {return Vector3.zero;};
	public static Vector2 GetCursorPositionAdjustedToScreen(){
		_cursorScalingQuantity = Screen.height;
		return new Vector2((Input.mousePosition.x - Screen.width / 2)/_cursorScalingQuantity, (Input.mousePosition.y - Screen.height / 2)/_cursorScalingQuantity);
	}
	public static void SetCameraPositionCallback(Func<Vector3?> callback){
		_callback = callback;
	}

	public static void ExecuteCameraPositionChange(){
		Camera.main.transform.position = _callback() ?? Camera.main.transform.position;
	}
}