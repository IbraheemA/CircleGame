using UnityEngine;

[CreateAssetMenu(menuName = "CircleGame/Controllers/SlashControllerConfig")]
public class SlashControllerConfig : ControllerConfig {

	private SlashControllerConfig(){}

	public override IEntityCallbackUser CreateController(){
		SlashController c = SelfTypeCreatorFactory<EntityController>.Create<SlashController>();
		return c;
	}
}