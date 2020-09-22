using UnityEngine;
using System;

public class PlayerController : EntityController {

	public InputSource InputSource;
	CommandQueue commandQueue;
	int _dashCooldownTimer;
	public uint DashCooldown = 25;

	public override ISelfTypeCreator CreateSelfType(){
		return new PlayerController(InputSource);
	}

	public PlayerController(InputSource inputSource){
		InputSource = inputSource;
		commandQueue = new CommandQueue();
	}

	public PlayerController() : this(new NullInputSource()){
	}

	protected override void GetCommands(Entity entity){
		commandQueue.Clear();
		if(InputSource == null){
			return;
		}
		if(entity.CanExecuteActionOfType(GameInfo.ActionType.Move)){
			if(InputSource.DashKeyPressed && _dashCooldownTimer <= 0){
				uint duration = 2;
				Vector2 velocity = InputSource.MoveVector * 4;
				Action<Entity> dash = (dashingEntity) => {
					if(!entity.CanExecuteActionOfType(GameInfo.ActionType.Move)){return;}
					entity.LockActionType(GameInfo.ActionType.Move,1);
					entity.Velocity = velocity;
					uint _counter = 0;
					Action<Entity> OnFixedUpdate = null;
					OnFixedUpdate = (e) => {
						if(_counter == duration){
							entity.Velocity = Vector2.zero;
							entity.UnlockActionType(GameInfo.ActionType.Move,1);
							entity.OnFixedUpdate -= OnFixedUpdate;
							return;
						}
						_counter++;
					};
					entity.OnFixedUpdate += OnFixedUpdate;
					_dashCooldownTimer = (int)DashCooldown;
				};
				commandQueue.Issue(dash);
				return;
			}
			commandQueue.Issue(Behaviours.GetEntityMoveCommandFromInput(entity, InputSource));
		}
		_dashCooldownTimer--;
	}

	protected override void ExecuteCommands(Entity entity){
		commandQueue.Execute(entity);
	}

	private void GetInput(Entity entity){
		InputSource.UpdateInput();
	}

	public override void LinkCallbacksToEntity(Entity entity){
		base.LinkCallbacksToEntity(entity);
		entity.OnUpdate += GetInput;
	}

	public override void UnlinkCallbacksFromEntity(Entity entity){
		base.UnlinkCallbacksFromEntity(entity);
		entity.OnUpdate -= GetInput;
	}

	public void UseConfig(PlayerControllerConfig config){
		if(config.ControlType == PlayerControllerConfig.InputSourceType.Keyboard){
			InputSource = new KeyboardInputSource();
		}
	}
}
