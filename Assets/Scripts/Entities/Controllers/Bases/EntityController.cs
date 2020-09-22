using System;

public abstract class EntityController : ISelfTypeCreator, IEntityCallbackUser {

	public abstract ISelfTypeCreator CreateSelfType();
	protected abstract void GetCommands(Entity entity);
	protected abstract void ExecuteCommands(Entity entity);
	
	public virtual void LinkCallbacksToEntity(Entity entity){
		entity.GetCommandsHandler += GetCommands;
		entity.ExecuteCommandsHandler += ExecuteCommands;
	}

	public virtual void UnlinkCallbacksFromEntity(Entity entity){
		entity.GetCommandsHandler -= GetCommands;
		entity.ExecuteCommandsHandler -= ExecuteCommands;
	}

	protected void ExecuteSingleCommand(ref Action<Entity> command, Entity entity){
		command?.Invoke(entity);
		command = null;
	}
}
