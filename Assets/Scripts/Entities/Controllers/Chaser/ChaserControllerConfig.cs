using UnityEngine;

[CreateAssetMenu(menuName = "CircleGame/Controllers/ChaserControllerConfig")]
public class ChaserControllerConfig : ControllerConfig {

	public ChaserControllerConfig(){
	}

	public override IEntityCallbackUser CreateController(){
		ChaserController c = SelfTypeCreatorFactory<EntityController>.Create<ChaserController>();
		c.UseConfig(this);
		return c;
	}

	[Header("Basic")]
	public float chaseDistance;
	public float aggroRange;
	public float wanderSpeedMultiplier;
	[Header("Frame Data")]
	public uint framesSpentWandering;
	public uint framesSpentStillAfterWander;
	public uint wanderingFramesVariance;
	public uint startingCounter;
	[Header("Targetting")]
	public TargettingModule.Technique targettingType;
	[HideInInspector]
	public Entity.EntityType targetEntityType;
}