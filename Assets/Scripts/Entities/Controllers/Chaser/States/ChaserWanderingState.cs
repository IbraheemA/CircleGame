public class ChaserWanderingState : ChaserState {

	public override void UpdateCommands(Entity entity, ChaserController controller, TargettingModule targettingModule, WanderingModule wanderingModule){
		Entity target = targettingModule.SeekAndSetTarget(entity);
		if(target != null){
			wanderingModule.IsControllingVelocity = false;
			wanderingModule.ResetCycle();
			controller.SetNextState(new ChasingState());
			return;
		}
		controller.IssueCommand(wanderingModule.CycleAndGetCommand(controller.WanderSpeedMultiplier * entity.GetStat(GameInfo.Stats.MoveSpeed)));
	}
	
	public override ISelfTypeCreator CreateSelfType(){
		return new ChaserWanderingState();
	}
}