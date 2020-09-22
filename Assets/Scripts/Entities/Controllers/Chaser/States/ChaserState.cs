public abstract class ChaserState : State {

	public abstract void UpdateCommands(Entity entity, ChaserController controller, TargettingModule targettingModule, WanderingModule wanderingModule);

}