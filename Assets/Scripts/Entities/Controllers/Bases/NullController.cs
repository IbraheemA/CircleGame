public class NullController : EntityController {
	
	protected override void GetCommands(Entity entity){
		return;
	}

	protected override void ExecuteCommands(Entity entity){
		return;
	}

	public override ISelfTypeCreator CreateSelfType(){
		return this;
	}
}