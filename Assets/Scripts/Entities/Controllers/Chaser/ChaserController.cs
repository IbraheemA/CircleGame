using System;

public class ChaserController : EntityController {

	ChaserState _state;
	ChaserState _nextState;
	Action<Entity> _command;
	TargettingModule _targettingModule;
	WanderingModule _wanderingModule;

	public float ChaseDistance { get; set; }
	public float WanderSpeedMultiplier { get; set; }

	public float AggroRange { 
		get { return _targettingModule.SeekRadius; }
		set { if(value >= 0){ _targettingModule.SeekRadius = value; } }
	}
	public uint FramesSpentWandering { 
		get { return _wanderingModule.FramesUntilStop; }
		set { _wanderingModule.FramesUntilStop = value; }
	}

	public uint FramesSpentStillAfterWander {
		get { return _wanderingModule.StillFramesAfterStop; }
		set { _wanderingModule.StillFramesAfterStop = value; }
	}

	public uint WanderingFramesVariance {
		get { return _wanderingModule.WanderingFramesVariance; }
		set { _wanderingModule.WanderingFramesVariance = value; }
	}

	public uint WanderingStartingCounter {
		get { return _wanderingModule.StartingCounter; }
		set { _wanderingModule.StartingCounter = value; }
	}

	public void ConfigureDefault(){
		ChaseDistance = 15;
		WanderSpeedMultiplier = 0.4f;
		AggroRange = 7;
		FramesSpentWandering = 120;
		FramesSpentStillAfterWander = 60;
		WanderingFramesVariance = 50;
		WanderingStartingCounter = 0;
	}

	public ChaserController(){
		_targettingModule = new TargettingModule();
		_wanderingModule = new WanderingModule();
		ConfigureDefault();
		_state = SelfTypeCreatorFactory<State>.Create<ChasingState>();
	}

	protected override void GetCommands(Entity entity){
		if(!entity.CanExecuteActionOfType(GameInfo.ActionType.Move)){ return; }
		_state.UpdateCommands(entity, this, _targettingModule, _wanderingModule);
	}
	
	protected override void ExecuteCommands(Entity entity){
		_state = _nextState ?? _state;
		_nextState = null;
		ExecuteSingleCommand(ref _command, entity);
	}

	public void IssueCommand(Action<Entity> command){
		_command = command ?? _command;
	}

	public void SetNextState(ChaserState state){
		_nextState = state ?? _nextState;
	}

	public override void LinkCallbacksToEntity(Entity entity){
		base.LinkCallbacksToEntity(entity);
		entity.OnCollision += ReceiveCollision;
	}

	public override void UnlinkCallbacksFromEntity(Entity entity){
		base.UnlinkCallbacksFromEntity(entity);
		entity.OnCollision -= ReceiveCollision;
	}

	private void ReceiveCollision(CollisionEventData data){
		if(!_wanderingModule.IsControllingVelocity){ return; }
		_wanderingModule.SetSeedDirection(data.MainEntity.Velocity);
		_wanderingModule.RecalculateVelocity(data.MainEntity.Velocity.magnitude);
	}

	public void UseConfig(ChaserControllerConfig config){
		if(config == null){ return; }
		ChaseDistance = config.chaseDistance;
		WanderSpeedMultiplier = config.wanderSpeedMultiplier;
		AggroRange = config.aggroRange;
		FramesSpentWandering = config.framesSpentWandering;
		FramesSpentStillAfterWander = config.framesSpentStillAfterWander;
		WanderingFramesVariance = config.wanderingFramesVariance;
		WanderingStartingCounter = config.startingCounter;
		if(config.targettingType == TargettingModule.Technique.ByEntityType){
			Entity.EntityType t = config.targetEntityType;
			_targettingModule.CheckEntityCallback = (callingEntity, entityBeingChecked) => {
				if(entityBeingChecked.IsDead){ return false; }
				if(!Behaviours.CheckIfEntityInSight(callingEntity,entityBeingChecked)){ return false; }
				return entityBeingChecked.Type == t;
			};
		}
	}

	public override ISelfTypeCreator CreateSelfType(){
		return new ChaserController();
	}
}