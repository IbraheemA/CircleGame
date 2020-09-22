using UnityEngine;

public class TestCameraController : IEntityCallbackUser, ISelfTypeCreator {

	public InputSource InputSource;

	public TestCameraController(){
		InputSource = new KeyboardInputSource();
	}

	public void LinkCallbacksToEntity(Entity entity){
		entity.OnUpdate += OnUpdate;
	}

	public void UnlinkCallbacksFromEntity(Entity entity){
		entity.OnUpdate -= OnUpdate;
	}

	private void OnUpdate(Entity entity){
		InputSource.UpdateInput();
		Vector2 cursorPos = InputSource.CursorPosition;
		CameraController.SetCameraPositionCallback(()=>{
			if(entity.IsDead){ return null; }
			Vector2 camPosition = new Vector2(entity.Transform.position.x, entity.Transform.position.y)  + cursorPos * 20;
			return new Vector3(camPosition.x, camPosition.y, -30);
		});
	}

	public ISelfTypeCreator CreateSelfType(){
		return new TestCameraController();
	}
}