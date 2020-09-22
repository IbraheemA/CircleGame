using UnityEngine;

[CreateAssetMenu(menuName = "CircleGame/Controllers/PlayerGunControllerConfig")]
public class PlayerGunControllerConfig : ControllerConfig {

	private PlayerGunControllerConfig(){}

	public override IEntityCallbackUser CreateController(){
		PlayerGunController c = SelfTypeCreatorFactory<EntityController>.Create<PlayerGunController>();
		return c;
	}
}