using UnityEngine;
using System;

public class PlayerGunController : EntityController, IEntityCallbackUser {

	public InputSource InputSource;
	Action<Entity> _command;
	public uint CooldownTime = 8;
	uint _cooldownCounter;

	public PlayerGunController(){
		InputSource = new KeyboardInputSource();
	}

	public override void LinkCallbacksToEntity(Entity entity){
		base.LinkCallbacksToEntity(entity);
		entity.OnUpdate += OnUpdate;
	}

	protected override void GetCommands(Entity entity)
	{
		if(_cooldownCounter > 0){
			_cooldownCounter--;
			return;
		}
		if(InputSource.BasicAttackHeld){
			Vector2 velocity = InputSource.CursorPosition.normalized * 2;
			float knockbackSpeed = 0.8f;
			_command = (attackingEntity) => {
				new PlayerGunProjectile(attackingEntity, entity.Position, velocity, knockbackSpeed);
				_cooldownCounter = CooldownTime;
				TrackingCircle tc = attackingEntity.GetController<TrackingCircle>();
				if(tc != null){tc.CurrentRadius += 0.4f;}
			};
		}
	}

	protected override void ExecuteCommands(Entity entity){
		ExecuteSingleCommand(ref _command, entity);
	}

	public override void UnlinkCallbacksFromEntity(Entity entity){
		base.UnlinkCallbacksFromEntity(entity);
		entity.OnUpdate -= OnUpdate;
	}

	private void OnUpdate(Entity entity){
		InputSource.UpdateInput();
		Vector2 cursorPos = InputSource.CursorPosition;
		CameraController.SetCameraPositionCallback(()=>{
			if(entity.IsDead){ return null; }
			Vector2 camPosition = new Vector2(entity.Transform.position.x, entity.Transform.position.y)  + cursorPos * 20;
			return new Vector3(camPosition.x, camPosition.y, -30);
		});
	}

	public override ISelfTypeCreator CreateSelfType(){
		return new PlayerGunController();
	}

}