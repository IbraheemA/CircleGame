using System;
public class ChasingState : ChaserState {

	public override void UpdateCommands(Entity entity, ChaserController controller, TargettingModule targettingModule, WanderingModule wanderingModule){
		Action<Entity> cmd = (!targettingModule.Target?.IsDead ?? false) ? Behaviours.MoveToTargetIfInRangeAndSight(entity, targettingModule.Target, controller.ChaseDistance) : null;
		if(cmd == null){
			wanderingModule.ForceVelocity(entity.Velocity);
			wanderingModule.SetSeedDirection(UnityEngine.Vector2.zero);
			wanderingModule.SetCycleStage(WanderingModule.CycleStage.StopPoint, -50);
			controller.SetNextState(new ChaserWanderingState());
			return;
		}
		controller.IssueCommand(cmd);
	}

	public override ISelfTypeCreator CreateSelfType(){
		return this;
	}
}