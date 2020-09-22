using UnityEngine;

[CreateAssetMenu(menuName = "CircleGame/Controllers/PlayerSwordControllerConfig")]
public class PlayerSwordControllerConfig : ControllerConfig {

	private PlayerSwordControllerConfig(){}

	public override IEntityCallbackUser CreateController(){
		PlayerSwordController c = SelfTypeCreatorFactory<EntityController>.Create<PlayerSwordController>();
		return c;
	}
}