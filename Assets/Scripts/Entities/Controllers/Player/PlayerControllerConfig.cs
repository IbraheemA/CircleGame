using UnityEngine;

[CreateAssetMenu(menuName = "CircleGame/Controllers/PlayerControllerConfig")]
public class PlayerControllerConfig : ControllerConfig {

	public InputSourceType ControlType;

	public enum InputSourceType {
		Null,
		Keyboard
	}

	private PlayerControllerConfig(){}

	public override IEntityCallbackUser CreateController(){
		PlayerController c = SelfTypeCreatorFactory<EntityController>.Create<PlayerController>();
		c.UseConfig(this);
		return c;
	}
}