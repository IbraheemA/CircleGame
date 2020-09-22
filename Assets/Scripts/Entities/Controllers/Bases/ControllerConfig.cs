using UnityEngine;

public abstract class ControllerConfig : ScriptableObject {
	public abstract IEntityCallbackUser CreateController();
}